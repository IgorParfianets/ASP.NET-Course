using System.Security.Claims;
using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.MvcApp.Models.UserModels;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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

    //------------------------------------------  Login
    [HttpGet]
    public async Task<IActionResult> Login()
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

    //------------------------------------------ Data for Authorize User

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Edit()  
    {
        var userEmail = User.Identity?.Name;

        if (string.IsNullOrEmpty(userEmail))
        {
            return BadRequest();
        }

        var user = _mapper.Map<UserEditViewModel>(await _userService.GetUserByEmailAsync(userEmail));
        //var user = _mapper.Map<UserEditModel>(await _userService.GetUserAsync(id));
        return View(user);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Edit(UserEditViewModel model) // todo not fully works need to create POST version
    {
        var userEmail = User.Identity?.Name;

        if (ModelState.IsValid && userEmail != null)
        {
            var dto = _mapper.Map<UserDto>(model);
            dto.Email = userEmail;
            dto.RoleName = await _roleService.GetRoleNameByIdAsync(model.RoleId);

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

    //[HttpGet]
    //public async Task<IActionResult> LoginLogoutUser()
    //{
    //    if (User.Identities.Any(identity => identity.IsAuthenticated))
    //    {
    //        var userEmail = User.Identity?.Name ;
    //        if (string.IsNullOrEmpty(userEmail))
    //        {
    //            return BadRequest();
    //        }

    //        var user = _mapper.Map<UserDisplayDataViewModel>(await _userService.GetUserByEmailAsync(userEmail));
    //        return View(user);
    //    }

    //    return View();
    //}

    // Удалить после теста

}
 