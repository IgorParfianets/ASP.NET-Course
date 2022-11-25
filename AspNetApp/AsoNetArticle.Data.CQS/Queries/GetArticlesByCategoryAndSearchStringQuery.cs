using AspNetArticle.Database.Entities;
using MediatR;

namespace AsoNetArticle.Data.CQS.Queries
{
    public class GetArticlesByCategoryAndSearchStringQuery : IRequest<IEnumerable<Article>>
    {
        public string Category { get; set; }
        public string SearchString { get; set; }
    }
}
