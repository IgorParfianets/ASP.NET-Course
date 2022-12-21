using AspNetArticle.Core.Abstractions;
using AspNetArticle.Database.Entities;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AspNetArticle.Api.Controllers
{
    /// <summary>
    /// Initialize resourse article's data
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleResourceInitializerController : ControllerBase
    {
        private readonly IArticleService _articleService;
        private readonly IArticleRateService _articleRateService;
        private readonly ISendMessageService _sendMessageService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="articleService"></param>
        /// <param name="articleRateService"></param>
        /// <param name="sendMessageService"></param>
        public ArticleResourceInitializerController(IArticleService articleService,
            IArticleRateService articleRateService,
            ISendMessageService sendMessageService)

        {
            _articleService = articleService;
            _articleRateService = articleRateService;
            _sendMessageService = sendMessageService;
        }

        /// <summary>
        /// Initialize Onliner and DevIo source articles
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet]
        public async Task<IActionResult> AddSourceArticles() 
        {
            try
            {
                RecurringJob.AddOrUpdate(() => _articleService.AggregateArticlesFromExternalSourcesAsync(),
                    "50 9,15 * * *");

                RecurringJob.AddOrUpdate(() => _articleService.AddArticlesDataAsync(),
                    "52 9,15 * * *");

                RecurringJob.AddOrUpdate(() => _articleRateService.AddRateToArticlesAsync(),
                    "54,36 9,15 * * *");

                RecurringJob.AddOrUpdate(() => _sendMessageService.GetArticlesAndUsersForMessage(),
                    "56 9,15 * * *");

                Log.Information("Background jobs initialized succesfully");
                return Ok();
            }

            catch (Exception e)
            {
                Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
                return StatusCode(500);
            }
        }
    }
}
