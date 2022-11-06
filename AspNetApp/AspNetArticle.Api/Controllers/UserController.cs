using AspNetArticle.Api.Models.Request;
using AspNetArticle.Api.Models.Response;
using AspNetArticle.Api.Utils;
using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AspNetArticle.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IJwtUtil _jwtUtil;
        private readonly IMapper _mapper;

        public UserController(IUserService userService,
            IRoleService roleService,
            IMapper mapper,
            IJwtUtil jwtUtil)
        {
            _userService = userService;
            _roleService = roleService;
            _mapper = mapper;
            _jwtUtil = jwtUtil;
        }

        /// <summary>
        /// Returns all users.
        /// Works only for authorized users
        /// </summary>
        /// <returns>List of users</returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get() //todo have the problem because no accept [Authorize]
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Register user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Nullable), StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RegisterUserRequestModel model)
        {
            try
            {
                var userRoleId = await _roleService.GetRoleIdByNameAsync("User");
                var dto = _mapper.Map<UserDto>(model);
                var userWIthSameEmailExists = await _userService.IsExistUserEmailAsync(model.Email);
                if (userRoleId != null
                    && dto != null
                    && !userWIthSameEmailExists
                    && model.Password.Equals(model.PasswordConfirmation))
                {
                    dto.RoleId = userRoleId.Value;
                    var result = await _userService.RegisterUserAsync(dto);

                    if (result > 0)
                    {
                        var currentUser = await _userService.GetUserByEmailAsync(dto.Email);
                        var response = _jwtUtil.GenerateToken(currentUser);

                        return Ok(response);
                    }
                }
                return BadRequest();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return StatusCode(500);
            }

        }
    }
}
