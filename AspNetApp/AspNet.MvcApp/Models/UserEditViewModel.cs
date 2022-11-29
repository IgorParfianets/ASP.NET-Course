using AspNetArticle.Core;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace AspNetArticle.MvcApp.Models;

public class UserEditViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Введите имя")]
    [MaxLength(12, ErrorMessage = "Слишком длинное имя. Не более 12 символов")]
    [MinLength(2, ErrorMessage = "Слишком короткое имя. Не менее 2 символов")]
    [Remote("CheckUsername",
        "Account",
        HttpMethod = WebRequestMethods.Http.Post,
        ErrorMessage = "Это имя уже занято")]
    public string UserName { get; set; } 

    public string Email { get; set; }
    [DataType(DataType.Password)]
    public string? OldPassword { get; set; }
    [DataType(DataType.Password)]
    public string? NewPassword { get; set; }

    [Compare("NewPassword",
         ErrorMessage = "Пароли не совпадают")]
    [DataType(DataType.Password)]
    public string? ConfirmNewPassword { get; set; }
    public bool Spam { get; set; }
    public MembershipStatus Status { get; set; }

    [DataType(DataType.Upload)]
    public IFormFile? SaveAvatar { get; set; }
    public string? LoadAvatar { get; set; }
}