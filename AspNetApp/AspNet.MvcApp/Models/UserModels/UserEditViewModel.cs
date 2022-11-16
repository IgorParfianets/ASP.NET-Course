using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace AspNetArticle.MvcApp.Models.UserModels;

public class UserEditViewModel
{
    public Guid Id { get; set; }

    public string? Avatar { get; set; }
    [Required(ErrorMessage = "Введите имя")]
    [MaxLength(12, ErrorMessage = "Слишком длинное имя. Не более 12 символов")]
    [MinLength(2, ErrorMessage = "Слишком короткое имя. Не менее 2 символов")]
    [Remote("CheckUserNameEditAccount",
        "Account",
        HttpMethod = WebRequestMethods.Http.Post,
        ErrorMessage = "Это имя уже занято")]
    public string UserName { get; set; } //todo Need to come up with own Username

    public string Email { get; set; }
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }

    [Compare("NewPassword",
        ErrorMessage = "Пароли не совпадают")]
    [DataType(DataType.Password)]
    public string ConfirmNewPassword { get; set; }
    public Guid RoleId { get; set; }
    public bool Spam { get; set; }
    public string? AboutSelf { get; set; }
}