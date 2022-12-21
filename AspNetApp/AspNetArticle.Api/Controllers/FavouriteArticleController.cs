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
                    Log.Warning("No articles in database");
                    return NotFound();
                }

                var user = await _userService.GetUserByEmailAsync(userEmail);
                if (user == null)
                {
                    Log.Warning("No articles in database");
                    return BadRequest();
                }

                var favouriteArticles = await _favouriteArticleService.GetAllFavouriteArticles(user.Id);
                if (favouriteArticles == null)
                {
                    Log.Warning("No articles in database");
                    return NotFound();
                }
                return Ok(favouriteArticles);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateFavouriteArticle([FromBody] AddFavouriteArticleRequestModel model)
        {
            try
            {
                if (model != null)
                {

                    //var userId = (await _userService.GetUserByEmailAsync(userEmail)).Id;

                    //if (userId != Guid.Empty)
                    //{
                    //    if (favouriteArticle.Answer)
                    //    {
                    //        var favouriteArticleDto = new FavouriteArticleDto()
                    //        {
                    //            Id = Guid.NewGuid(),
                    //            UserId = userId,
                    //            ArticleId = favouriteArticle.ArticleId,
                    //        };
                    //        await _favouriteArticleService.CreateFavouriteArticle(favouriteArticleDto);

                    //        return Ok();
                    //    }
                    //    else
                    //    {
                    //        var favouriteArticleDto = new FavouriteArticleDto()
                    //        {
                    //            UserId = userId,
                    //            ArticleId = favouriteArticle.ArticleId,
                    //        };
                    //        await _favouriteArticleService.RemoveFavouriteArticle(favouriteArticleDto);

                    //        return Ok();
                    //    }
                    //}
                }
                return BadRequest();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //[HttpDelete]
        //[Authorize]
        //public async Task<IActionResult> DeleteFavouriteArticle([FromBody] AddFavouriteArticleRequestModel model)
        //{
        //    try
        //    {
        //        var user = await _userService.GetUserByIdAsync(userId);

        //        if (user == null)
        //            return NotFound();

        //        return Ok(user);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
    }
}
