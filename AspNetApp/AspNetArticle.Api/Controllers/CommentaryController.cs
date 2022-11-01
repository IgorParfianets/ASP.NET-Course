using AspNetArticle.Api.Models.Request;
using AspNetArticle.Business.Services;
using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetArticle.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentaryController : ControllerBase
    {
        private readonly ICommentaryService _commentaryService;
        private readonly IMapper _mapper;

        public CommentaryController(ICommentaryService commentaryService, IMapper mapper)
        {
            _commentaryService = commentaryService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommentById(Guid id)
        {
            try
            {
                var comment = await _commentaryService.GetCommentByIdAsync(id);

                if (comment == null)
                    return NotFound();

                return Ok(comment);
            }
            catch (Exception e)
            {
                
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetComments([FromQuery] GetCommentsRequestModel? model) // work like filter by user/article
        {
            var articles = await _commentaryService.GetCommentsByUserIdAndArticleName(model?.ArticleId, model?.UserId);

            return Ok(articles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] AddCommentRequestModel model)
        {
            var dto = _mapper.Map<CommentDto>(model);
            
            if (dto == null)
                return BadRequest();

            var result = await _commentaryService.CreateCommentAsync(dto);
            if (result > 0)
                return Ok();

            return BadRequest(); // todo need to clarify which StatusCode
        }
    }
}
