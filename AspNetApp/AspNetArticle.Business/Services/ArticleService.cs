using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Data.Abstractions;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using AutoMapper;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System.ServiceModel.Syndication;
using System.Xml;

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

    public Task<int> UpdateArticleAsync(Guid id, ArticleDto? patchList) // todo need implement
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

    public async Task<List<ArticleDto>> GetArticlesByNameAndSourcesAsync(string? name, Guid? category) // for Article API
    {
        var entities = _unitOfWork.Articles.Get();

        if (!string.IsNullOrEmpty(name))
        {
            entities = entities.Where(ent => ent.Title.Contains(name));
        }

        if (category != null && !Guid.Empty.Equals(category))
        {
            entities = entities.Where(ent => ent.SourceId.Equals(category));
        }

        var result = (await entities.ToListAsync())
            .Select(ent => _mapper.Map<ArticleDto>(ent))
            .ToList();

        return result;
    }

    //---------------------------------------------------------- for controllerInitializer
    public async Task GetAllArticleDataFromRssAsync(Guid sourceId, string? sourceRssUrl)
    {
        if (!string.IsNullOrEmpty(sourceRssUrl))
        {
            var list = new List<ArticleDto>();

            using (var reader = XmlReader.Create(sourceRssUrl))
            {
                var feed = SyndicationFeed.Load(reader);

                foreach (var item in feed.Items)
                {
                    //should be checked for different rss sources 
                    
                    var articleDto = new ArticleDto() //todo I don't know is it correctly maybe need Mapper
                    {
                        Id = Guid.NewGuid(),
                        Title = item.Title.Text,
                        PublicationDate = item.PublishDate.UtcDateTime,
                        ShortDescription = item.Summary.Text,
                        Category = item.Categories.FirstOrDefault()?.Name,
                        SourceId = sourceId,
                        SourceUrl = item.Id
                    };
                    list.Add(articleDto);
                }
            }

            var oldArticleUrls = await _unitOfWork.Articles.Get()
                .Select(article => article.SourceUrl)
                .Distinct()
                .ToArrayAsync();

            var entities = list.Where(dto => !oldArticleUrls.Contains(dto.SourceUrl))
                .Select(dto => _mapper.Map<Article>(dto)).ToArray();

            await _unitOfWork.Articles.AddRangeAsync(entities);
            await _unitOfWork.Commit();
        }
    }


    public async Task AddArticleTextToArticlesAsync()
    {
        var articlesWithEmptyTextIds = _unitOfWork.Articles.Get()
            .Where(article => string.IsNullOrEmpty(article.Text))
            .Select(article => article.Id)
            .ToList();

        foreach (var articleId in articlesWithEmptyTextIds)
        {
            await AddArticleTextToArticleAsync(articleId);
        }
    }

    private async Task AddArticleTextToArticleAsync(Guid articleId)
    {
        try
        {
            var article = await _unitOfWork.Articles.GetByIdAsync(articleId);

            if (article == null)
            {
                throw new ArgumentException($"Article with id: {articleId} doesn't exists",
                    nameof(articleId));
            }

            var articleSourceUrl = article.SourceUrl;

            //var web = new HtmlWeb();
            //var htmlDoc = web.Load(articleSourceUrl);
            //var nodes =
            //    htmlDoc.DocumentNode.Descendants(0)
            //        .Where(n => n.HasClass("news-text"));
            var web = new HtmlWeb();
            var htmlDoc = web.Load(articleSourceUrl);
            var nodes =
                htmlDoc.DocumentNode.Descendants(0)
                    .Where(n => n.HasClass("se-material-page__body"));

            if (nodes.Any())
            {
                //var articleText = nodes.FirstOrDefault()?
                //    .ChildNodes
                //    .Where(node => (node.Name.Equals("p") || node.Name.Equals("div") || node.Name.Equals("h2"))
                //                   && !node.HasClass("news-reference")
                //                   && !node.HasClass("news-banner")
                //                   && !node.HasClass("news-widget")
                //                   && !node.HasClass("news-vote")
                //                   && node.Attributes["style"] == null)
                //    .Select(node => node.OuterHtml)
                //    .Aggregate((i, j) => i + Environment.NewLine + j);

                var articleText = nodes.FirstOrDefault()?
                    .ChildNodes
                    .Where(node => (node.Name.Equals("p") || node.Name.Equals("div") || node.Name.Equals("h1"))
                                && !node.HasClass("se-material-page__social")
                                && !node.HasClass("se-material-page__authors")
                                && !node.HasClass("e-material-page__blog-buttons")
                                && !node.HasClass("se-material-page__zoomer")  
                                && !node.HasClass("se-banner")
                                && !node.HasClass("se-material-page__blog-buttons")
                                && !node.Name.Equals("a")
                                && !node.Name.Equals("script")
                                )
                    .Select(node => node.OuterHtml)
                    .Aggregate((i, j) => i + Environment.NewLine + j);


                await _unitOfWork.Articles.UpdateArticleTextAsync(articleId, articleText); 
                await _unitOfWork.Commit();
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
