using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Data.Abstractions;
using AspNetArticle.Database.Entities;
using AutoMapper;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;

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
        return _mapper.Map<ArticleDto>(await _unitOfWork.Articles.GetByIdAsync(id));
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

    public async Task RemoveArticleByIdSourceAsync(Guid id)
    {
        var entities = await  _unitOfWork.Articles
            .Get()
            .Where(ent => ent.SourceId == id)
            .ToListAsync();
        
        _unitOfWork.Articles.RemoveRange(entities);
        
        await _unitOfWork.Commit();
    }


    //Onliner 
    #region GetArticlesOnlinerRss

    public async Task GetAllArticleDataFromOnlinerRssAsync(Guid sourceId, string? sourceRssUrl)
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

                    var articleDto = new ArticleDto() 
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


    #endregion
    #region FixArticleOnlinerTextAndShortDescriptionMethods

    
    public async Task AddArticleTextAndFixShortDescriptionToArticlesOnlinerAsync()
    {
        var articlesWithEmptyTextIds = _unitOfWork.Articles.Get()
            .Where(article => string.IsNullOrEmpty(article.Text))
            .Select(article => article.Id)
            .ToList();

        foreach (var articleId in articlesWithEmptyTextIds)
        {
            await AddArticleTextToArticlesOnlinerAsync(articleId);
        }
    }

    private async Task AddArticleTextToArticlesOnlinerAsync(Guid articleId)
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
            var shortDescription = article.ShortDescription;

            var web = new HtmlWeb();
            var htmlDoc = web.Load(articleSourceUrl);
            var nodes =
                htmlDoc.DocumentNode.Descendants(0)
                    .Where(n => n.HasClass("news-text"));

            shortDescription = FixArticleOnlinerShortDescription(shortDescription);

            if (nodes.Any())
            {
                var articleText = nodes.FirstOrDefault()?
                    .ChildNodes
                    .Where(node => (node.Name.Equals("p") || node.Name.Equals("div") || node.Name.Equals("h2"))
                                   && !node.HasClass("news-reference")
                                   && !node.HasClass("news-banner")
                                   && !node.HasClass("news-widget")
                                   && !node.HasClass("news-vote")
                                   && !node.HasClass("news-incut")
                                   && node.Attributes["style"] == null)
                    .Select(node => node.OuterHtml)
                    .Aggregate((i, j) => i + Environment.NewLine + j);

                await _unitOfWork.Articles.UpdateArticleTextAsync(articleId, articleText);
                await _unitOfWork.Articles.UpdateArticleShortDescriptionAsync(articleId, shortDescription);

                await _unitOfWork.Commit();
            }

        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private string FixArticleOnlinerShortDescription(string shortDescription)
    {
        if (!string.IsNullOrEmpty(shortDescription))
        {

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(shortDescription);

            var text = htmlDoc.DocumentNode.SelectNodes("p")[1].InnerText;

            return text ?? string.Empty;
        }
        return string.Empty;
    }
    #endregion
    #region FixArticleOnlinerImageMethod
    public async Task AddArticleImageUrlToArticlesOnlinerAsync()
    {
        var articlesWithEmptyImageUrlIds = _unitOfWork.Articles.Get()
            .Where(article => string.IsNullOrEmpty(article.ImageUrl))
            .Select(article => article.Id)
            .ToList();

        foreach (var articleId in articlesWithEmptyImageUrlIds)
        {
            await AddArticleImageUrlToArticlesOnlinerAsync(articleId);
        }
    }

    private async Task AddArticleImageUrlToArticlesOnlinerAsync(Guid articleId)
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

            var web = new HtmlWeb();
            var htmlDoc = web.Load(articleSourceUrl);
            var nodes =
                htmlDoc.DocumentNode.Descendants(0) 
                    .Where(n => n.HasClass("news-header__image"));

            if (nodes.Any())
            {
                var articleImageUrl = nodes.FirstOrDefault()?.GetAttributeValue("style", null);

                if (!string.IsNullOrEmpty(articleImageUrl))
                {
                    articleImageUrl = FixArticleOnlinerStringUrl(articleImageUrl);

                    await _unitOfWork.Articles.UpdateArticleImageUrlAsync(articleId, articleImageUrl);
                    await _unitOfWork.Commit();
                }
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }  
    
    private string FixArticleOnlinerStringUrl(string url) 
    {                                                    
        if(url.StartsWith("background-image: url('") && url.EndsWith("');"))
            return url
                .Replace("background-image: url('", "")
                .Replace("');", "");
        
        return url
            .Replace("background-image: url(", "")
            .Replace(");", "");
    }
    #endregion


    //DevIo
    #region GetArticleDevIoRss
    public async Task GetAllArticleDataFromDevIoRssAsync(Guid sourceId, string? sourceRssUrl)
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

                    var articleDto = new ArticleDto()
                    {
                        Id = Guid.NewGuid(),
                        Title = item.Title.Text,
                        PublicationDate = item.PublishDate.UtcDateTime,
                        ShortDescription = item.Summary?.Text ?? string.Empty,
                        Category = item.Categories.FirstOrDefault()?.Name ?? "Обо Всём",
                        SourceId = sourceId,
                        SourceUrl = item.Id,
                        ImageUrl = item.Links[1]?.Uri.AbsoluteUri
                    };

                    if (!string.IsNullOrEmpty(articleDto.ShortDescription))
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


    #endregion
    #region FixArticleDevIoTextMethods
    public async Task AddArticleTextToArticlesDevIoAsync()
    {
        var articlesWithEmptyTextIds = _unitOfWork.Articles.Get()
            .Where(article => string.IsNullOrEmpty(article.Text))
            .Select(article => article.Id)
            .ToList();

        foreach (var articleId in articlesWithEmptyTextIds)
        {
            await AddArticleTextToArticleDevIoAsync(articleId);
        }
    }

    private async Task AddArticleTextToArticleDevIoAsync(Guid articleId)
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

            var web = new HtmlWeb();
            var htmlDoc = web.Load(articleSourceUrl);
            var nodes =
                htmlDoc.DocumentNode.Descendants(0)
                    .Where(n => n.HasClass("article__body"));

            if (nodes.Any())
            {
                var articleText = (nodes.FirstOrDefault()?.ChildNodes
                        .Where(node => node.Name.Equals("div")))?
                    .FirstOrDefault()?.ChildNodes
                    .Where(node => node.Name.Equals("p"))
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


    #endregion
}