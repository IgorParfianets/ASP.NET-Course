using AspNetArticle.Core.Abstractions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetArticle.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SourceController : ControllerBase
    {
        private readonly ISourceService _sourceService;
        private readonly IMapper _mapper;

        public SourceController(ISourceService sourceService, IMapper mapper)
        {
            _sourceService = sourceService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSourceById(Guid id)
        {
            try
            {
                var source = await _sourceService.GetSourcesByIdAsync(id);

                if (source == null)
                {
                    return NotFound();
                }

                return Ok(source);
            }
            catch (Exception e)
            {
                
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSources()
        {
            var sources = await _sourceService.GetSourcesAsync();

            if (sources.Any())
            {
                return Ok(sources);
            }
            return BadRequest();
        }
    }
}
