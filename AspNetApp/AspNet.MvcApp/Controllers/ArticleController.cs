using AspNetArticle.Core.Abstractions;
using AspNetArticle.MvcApp.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog;

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
            var articles = (await _articleService.GetAllArticlesAsync())
                .Select(dto => _mapper.Map<ArticleModel>(dto));

            var existCategories = await _articleService.GetArticlesCategoryAsync();

            if (articles != null)
            {
                var articlesModel = new ArticlesCategoryViewModel()
                {
                    Articles = articles.ToList(),
                    Categories = new SelectList(existCategories.ToList())
                };
                return View(articlesModel);
            }
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Index(string articleCategory, string searchString)
        {
            try
            {
                var searchingArticles =
                    (await _articleService.GetArticlesByCategoryAndSearchStringAsync(articleCategory, searchString))
                    .Select(dto => _mapper.Map<ArticleModel>(dto));

                var existCategories = await _articleService.GetArticlesCategoryAsync();

                var model = new ArticlesCategoryViewModel();

                if (searchingArticles != null)
                {
                    model.Articles = searchingArticles.ToList();
                    model.Categories = new SelectList(existCategories.ToList());
                }
                return View(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(Index)} with arguments {articleCategory}, {searchString} method failed");
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var existCategories = await _articleService.GetArticlesCategoryAsync();

            return Ok(existCategories.ToList());
        }


        [HttpGet]
        public async Task<IActionResult> Details(Guid id, CommentaryModel? model) // todo Кривой до невозможности метод
        {
            try
            {
                if (model != null && !string.IsNullOrEmpty(model.Description))
                {
                    var articleModel = _mapper.Map<ArticleModel>(await _articleService.GetArticleByIdAsync(model.ArticleId));

                    if (articleModel != null)
                    {
                        var articleWithUsersComments = new ArticleWIthCommentaryViewModel
                        {
                            Article = articleModel,
                            ExistComments = await _commentaryService.GetAllCommentsWithUsersByArticleIdAsync(model.ArticleId),
                            Comment = model
                        };
                        return View(articleWithUsersComments);
                    }
                }
                else
                {
                    var articleModel = _mapper.Map<ArticleModel>(await _articleService.GetArticleByIdAsync(id));

                    if (articleModel != null)
                    {
                        var articleWithUsersComments = new ArticleWIthCommentaryViewModel
                        {
                            Article = articleModel,
                            ExistComments = await _commentaryService.GetAllCommentsWithUsersByArticleIdAsync(id),
                            Comment = new CommentaryModel() { ArticleId = id }
                        };
                        return View(articleWithUsersComments);
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(Details)} with arguments {model} and Guid {id} method failed");
                return BadRequest();
            }
        }

        //[Authorize]
        //[HttpPost]
        //public async Task<IActionResult> Search(SearchModel model)
        //{
        //    if (!string.IsNullOrEmpty(model.Text))
        //    {
        //        var articles = await _articleService.GetArticlesBySearchStringAsync(model.Text);

        //        return RedirectToAction("Index", articles);
        //    }
            
        //    return View();
        //}



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
       
    }
}
