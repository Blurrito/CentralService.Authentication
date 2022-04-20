using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Authentication.DTO.Api.Authentication
{
    public struct UserChallengeResult
    {
        public string NasToken { get; set; }
        public string Challenge { get; set; }
        public string Result { get; set; }
    }
}
