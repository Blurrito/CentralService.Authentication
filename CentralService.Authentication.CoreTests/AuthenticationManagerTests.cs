using Xunit;
using CentralService.Authentication.Core;
using CentralService.Authentication.DTO.Api.Authentication;
using CentralService.Authentication.DTO.Api.Common;
using CentralService.Authentication.DTO.Api.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace CentralService.Authentication.Core.Tests
{
    public class AuthenticationManagerTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        public void GetChallenge_NoErrors_Success(int GameProfileId)
        {
            Session Session = new Session(1, 1, GameProfileId, "6kcsjpp79AMCJ0rfcd4v", default(SessionStatus));
            AuthenticationManager Manager = new AuthenticationManager();

            UserChallenge? Response = Manager.GetUserChallenge(Session);

            Assert.True(Response.HasValue);
        }

        [Fact]
        public void GetChallenge_NoExistingChallenge_ThrowsArgumentException()
        {
            AuthenticationManager Manager = new AuthenticationManager();

            Assert.Throws<ArgumentException>(() => Manager.GetUserChallenge(new Session()));
        }

        [Fact]
        public void ValidateChallenge_ResponseCorrect_Success()
        {
            Session Session = new Session(1, 1, 1, "6kcsjpp79AMCJ0rfcd4v", default(SessionStatus));
            AuthenticationManager Manager = new AuthenticationManager();
            UserChallenge? Response = Manager.GetUserChallenge(Session);

            string ClientChallenge = "01234567890123456789012345678901";
            string ServerChallenge = "ABCDEFGHIJ";
            string Result = GenerateChallengeResult(ServerChallenge, ClientChallenge, Response.Value.Challenge, Response.Value.Token);
            string Proof = GenerateChallengeResult(ServerChallenge, ClientChallenge, Response.Value.Challenge, Response.Value.Token, true);
            UserChallengeResult ResultStruct = new UserChallengeResult(Response.Value.Token, ClientChallenge, ServerChallenge, Result);

            UserChallengeProof? ChallengeProof = Manager.ValidateUserChallengeResult(ResultStruct);

            Assert.True(ChallengeProof.HasValue);
        }

        [Theory]
        [InlineData(null, "ABCDEFGHIJ", typeof(ArgumentNullException))]
        [InlineData("", "ABCDEFGHIJ", typeof(ArgumentException))]
        [InlineData("01234567890123456789012345678901", "HGFEDCBA", typeof(System.Security.Authentication.AuthenticationException))]
        public void ValidateChallenge_ResponseIncorrect_ThrowAuthenticationException(string ClientChallenge, string ServerChallenge, Type ExpectedException)
        {
            Session Session = new Session(1, 1, 1, "6kcsjpp79AMCJ0rfcd4v", default(SessionStatus));
            AuthenticationManager Manager = new AuthenticationManager();
            UserChallenge? Response = Manager.GetUserChallenge(Session);

            string Result = GenerateChallengeResult(ServerChallenge, ClientChallenge, Response.Value.Challenge, Response.Value.Token);
            string Proof = GenerateChallengeResult(ServerChallenge, ClientChallenge, Response.Value.Challenge, Response.Value.Token, true);
            UserChallengeResult ResultStruct = new UserChallengeResult(Response.Value.Token, ClientChallenge, "ABCDEFGH", Result);

            Assert.Throws(ExpectedException, () => Manager.ValidateUserChallengeResult(ResultStruct));
        }

        [Fact]
        public void GetMatchmakingChallenge_NoErrors_Success()
        {
            AuthenticationManager Manager = new AuthenticationManager();

            MatchmakingChallenge Challenge = Manager.GetMatchmakingChallenge(1, "pokemondpds", "127.0.0.1", "13000");

            Assert.True(Challenge.Challenge != null);
        }

        [Theory]
        [InlineData(1, null, "127.0.0.1", "13000", typeof(ArgumentNullException))]
        [InlineData(1, "pokemondpds", "", "13000", typeof(ArgumentException))]
        [InlineData(0, "pokemondpds", "127.0.0.1", "13000", typeof(ArgumentException))]
        public void GetMatchmakingChallenge_InvalidInput_ThrowsException(int SessionId, string GameName, string Address, string Port, Type ExpectedException)
        {
            AuthenticationManager Manager = new AuthenticationManager();

            Assert.Throws(ExpectedException, () => Manager.GetMatchmakingChallenge(SessionId, GameName, Address, Port));
        }

        [Fact]
        public void ValidateMatchmakingChallengeResult_NoErrors_Success()
        {
            AuthenticationManager Manager = new AuthenticationManager();
            MatchmakingChallenge Challenge = Manager.GetMatchmakingChallenge(1, "pokemondpds", "127.0.0.1", "13000");
            string GameKey = GameKeyCollection.GetGameKey(Challenge.GameName);
            string CalculatedResult = Convert.ToBase64String(RC4.Compute(Challenge.Challenge, GameKey));
            MatchmakingChallengeResult Result = new MatchmakingChallengeResult(Challenge.SessionId, CalculatedResult);

            MatchmakingChallengeProof Proof = Manager.ValidateMatchmakingChallengeResult(Result);

            Assert.True(Proof.Passed);
        }

        [Fact]
        public void ValidateMatchmakingChallengeResult_IncorrectResult_Success()
        {
            AuthenticationManager Manager = new AuthenticationManager();
            MatchmakingChallenge Challenge = Manager.GetMatchmakingChallenge(1, "pokemondpds", "127.0.0.1", "13000");
            MatchmakingChallengeResult Result = new MatchmakingChallengeResult(Challenge.SessionId, "Dummy");

            MatchmakingChallengeProof Proof = Manager.ValidateMatchmakingChallengeResult(Result);

            Assert.False(Proof.Passed);
        }

        [Theory]
        [InlineData(1, null, typeof(ArgumentNullException))]
        [InlineData(1, "", typeof(ArgumentException))]
        [InlineData(0, "Dummy", typeof(ArgumentException))]
        public void ValidateMatchmakingChallengeResult_InvalidData_ThrowsException(int SessionId, string ChallengeResult, Type ExpectedException)
        {
            AuthenticationManager Manager = new AuthenticationManager();
            MatchmakingChallenge Challenge = Manager.GetMatchmakingChallenge(1, "pokemondpds", "127.0.0.1", "13000");
            MatchmakingChallengeResult Result = new MatchmakingChallengeResult(SessionId, ChallengeResult);

            Assert.Throws(ExpectedException, () => Manager.ValidateMatchmakingChallengeResult(Result));
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