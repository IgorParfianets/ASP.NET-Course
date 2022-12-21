using AspNetArticle.Api.Models.Request;
using AspNetArticle.Api.Models.Response;
using AspNetArticle.Api.Utils;
using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Database.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;

namespace AspNetArticle.Api.Controllers
{
    /// <summary>
    /// User resource controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IJwtUtil _jwtUtil;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="roleService"></param>
        /// <param name="mapper"></param>
        /// <param name="jwtUtil"></param>
        /// <param name="configuration"></param>
        public UserController(IUserService userService,
            IRoleService roleService,
            IMapper mapper,
            IJwtUtil jwtUtil,
            IConfiguration configuration)
        {
            _userService = userService;
            _roleService = roleService;
            _mapper = mapper;
            _jwtUtil = jwtUtil;
            _configuration = configuration;
        }

        /// <summary>
        /// Returns all users.
        /// Works only for Admin
        /// </summary>
        /// <returns>Enumerable of users</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();

                if (users == null)
                {
                    Log.Warning("Users not found in database");
                    return NotFound();
                }

                Log.Information("Users successfully received", users);
                return Ok(users);
            }
            catch (Exception e)
            {
                Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Return user by id.
        /// Works only for Admin
        /// </summary>
        /// <param name="id">User's id</param>
        /// <returns>User</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                {
                    Log.Warning($"User with id {id} not found");
                    return NotFound();
                }

                Log.Information("User successfully received", user);
                return Ok(user);
            }
            catch (Exception e)
            {
                Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Register user.
        /// </summary>
        /// <param name="model">Registration model</param>
        /// <returns>Token</returns>
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] RegisterUserRequestModel model)
        {
            try
            {
                var userWIthSameEmailExists = await _userService.IsExistUserEmailAsync(model.Email);

                if (!userWIthSameEmailExists)
                {
                    var userRoleId = await _roleService.GetRoleIdByNameAsync(_configuration["DefaultRole"]);
                    var dto = _mapper.Map<UserDto>(model);

                    if (userRoleId != null
                        && dto != null
                        && model.Password.Equals(model.PasswordConfirmation))
                    {
                        dto.RoleId = userRoleId.Value;
                        var result = await _userService.RegisterUserAsync(dto, model.Password);

                        if (result > 0)
                        {
                            var currentUser = await _userService.GetUserByEmailAsync(dto.Email);
                            var response = _jwtUtil.GenerateTokenAsync(currentUser);

                            Log.Information("User successfully created", model);
                            return StatusCode(201, response);
                        }
                        Log.Warning("The data entered by the user was not saved to the database.", model);
                        return BadRequest();
                    }
                    Log.Warning("Failed Request", userRoleId, model);
                    return BadRequest();
                }
                Log.Warning("Such user already exists", model);
                return BadRequest();
            }
            catch (Exception e)
            {
                Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Update user's data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPatch]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequestModel model)
        {
            try
            {
                var userEmail = User.Claims.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userEmail))
                {
                    Log.Warning("ClaimTypes.NameIdentifier is null");
                    return BadRequest();
                }

                var userId = (await _userService.GetUserByEmailAsync(userEmail)).Id;

                var dto = _mapper.Map<UserDto>(model);

                if (!string.IsNullOrEmpty(model.NewPassword) && !string.IsNullOrEmpty(model.OldPassword))
                {
                    var isCorrectPassword = await _userService.CheckUserByEmailAndPasswordAsync(model.Email, model.OldPassword);

                    if (!isCorrectPassword)
                    {
                        Log.Warning("User entered invalid password", model.OldPassword);
                        return BadRequest();
                    }                       
                    dto.Password = model.NewPassword;
                }

                if (dto != null)
                {
                    var result = await _userService.UpdateUserAsync(userId, dto);

                    if(result == 0)
                    {
                        Log.Warning("The data entered by the user was not saved to the database.", model.OldPassword, model.NewPassword);
                        return BadRequest();
                    }

                    Log.Information("User entered data successfully updated", model.NewPassword);
                    return Ok();
                }

                Log.Warning("Model failed to map", model);
                return BadRequest();
            }
            catch (Exception e)
            {
                Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
                return StatusCode(500);
            }
        }
    }
}
