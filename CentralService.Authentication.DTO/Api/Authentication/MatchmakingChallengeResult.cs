using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Authentication.DTO.Api.Authentication
{
    public struct MatchmakingChallengeResult
    {
        public int SessionId { get; set; }
        public string ChallengeResult { get; set; }
    }
}
