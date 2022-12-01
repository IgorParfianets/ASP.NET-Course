using AspNetArticle.Core.Abstractions;
using AspNetArticle.Database.Entities;
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
        public async Task<IActionResult> Index(string selectedCategory, string searchString, int page = 1)
        {
            try
            {
                int pageSize = 5;

                var searchingArticles =
                    (await _articleService.GetArticlesByCategoryAndSearchStringAsync(selectedCategory, searchString))
                    .Select(dto => _mapper.Map<ArticleModel>(dto))
                    .OrderByDescending(art => art.Rate)
                    .ToList();

                var existCategories = (await _articleService.GetArticlesCategoryAsync())
                    .ToList();

                var model = new ArticlesCategoryViewModel();

                if (searchingArticles != null)
                {
                    int countArticles = searchingArticles.Count();
                    var items = searchingArticles.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                    model.Articles = items;
                    model.Categories = new SelectList(existCategories);
                    model.SelectedCategory = selectedCategory;
                    model.PageViewModel = new PageViewModel(countArticles, page, pageSize);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(Index)} with arguments {selectedCategory}, {searchString} method failed");
                return BadRequest();
            }
        }


        [HttpGet]
        public async Task<IActionResult> Details(Guid id, CreateCommentModel? model) 
        {
            try
            {
                if (model != null && !string.IsNullOrEmpty(model.Description))
                {
                    var articleModel = _mapper.Map<ArticleModel>(await _articleService.GetArticleByIdAsync(model.ArticleId));

                    if (articleModel != null)
                    {
                        var articleWithUsersComments = new ArticleDetailViewModel
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
                        var articleWithUsersComments = new ArticleDetailViewModel
                        {
                            Article = articleModel,
                            ExistComments = await _commentaryService.GetAllCommentsWithUsersByArticleIdAsync(id),
                            Comment = new CreateCommentModel() { ArticleId = id }
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
