using AspNetArticle.Core.Abstractions;
using AspNetArticle.Database.Entities;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetArticle.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleResourceInitializerController : ControllerBase
    {
        private readonly IArticleService _articleService;
        private readonly IArticleRateService _articleRateService;
        private readonly ISendMessageService _sendMessageService;
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
        public async Task<IActionResult> AddSourceArticles() //todo rework with last lecture
        {
            try
            {
                RecurringJob.AddOrUpdate(() => _articleService.AggregateArticlesFromExternalSourcesAsync(),
                    "35 19 * * *");

                RecurringJob.AddOrUpdate(() => _articleService.AddArticlesDataAsync(),
                    "36 19 * * *");

                RecurringJob.AddOrUpdate(() => _articleRateService.AddRateToArticlesAsync(),
                    "38,39,40 19 * * *");

                RecurringJob.AddOrUpdate(() => _sendMessageService.GetArticlesAndUsersForMessage(),
                    "42 19 * * *");

                return Ok();
            }

            catch (Exception e)
            {
                throw new Exception();
            }
        }
    }
}
