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
    public class ArticleResourceInitializer : ControllerBase
    {
        private readonly IArticleService _articleService;
        private readonly IArticleRateService _articleRateService;
        private readonly ISendMessageService _sendMessageService;
        public ArticleResourceInitializer(IArticleService articleService,
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
        public async Task<IActionResult> AddOnlinerSourceArticles() //todo rework with last lecture
        {
            try
            {
                await _articleRateService.AddRateToArticlesAsync();
                //RecurringJob.AddOrUpdate(() => _articleService.AggregateArticlesFromExternalSourcesAsync(),
                //    "0 21 * * *");

                //RecurringJob.AddOrUpdate(() => _articleService.AddArticlesDataAsync(),
                //    "1 21 * * *");

                //RecurringJob.AddOrUpdate(() => _articleRateService.AddRateToArticlesAsync(),
                //    "2,3,4,5,6,7 21 * * *");

                //RecurringJob.AddOrUpdate(() => _sendMessageService.GetArticlesAndUsersForMessage(),
                //    "8 21 * * *");

                return Ok();
            }

            catch (Exception e)
            {
                throw new Exception();
            }
        }
    }
}
