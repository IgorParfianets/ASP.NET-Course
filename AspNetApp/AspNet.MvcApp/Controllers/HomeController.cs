using AspNet.MvcApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AspNetArticle.Core.Abstractions;
using AspNetArticle.MvcApp.Models;
using AutoMapper;

namespace AspNet.MvcApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IArticleService _articleService;
        private readonly IMapper _mapper;
        public HomeController(ILogger<HomeController> logger, 
            IArticleService articleService, 
            IMapper mapper)
        {
            _logger = logger;
            _articleService = articleService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var articles = (await _articleService.GetAllArticlesAsync())
                .OrderByDescending(art => art.Rate)
                .Take(3)
                .Select(art => _mapper.Map<ArticleModel>(art)).ToArray();
            
            return View(articles);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}