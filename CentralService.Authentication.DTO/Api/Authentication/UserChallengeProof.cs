using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Authentication.DTO.Api.Authentication
{
    public struct UserChallengeProof
    {
        public string Address { get; set; }
        public int DeviceProfileId { get; set; }
        public int GameProfileId { get; set; }
        public string UniqueNickname { get; set; }
        public string GameCode { get; set; }
        public string RegionalGameCode { get; set; }
        public string Proof { get; set; }
        public string LoginToken { get; set; }

        public UserChallengeProof(UserChallenge Challenge, string Proof)
        {
            LoginToken = Convert.ToBase64String(new byte[16] { 0x4E, 0x69, 0x6E, 0x74, 0x65, 0x6E, 0x64, 0x6F, 0x57, 0x69, 0x66, 0x69, 0x43, 0x6F, 0x6E, 0x6E }).Replace('=', '_').Replace('+', '[').Replace('/', ']');
            Address = Challenge.Address;
            DeviceProfileId = Challenge.DeviceProfileId;
            GameProfileId = Challenge.GameProfileId;
            GameCode = Challenge.GameCode;
            RegionalGameCode = Challenge.RegionalGameCode;
            UniqueNickname = Challenge.UniqueNickname;
            this.Proof = Proof;
        }
    }
}
