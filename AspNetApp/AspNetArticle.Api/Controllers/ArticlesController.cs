using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AspNetArticle.Api.Controllers
{
    /// <summary>
    /// Article resource controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _articleService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="articleService"></param>
        public ArticlesController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        /// <summary>
        ///  Get all articles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ArticleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetArticles() 
        {
            try
            {
                var articles = await _articleService.GetAllArticlesAsync();

                if (articles == null)
                {
                    Log.Warning("Articles not found in database");
                    return NotFound();
                }
                Log.Information("Articles successfully received", articles);
                return Ok(articles);
            }
            catch (Exception e)
            {
                Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
                return StatusCode(500);
            }
        }

        /// <summary>
        ///  Get article by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Article</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ArticleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetArticleById(Guid id)
        {
            try
            {
                var article = await _articleService.GetArticleByIdAsync(id);

                if (article == null)
                {
                    Log.Warning($"Article with id {id} not found in database");
                    return NotFound();
                }
                Log.Information("Article successfully received", article);
                return Ok(article);
            }
            catch (Exception e)
            {
                Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
                return StatusCode(500);
            }
        }
    }
}
