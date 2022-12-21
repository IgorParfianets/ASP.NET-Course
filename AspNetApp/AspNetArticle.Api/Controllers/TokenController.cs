using AspNetArticle.Api.Models.Request;
using AspNetArticle.Api.Models.Response;
using AspNetArticle.Api.Utils;
using AspNetArticle.Core.Abstractions;
using AspNetSample.WebAPI.Models.Requests;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AspNetArticle.Api.Controllers
{
    /// <summary>
    /// Token resource controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtUtil _jwtUtil;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="jwtUtil"></param>
        public TokenController(IUserService userService, IJwtUtil jwtUtil)
        {
            _userService = userService;
            _jwtUtil = jwtUtil;
        }

        /// <summary>
        /// Create token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateJwtToken([FromBody] AuthenticateRequestModel model) 
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(model.Email);

                if (user == null)
                {
                    Log.Warning($"User with email {model.Email} not found");
                    return BadRequest();
                }

                var isPassCorrect = await _userService.CheckUserByEmailAndPasswordAsync(model.Email, model.Password);

                if (!isPassCorrect)
                {
                    Log.Information($"User entered invalid password");
                    return BadRequest();
                }

                var response = await _jwtUtil.GenerateTokenAsync(user);

                if(response == null)
                {
                    Log.Warning($"TokenResponse failed");
                    return StatusCode(228); // something other case
                }
                Log.Information($"User logged in succesfully");
                return Ok(response);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Refresh token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("Refresh")]
        [HttpPost]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestModel request)
        {
            try
            {
                var user = await _userService.GetUserByRefreshTokenAsync(request.RefreshToken);

                if (user == null)
                {
                    Log.Warning($"User with refresh token {request.RefreshToken} not found");
                    return BadRequest();
                }

                var response = await _jwtUtil.GenerateTokenAsync(user);

                if (response == null)
                {
                    Log.Warning($"TokenResponse failed");
                    return StatusCode(228); // something other case
                }

                await _jwtUtil.RemoveRefreshTokenAsync(request.RefreshToken);

                Log.Information("Token updated succesfully");
                return Ok(response);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return StatusCode(500);
            }
        }


        /// <summary>
        /// Revoke token
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("Revoke")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequestModel request)
        {
            try
            {
                await _jwtUtil.RemoveRefreshTokenAsync(request.RefreshToken);

                Log.Information("Token removed succesfully");
                return Ok();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return StatusCode(500);
            }
        }
    }
}
