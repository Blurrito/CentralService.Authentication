using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Authentication.DTO.Api.Authentication
{
    public struct MatchmakingChallengeProof
    {
        public int SessionId { get; set; }
        public bool Passed { get; set; }

        public MatchmakingChallengeProof(int SessionId, bool Passed)
        {
            this.SessionId = SessionId;
            this.Passed = Passed;
        }
    }
}
