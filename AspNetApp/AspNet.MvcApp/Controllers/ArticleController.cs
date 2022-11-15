using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.MvcApp.Models;
using AspNetArticle.MvcApp.Models.ArticleModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetArticle.MvcApp.Controllers
{
    
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICommentaryService _commentaryService;
        private readonly IMapper _mapper;

        public ArticleController(IMapper mapper, 
            IArticleService articleService, 
            ICommentaryService commentaryService)
        {
            _mapper = mapper;
            _articleService = articleService;
            _commentaryService = commentaryService;
        }

        [HttpGet]
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


        [HttpGet]
        public async Task<IActionResult> Details(Guid id, CommentaryModel? model) // todo Кривой до невозможности метод
        {
            if (model != null && !string.IsNullOrEmpty(model.Description))
            {
                var article = await _articleService.GetArticleByIdAsync(model.ArticleId);

                if (article != null)
                {
                    var articleWithUsersComments = new ArticleWIthCommentaryViewModel
                    {
                        Article = article,
                        ExistComments = await _commentaryService.GetAllCommentsWithUsersByArticleIdAsync(model.ArticleId),
                        Comment = model

                    };
                    return View(articleWithUsersComments); 
                }
            }
            else
            {
                var article = await _articleService.GetArticleByIdAsync(id);

                if (article != null)
                {
                    var articleWithUsersComments = new ArticleWIthCommentaryViewModel
                    {
                        Article = article,
                        ExistComments = await _commentaryService.GetAllCommentsWithUsersByArticleIdAsync(id),
                        Comment = new CommentaryModel() { ArticleId = id }
                        
                    };
                    return View(articleWithUsersComments);
                }
            }
            return View();
        }
        //[HttpGet]
        //public async Task<IActionResult> Details(CommentaryModel model)
        //{
        //    var article = await _articleService.GetArticleByIdAsync(model.ArticleId);

        //    if (article != null)
        //    {
        //        var articleWithUsersComments = new ArticleWIthCommentaryViewModel
        //        {
        //            Article = article,
        //            ExistComments = await _commentaryService.GetAllCommentsWithUsersByArticleIdAsync(model.ArticleId),
        //            Comment = model

        //        };

        //        return View(articleWithUsersComments); todo trouble multiple endpoints
        //    }

        //    return View();
        //}
        //[HttpGet]
        //public async Task<IActionResult> Search()
        //{

        //}
    }
}
