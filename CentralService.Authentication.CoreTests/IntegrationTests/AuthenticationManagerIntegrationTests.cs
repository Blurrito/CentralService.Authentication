using CentralService.Authentication.Controllers;
using CentralService.Authentication.Core;
using CentralService.Authentication.DTO.Api.Authentication;
using CentralService.Authentication.DTO.Api.User;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CentralService.Authentication.CoreTests.IntegrationTests
{
    public class AuthenticationManagerIntegrationTests
    {
        [Fact]
        public async void UserChallengeIntegrationTest()
        {
            //Setup objects that will be used in the test
            AuthenticationController Controller = new AuthenticationController();
            Session Session1 = new Session(10, 10, 10, "6kcsjpp79AMCJ0rfcd4v", default(SessionStatus));
            Session Session2 = new Session(20, 10, 10, "6kcsjpp79AMCJ0rfcd4v", default(SessionStatus));
            Session Session3 = new Session(30, 20, 20, "6kcsjpp79AMCJ0rfcd4v", default(SessionStatus));
            string ClientChallenge = "01234567890123456789012345678901";
            string ServerChallenge = "ABCDEFGHIJ";

            //Get a new user challenge
            var OkResponse = (OkObjectResult)Controller.GetChallenge(Session1).Result;
            UserChallenge Challenge = (UserChallenge)OkResponse.Value;
            Assert.True(Challenge.Session.SessionKey == Session1.SessionKey && Challenge.Session.DeviceProfileId == Session1.DeviceProfileId);

            //Get a new user challenge for a different session ID using the same device ID
            OkResponse = (OkObjectResult)Controller.GetChallenge(Session2).Result;
            Challenge = (UserChallenge)OkResponse.Value;
            Assert.True(Challenge.Session.SessionKey == Session2.SessionKey && Challenge.Session.DeviceProfileId == Session2.DeviceProfileId);

            //Validate the user challenge
            string ChallengeResult = GenerateChallengeResult(ServerChallenge, ClientChallenge, Challenge.Challenge, Challenge.Token);
            string ChallengeProof = GenerateChallengeResult(ServerChallenge, ClientChallenge, Challenge.Challenge, Challenge.Token, true);
            UserChallengeResult Result = new UserChallengeResult(Challenge.Token, ClientChallenge, ServerChallenge, ChallengeResult);
            OkResponse = (OkObjectResult)Controller.ValidateChallengeResponse(Result).Result;
            UserChallengeProof ChallengeResponse = (UserChallengeProof)OkResponse.Value;
            Assert.True(ChallengeResponse.Proof == ChallengeProof);

            //Validate a non-existant user challenge
            Result = new UserChallengeResult("DummyToken", ClientChallenge, ServerChallenge, ChallengeResult);
            var NotFoundResponse = (NotFoundResult)Controller.ValidateChallengeResponse(Result).Result;
            Assert.True(NotFoundResponse.StatusCode == (int)System.Net.HttpStatusCode.NotFound);

            //Validate an incorrect response
            ChallengeResult = GenerateChallengeResult("JIHGEDCBA", ClientChallenge, Challenge.Challenge, Challenge.Token);
            Result = new UserChallengeResult(Challenge.Token, ClientChallenge, ServerChallenge, ChallengeResult);
            var UnauthorizedResponse = (UnauthorizedObjectResult)Controller.ValidateChallengeResponse(Result).Result;
            Assert.True(UnauthorizedResponse.StatusCode == (int)System.Net.HttpStatusCode.Unauthorized);

            //Add second user challenge
            OkResponse = (OkObjectResult)Controller.GetChallenge(Session3).Result;
            Challenge = (UserChallenge)OkResponse.Value;
            Assert.True(Challenge.Session.SessionKey == Session3.SessionKey && Challenge.Session.DeviceProfileId == Session3.DeviceProfileId);

            //Validate second user challenge
            ChallengeResult = GenerateChallengeResult(ServerChallenge, ClientChallenge, Challenge.Challenge, Challenge.Token);
            ChallengeProof = GenerateChallengeResult(ServerChallenge, ClientChallenge, Challenge.Challenge, Challenge.Token, true);
            Result = new UserChallengeResult(Challenge.Token, ClientChallenge, ServerChallenge, ChallengeResult);
            OkResponse = (OkObjectResult)Controller.ValidateChallengeResponse(Result).Result;
            ChallengeResponse = (UserChallengeProof)OkResponse.Value;
            Assert.True(ChallengeResponse.Proof == ChallengeProof);
        }

        [Fact]
        public void MatchmakingChallengeIntegrationtest()
        {
            //Setup objects that will be used in the test
            AuthenticationController Controller = new AuthenticationController();
            //AuthenticationManager Manager = new AuthenticationManager();
            uint[] SessionIds = { 10, 20 };
            string[] GameNames = { "pokemondpds", "mariokartds" };
            string[] Addresses = { "127.0.0.10", "127.0.0.20" };
            string[] Ports = { "13010", "13020" };

            //Create a matchmaking challenge
            var OkResponse = (OkObjectResult)Controller.GetMatchmakingChallenge(SessionIds[0], GameNames[0], Addresses[0], Ports[0]).Result;
            MatchmakingChallenge Challenge = (MatchmakingChallenge)OkResponse.Value;
            Assert.True(Challenge.SessionId == SessionIds[0] && Challenge.GameName == GameNames[0]);

            //Overwrite matchmaking challenge
            OkResponse = (OkObjectResult)Controller.GetMatchmakingChallenge(SessionIds[0], GameNames[1], Addresses[0], Ports[0]).Result;
            Challenge = (MatchmakingChallenge)OkResponse.Value;
            Assert.True(Challenge.SessionId == SessionIds[0] && Challenge.GameName == GameNames[1]);

            //Validate matchmaking challenge result
            string GameKey = GameKeyCollection.GetGameKey(Challenge.GameName);
            string ChallengeResult = Convert.ToBase64String(RC4.Compute(Challenge.Challenge, GameKey));
            MatchmakingChallengeResult Result = new MatchmakingChallengeResult(Challenge.SessionId, ChallengeResult);
            OkResponse = (OkObjectResult)Controller.ValidateMatchmakingChallengeResult(Result).Result;
            MatchmakingChallengeProof Proof = (MatchmakingChallengeProof)OkResponse.Value;
            Assert.True(Proof.Passed);

            //Validate non-existant challenge result
            Result = new MatchmakingChallengeResult(SessionIds[1], ChallengeResult);
            OkResponse = (OkObjectResult)Controller.ValidateMatchmakingChallengeResult(Result).Result;
            Proof = (MatchmakingChallengeProof)OkResponse.Value;
            Assert.False(Proof.Passed);

            //Validate incorrect challenge result
            ChallengeResult = Convert.ToBase64String(RC4.Compute(GameNames[0], GameKey));
            Result = new MatchmakingChallengeResult(Challenge.SessionId, ChallengeResult);
            OkResponse = (OkObjectResult)Controller.ValidateMatchmakingChallengeResult(Result).Result;
            Proof = (MatchmakingChallengeProof)OkResponse.Value;
            Assert.False(Proof.Passed);

            //Add second challenge
            OkResponse = (OkObjectResult)Controller.GetMatchmakingChallenge(SessionIds[1], GameNames[0], Addresses[1], Ports[1]).Result;
            Challenge = (MatchmakingChallenge)OkResponse.Value;
            Assert.True(Challenge.SessionId == SessionIds[1] && Challenge.GameName == GameNames[0]);

            //Verify second challenge
            GameKey = GameKeyCollection.GetGameKey(Challenge.GameName);
            ChallengeResult = Convert.ToBase64String(RC4.Compute(Challenge.Challenge, GameKey));
            Result = new MatchmakingChallengeResult(Challenge.SessionId, ChallengeResult);
            OkResponse = (OkObjectResult)Controller.ValidateMatchmakingChallengeResult(Result).Result;
            Proof = (MatchmakingChallengeProof)OkResponse.Value;
            Assert.True(Proof.Passed);
        }

        private string GenerateChallengeResult(string GpcmServerChallenge, string GpcmClientChallenge, string NasChallenge, string Token, bool IsProof = false)
        {
            //md5sum(md5sum(challenge) + (" " * 48) + authtoken + clientChallenge + serverChallenge + md5sum(challenge))
            string CompleteChallenge = GenerateMD5Hash(NasChallenge);
            CompleteChallenge += new string(' ', 48);
            CompleteChallenge += Token;
            //Generating the proof token is identical other than switching the order of the GPCM client/server challenge
            if (IsProof)
                CompleteChallenge += GpcmServerChallenge + GpcmClientChallenge;
            else
                CompleteChallenge += GpcmClientChallenge + GpcmServerChallenge;
            CompleteChallenge += GenerateMD5Hash(NasChallenge);
            return GenerateMD5Hash(CompleteChallenge);
        }

        public string GenerateMD5Hash(string Input)
        {
            string Result = "";
            using (MD5 Hash = MD5.Create())
            {
                byte[] HashedInput = Hash.ComputeHash(Encoding.UTF8.GetBytes(Input));
                Result = BitConverter.ToString(HashedInput).Replace("-", "");
            }
            return Result.ToLower();
        }
    }
}
