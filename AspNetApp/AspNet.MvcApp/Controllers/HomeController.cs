using AspNet.MvcApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AspNetArticle.Core.Abstractions;
using AspNetArticle.MvcApp.Models;
using AutoMapper;
using Serilog;
using Hangfire;

namespace AspNet.MvcApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IArticleService _articleService;
        private readonly IArticleRateService _articleRateService;
        private readonly ISendMessageService _sendMessageService;
        private readonly IMapper _mapper;
        public HomeController(ILogger<HomeController> logger,
            IArticleService articleService,
            IMapper mapper,
            IArticleRateService articleRateService,
            ISendMessageService sendMessageService)
        {
            _logger = logger;
            _articleService = articleService;
            _mapper = mapper;
            _articleRateService = articleRateService;
            _sendMessageService = sendMessageService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var articles = (await _articleService.GetAllArticlesAsync())
               .Where(art => art.PublicationDate.AddDays(7) >= DateTime.UtcNow)
               .OrderByDescending(art => art.Rate)
               .Take(3)
               .Select(art => _mapper.Map<ArticleModel>(art))
               .ToArray();

                return View(articles);
            }
            catch (Exception e)
            {
                Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
                throw new Exception($"Method {nameof(Index)} is failed, stack trace {e.StackTrace}. {e.Message}");
            }
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

        public IActionResult AboutProject()
        {
            return View();
        }
    }
}