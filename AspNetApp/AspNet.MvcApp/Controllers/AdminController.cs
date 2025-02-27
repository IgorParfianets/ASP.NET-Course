﻿using AspNetArticle.Core.Abstractions;
using AspNetArticle.MvcApp.Models;
using AutoMapper;
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
        private readonly ISendMessageService _sendMessageService;
        private readonly IMapper _mapper;

        public AdminController(IUserService userService,
            IArticleService articleService,
            ICommentaryService commentaryService,
            IArticleRateService articleRateService,
            IMapper mapper,
            ISendMessageService sendMessageService)
        {
            _userService = userService;
            _articleService = articleService;
            _commentaryService = commentaryService;
            _articleRateService = articleRateService;
            _mapper = mapper;
            _sendMessageService = sendMessageService;
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            try
            {
                var users = (await _userService.GetAllUsersAsync())
                    .Select(dto => _mapper.Map<UserModel>(dto));

                List<AdminPageUserViewModel> listUsersForModel = new List<AdminPageUserViewModel>();

                foreach (var user in users)
                {
                    var userComments = (await _commentaryService.GetAllCommentsByUserIdAsync(user.Id)).ToList();

                    var userWithComment = new AdminPageUserViewModel()
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
            catch (Exception e)
            {
                Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
                throw new Exception($"Method {nameof(Users)} is failed, stack trace {e.StackTrace}. {e.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> UserDetails(Guid id)
        {
            try
            {
                var user = _mapper.Map<UserModel>(await _userService.GetUserByIdAsync(id));
                var model = new AdminPageUserViewModel();

                if (user != null)
                {
                    var comments = (await _commentaryService.GetAllCommentsByUserIdAsync(id)).ToList();

                    model.User = user;
                    model.Comments = comments;

                    return View(model);
                }

                return View(model);
            }
            catch (Exception e)
            {
                Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
                throw new Exception($"Method {nameof(UserDetails)} is failed, stack trace {e.StackTrace}. {e.Message}");
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
            catch (Exception e)
            {
                Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
                throw new Exception($"Method {nameof(Articles)} is failed, stack trace {e.StackTrace}. {e.Message}");
            }
        }
    }
}
