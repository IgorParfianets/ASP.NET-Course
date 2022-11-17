using AspNetArticle.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace AspNetArticle.MvcApp.Controllers
{
    public class ManagerAccountController : Controller
    {
        private readonly IUserService _userService;

        public ManagerAccountController(IUserService userService)
        {
            _userService = userService;
        }



        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
