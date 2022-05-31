using CentralService.Authentication.DTO.Api.Authentication;
using CentralService.Authentication.DTO.Api.Common;
using CentralService.Authentication.DTO.Api.User;
using CentralService.Authentication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
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

        public UserChallenge GetUserChallenge(Session Session)
        {
            if (Session.SessionKey <= 0 || Session.DeviceProfileId <= 0)
                throw new ArgumentException(nameof(Session), "One or more numerical values of the provided session object are (lower than) zero.");

            if (Session.GameProfileId > 0)
                lock (_UserChallengeLock)
                {
                    _UserChallenges.RemoveAll(x => x.ValidUntil <= DateTime.Now || x.Session.DeviceProfileId == Session.DeviceProfileId);

                    string Token = string.Empty;
                    do { Token = GenerateToken(); }
                    while (_UserChallenges.Any(x => x.Token == Token));

                    UserChallenge Challenge = new UserChallenge(GenerateToken(), GenerateRandomString(8, "ABCDEFGHIJKLMNOPQRSTUVWXYZ"), Session);
                    _UserChallenges.Add(Challenge);
                    return Challenge;
                }
            else
                return new UserChallenge(GenerateToken(), GenerateRandomString(8, "ABCDEFGHIJKLMNOPQRSTUVWXYZ"), Session);
        }

        public UserChallengeProof ValidateUserChallengeResult(UserChallengeResult Result)
        {
            if (Result.Token == null || Result.ClientChallenge == null || Result.ServerChallenge == null || Result.Result == null)
                throw new ArgumentNullException(nameof(Result), "One or more properties of the provided challenge result object are null.");
            if (Result.Token == string.Empty || Result.ClientChallenge == string.Empty || Result.ServerChallenge == string.Empty || Result.Result == string.Empty)
                throw new ArgumentException(nameof(Result), "One or more properties of the provided challenge result object are empty.");

            lock (_UserChallengeLock)
            {
                _UserChallenges.RemoveAll(x => x.ValidUntil <= DateTime.Now);;
                UserChallenge ClientChallenge = _UserChallenges.FirstOrDefault(x => x.Token == Result.Token);
                if (ClientChallenge.Token == null)
                    throw new KeyNotFoundException("No authentication token could be found for provided challenge result.");

                string ChallengeResult = GenerateChallengeResult(ClientChallenge, Result);
                if (ChallengeResult != Result.Result)
                    throw new AuthenticationException("The generated login challenge result does not match the provided login challenge result.");
                return new UserChallengeProof(ClientChallenge.Session, GenerateChallengeResult(ClientChallenge, Result, true));
            }
        }

        public MatchmakingChallenge GetMatchmakingChallenge(uint SessionId, string GameName, string Address, string Port)
        {
            if (GameName == null || Address == null || Port == null)
                throw new ArgumentNullException();
            if (GameName == string.Empty || Address == string.Empty || Port == string.Empty)
                throw new ArgumentException();
            if (SessionId < 1)
                throw new ArgumentException(nameof(SessionId), "The provided session ID is lower than 1.");

            lock (_MatchmakingChallengeLock)
            {
                MatchmakingChallenge ExistingChallenge = _MatchmakingChallenges.FirstOrDefault(x => x.SessionId == SessionId);
                if (ExistingChallenge.Challenge != null)
                    _MatchmakingChallenges.Remove(ExistingChallenge);
                string ChallengeString = GenerateRandomString(6, "!\"#$%&\'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~");
                ExistingChallenge = new MatchmakingChallenge(SessionId, GameName, $"{ ChallengeString }00{ Address }{ Port }");
                _MatchmakingChallenges.Add(ExistingChallenge);
                return ExistingChallenge;
            }
        }

        public MatchmakingChallengeProof ValidateMatchmakingChallengeResult(MatchmakingChallengeResult Result)
        {
            if (Result.ChallengeResult == null)
                throw new ArgumentNullException(nameof(Result.ChallengeResult), "The challenge result of the provided result object is null.");
            if (Result.ChallengeResult == string.Empty)
                throw new ArgumentException(nameof(Result.ChallengeResult), "The challenge result of the provided result object is empty.");
            if (Result.SessionId < 1)
                throw new ArgumentException(nameof(Result.SessionId), "The provided session ID is (lower than) zero.");

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
            return new MatchmakingChallengeProof(Result.SessionId, Passed);
        }

        public void Dispose() { }

        private string GenerateRandomString(int Length, string CharSet)
        {
            string ReturnString = string.Empty;
            List<int> CharacterIndexes = GenerateRandomNumbers(Length, CharSet.Length - 1);
            for (int i = 0; i < Length; i++)
                ReturnString += CharSet[CharacterIndexes[i]];
            return ReturnString;
        }

        private string GenerateToken()
        {
            byte[] TokenBase = new byte[96];
            List<int> CharacterIndexes = GenerateRandomNumbers(TokenBase.Length, 255);
            for (int i = 0; i < TokenBase.Length; i++)
                TokenBase[i] = Convert.ToByte(CharacterIndexes[i]);
            return $"NDS{ Convert.ToBase64String(TokenBase) }";
        }

        private List<int> GenerateRandomNumbers(int Count, int Max)
        {
            List<int> Numbers = new List<int>();
            byte[] Output = new byte[Count * 4];

            using (RandomNumberGenerator Generator = RandomNumberGenerator.Create())
                Generator.GetBytes(Output);

            for (int i = 0; i < Count; i++)
                Numbers.Add((BitConverter.ToInt32(Output, i * 4) & 0x7FFFFFFF) % Max);
            return Numbers;
        }

        private string GenerateChallengeResult(UserChallenge Challenge, UserChallengeResult Result, bool IsProof = false)
        {
            //md5sum(md5sum(challenge) + (" " * 48) + authtoken + clientChallenge + serverChallenge + md5sum(challenge))
            string CompleteChallenge = GenerateMD5Hash(Challenge.Challenge);
            CompleteChallenge += new string(' ', 48);
            CompleteChallenge += Challenge.Token;
            //Generating the proof token is identical other than switching the order of the GPCM client/server challenge
            if (IsProof)
                CompleteChallenge += Result.ServerChallenge + Result.ClientChallenge;
            else
                CompleteChallenge += Result.ClientChallenge + Result.ServerChallenge;
            CompleteChallenge += GenerateMD5Hash(Challenge.Challenge);
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
