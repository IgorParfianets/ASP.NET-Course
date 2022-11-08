using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace AspNetArticle.MvcApp.Models.UserModels;

public class UserEditViewModel
{
    public Guid Id { get; set; }

    public string? Avatar { get; set; }
    [Required(ErrorMessage = "Input your name")]
    [MaxLength(12, ErrorMessage = "You entered long name")]
    [MinLength(2, ErrorMessage = "You entered too short name")]
    //[Remote("CheckUserName",
    //    "Account",
    //    HttpMethod = WebRequestMethods.Http.Post,
    //    ErrorMessage = "Username is already exists")]
    public string UserName { get; set; } //todo Need to come up with own Username

    public string Email { get; set; }
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Compare("NewPassword",
        ErrorMessage = "Passwords mismatch")]
    [DataType(DataType.Password)]
    public string ConfirmNewPassword { get; set; }
    public Guid RoleId { get; set; }
    public bool Spam { get; set; }
    public string? AboutSelf { get; set; }
}