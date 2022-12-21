using AspNetArticle.Core.Abstractions;
using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Data.Abstractions;
using AspNetArticle.Database.Entities;
using AutoMapper;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System.ServiceModel.Syndication;
using System.Xml;
using Serilog;
using System.Text.RegularExpressions;
using AspNetArticle.Core;
using MediatR;
using AsoNetArticle.Data.CQS.Queries;
using AsoNetArticle.Data.CQS.Commands;

namespace AspNetArticle.Business.Services;

public class ArticleService : IArticleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public ArticleService(IMapper mapper,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }


    public async Task<ArticleDto> GetArticleByIdAsync(Guid id)
    {
        return _mapper.Map<ArticleDto>(await _mediator.Send(new GetArticleByIdQuery() { Id = id}));
    }

    public async Task<IEnumerable<ArticleDto>> GetAllArticlesAsync() 
    {
        var articleDto = (await _mediator.Send(new GetAllArticlesQuery()))
           .Select(entity => _mapper.Map<ArticleDto>(entity));

        return articleDto;
    }

    public async Task<IEnumerable<ArticleDto>> GetFilteredArticles(string selectedCategory, Raiting selectedRaiting, string searchString)
    {
        var articles = await _mediator.Send(new GetArticlesFilteredQuery()
        {
            SelectedCategory = selectedCategory,
            SelectedRaiting = selectedRaiting,
            SearchString = searchString
        });

        if (articles != null && articles.Any())
        {
            return articles.Select(art => _mapper.Map<ArticleDto>(art))
                .ToArray();
        }
        return Array.Empty<ArticleDto>();
    }

    public async Task<IEnumerable<string>> GetArticlesCategoryAsync()
    {
        var categories = await _mediator.Send(new GetArticleCategoriesQuery());

        if(categories == null) 
            throw new ArgumentNullException($"{nameof(categories)} reference null");

        return categories.ToArray();
    }

    public async Task<Guid?> GetArticleIdByCommentId(Guid commentId)
    {
        return await _mediator.Send(new GetArticleIdByCommentIdQuery() { CommentId = commentId });
    }


    public async Task AggregateArticlesFromExternalSourcesAsync()
    {
        var sources = await _mediator.Send(new GetAllSourcesQuery());

        foreach (var source in sources)
        {
            await GetAllArticleDataFromRssAsync(source.Id, source.RssUrl);
        }
    }

    public async Task AddArticlesDataAsync()
    {
        var articlesWithEmptyTextIds = await _mediator.Send(new GetArticlesIdWithEmptyTextQuery());

        foreach (var articleId in articlesWithEmptyTextIds ?? Array.Empty<Guid>())
        {
            await AddArticleTextToArticles(articleId);
        }
    }

    private async Task GetAllArticleDataFromRssAsync(Guid sourceId, string? sourceUrl)
    {
        try
        {
            if (!string.IsNullOrEmpty(sourceUrl))
            {
                var list = new List<ArticleDto>();

                using (var reader = XmlReader.Create(sourceUrl))
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
                            ImageUrl = item.Links.Count > 1 ? item.Links[1].Uri.AbsoluteUri : null
                        };

                        if (!string.IsNullOrEmpty(item.Summary?.Text))
                            articleDto.ShortDescription =
                                Regex.Replace(item.Summary.Text, @"<[^>]+>|&nbsp|\n|Читать далее.", " ").Trim();

                        if (!string.IsNullOrEmpty(articleDto.ShortDescription))
                            list.Add(articleDto);
                    }
                }
                var result = list.Select(art => _mapper.Map<Article>(art));

                await _mediator.Send(new AddArticleDataFromRssFeedCommand() { Articles = result });
            }
        }
        catch (Exception e)
        {
            Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
            throw new Exception($"Method {nameof(GetAllArticleDataFromRssAsync)} is failed, stack trace {e.StackTrace}. {e.Message}");
        }
    }

    private async Task AddArticleTextToArticles(Guid articleId)
    {
        try
        {
            var article = await _mediator.Send(new GetArticleByIdQuery() { Id = articleId });

            if (article == null)
            {
                throw new ArgumentException($"Article with id: {articleId} doesn't exists",
                    nameof(articleId));
            }

            var articleSourceUrl = article.SourceUrl;

            var web = new HtmlWeb();
            var htmlDoc = web.Load(articleSourceUrl);

            bool isOnlinerText = htmlDoc.DocumentNode.Descendants(0).Any(tg => tg.HasClass("news-text"));
            bool isDevIoText = htmlDoc.DocumentNode.Descendants(0).Any(tg => tg.HasClass("article__body"));

            if (isOnlinerText)
            {
                var nodes = htmlDoc.DocumentNode
                     .Descendants(0)
                     .Where(n => n.HasClass("news-text"));

                var imageBlock = htmlDoc.DocumentNode
                    .Descendants(0)
                    .Where(n => n.HasClass("news-header__image"))
                    .FirstOrDefault()?
                    .GetAttributeValue("style", null);

                string imageUrl = string.Empty;

                if (imageBlock != null)
                    imageUrl = Regex.Match(imageBlock, @"http[^>]+jpeg|http[^>]+jpg").Value;

                var articleText = nodes.FirstOrDefault()?
                    .ChildNodes
                    .Where(node => (node.Name.Equals("p")
                    || node.Name.Equals("div")
                    || node.Name.Equals("h2"))
                                   && !node.HasClass("news-reference")
                                   && !node.HasClass("news-banner")
                                   && !node.HasClass("news-widget")
                                   && !node.HasClass("news-vote")
                                   && !node.HasClass("news-incut")
                                   && node.Attributes["style"] == null)
                    .Select(node => node.OuterHtml)
                    .Aggregate((i, j) => i + Environment.NewLine + j);

                await _mediator.Send(new UpdateArticleOnlinerCommand() {
                    ArticleId = articleId,
                    Text = articleText,
                    ImageUrl = imageUrl
                });
            }

            if (isDevIoText)
            {
                var nodes = htmlDoc.DocumentNode
                    .Descendants(0)
                    .Where(n => n.HasClass("article__body"));

                var articleText = (nodes.FirstOrDefault()?.ChildNodes
                           .Where(node => node.Name.Equals("div")))?
                       .FirstOrDefault()?.ChildNodes
                       .Where(node => node.Name.Equals("p")
                       || node.Name.Equals("ol")
                       || node.Name.Equals("ul")
                       || node.Name.Equals("h4")
                       || node.Name.Equals("h3")
                       || node.Name.Equals("figure"))
                       .Select(node => node.OuterHtml)
                       .Aggregate((i, j) => i + Environment.NewLine + j);

                await _mediator.Send(new UpdateArticleDevIoCommand()
                {
                    ArticleId = articleId,
                    Text = articleText,
                });
            }
        }
        catch (Exception e)
        {
            Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
            throw new Exception($"Method {nameof(AddArticleTextToArticles)} is failed, stack trace {e.StackTrace}. {e.Message}");
        }
    }
}