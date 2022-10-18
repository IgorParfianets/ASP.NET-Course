using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using AspNetArticle.MvcApp.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AspNetArticle.MvcApp.Controllers;

public class AccountController : Controller
{
    // какой то сервис
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public AccountController(IMapper mapper, IUserService userService)
    {
        _mapper = mapper;
        _userService = userService;
    }

    // Login
    [HttpGet]
    public async Task<IActionResult> Login(Guid id)
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
                string password = loginModel.Password;
                string email = loginModel.Email;

                var registedUser = await _userService.GetUserByEmailAndPassword(email, password);

                if (registedUser != null)
                    return RedirectToAction("Index", "Home");

                return View();
            }

            return View();
        }
        catch (Exception)
        {
            return NotFound();
        }
        
    }
     // Registration
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
                var userDto = _mapper.Map<UserDto>(user);
                await _userService.RegisterUser(userDto);

                return RedirectToAction("MyIndex", "Test"); // Redirect на Main для Registred User
            }
                return View(user);
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }
    //to do разобраться с Guid Edit
    [HttpPost]
    public async Task<IActionResult> Edit(UserRegistrationModel user)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var userDto = _mapper.Map<UserDto>(user);
                
                if(userDto != null)
                {
                    await _userService.UpdateUser(userDto);

                    return RedirectToAction("MyIndex", "Test");
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
    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id != null)
        {
            //var user = _userService.GetUser()
        }
            

        return View();
    }


    // Check User Email при Registration
    [HttpPost]
    public async Task<IActionResult> CheckEmail(string email)   // Проверка email в DB 
    {
        if (!string.IsNullOrEmpty(email))
        {
            var isExistEmail = await _userService.IsExistUserEmailAsync(email);

            if(isExistEmail)
                 return Ok(false);
        }
        return Ok(true);
    }
}
 