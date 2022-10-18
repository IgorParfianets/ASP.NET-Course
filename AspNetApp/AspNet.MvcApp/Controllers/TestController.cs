using AspNetArticle.Core.Abstractions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AspNetArticle.MvcApp.Controllers
{
    public class TestController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IMapper _mapper;
        public TestController(IArticleService articleService, IMapper mapper)
        {
            _articleService = articleService;
            _mapper = mapper;
        }

        public IActionResult MyIndex()
        {
            return View();
        }
    }
}
