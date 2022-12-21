using AspNetArticle.Core;
using AspNetArticle.Core.Abstractions;
using AspNetArticle.MvcApp.Models;
using AutoMapper;
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
        public async Task<IActionResult> Index(string selectedCategory, Raiting selectedRaiting, string searchString, int page = 1)
        {
            try
            {
                int pageSize = 5;

                var searchingArticles =
                    (await _articleService.GetFilteredArticles(selectedCategory, selectedRaiting, searchString))
                    .Where(art => art.PublicationDate.AddDays(7) >= DateTime.UtcNow)
                    .Select(dto => _mapper.Map<ArticleModel>(dto))
                    .ToArray();

                var existCategories = (await _articleService.GetArticlesCategoryAsync())
                    .ToList();

                var model = new ArticlesCategoryViewModel();

                if (searchingArticles != null)
                {
                    int countArticles = searchingArticles.Count();

                    var items = searchingArticles.Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    model.Articles = items;
                    model.Categories = new SelectList(existCategories);

                    model.Raiting = new SelectList(new List<SelectListItem>()
                    {
                       new SelectListItem()
                       {
                           Text =  "Все",
                           Value = Raiting.None.ToString()
                       },
                       new SelectListItem()
                       {
                           Text =  "Только хорошие",
                           Value = Raiting.TopRaiting.ToString()
                       },
                       new SelectListItem()
                       {
                           Text = "Только плохие",
                           Value = Raiting.LowRaiting.ToString()
                       }
                    }, "Value", "Text");

                    model.SelectedCategory = selectedCategory;
                    model.SelectedRaiting = selectedRaiting;
                    model.PageViewModel = new PageViewModel(countArticles, page, pageSize);
                }
                return View(model);
            }
            catch (Exception e)
            {
                Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
                throw new Exception($"Method {nameof(Index)} is failed, stack trace {e.StackTrace}. {e.Message}");
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
            catch (Exception e)
            {
                Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
                throw new Exception($"Method {nameof(Details)} is failed, stack trace {e.StackTrace}. {e.Message}");
            }
        }
    }
}
