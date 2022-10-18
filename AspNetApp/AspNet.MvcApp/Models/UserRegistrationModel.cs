using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace AspNetArticle.MvcApp.Models;

public class UserRegistrationModel
{
    [Required(ErrorMessage = "Input your name")]
    [MaxLength(12, ErrorMessage = "You inputed too long name")]
    [MinLength(2, ErrorMessage = "You inputed too short name")]
    public string UserName { get; set; }

    [EmailAddress]
    [Remote("CheckEmail",
        "Account",
        HttpMethod = WebRequestMethods.Http.Post,
        ErrorMessage = "Email is already exists")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Input your email")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Compare("Password",
        ErrorMessage = "Passwords mismatch")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }

    public bool Spam { get; set; }
}
