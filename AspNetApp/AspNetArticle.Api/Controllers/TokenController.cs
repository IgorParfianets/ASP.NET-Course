using AspNetArticle.Api.Models.Request;
using AspNetArticle.Api.Models.Response;
using AspNetArticle.Api.Utils;
using AspNetArticle.Core.Abstractions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AspNetArticle.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IJwtUtil _jwtUtil;


        public TokenController(IUserService userService, 
            IMapper mapper, 
            IJwtUtil jwtUtil)
        {
            _userService = userService;
            _mapper = mapper;
            _jwtUtil = jwtUtil;
        }
        /// <summary>
        /// Create token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Nullable), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateJwtToken([FromBody] AuthenticateRequestModel model)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    return BadRequest();
                }
                var isPassCorrect = await _userService.CheckUserByEmailAndPasswordAsync(model.Email, model.Password);

                if (!isPassCorrect)
                {
                    return BadRequest();
                }

                var response = _jwtUtil.GenerateToken(user);
                return Ok(response);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return StatusCode(500);
            }
        }
    }
}
