using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace AspNetArticle.MvcApp.Models;

public class UserRegistrationViewModel
{
    [Required(ErrorMessage = "Введите имя")]
    [MaxLength(12, ErrorMessage = "Слишком длинное имя. Не более 12 символов")]
    [MinLength(2, ErrorMessage = "Слишком короткое имя. Не менее 2 символов")]
    [Remote("CheckUserNameRegistrationAccount",
        "Account",
        HttpMethod = WebRequestMethods.Http.Post,
        ErrorMessage = "Пользователь с таким именем уже существует")]
    public string UserName { get; set; }

    [EmailAddress]
    [Remote("CheckEmailRegistrationAccount",
        "Account",
        HttpMethod = WebRequestMethods.Http.Post,
        ErrorMessage = "Пользователь с таким e-mail уже существует")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Введите пароль")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Compare("Password",
        ErrorMessage = "Пароли не совпадают")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }

    public bool Spam { get; set; }
}
