using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Authentication.DTO.Api.Authentication
{
    public struct UserChallengeResult
    {
        public string Token { get; set; }
        public string ClientChallenge { get; set; }
        public string ServerChallenge { get; set; }
        public string Result { get; set; }

        public UserChallengeResult(string Token, string ClientChallenge, string ServerChallenge, string Result)
        {
            this.Token = Token;
            this.ClientChallenge = ClientChallenge;
            this.ServerChallenge = ServerChallenge;
            this.Result = Result;
        }
    }
}
