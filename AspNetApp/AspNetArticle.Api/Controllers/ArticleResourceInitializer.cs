using AspNetArticle.Core.Abstractions;
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

        [HttpPost]
        public async Task<IActionResult> AddArticles()
        {
            try
            {
                var sources = await _sourceService.GetSourcesAsync();

                foreach (var source in sources)
                {
                    await _articleService.GetAllArticleDataFromRssAsync(source.Id, source.RssUrl);
                    await _articleService.AddArticleTextToArticlesAsync();
                }

                return Ok();
            }
            catch (Exception e)
            {

                throw new Exception();
            }
        }
    }
}
