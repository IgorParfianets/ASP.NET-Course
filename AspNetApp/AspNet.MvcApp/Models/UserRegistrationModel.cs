using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace AspNetArticle.MvcApp.Models;

public class UserRegistrationModel
{
    [Required(ErrorMessage = "Input your name")]
    [MaxLength(12, ErrorMessage = "You entered long name")]
    [MinLength(2, ErrorMessage = "You entered too short name")]
    [Remote("CheckUserName",
        "Account",
        HttpMethod = WebRequestMethods.Http.Post,
        ErrorMessage = "Username is already exists")]
    public string UserName { get; set; }

    [EmailAddress]
    [Remote("CheckEmail",
        "Account",
        HttpMethod = WebRequestMethods.Http.Post,
        ErrorMessage = "Email is already exists")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Input your password")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Compare("Password",
        ErrorMessage = "Passwords mismatch")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }

    public bool Spam { get; set; }
}
