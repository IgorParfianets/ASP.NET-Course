using AspNetArticle.Database.Entities;
using MediatR;

namespace AsoNetArticle.Data.CQS.Queries
{
    public class GetArticleByIdQuery : IRequest<Article>
    {
        public Guid Id { get; set; }
    }
}
