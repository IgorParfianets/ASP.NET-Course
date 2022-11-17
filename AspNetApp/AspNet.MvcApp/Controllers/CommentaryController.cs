using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Database.Entities;
using AspNetArticle.MvcApp.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetArticle.MvcApp.Controllers
{
    [Authorize]
    public class CommentaryController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly ICommentaryService _commentaryService;

        public CommentaryController(IMapper mapper, 
            IUserService userService,
            ICommentaryService commentaryService)
        {
            _mapper = mapper;
            _userService = userService;
            _commentaryService = commentaryService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CommentaryModel model)
        {
            if (ModelState.IsValid)
            {
                var userEmail = User.Identity.Name;
                var userId = (await _userService.GetUserByEmailAsync(userEmail))?.Id;

                var dto = _mapper.Map<CommentDto>(model);

                if (dto != null && userId != null)
                {
                    dto.UserId = userId.Value;

                    var result = await _commentaryService.CreateCommentAsync(dto);
                    if (result > 0)
                    {
                        return Redirect($"~/Article/Details/{model.ArticleId}"); //todo need to make url string
                    }
                }
            }

            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid id) 
        {
            var comment = _mapper.Map<CommentaryModel>(await _commentaryService.GetCommentByIdAsync(id));

            if (comment != null)
                return RedirectToAction( "Details", "Article", comment );

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Update(CommentaryModel model) 
        {
            if (ModelState.IsValid)
            {
                var userEmail = User.Identity.Name;
                var userId = (await _userService.GetUserByEmailAsync(userEmail))?.Id;

                if (userId != null)
                {

                    var dto = new CommentDto()  //todo should be refactored maybe create separate EditModel or UpdateModel
                    {
                        Id = model.Id,
                        UserId = userId.Value,
                        ArticleId = model.ArticleId,
                        Description = model.Description,
                        PublicationDate = DateTime.Now,
                        IsEdited = true
                    };

                    var result = await _commentaryService.UpdateCommentAsync(dto);
                    if (result > 0)
                    {
                        return Redirect($"~/Article/Details/{model.ArticleId}"); //todo need to make url string
                    }
                }
            }
            return RedirectToAction("Details", "Article", model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
             await _commentaryService.DeleteCommentById(id);

             return RedirectToAction("Index","Article"); //todo need to make url string
        }
    }
}
