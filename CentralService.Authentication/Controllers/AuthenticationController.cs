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
        [HttpPost("getuserchallenge")]
        public ActionResult<UserChallenge> GetChallenge(Session Session)
        {
            try
            {
                using (IAuthenticationManager Manager = AuthenticationManagerFactory.GetManager())
                    return Ok(Manager.GetUserChallenge(Session));
            }
            catch (Exception Ex)
            {
                if (Ex is ArgumentException || Ex is ArgumentNullException)
                    return BadRequest();
                return StatusCode(500);
            }
        }

        [HttpPost("validateuserchallengeresponse")]
        public ActionResult<UserChallengeProof> ValidateChallengeResponse(UserChallengeResult Result)
        {
            try
            {
                using (IAuthenticationManager Manager = AuthenticationManagerFactory.GetManager())
                    return Ok(Manager.ValidateUserChallengeResult(Result));
            }
            catch (System.Security.Authentication.AuthenticationException)
            {
                return Unauthorized(new ApiError(266, "There was an error validating the pre-authentication.", true));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception Ex)
            {
                if (Ex is ArgumentException || Ex is ArgumentNullException)
                    return BadRequest();
                return StatusCode(500);
            }
        }

        [HttpGet("getmatchmakingchallenge")]
        public ActionResult<MatchmakingChallenge> GetMatchmakingChallenge(uint SessionId, string GameName, string Address, string Port)
        {
            try
            {
                using (IAuthenticationManager Manager = AuthenticationManagerFactory.GetManager())
                    return Ok(Manager.GetMatchmakingChallenge(SessionId, GameName, Address, Port));
            }
            catch (Exception Ex)
            {
                if (Ex is ArgumentException || Ex is ArgumentNullException)
                    return BadRequest();
                return StatusCode(500);
            }
        }

        [HttpPost("validatematchmakingchallengeresult")]
        public ActionResult<MatchmakingChallengeProof> ValidateMatchmakingChallengeResult(MatchmakingChallengeResult Result)
        {
            try
            {
                using (IAuthenticationManager Manager = AuthenticationManagerFactory.GetManager())
                    return Ok(Manager.ValidateMatchmakingChallengeResult(Result));
            }
            catch (Exception Ex)
            {
                if (Ex is ArgumentException || Ex is ArgumentNullException)
                    return BadRequest();
                return BadRequest();
            }
        }
    }
}
