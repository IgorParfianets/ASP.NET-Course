using AsoNetArticle.Data.CQS.Commands;
using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Data.Abstractions;
using AspNetArticle.Database.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AspNetArticle.Business.Services
{
    public class FavouriteArticleService : IFavouriteArticleService
    {
        public readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        public readonly IMapper _mapper;
        public FavouriteArticleService(IUnitOfWork unitOfWork,
            IMapper mapper,
            IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task CreateFavouriteArticle(FavouriteArticleDto favouriteArticleDto)
        {
            var entity = _mapper.Map<FavouriteArticle>(favouriteArticleDto);

            if (entity == null)
                throw new NullReferenceException($"{nameof(entity)} is null");

            await _mediator.Send(new AddFavouriteArticleCommand() { FavouriteArticle = entity });
        }

        public async Task RemoveFavouriteArticle(FavouriteArticleDto favouriteArticleDto)
        {
            var entity = await _mediator.Send(new GetFavouriteArticleByUserIdAndArticleIdQuery() 
            { 
                ArticleId = favouriteArticleDto.ArticleId,
                UserId = favouriteArticleDto.UserId
            });

            if (entity == null)
                throw new NullReferenceException($"{nameof(entity)} is null");

            await _mediator.Send(new DeleteFavouriteArticleCommand() { FavouriteArticle = entity });
        }

        public async Task<bool> CheckFavouriteArticle(Guid userId, Guid articleId)
        {
            var entity = await _mediator.Send(new GetFavouriteArticleByUserIdAndArticleIdQuery()
            {
                ArticleId = articleId,
                UserId = userId
            });

            return entity != null; 
        }

        public async Task<IEnumerable<ArticleDto>> GetAllFavouriteArticles(Guid userId)
        {
            var favouriteArticles = await _mediator.Send(new GetAllFavouriteArticlesIdByUserIdQuery() { UserId = userId });

            var articles = await _mediator.Send(new GetAllArticlesQuery());

            if (favouriteArticles == null)
                throw new NullReferenceException($"{nameof(favouriteArticles)} is null");

            if (articles == null)
                throw new NullReferenceException($"{nameof(articles)} is null");


            var listArticles = new List<ArticleDto>();
            if (favouriteArticles.Any())
            {
                foreach (var id in favouriteArticles)
                {
                    var article = articles.FirstOrDefault(art => art.Id.Equals(id));

                    if (article != null)
                        listArticles.Add(_mapper.Map<ArticleDto>(article));
                }
            }
            return listArticles; 
        }
    }
}
