using AspNetArticle.Api.Models.Request;
using AspNetArticle.Core.Abstractions;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetArticle.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _articleService;
        private readonly IMediator mediator;
        private readonly IMapper _mapper;

        public ArticlesController(IArticleService articleService, 
            IMapper mapper, 
            IMediator mediator)
        {
            _articleService = articleService;
            _mapper = mapper;
            this.mediator = mediator;
        }

        [HttpGet] 
        public async Task<IActionResult> GetArticles([FromQuery] GetArticlesRequestModel? model) // Having injected filter by Title and Source(onliner ...)
        {
            var articles = await _articleService.GetArticlesByNameAndSourcesAsync(model?.Name, model?.SourceId); // need replace method

            return Ok(articles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticleById(Guid id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            return Ok(article);
        }
        ////todo decide how to act with filtering by category name

        //[HttpDelete] // todo unnecessary method use only clearing Db
        //public async Task<IActionResult> DeleteArticlesBySourceId(Guid id)
        //{
        //    await _articleService.RemoveArticleByIdSourceAsync(id);
        //    return Ok();
        //}
    }
}
