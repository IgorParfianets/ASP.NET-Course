using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Database;
using AutoMapper;

namespace AspNetArticle.Business.Services;

public class ArticleService
{
    private readonly AggregatorContext _context;
    private readonly IMapper _mapper;

    public ArticleService(AggregatorContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    //public async Task<ArticleDto> GetArticleByIdAsync(Guid id)
    //{
    //    var article = _context.Articles.FirstOrDefault(x => x.Id == id);

    //    if (article != null)
    //    {
    //        var result = _mapper.Map<ArticleDto>(article);
    //        return result;
    //    }
        
    //}

    //public async Task<int> CreateArticleAsync(ArticleDto article)
    //{

    //}
}
