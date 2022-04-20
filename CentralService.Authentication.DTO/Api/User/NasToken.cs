using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Authentication.DTO.Api.User
{
    public struct NasToken
    {
        public string Address { get; set; }
        public string Token { get; set; }
        public string Challenge { get; set; }
        public int DeviceProfileId { get; set; }
        public int GameProfileId { get; set; }
        public string GameCode { get; set; }
        public string RegionalGameCode { get; set; }
        public string UniqueNickname { get; set; }
        public DateTime ValidUntil { get; set; }
    }
}
