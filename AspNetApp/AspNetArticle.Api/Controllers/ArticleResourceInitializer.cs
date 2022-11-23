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
                //RecurringJob.AddOrUpdate(() => _articleService.AggregateArticlesFromExternalSourcesAsync(),
                //    "*/5 * * * *");

                //RecurringJob.AddOrUpdate(() => _articleService.AddArticlesDataAsync(),
                //    "*/7 * * * *");

                //RecurringJob.AddOrUpdate(() => _articleRateService.AddRateToArticlesAsync(),
                //    "*/10 * * * *");

                //await _articleService.AddArticlesDataAsync();
                await _sendMessageService.GetArticlesAndUsersForMessage();

                return Ok();
            }
            catch (Exception e)
            {
                throw new Exception();
            }
        }
    }
}
