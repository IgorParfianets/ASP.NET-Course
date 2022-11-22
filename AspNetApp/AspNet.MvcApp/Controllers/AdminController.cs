using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Database.Entities;
using AspNetArticle.MvcApp.Models;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AspNetArticle.MvcApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IArticleService _articleService;
        private readonly ICommentaryService _commentaryService;
        private readonly IArticleRateService _articleRateService;
        private readonly IMapper _mapper;
        public AdminController(IUserService userService,
            IArticleService articleService,
            ICommentaryService commentaryService,
            IArticleRateService articleRateService,
            IMapper mapper)
        {
            _userService = userService;
            _articleService = articleService;
            _commentaryService = commentaryService;
            _articleRateService = articleRateService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult PersonalArea()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            try
            {
                var users = (await _userService.GetAllUsersAsync())
                    .Select(dto => _mapper.Map<UserModel>(dto));

                List<AdminPageUserModel> listUsersForModel = new List<AdminPageUserModel>();

                foreach (var user in users)
                {
                    var userComments = (await _commentaryService.GetAllCommentsByUserIdAsync(user.Id)).ToList();

                    var userWithComment = new AdminPageUserModel()
                    {
                        User = user,
                        Comments = userComments
                    };
                    listUsersForModel.Add(userWithComment);
                }

                if (listUsersForModel != null)
                    return View(listUsersForModel);

                return View();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(Users)} method failed");
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> UserDetails(Guid id)
        {
            try
            {
                var user = _mapper.Map<UserModel>(await _userService.GetUserByIdAsync(id));
                var model = new AdminPageUserModel();

                if (user != null)
                {
                    var comments = (await _commentaryService.GetAllCommentsByUserIdAsync(id)).ToList();

                    model.User = user;
                    model.Comments = comments;

                    return View(model);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(UserDetails)} with Guid {id} method failed");
                return BadRequest();
            }
        }


        [HttpGet]
        public async Task<IActionResult> Articles()
        {
            try
            {
                var articles = await _articleService.GetAllArticlesAsync();

                if (articles != null)
                    return View(articles);

                return View();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(Articles)} method failed");
                return BadRequest();
            }
        }


        [HttpGet]
        public async Task<IActionResult> Comments()
        {
            try
            {
                var comments = await _commentaryService.GelAllCommentsAsync();

                if (comments != null)
                    return View(comments);

                return View();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"{nameof(Comments)} method failed");
                return BadRequest();
            }
        }

        //[HttpGet]
        //public async Task<IActionResult> RemoveArticleToArchives(Guid articleId)
        //{
        //    await _articleService.RemoveArticleToArchiveByIdAsync(articleId);

        //}

    }
}
