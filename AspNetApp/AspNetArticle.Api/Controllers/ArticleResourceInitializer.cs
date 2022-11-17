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
        private readonly ISourceService _sourceService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public ArticleResourceInitializer(IArticleService articleService,
            IMapper mapper,
            ISourceService sourceService, 
            IConfiguration configuration, 
            IArticleRateService articleRateService)
        {
            _articleService = articleService;
            _mapper = mapper;
            _sourceService = sourceService;
            _configuration = configuration;
            _articleRateService = articleRateService;
        }

        /// <summary>
        /// Initialize Onliner and DevIo source articles
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        public async Task<IActionResult> AddOnlinerSourceArticles() //todo rework with last lecture
        {
            try
            {
                RecurringJob.AddOrUpdate(() => _articleService.AggregateArticlesFromExternalSourcesAsync(),
                    "*/5 * * * *");

                RecurringJob.AddOrUpdate(() => _articleService.AddArticlesDataAsync(),
                    "*/7 * * * *");

                RecurringJob.AddOrUpdate(() => _articleRateService.AddRateToArticlesAsync(),
                    "*/10 * * * *");

                return Ok();
            }
            catch (Exception e)
            {
                throw new Exception();
            }
        }
    }
}
