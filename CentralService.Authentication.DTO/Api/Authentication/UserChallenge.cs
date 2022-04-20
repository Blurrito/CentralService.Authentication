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
        public string Address { get; set; }
        public string Token { get; set; }
        public string NasChallenge { get; set; }
        public string GpcmChallenge { get; set; }
        public int DeviceProfileId { get; set; }
        public int GameProfileId { get; set; }
        public string GameCode { get; set; }
        public string RegionalGameCode { get; set; }
        public string UniqueNickname { get; set; }
        public DateTime ValidUntil { get; set; }

        public UserChallenge(NasToken Token, string Challenge)
        {
            Address = Token.Address;
            this.Token = Token.Token;
            NasChallenge = Token.Challenge;
            ValidUntil = Token.ValidUntil;
            GpcmChallenge = Challenge;
            DeviceProfileId = Token.DeviceProfileId;
            GameProfileId = Token.GameProfileId;
            GameCode = Token.GameCode;
            RegionalGameCode = Token.RegionalGameCode;
            UniqueNickname = Token.UniqueNickname;
        }
    }
}
