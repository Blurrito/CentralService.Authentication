using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Authentication.DTO.Api.Common
{
    public struct ApiError
    {
        public int ErrorCode { get; set; }
        public string Reason { get; set; }
        public bool IsFatal { get; set; }

        public ApiError(int ErrorCode, string Reason, bool IsFatal)
        {
            this.ErrorCode = ErrorCode;
            this.Reason = Reason;
            this.IsFatal = IsFatal;
        }
    }
}
