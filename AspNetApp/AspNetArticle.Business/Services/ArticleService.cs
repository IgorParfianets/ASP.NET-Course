using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Data.Abstractions;
using AspNetArticle.Database.Entities;
using AutoMapper;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace AspNetArticle.Business.Services;

public class ArticleService : IArticleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public ArticleService(IMapper mapper, 
        IUnitOfWork unitOfWork, 
        IConfiguration configuration)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }


    public async Task<ArticleDto> GetArticleByIdAsync(Guid id)
    {
        return _mapper.Map<ArticleDto>(await _unitOfWork.Articles.GetByIdAsync(id));
    }

    public async Task<IEnumerable<ArticleDto>> GetAllArticlesAsync() 
    {
        var articleDto = await _unitOfWork.Articles
            .Get()
            .Select(entity => _mapper.Map<ArticleDto>(entity))
            .ToListAsync();

        return articleDto;
    }

    public async Task<List<ArticleDto>> GetArticlesByNameAndSourcesAsync(string? name, Guid? category) // todo can remove unnecessary
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

    public async Task RemoveArticleToArchiveByIdAsync(Guid id)
    {
        var entities = await  _unitOfWork.Articles
            .Get()
            .Where(ent => ent.SourceId == id)
            .ToListAsync();
        
        _unitOfWork.Articles.RemoveRange(entities);
        
        await _unitOfWork.Commit();
    }

    public async Task<IEnumerable<ArticleDto>> GetArticlesByCategoryAndSearchStringAsync(string selectedCategory, string searchString)
    {
        var articles =  _unitOfWork.Articles.Get();

        if (!string.IsNullOrEmpty(searchString))
        {
            articles = articles.Where(art => art.Title.Contains(searchString));
        }

        if (!string.IsNullOrEmpty(selectedCategory))
        {
            articles = articles.Where(art => art.Category.Equals(selectedCategory));
        }

        var result = (await articles.ToListAsync())
            .Select(ent => _mapper.Map<ArticleDto>(ent))
            .ToList();

        return result;
    }

    public async Task<IEnumerable<string>> GetArticlesCategoryAsync()
    {
        var categories = await _unitOfWork.Articles
            .Get()
            .Select(art => art.Category)
            .Distinct()
            .ToListAsync();

        return categories;
    }

    public async Task<Guid?> GetArticleIdByCommentId(Guid commentId)
    {
        return (await _unitOfWork.Comments
            .Get()
            .FirstOrDefaultAsync(com => com.Id.Equals(commentId)))
            ?.ArticleId;
    }

    public async Task AggregateArticlesFromExternalSourcesAsync()
    {
        var onlinerSourceId = Guid.Parse(_configuration["Sources:Onliner"]);
        var onlinerSourceUrl = (await _unitOfWork.Sources.GetByIdAsync(onlinerSourceId))?.RssUrl;

        var devIoSourceId = Guid.Parse(_configuration["Sources:DevIo"]);
        var devIoSourceUrl = (await _unitOfWork.Sources.GetByIdAsync(devIoSourceId))?.RssUrl;

        await GetAllArticleDataFromOnlinerRssAsync(onlinerSourceId, onlinerSourceUrl);
        await GetAllArticleDataFromDevIoRssAsync(devIoSourceId, devIoSourceUrl);
    }


    public async Task AddArticlesDataAsync()
    {
        var onlinerSourceId = Guid.Parse(_configuration["Sources:Onliner"]);
        var devIoSourceId = Guid.Parse(_configuration["Sources:DevIo"]);

        await AddArticleTextAndFixShortDescriptionToArticlesOnlinerAsync(onlinerSourceId);
        await AddArticleImageUrlToArticlesOnlinerAsync(onlinerSourceId);

        await AddArticleTextToArticlesDevIoAsync(devIoSourceId);
    }

    //Onliner 
    #region GetArticlesOnlinerRss

    private async Task GetAllArticleDataFromOnlinerRssAsync(Guid sourceId, string? sourceRssUrl)
    {
        try
        {
            if (!string.IsNullOrEmpty(sourceRssUrl))
            {
                var list = new List<ArticleDto>();

                using (var reader = XmlReader.Create(sourceRssUrl))
                {
                    var feed = SyndicationFeed.Load(reader);

                    foreach (var item in feed.Items)
                    {
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

                // Command AddRangeArticlesCommand
                await _unitOfWork.Articles.AddRangeAsync(entities);
                await _unitOfWork.Commit();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"{nameof(GetAllArticleDataFromOnlinerRssAsync)} with arguments SourceGuid {sourceId}, SourceUrl {sourceRssUrl}");
            throw;
        }
        
    }


    #endregion

    #region FixArticleOnlinerTextAndShortDescriptionMethods

    
    private async Task AddArticleTextAndFixShortDescriptionToArticlesOnlinerAsync(Guid sourceId)
    {
        var articlesIdWithEmptyTextIds = _unitOfWork.Articles.Get()
            .Where(article => article.SourceId.Equals(sourceId) && string.IsNullOrEmpty(article.Text))
            .Select(article => article.Id)
            .ToList();

        foreach (var articleId in articlesIdWithEmptyTextIds)
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
            Log.Error(ex, $"{nameof(AddArticleTextToArticlesOnlinerAsync)} with ArticleGuid {articleId} failed");
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
    private async Task AddArticleImageUrlToArticlesOnlinerAsync(Guid sourceId)
    {
        var articlesWithEmptyImageUrlIds = _unitOfWork.Articles.Get()
            .Where(article => article.SourceId.Equals(sourceId) && string.IsNullOrEmpty(article.ImageUrl))
            .Select(article => article.Id)
            .ToList();

        foreach (var articleId in articlesWithEmptyImageUrlIds)
        {
            await AddArticleImageUrlToArticlesAsync(articleId);
        }
    }

    private async Task AddArticleImageUrlToArticlesAsync(Guid articleId)
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
            Log.Error(ex, $"{nameof(AddArticleImageUrlToArticlesAsync)} with ArticleGuid {articleId} failed");
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
    private async Task GetAllArticleDataFromDevIoRssAsync(Guid sourceId, string? sourceRssUrl)
    {
        try
        {
            if (!string.IsNullOrEmpty(sourceRssUrl))
            {
                var list = new List<ArticleDto>();

                using (var reader = XmlReader.Create(sourceRssUrl))
                {
                    var feed = SyndicationFeed.Load(reader);

                    foreach (var item in feed.Items)
                    {
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
        catch (Exception ex)
        {
            Log.Error(ex, $"{nameof(GetAllArticleDataFromDevIoRssAsync)} with arguments SourceGuid {sourceId}, SourceUrl {sourceRssUrl}");
            throw;
        }
        
    }


    #endregion

    #region FixArticleDevIoTextMethods
    private async Task AddArticleTextToArticlesDevIoAsync(Guid sourceId)
    {
        var articlesWithEmptyTextIds = _unitOfWork.Articles.Get()
            .Where(article => article.SourceId.Equals(sourceId) && string.IsNullOrEmpty(article.Text))
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
            Log.Error(ex, $"{nameof(AddArticleTextToArticleDevIoAsync)} with ArticleId {articleId}");
            throw;
        }
    }


    #endregion
}