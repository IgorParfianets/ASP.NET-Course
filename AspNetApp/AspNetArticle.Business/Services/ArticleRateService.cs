﻿using AspNetArticle.Data.Abstractions;
using System.Net.Http.Json;
using AspNetSample.Business.Models;
using AspNetSample.Core;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using AspNetArticle.Core.Abstractions;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using Serilog;
using MediatR;
using AsoNetArticle.Data.CQS.Handers.QueryHanders;
using AsoNetArticle.Data.CQS.Queries;
using AsoNetArticle.Data.CQS.Commands;

namespace AspNetArticle.Business.Services
{
    public class ArticleRateService : IArticleRateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public ArticleRateService(IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mediator = mediator;
        }


        public async Task AddRateToArticlesAsync()
        {
            try
            {
                var articlesWithEmptyRateIds = await _mediator.Send(new GetArticlesIdWithEmptyRateQuery());

                foreach (var articleId in articlesWithEmptyRateIds ?? Array.Empty<Guid>())
                {
                    string articleFixedText = await RemoveHtmlTagsFromArticleTestAsync(articleId);
                    await RateArticleAsync(articleId, articleFixedText);

                }
            }
            catch (Exception e)
            {
                Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
                throw new Exception($"Method {nameof(AddRateToArticlesAsync)} is failed, stack trace {e.StackTrace}. {e.Message}");
            }
        }

        private async Task<string> RemoveHtmlTagsFromArticleTestAsync(Guid articleId)
        {
            var text = (await _mediator.Send(new GetArticleByIdQuery() { Id = articleId }))?.Text;

            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException($"Text with id: {articleId} doesn't exists",
                    nameof(articleId));
            }

            var textWithoutHtml = Regex.Replace(text, @"<[^>]+>|&nbsp;|\n;", " ")
                .Trim()
                .ToLower();

            if (string.IsNullOrEmpty(textWithoutHtml))
                throw new ArgumentNullException($"String {textWithoutHtml} invalid");
            
            return textWithoutHtml;
        }

        private async Task RateArticleAsync(Guid articleId, string articleFixedText)
        {
            try
            {
                if (string.IsNullOrEmpty(articleFixedText))
                    throw new Exception();

                using (var client = new HttpClient())
                {
                    var isprasUrl = _configuration["IsprasUrl"];
                    var affinPath = _configuration["AffinPath"];

                    var httpRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(@isprasUrl));

                    httpRequest.Headers.Add("Accept", "application/json");
                    httpRequest.Content = JsonContent.Create(new[] { new TextRequestModel() { Text = articleFixedText } });

                    var response = await client.SendAsync(httpRequest);
                    Log.Information($"Responce from Isprass Api successfully. {response}");

                    var responseStr = await response.Content.ReadAsStreamAsync();

                    
                    using (var sr = new StreamReader(responseStr))
                    {
                        var data = await sr.ReadToEndAsync();
                        var isprassResponce = JsonConvert.DeserializeObject<IsprassResponseObject[]>(data);

                        var affinJsonText = await File.ReadAllTextAsync(@affinPath);
                        var affinJsonObject = Affin.FromJson(affinJsonText);
                        
                        if (isprassResponce != null && affinJsonObject.Any())
                        {
                            Log.Information($"IsprassJsonResponce and AffinJsonText converted to object successfully.");
                            double overallRate = 0 , resultRate = 0;
                            int numberRecognizedWords = 0;

                            foreach (var lem in isprassResponce[0].Annotations.Lemma) 
                            { 
                                long? temp = 0;
                                affinJsonObject.TryGetValue(lem.Value, out temp);

                                if (temp != null)
                                {
                                    overallRate += (double)temp;
                                    numberRecognizedWords++;
                                }
                            }
                            
                            if(numberRecognizedWords > 0)
                                resultRate = overallRate / numberRecognizedWords;

                            await _mediator.Send(new UpdateArticleRateCommand() { ArticleId = articleId, Rate = resultRate });
                            Thread.Sleep(500);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error: {e.Message}. StackTrace: {e.StackTrace}, Source: {e.Source}");
                throw new Exception($"Method {nameof(RateArticleAsync)} is failed, stack trace {e.StackTrace}. {e.Message}");
            }
        }
    }
}
