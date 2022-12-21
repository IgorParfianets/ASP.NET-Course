using AspNetArticle.Core.Abstractions;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Serilog;

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
        public IActionResult InitializedArticles() // hangfire
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
                Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
                throw new Exception($"Method {nameof(InitializedArticles)} is failed, stack trace {e.StackTrace}. {e.Message}");
            }  
        }
    }
}
