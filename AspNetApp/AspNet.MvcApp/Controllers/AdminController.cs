using AspNetArticle.Core.Abstractions;
using AspNetArticle.Database.Entities;
using AspNetArticle.MvcApp.Models;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetArticle.MvcApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IArticleService _articleService;
        private readonly ISourceService _sourceService;
        private readonly ICommentaryService _commentaryService;
        private readonly IArticleRateService _articleRateService;
        private readonly IMapper _mapper;

        public AdminController(IUserService userService,
            IRoleService roleService,
            IArticleService articleService,
            ISourceService sourceService,
            ICommentaryService commentaryService,
            IMapper mapper, IArticleRateService articleRateService)
        {
            _userService = userService;
            _roleService = roleService;
            _articleService = articleService;
            _sourceService = sourceService;
            _commentaryService = commentaryService;
            _mapper = mapper;
            _articleRateService = articleRateService;
        }

        [HttpGet]
        public IActionResult PersonalArea()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await _userService.GetAllUsersAsync();

            if (users != null)
                return View(users);

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Articles()
        {
            var articles = await _articleService.GetAllArticlesAsync();

            if (articles != null)
                return View(articles);

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Comments()
        {
            var comments = await _commentaryService.GelAllCommentsAsync();

            if (comments != null)
                return View(comments);

            return View();
        }

        //[HttpGet]
        //public async Task<IActionResult> RemoveArticleToArchives(Guid articleId)
        //{
        //    await _articleService.RemoveArticleToArchiveByIdAsync(articleId);

        //}
        [HttpGet]
        public async Task<IActionResult> InitializedArticles()
        {

            RecurringJob.AddOrUpdate(() =>  _articleService.AggregateArticlesFromExternalSourcesAsync(),
                "*/5 * * * *");

            RecurringJob.AddOrUpdate(() => _articleService.AddArticlesDataAsync(),
                "*/7 * * * *");

            RecurringJob.AddOrUpdate(() => _articleRateService.AddRateToArticlesAsync(),
                "*/10 * * * *");

            return Ok();
        }
    }
}
