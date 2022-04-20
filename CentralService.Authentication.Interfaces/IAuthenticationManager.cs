using CentralService.Authentication.DTO.Api.Authentication;
using CentralService.Authentication.DTO.Api.Common;
using CentralService.Authentication.DTO.Api.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Authentication.Interfaces
{
    public interface IAuthenticationManager : IDisposable
    {
        void GenerateUserChallenge(NasToken Token);
        ApiResponse? GetUserChallenge(string Address);
        ApiResponse? ValidateUserChallengeResult(string Address, UserChallengeResult Result);
        ApiResponse? GetMatchmakingChallenge(int SessionId, string GameName, string Address, string Port);
        ApiResponse? ValidateMatchmakingChallengeResult(MatchmakingChallengeResult Result);
    }
}
