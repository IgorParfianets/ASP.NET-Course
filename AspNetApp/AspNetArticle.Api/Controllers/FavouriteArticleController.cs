using AspNetArticle.Api.Models.Request;
using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Database.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AspNetArticle.Api.Controllers
{
    /// <summary>
    /// Favourite article resource controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FavouriteArticleController : ControllerBase
    {
        private readonly IFavouriteArticleService _favouriteArticleService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="favouriteArticleService"></param>
        /// <param name="mapper"></param>
        public FavouriteArticleController(IFavouriteArticleService favouriteArticleService,
            IMapper mapper,
            IUserService userService)
        {
            _favouriteArticleService = favouriteArticleService;
            _mapper = mapper;
            _userService = userService;
        }

        /// <summary>
        /// Get all user's favourite articles
        /// </summary>
        /// <returns>Favourite articles</returns>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<FavouriteArticle>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFavouriteArticle()
        {
            try
            {              
                var userEmail = User.Claims.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userEmail))
                {
                    Log.Warning("ClaimTypes.NameIdentifier is null");
                    return BadRequest();
                }

                var userId = (await _userService.GetUserByEmailAsync(userEmail)).Id;

                var favouriteArticles = await _favouriteArticleService.GetAllFavouriteArticles(userId);
                if (favouriteArticles == null)
                {
                    Log.Warning("Articles not found in database");
                    return BadRequest();
                }
                Log.Information("Articles successfully received", favouriteArticles);
                return Ok(favouriteArticles);
            }
            catch (Exception e)
            {
                Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Add/Remove favourite article
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Exception), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateFavouriteArticle([FromBody] AddFavouriteArticleRequestModel model)
        {
            try
            {
                if (model != null)
                {
                    var userEmail = User.Claims.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userEmail))
                    {
                        Log.Warning("ClaimTypes.NameIdentifier is null");
                        return BadRequest();
                    }

                    var userId = (await _userService.GetUserByEmailAsync(userEmail)).Id;

                    if (model.Answer)
                    {
                        var favouriteArticleDto = new FavouriteArticleDto()
                        {
                            Id = Guid.NewGuid(),
                            UserId = userId,
                            ArticleId = model.ArticleId
                        };
                        await _favouriteArticleService.CreateFavouriteArticle(favouriteArticleDto);

                        Log.Information("Favourite article added successfully");
                        return Ok();
                    }
                    else
                    {
                        var favouriteArticleDto = new FavouriteArticleDto()
                        {
                            UserId = userId,
                            ArticleId = model.ArticleId
                        };
                        await _favouriteArticleService.RemoveFavouriteArticle(favouriteArticleDto);

                        Log.Information("Favourite article removed successfully");
                        return NoContent();
                    }
                }
                Log.Warning("Model is invalid", model);
                return BadRequest();
            }
            catch (Exception e)
            {
                Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
                return StatusCode(500);
            }
        }
    }
}
