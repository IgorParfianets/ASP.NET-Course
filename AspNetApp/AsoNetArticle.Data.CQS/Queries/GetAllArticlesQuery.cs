using AspNetArticle.Database.Entities;
using MediatR;

namespace AsoNetArticle.Data.CQS.Queries
{
    public class GetAllArticlesQuery : IRequest<IQueryable<Article>>
    {
    }
}
