using AspNetArticle.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace AspNetArticle.MvcApp.Controllers
{
    public class ManageController : Controller
    {
        private readonly IUserService _userService;
    }
}
