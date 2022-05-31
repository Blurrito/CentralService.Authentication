using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Authentication.DTO.Api.Authentication
{
    public struct MatchmakingChallengeResult
    {
        public uint SessionId { get; set; }
        public string ChallengeResult { get; set; }

        public MatchmakingChallengeResult(uint SessionId, string ChallengeResult)
        {
            this.SessionId = SessionId;
            this.ChallengeResult = ChallengeResult;
        }
    }
}
