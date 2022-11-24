using System.Security.Claims;
using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.MvcApp.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Serilog;

namespace AspNetArticle.MvcApp.Controllers;

public class AccountController : Controller
{
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IConfiguration _configuration;

    public AccountController(IMapper mapper, 
        IUserService userService, 
        IRoleService roleService, 
        IConfiguration configuration)
    {
        _mapper = mapper;
        _userService = userService;
        _roleService = roleService;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(UserLoginViewModel loginModel)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var isCorrectPassword = await _userService.CheckUserByEmailAndPasswordAsync(loginModel.Email, loginModel.Password);
                
                if (isCorrectPassword)
                {
                    await Authenticate(loginModel.Email);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(loginModel);
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"{nameof(Login)} method failed");
            return BadRequest();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Registration()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registration(UserRegistrationViewModel user)
    {
        try
        {  
            if (ModelState.IsValid)
            {
                var userRoleId = await _roleService.GetRoleIdByNameAsync(_configuration["DefaultRole"]);
                var userDto = _mapper.Map<UserDto>(user);

                if (userRoleId != null && userDto != null)
                {
                    userDto.RoleId = userRoleId.Value;
                    var result = await _userService.RegisterUserAsync(userDto, user.Password);

                    if (result > 0)
                    {
                        await Authenticate(user.Email);
                        return RedirectToAction("Index", "Home"); 
                    }
                }
            }
            return View(user);
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"{nameof(Registration)} method failed");
            return BadRequest();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> CheckEmailRegistrationAccount(string email)   
    {
        try
        {
            if (!string.IsNullOrEmpty(email))
            {
                var isExistEmail = await _userService.IsExistUserEmailAsync(email);

                if (isExistEmail)
                    return Ok(false);
            }
            return Ok(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"{nameof(CheckEmailRegistrationAccount)} method failed");
            return BadRequest();
        }
    }

    [HttpPost]
    public async Task<IActionResult> CheckUserNameRegistrationAccount(string username)   
    {
        try
        {
            if (!string.IsNullOrEmpty(username))
            {
                var isExistName = await _userService.IsExistUserNameAsync(username);

                if (isExistName)
                    return Ok(false);
            }
            return Ok(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"{nameof(CheckUserNameRegistrationAccount)} with username {username} method failed");
            return BadRequest();
        }
    }

    [HttpPost]
    public async Task<IActionResult> CheckUserNameEditAccount(string username)
    {
        try
        {
            if (!string.IsNullOrEmpty(username))
            {
                var email = User.Identity.Name;
                var user = await _userService.GetUserByEmailAsync(email);

                if (user.UserName.Equals(username))
                    return Ok(false);
            }
            return Ok(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"{nameof(CheckUserNameEditAccount)} with username {username} method failed");
            return BadRequest();
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Edit()  
    {
        try
        {
            var userEmail = User.Identity?.Name;

            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest();
            }

            var user = _mapper.Map<UserEditViewModel>(await _userService.GetUserByEmailAsync(userEmail));

            return View(user);
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"{nameof(Edit)} method failed");
            return BadRequest();
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Edit(UserEditViewModel model) 
    {
        try
        {
            var userEmail = User.Identity?.Name;

            if (ModelState.IsValid && userEmail != null)
            {
                var dto = _mapper.Map<UserDto>(model);
                dto.Email = userEmail;
                //dto.RoleName = await _roleService.GetRoleNameByIdAsync(model.RoleName);

                if (dto != null)
                {
                    var result = await _userService.UpdateUserAsync(model.Id, dto);

                    return result > 0
                        ? RedirectToAction("Index", "Home")
                        : BadRequest();
                }
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"{nameof(Edit)} method with model {model} failed");
            return BadRequest();
        }
    }

    private async Task Authenticate(string email)
    {
        var userDto = await _userService.GetUserByEmailAsync(email);

        var claims = new List<Claim>()
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, userDto.Email),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, userDto.RoleName),
            new Claim(ClaimTypes.Actor, userDto.UserName)

        };

        var identity = new ClaimsIdentity(claims,
            "ApplicationCookie",
            ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType
        );

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));
    }
}
 