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
                throw new NullReferenceException();

            await _mediator.Send(new AddFavouriteArticleCommand() { FavouriteArticle = entity });
            //await _unitOfWork.FavouriteArticle.AddAsync(entity);
            //return await _unitOfWork.Commit();
        }

        public async Task RemoveFavouriteArticle(FavouriteArticleDto favouriteArticleDto)
        {
            var entity = await _mediator.Send(new GetFavouriteArticleByUserIdAndArticleIdQuery() 
            { 
                ArticleId = favouriteArticleDto.ArticleId,
                UserId = favouriteArticleDto.UserId
            });

            if (entity == null)
                throw new NullReferenceException();

            await _mediator.Send(new DeleteFavouriteArticleCommand() { FavouriteArticle = entity });
            //var entity = _mapper.Map<FavouriteArticle>(await _unitOfWork.FavouriteArticle.Get()
            //    .FirstOrDefaultAsync(fav => favouriteArticleDto.UserId.Equals(fav.UserId) 
            //    && favouriteArticleDto.ArticleId.Equals(fav.ArticleId)));

            //if (entity == null)
            //    throw new NullReferenceException();

            //_unitOfWork.FavouriteArticle.Remove(entity);
            //await _unitOfWork.Commit();
        }

        public async Task<bool> CheckFavouriteArticle(Guid userId, Guid articleId)
        {
            var entity = await _mediator.Send(new GetFavouriteArticleByUserIdAndArticleIdQuery()
            {
                ArticleId = articleId,
                UserId = userId
            });

            return entity != null; 

            //return await _unitOfWork.FavouriteArticle.Get()
            //    .AnyAsync(fav => userId.Equals(fav.UserId)
            //    && articleId.Equals(fav.ArticleId));
        }

        public async Task<List<ArticleDto>> GetAllFavouriteArticles(Guid userId)
        {
            var favouriteArticles = await _mediator.Send(new GetAllFavouriteArticlesIdByUserIdQuery() { UserId = userId });

            var articles = await _mediator.Send(new GetAllArticlesQuery());

            if (favouriteArticles == null)
                throw new NullReferenceException();

            if (articles == null)
                throw new NullReferenceException();


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
            //var favouriteArticles = await _unitOfWork.FavouriteArticle
            //    .Get()
            //    .AsNoTracking()
            //    .Where(art => userId.Equals(art.UserId))
            //    .Select(art => art.ArticleId)
            //    .ToArrayAsync();

            //var articles = await _unitOfWork.Articles.GetAllAsync();

            //if (favouriteArticles == null)
            //    throw new NullReferenceException();

            //if (articles == null)
            //    throw new NullReferenceException();

            //var listArticles = new List<ArticleDto>();
            //if (favouriteArticles.Any())
            //{
            //    foreach (var id in favouriteArticles)
            //    {
            //        var article = articles.FirstOrDefault(art => art.Id.Equals(id));

            //        if (article != null)
            //            listArticles.Add(_mapper.Map<ArticleDto>(article));
            //    }
            //}

        }
    }
}
