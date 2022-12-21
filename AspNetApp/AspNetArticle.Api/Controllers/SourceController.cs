using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace AspNetArticle.Api.Controllers
{
    /// <summary>
    /// Source resource controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SourceController : ControllerBase
    {
        private readonly ISourceService _sourceService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceService"></param>
        public SourceController(ISourceService sourceService)
        {
            _sourceService = sourceService;
        }

        /// <summary>
        /// Get source by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Source</returns>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(SourceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSourceById(Guid id)
        {
            try
            {
                var source = await _sourceService.GetSourcesByIdAsync(id);

                if (source == null)
                {
                    Log.Warning($"Source with id {id} not found in database");
                    return NotFound();
                }
                Log.Information("Source successfully received", source);
                return Ok(source);
            }
            catch (Exception e)
            {
                Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
                return StatusCode(500);
            }
        }
        /// <summary>
        /// Get all sources
        /// </summary>
        /// <returns>Sources</returns>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<SourceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSources()
        {
            try
            {
                var sources = await _sourceService.GetSourcesAsync();

                if (sources == null)
                {
                    Log.Warning("No sources in database");
                    return NotFound();
                }
                Log.Information("Sources successfully received", sources);
                return Ok(sources);
            }
            catch (Exception e)
            {
                Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
                return StatusCode(500);
            }          
        }
    }
}
