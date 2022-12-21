using AspNetArticle.Api.Models.Request;
using AspNetArticle.Api.Models.Response;
using AspNetArticle.Business.Services;
using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Database.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;

namespace AspNetArticle.Api.Controllers
{
    /// <summary>
    /// Comments resource controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CommentaryController : ControllerBase
    {
        private readonly ICommentaryService _commentaryService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commentaryService"></param>
        /// <param name="mapper"></param>
        /// <param name="userService"></param>
        public CommentaryController(ICommentaryService commentaryService, 
            IMapper mapper, 
            IUserService userService)
        {
            _commentaryService = commentaryService;
            _mapper = mapper;
            _userService = userService;
        }

        /// <summary>
        /// Get comment by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Comment</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CommentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCommentById(Guid id)
        {
            try
            {
                var comment = await _commentaryService.GetCommentByIdAsync(id);

                if (comment == null)
                {
                    Log.Warning($"Comment with id {id} not found");
                    return NotFound();
                }
                Log.Information("Comment successfully received", comment);
                return Ok(comment);
            }
            catch (Exception e)
            {
                Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
                return StatusCode(500);
            }
        }
        /// <summary>
        /// Get all by user id and article id comments
        /// </summary>
        /// <param name="model"></param>
        /// <returns>All comments</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CommentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllComments([FromQuery] GetCommentsRequestModel? model) 
        {
            try
            {
                if (model == null)
                {
                    Log.Warning($"Model is null");
                    return BadRequest();
                }
                var comments = await _commentaryService.GetCommentsByUserIdAndArticleId(model.ArticleId, model.UserId);

                if (comments == null)
                {
                    Log.Warning($"Comments are not found in database");
                    return NotFound();
                }
                Log.Information("Comments successfully received", comments);
                return Ok(comments);
            }
            catch (Exception e)
            {
                Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Create comment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateComment([FromBody] AddCommentRequestModel model)
        {
            try
            {
                var userEmail = User.Claims.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userEmail))
                {
                    Log.Warning("ClaimTypes.NameIdentifier is null");
                    return NotFound();
                }

                var userId = (await _userService.GetUserByEmailAsync(userEmail)).Id;

                var dto = _mapper.Map<CommentDto>(model);
                if (dto == null)
                {
                    Log.Warning($"Mapping failed", model);
                    return BadRequest();
                }
                dto.UserId = userId;
                await _commentaryService.CreateCommentAsync(dto);

                Log.Information("Comment successfully created");
                return Ok();
            }
            catch (Exception e)
            {
                Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Update comment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateComment([FromBody] UpdateCommentRequestModel model)
        {
            try
            {
                var userEmail = User.Claims.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userEmail))
                {
                    Log.Warning("ClaimTypes.NameIdentifier is null");
                    return NotFound();
                }

                var userId = (await _userService.GetUserByEmailAsync(userEmail)).Id;

                var dto = _mapper.Map<CommentDto>(model);

                if (dto == null)
                {
                    Log.Warning($"Mapping is failed", model);
                    return BadRequest();
                }
                dto.UserId = userId;
                await _commentaryService.UpdateCommentAsync(dto);

                Log.Information("Comment successfully created");
                return Ok(); 
            }
            catch (Exception e)
            {
                Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
                return StatusCode(500);
            }
            
        }

        /// <summary>
        /// Deleted comment by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            try
            {
                await _commentaryService.DeleteCommentById(id);

                Log.Information("Comment successfully deleted");
                return NoContent();
            }
            catch (Exception e)
            {
                Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
                return StatusCode(500);
            }
        }
    }
}
