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
        private readonly ISourceService _sourceService;
        private readonly IMapper _mapper;

        public ArticleResourceInitializer(IArticleService articleService,
            IMapper mapper,
            ISourceService sourceService)
        {
            _articleService = articleService;
            _mapper = mapper;
            _sourceService = sourceService;
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
                await _articleService.GetAllArticleDataFromOnlinerRssAsync(Guid.Parse("B4702318-EBB2-4141-AB98-9113851063DB"),
                    "https://www.onliner.by/feed");
                await _articleService.AddArticleTextAndFixShortDescriptionToArticlesOnlinerAsync();

                await _articleService.AddArticleImageUrlToArticlesOnlinerAsync();

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
                await _articleService.GetAllArticleDataFromDevIoRssAsync(Guid.Parse("EC8CCBC6-EA52-4BFB-8578-176AAE285309"),
                    "https://devby.io/rss");

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
