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
        //return _mapper.Map<ArticleDto>(await _unitOfWork.Articles.GetByIdAsync(id));
        return _mapper.Map<ArticleDto>(await _mediator.Send(new GetArticleByIdQuery() { Id = id}));
    }

    public async Task<IEnumerable<ArticleDto>> GetAllArticlesAsync() 
    {
        //var articleDto = await _unitOfWork.Articles
        //    .Get()
        //    .Select(entity => _mapper.Map<ArticleDto>(entity))
        //    .ToListAsync();

        var articleDto = (await _mediator.Send(new GetAllArticlesQuery()))
           .Select(entity => _mapper.Map<ArticleDto>(entity));

        return articleDto;
    }

    //public async Task<List<ArticleDto>> GetArticlesByNameAndSourcesAsync(string? name, Guid? category) // todo can remove unnecessary
    //{
    //    var entities = _unitOfWork.Articles.Get();

    //    if (!string.IsNullOrEmpty(name))
    //    {
    //        entities = entities.Where(ent => ent.Title.Contains(name));
    //    }

    //    if (category != null && !Guid.Empty.Equals(category))
    //    {
    //        entities = entities.Where(ent => ent.SourceId.Equals(category));
    //    }

    //    var result = (await entities.ToListAsync())
    //        .Select(ent => _mapper.Map<ArticleDto>(ent))
    //        .ToList();

    //    return result;
    //}

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

        //if (!string.IsNullOrEmpty(searchString))
        //{
        //    articles = articles.Where(art => art.Title.Contains(searchString));
        //}

        //if (!string.IsNullOrEmpty(selectedCategory) && articles.Any(art => art.Category != null))
        //{
        //    articles = articles.Where(art => art.Category.Equals(selectedCategory));
        //}

        //if (!selectedRaiting.Equals(Raiting.None))
        //{
        //    if (selectedRaiting.Equals(Raiting.TopRaiting))
        //    {
        //        articles = articles.Where(art => art.Rate >= 0).OrderByDescending(art => art.Rate);
        //    }
        //    else
        //    {
        //        articles = articles.Where(art => art.Rate < 0).OrderBy(art => art.Rate);
        //    } 
        //}
        //var result = (await articles.ToArrayAsync())
        //    .Select(ent => _mapper.Map<ArticleDto>(ent))
        ////    .ToArray();
        //var result = articles.Select(ent => _mapper.Map<ArticleDto>(ent))
        //    .ToArray();

        //return result;
    }

    public async Task<IEnumerable<string>> GetArticlesCategoryAsync()
    {
        var categories = await _mediator.Send(new GetArticleCategoriesQuery());

        if(categories == null) 
            throw new ArgumentNullException($"{nameof(categories)} reference null");

        return categories.ToArray();
        //var categories = await _unitOfWork.Articles
        //    .Get()
        //    .Select(art => art.Category)
        //    .Distinct()
        //    .ToListAsync();

        //var articles = await _mediator.Send(new GetAllArticlesQuery());

        //if(articles != null && articles.Any())
        //{
        //    return await articles.Select(art => art.Category)
        //    .Distinct()
        //    .ToArrayAsync();

        //}
        //return await categories ?? Array.Empty<string>();
    }

    public async Task<Guid?> GetArticleIdByCommentId(Guid commentId)
    {
        return await _mediator.Send(new GetArticleIdByCommentIdQuery() { CommentId = commentId });

        //return (await _unitOfWork.Comments
        //    .Get()
        //    .FirstOrDefaultAsync(com => com.Id.Equals(commentId)))
        //    ?.ArticleId;
    }


    public async Task AggregateArticlesFromExternalSourcesAsync()
    {
        var sources = await _mediator.Send(new GetAllSourcesQuery());
        //var sources = await _unitOfWork.Sources.GetAllAsync();

        //Parallel.ForEach(sources, (source) => GetAllArticleDataFromRssAsync(source.Id, source.RssUrl).Wait());

        foreach (var source in sources)
        {
            await GetAllArticleDataFromRssAsync(source.Id, source.RssUrl);
        }
    }

    public async Task AddArticlesDataAsync()
    {
        var articlesWithEmptyTextIds = await _mediator.Send(new GetArticlesIdWithEmptyTextQuery());
        //var articlesWithEmptyTextIds = _unitOfWork.Articles.Get()
        //    .Where(article => string.IsNullOrEmpty(article.Text))
        //    .Select(article => article.Id)
        //    .ToList();

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
                //var oldArticleUrls = await _unitOfWork.Articles.Get()
                //    .Select(article => article.SourceUrl)
                //    .Distinct()
                //    .ToArrayAsync();

                //var entities = list.Where(dto => !oldArticleUrls.Contains(dto.SourceUrl))
                //    .Select(dto => _mapper.Map<Article>(dto)).ToArray();


                //await _unitOfWork.Articles.AddRangeAsync(entities);
                //await _unitOfWork.Commit();
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
            //var article = await _unitOfWork.Articles.GetByIdAsync(articleId);

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
                //if (!string.IsNullOrEmpty(articleText))
                //{
                //    articleText = Regex.Replace(articleText, @"<a([^>]+)>(.+?)<\/a>", " ");

                //    await _unitOfWork.Articles.UpdateArticleTextAsync(articleId, articleText);
                //}

                //if (!string.IsNullOrEmpty(imageUrl))
                //    await _unitOfWork.Articles.UpdateArticleImageUrlAsync(articleId, imageUrl);

                //await _unitOfWork.Commit();
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
                //if (!string.IsNullOrEmpty(articleText))
                //{
                //    await _unitOfWork.Articles.UpdateArticleTextAsync(articleId, articleText);
                //    await _unitOfWork.Commit();
                //}
            }
        }
        catch (Exception e)
        {
            Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
            throw new Exception($"Method {nameof(AddArticleTextToArticles)} is failed, stack trace {e.StackTrace}. {e.Message}");
        }
    }
}