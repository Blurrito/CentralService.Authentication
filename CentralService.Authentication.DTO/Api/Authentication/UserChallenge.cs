using CentralService.Authentication.DTO.Api.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Authentication.DTO.Api.Authentication
{
    public struct UserChallenge
    {
        public string Token { get; set; }
        public string Challenge { get; set; }
        public Session Session { get; set; }
        public DateTime ValidUntil { get; set; }

        public UserChallenge(string Token, string Challenge, Session Session)
        {
            this.Token = Token;
            this.Challenge = Challenge;
            this.Session = Session;
            ValidUntil = DateTime.Now.AddMinutes(5);
        }
    }
}
