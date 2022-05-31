using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Authentication.DTO.Api.Authentication
{
    public struct MatchmakingChallenge
    {
        public uint SessionId { get; set; }
        public string GameName { get; set; }
        public string Challenge { get; set; }

        public MatchmakingChallenge(uint SessionId, string GameName, string Challenge)
        {
            this.SessionId = SessionId;
            this.GameName = GameName;
            this.Challenge = Challenge;
        }
    }
}
