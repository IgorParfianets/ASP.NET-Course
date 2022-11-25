using AsoNetArticle.Data.CQS.Queries;
using AspNetArticle.Core.DataTransferObjects;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace AspNetArticle.Business.API
{
    public class ArticleService
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public ArticleService(IMapper mapper,
            IMediator mediator,
            IConfiguration configuration)
        {
            _mapper = mapper;
            _mediator = mediator;
            _configuration = configuration;
        }

        public async Task<ArticleDto> GetArticleByIdAsync(Guid articleId)
        {
            return _mapper.Map<ArticleDto>(await _mediator.Send(new GetArticleByIdQuery() { Id = articleId }));
        }


    }
}