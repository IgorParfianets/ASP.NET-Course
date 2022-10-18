using AspNetArticle.Core.Abstractions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AspNetArticle.MvcApp.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IMapper _mapper;

        public ArticleController(IMapper mapper, IArticleService articleService)
        {
            _mapper = mapper;
            _articleService = articleService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
