using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Authentication.DTO.Api.Authentication
{
    public struct MatchmakingChallengeProof
    {
        public uint SessionId { get; set; }
        public bool Passed { get; set; }

        public MatchmakingChallengeProof(uint SessionId, bool Passed)
        {
            this.SessionId = SessionId;
            this.Passed = Passed;
        }
    }
}
