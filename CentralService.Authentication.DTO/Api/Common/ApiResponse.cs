using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Authentication.DTO.Api.Common
{
    public struct ApiResponse
    {
        public bool Success { get; set; }
        public object Content { get; set; }

        public ApiResponse(bool Success, object Content)
        {
            this.Success = Success;
            this.Content = Content;
        }
    }
}
