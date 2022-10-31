using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.MvcApp.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetArticle.MvcApp.Controllers
{
    [Authorize(Roles = "User")] //todo remove after
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
            var articles = await _articleService.GetAllArticlesAsync();

            if(articles != null)
                return View(articles);

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid id)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ArticleModel article)
        {
            if (ModelState.IsValid)
            {
                var articleDto = _mapper.Map<ArticleDto>(article);
                var result = await _articleService.CreateArticleAsync(articleDto);
                if(result > 0)
                    return RedirectToAction("Index");

                return NotFound(); //todo переделать
            }
            return View(article);
        } 
    }
}
