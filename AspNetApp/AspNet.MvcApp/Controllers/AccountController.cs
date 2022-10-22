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

namespace AspNetArticle.MvcApp.Controllers;

public class AccountController : Controller
{
    // какой то сервис
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IConfiguration _configuration;

    public AccountController(IMapper mapper, IUserService userService, IRoleService roleService)
    {
        _mapper = mapper;
        _userService = userService;
        _roleService = roleService;
    }

    //------------------------------------------  Login
    [HttpGet]
    public async Task<IActionResult> Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(UserLoginModel loginModel)
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
        catch (Exception)
        {
            return NotFound();
        }
        
    }
    //------------------------------------------  Registration
    [HttpGet]
    public async Task<IActionResult> Registration()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registration(UserRegistrationModel user)
    {
        try
        {  
            if (ModelState.IsValid)
            {
                var userRoleId = await _roleService.GetRoleIdByNameAsync(_configuration["DefaultRole"]);
                var userDto = _mapper.Map<UserDto>(user);

                if (userRoleId != Guid.Empty && userDto != null)
                {
                    userDto.RoleId = userRoleId;
                    var result = await _userService.RegisterUser(userDto);

                    if (result > 0)
                    {
                        await Authenticate(user.Email);
                        return RedirectToAction("Index", "Home"); // Redirect на Main для Registered User
                    }
                }
            }
            return View(user);
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }
    //------------------------------------------  Edit
    [HttpPost]
    public async Task<IActionResult> Edit(UserEditModel user)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var userDto = _mapper.Map<UserDto>(user);
                
                if (userDto != null)
                {
                    await _userService.UpdateUser(user.Id, userDto);

                    return RedirectToAction("Index", "Home");
                }
            }
            return View(user);
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        if (id != Guid.Empty)
        {
            var user = await _userService.GetUser(id);

            if (user == null)
                return BadRequest();

            var userEdit = _mapper.Map<UserEditModel>(user);
            return View(userEdit);
        }
        return View();
    }
    //------------------------------------------  Logout
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Home");


    }
    //------------------------------------------  [Remote] Check UserName and Email при Registration

    [HttpPost]
    public async Task<IActionResult> CheckEmail(string email)   
    {
        if (!string.IsNullOrEmpty(email))
        {
            var isExistEmail = await _userService.IsExistUserEmailAsync(email);

            if(isExistEmail)
                 return Ok(false);
        }
        return Ok(true);
    }

    [HttpPost]
    public async Task<IActionResult> CheckUserName(string username)   
    {
        if (!string.IsNullOrEmpty(username))
        {
            var isExistName = await _userService.IsExistUserNameAsync(username);

            if (isExistName)
                return Ok(false);
        }
        return Ok(true);
    }
    //------------------------------------------  Authenticate
    private async Task Authenticate(string email)
    {
        var userDto = await _userService.GetUserByEmailAsync(email);

        var claims = new List<Claim>()
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, userDto.Email),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, userDto.RoleName)
        };

        var identity = new ClaimsIdentity(claims,
            "ApplicationCookie",
            ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType
        );

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetUserData()
    {
        var userEmail = User.Identity?.Name;

        if (string.IsNullOrEmpty(userEmail))
        {
            return BadRequest();
        }

        var user = _mapper.Map<UserDataModel>(await _userService.GetUserByEmailAsync(userEmail));
        return Ok(user);
    }
}
 