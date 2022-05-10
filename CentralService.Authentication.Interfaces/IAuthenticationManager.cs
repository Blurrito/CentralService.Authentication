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
        UserChallenge GetUserChallenge(Session Session);
        UserChallengeProof ValidateUserChallengeResult(UserChallengeResult Result);
        MatchmakingChallenge GetMatchmakingChallenge(int SessionId, string GameName, string Address, string Port);
        MatchmakingChallengeProof ValidateMatchmakingChallengeResult(MatchmakingChallengeResult Result);
    }
}
