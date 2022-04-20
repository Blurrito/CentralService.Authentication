using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CentralService.Authentication.DTO.Api.Common;
using CentralService.Authentication.DTO.Api.Authentication;
using CentralService.Authentication.DTO.Api.User;
using CentralService.Authentication.Core.Factories;
using CentralService.Authentication.Interfaces;

namespace CentralService.Authentication.Controllers
{
    [ApiController]
    [Route("api/ds/auth")]
    public class AuthenticationController : ControllerBase
    {
        [HttpPost("generateuserchallenge")]
        public ActionResult GenerateChallenge(NasToken Token)
        {
            using (IAuthenticationManager Manager = AuthenticationManagerFactory.GetManager())
                Manager.GenerateUserChallenge(Token);
            return Ok();
        }

        [HttpGet("getuserchallenge")]
        public ActionResult<ApiResponse?> GetChallenge(string Address)
        {
            try
            {
                using (IAuthenticationManager Manager = AuthenticationManagerFactory.GetManager())
                    return Ok(Manager.GetUserChallenge(Address));
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("validateuserchallengeresponse")]
        public ActionResult<ApiResponse?> ValidateChallengeResponse(string Address, UserChallengeResult Result)
        {
            try
            {
                using (IAuthenticationManager Manager = AuthenticationManagerFactory.GetManager())
                    return Ok(Manager.ValidateUserChallengeResult(Address, Result));
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("getmatchmakingchallenge")]
        public ActionResult<ApiResponse?> GetMatchmakingChallenge(int SessionId, string GameName, string Address, string Port)
        {
            try
            {
                using (IAuthenticationManager Manager = AuthenticationManagerFactory.GetManager())
                    return Ok(Manager.GetMatchmakingChallenge(SessionId, GameName, Address, Port));
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost("validatematchmakingchallengeresult")]
        public ActionResult<ApiResponse?> ValidateMatchmakingChallengeResult(MatchmakingChallengeResult Result)
        {
            try
            {
                using (IAuthenticationManager Manager = AuthenticationManagerFactory.GetManager())
                    return Ok(Manager.ValidateMatchmakingChallengeResult(Result));
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
