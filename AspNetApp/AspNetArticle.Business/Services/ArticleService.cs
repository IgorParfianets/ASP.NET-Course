using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Data.Abstractions;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AspNetArticle.Business.Services;

public class ArticleService : IArticleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ArticleService(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ArticleDto> GetArticleByIdAsync(Guid id)
    {

        throw new Exception("User not found");
    }

    public async Task<int> CreateArticleAsync(ArticleDto article)
    {
       var entity = _mapper.Map<Article>(article);
       await _unitOfWork.Articles.AddAsync(entity);
        
       return await _unitOfWork.Commit();
    }

    public Task<int> UpdateArticleAsync(Guid id, ArticleDto? patchList)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ArticleDto>> GetAllArticlesAsync() // for Home / Index
    {
        var articleDto = await _unitOfWork.Articles
            .Get()
            .Select(entity => _mapper.Map<ArticleDto>(entity))
            .ToListAsync();
        
        return articleDto;
    }

}
