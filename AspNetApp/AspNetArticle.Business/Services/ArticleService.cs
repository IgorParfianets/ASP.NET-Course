using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using AutoMapper;

namespace AspNetArticle.Business.Services;

public class ArticleService : IArticleService
{
    private readonly AggregatorContext _context;
    private readonly IMapper _mapper;

    public ArticleService(AggregatorContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ArticleDto> GetArticleByIdAsync(Guid id)
    {
        var article = _context.Articles.FirstOrDefault(x => x.Id == id);

        if (article != null)
        {
            var result = _mapper.Map<ArticleDto>(article);
            return result;
        }
        throw new Exception("User not found");
    }

    public async Task<int> CreateArticleAsync(ArticleDto article)
    {
        var user = _mapper.Map<Article>(article);
        await _context.Articles.AddAsync(user);

        return await _context.SaveChangesAsync();
    }
}
