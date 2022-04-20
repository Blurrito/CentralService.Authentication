using CentralService.Authentication.DTO.Api.Authentication;
using CentralService.Authentication.DTO.Api.Common;
using CentralService.Authentication.DTO.Api.User;
using CentralService.Authentication.Interfaces;
using CentralService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Authentication.Core
{
    public class AuthenticationManager : IAuthenticationManager
    {
        private static List<UserChallenge> _UserChallenges = new List<UserChallenge>();
        private static readonly object _UserChallengeLock = new object();

        private static List<MatchmakingChallenge> _MatchmakingChallenges = new List<MatchmakingChallenge>();
        private static readonly object _MatchmakingChallengeLock = new object();

        public AuthenticationManager() { }

        public void GenerateUserChallenge(NasToken Token)
        {
            UserChallenge Challenge = new UserChallenge(Token, GenerateRandomString(10, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
            lock (_UserChallengeLock)
            {
                _UserChallenges.RemoveAll(x => x.ValidUntil < DateTime.Now || x.Address == Token.Address);
                _UserChallenges.Add(Challenge);
            }
        }

        public ApiResponse? GetUserChallenge(string Address)
        {
            UserChallenge Challenge = GetExistingUserChallenge(Address);
            if (Challenge.Address == null)
                return new ApiResponse(false, null);
            return new ApiResponse(true, Challenge.GpcmChallenge);
        }

        public ApiResponse? ValidateUserChallengeResult(string Address, UserChallengeResult Result)
        {
            UserChallenge ClientChallenge = GetExistingUserChallenge(Address);
            string ChallengeResult = GenerateChallengeResult(ClientChallenge, Result);
            if (ChallengeResult != Result.Result)
                return new ApiResponse(false, new ApiError(266, "There was an error validating the pre-authentication.", true));
            return new ApiResponse(true, new UserChallengeProof(ClientChallenge, GenerateChallengeResult(ClientChallenge, Result, true)));
        }

        public ApiResponse? GetMatchmakingChallenge(int SessionId, string GameName, string Address, string Port)
        {
            string ChallengeString;
            lock (_MatchmakingChallengeLock)
            {
                MatchmakingChallenge ExistingChallenge = _MatchmakingChallenges.FirstOrDefault(x => x.SessionId == SessionId);
                if (ExistingChallenge.Challenge != null)
                    _MatchmakingChallenges.Remove(ExistingChallenge);
                ChallengeString = GenerateRandomString(6, "!\"#$%&\'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
                _MatchmakingChallenges.Add(new MatchmakingChallenge(SessionId, GameName, $"{ ChallengeString }00{ Address }{ Port }"));
            }
            return new ApiResponse(true, ChallengeString);
        }

        public ApiResponse? ValidateMatchmakingChallengeResult(MatchmakingChallengeResult Result)
        {
            bool Passed = false;
            lock (_MatchmakingChallengeLock)
            {
                MatchmakingChallenge ExistingChallenge = _MatchmakingChallenges.FirstOrDefault(x => x.SessionId == Result.SessionId);
                if (ExistingChallenge.Challenge != null)
                {
                    string GameKey = GameKeyCollection.GetGameKey(ExistingChallenge.GameName);
                    if (GameKey != null)
                    {
                        string CalculatedResult = Convert.ToBase64String(RC4.Compute(ExistingChallenge.Challenge, GameKey));
                        if (CalculatedResult == Result.ChallengeResult)
                            Passed = true;
                    }
                    _MatchmakingChallenges.Remove(ExistingChallenge);
                }
            }
            return new ApiResponse(true, Passed);
        }

        public void Dispose() { }

        private UserChallenge GetExistingUserChallenge(string Address)
        {
            UserChallenge Challenge;
            lock (_UserChallengeLock)
            {
                _UserChallenges.RemoveAll(x => x.ValidUntil < DateTime.Now);
                Challenge = _UserChallenges.FirstOrDefault(x => x.Address == Address);
            }
            return Challenge;
        }

        private string GenerateRandomString(int Length, string CharSet)
        {
            string ReturnString = string.Empty;
            Random Random = new Random();
            for (int i = 0; i < Length; i++)
                ReturnString += CharSet[Random.Next(CharSet.Length)];
            return ReturnString;
        }

        private string GenerateChallengeResult(UserChallenge Challenge, UserChallengeResult Result, bool IsProof = false)
        {
            //md5sum(md5sum(challenge) + (" " * 48) + authtoken + clientChallenge + serverChallenge + md5sum(challenge))
            string CompleteChallenge = GenerateMD5Hash(Challenge.NasChallenge);
            CompleteChallenge += new string(' ', 48);
            CompleteChallenge += Challenge.Token;
            //Generating the proof token is identical other than switching the order of the GPCM client/server challenge
            if (IsProof)
                CompleteChallenge += Challenge.GpcmChallenge + Result.Challenge;
            else
                CompleteChallenge += Result.Challenge + Challenge.GpcmChallenge;
            CompleteChallenge += GenerateMD5Hash(Challenge.NasChallenge);
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
