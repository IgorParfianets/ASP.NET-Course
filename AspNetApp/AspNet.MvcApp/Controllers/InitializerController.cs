using AspNetArticle.Core.Abstractions;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace AspNetArticle.MvcApp.Controllers
{
    public class InitializerController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IArticleRateService _articleRateService;
        private readonly ISendMessageService _sendMessageService;

        public InitializerController(IArticleService articleService,
            IArticleRateService articleRateService,
            ISendMessageService sendMessageService)
        {
            _articleService = articleService;
            _articleRateService = articleRateService;
            _sendMessageService = sendMessageService;
        }

        [HttpGet]
        public async Task<IActionResult> InitializedArticles()
        {

            RecurringJob.AddOrUpdate(() => _articleService.AggregateArticlesFromExternalSourcesAsync(),
                "*/5 * * * *");

            RecurringJob.AddOrUpdate(() => _articleService.AddArticlesDataAsync(),
                "*/7 * * * *");

            RecurringJob.AddOrUpdate(() => _articleRateService.AddRateToArticlesAsync(),
                "*/10 * * * *");

            return Ok();
        }
    }
}
