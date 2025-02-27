﻿using System.Security.Claims;
using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.MvcApp.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

namespace AspNetArticle.MvcApp.Controllers;

public class AccountController : Controller
{
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IFavouriteArticleService _favouriteArticleService;
    private readonly IConfiguration _configuration;

    public AccountController(IMapper mapper,
        IUserService userService,
        IRoleService roleService,
        IConfiguration configuration,
        IFavouriteArticleService favouriteArticleService)
    {
        _mapper = mapper;
        _userService = userService;
        _roleService = roleService;
        _configuration = configuration;
        _favouriteArticleService = favouriteArticleService;
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
        catch (Exception e)
        {
            Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
            throw new Exception($"Method {nameof(Login)} is failed, stack trace {e.StackTrace}. {e.Message}");
        }
    }

    [HttpGet]
    public IActionResult Registration()
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
        catch (Exception e)
        {
            Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
            throw new Exception($"Method {nameof(Registration)} is failed, stack trace {e.StackTrace}. {e.Message}");
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
        catch (Exception e)
        {
            Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
            throw new Exception($"Method {nameof(CheckEmailRegistrationAccount)} is failed, stack trace {e.StackTrace}. {e.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CheckUsername(string username)
    {
        try
        {
            if (!string.IsNullOrEmpty(username))
            {
                var email = User.Identity?.Name;

                if(email != null)
                {
                    var currentUsername = (await _userService.GetUserByEmailAsync(email))?.UserName;

                    bool isSameUser = currentUsername != null 
                        && currentUsername.Equals(username);

                    if (isSameUser)
                        return Ok(true);
                }
               
                bool isUsernameExist = await _userService.IsExistUsernameAsync(username);

                if (isUsernameExist)
                    return Ok(false);
            }
            return Ok(true);
        }
        catch (Exception e)
        {
            Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
            throw new Exception($"Method {nameof(CheckUsername)} is failed, stack trace {e.StackTrace}. {e.Message}");
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
        catch (Exception e)
        {
            Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
            throw new Exception($"Method {nameof(Edit)} is failed, stack trace {e.StackTrace}. {e.Message}");
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Edit(UserEditViewModel model) 
    {
        try
        {
            if (ModelState.IsValid)
            {
                var dto = _mapper.Map<UserDto>(model);

                if (!string.IsNullOrEmpty(model.NewPassword) && !string.IsNullOrEmpty(model.OldPassword))
                {
                    var isCorrectPassword = await _userService.CheckUserByEmailAndPasswordAsync(model.Email, model.OldPassword);

                    if (!isCorrectPassword)
                        return View(model);

                    dto.Password = model.NewPassword;
                }

                if (dto != null)
                {
                    var result = await _userService.UpdateUserAsync(model.Id, dto);

                    return result > 0
                        ? RedirectToAction("Index", "Home")
                        : BadRequest();
                }
            }
            return View(model);
        }
        catch (Exception e)
        {
            Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
            throw new Exception($"Method {nameof(Edit)} is failed, stack trace {e.StackTrace}. {e.Message}");
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> FavouriteArticles(int page = 1)
    {
        try
        {
            int pageSize = 5;

            var userEmail = User.Identity?.Name;
            if (userEmail == null)
                throw new ArgumentNullException();

            var user = await _userService.GetUserByEmailAsync(userEmail);
            if (user == null)
                throw new ArgumentNullException();

            var favouriteArticles = (await _favouriteArticleService.GetAllFavouriteArticles(user.Id))
                .Select(art => _mapper.Map<ArticleModel>(art));

            int countArticles = favouriteArticles.Count();

            var items = favouriteArticles.Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new FavouriteArticlesViewModel()
            {
                Articles = items,
                PageViewModel = new PageViewModel(countArticles, page, pageSize)
            };

            return View(model);
        }
        catch (Exception e)
        {
            Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
            throw new Exception($"Method {nameof(FavouriteArticles)} is failed, stack trace {e.StackTrace}. {e.Message}");
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddFavourite([FromBody]FavouriteArticleModel favouriteArticle) // FromForm
    {
        try
        {
            if(favouriteArticle != null)
            {
                var userEmail = User.Identity?.Name;
                if (userEmail == null)
                    throw new ArgumentNullException();

                var userId = (await _userService.GetUserByEmailAsync(userEmail)).Id;

                if (userId != Guid.Empty)
                {
                    if (favouriteArticle.Answer) 
                    {
                        var favouriteArticleDto = new FavouriteArticleDto()
                        {
                            Id = Guid.NewGuid(),
                            UserId = userId,
                            ArticleId = favouriteArticle.ArticleId,
                        };
                        await _favouriteArticleService.CreateFavouriteArticle(favouriteArticleDto);

                        return Ok();
                    }
                    else 
                    {
                        var favouriteArticleDto = new FavouriteArticleDto()
                        {                           
                            UserId = userId,
                            ArticleId = favouriteArticle.ArticleId,
                        };
                        await _favouriteArticleService.RemoveFavouriteArticle(favouriteArticleDto);

                        return Ok();
                    }             
                }
            }
            return BadRequest();
        }
        catch (Exception e)
        {
            Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
            throw new Exception($"Method {nameof(AddFavourite)} is failed, stack trace {e.StackTrace}. {e.Message}");
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<string> CheckFavourite([FromBody] FavouriteArticleModel favouriteArticle) // User.Identities.Any(identity => identity.IsAuthenticated)
    {
        try
        {
            var userEmail = User.Identity?.Name;
            if (userEmail == null)
                throw new ArgumentNullException("User is not authorized", userEmail);

            var userId = (await _userService.GetUserByEmailAsync(userEmail)).Id;

            bool isExist = await _favouriteArticleService.CheckFavouriteArticle(userId, favouriteArticle.ArticleId);
            var dictionary = new Dictionary<string, bool>() { { "exist", isExist } };
            var json = JsonConvert.SerializeObject(dictionary);

            return json;
        }
        catch (Exception e)
        {
            Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
            throw new Exception($"Method {nameof(CheckFavourite)} is failed, stack trace {e.StackTrace}. {e.Message}");
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
 