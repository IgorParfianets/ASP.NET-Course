using AspNetArticle.Core.Abstractions;
using AspNetArticle.Database.Entities;
using AutoMapper;
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
            ISourceService sourceService, IConfiguration configuration, IArticleRateService articleRateService)
        {
            _articleService = articleService;
            _mapper = mapper;
            _sourceService = sourceService;
            _configuration = configuration;
            _articleRateService = articleRateService;
        }

        /// <summary>
        /// Initialize Onliner source articles
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        public async Task<IActionResult> AddOnlinerSourceArticles() //todo rework with last lecture
        {
            try
            {
                //var onlinerId = Guid.Parse(_configuration["Sources:Onliner"]);
                //var sourceRssUrl = (await _sourceService.GetSourcesByIdAsync(onlinerId))?.RssUrl;

                //await _articleService.GetAllArticleDataFromOnlinerRssAsync(onlinerId, sourceRssUrl);
                //await _articleService.AddArticleTextAndFixShortDescriptionToArticlesOnlinerAsync();
                //await _articleService.AddArticleImageUrlToArticlesOnlinerAsync();

                await _articleRateService.AddRateToArticlesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Initialize DevIo source articles
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet]
        public async Task<IActionResult> AddDevIoSourceArticles() //todo rework with last lecture
        {
            try
            {
                var devIoId = Guid.Parse(_configuration["Sources:DevIo"]);
                var sourceRssUrl = (await _sourceService.GetSourcesByIdAsync(devIoId))?.RssUrl;

                await _articleService.GetAllArticleDataFromDevIoRssAsync(devIoId, sourceRssUrl);
                await _articleService.AddArticleTextToArticlesDevIoAsync();

                return Ok();
            }
            catch (Exception e)
            {
                throw new Exception();
            }
        }
    }
}
