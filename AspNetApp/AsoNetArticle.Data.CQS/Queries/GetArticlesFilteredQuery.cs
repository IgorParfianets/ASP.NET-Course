using AspNetArticle.Core;
using AspNetArticle.Database.Entities;
using MediatR;

namespace AsoNetArticle.Data.CQS.Queries
{
    public class GetArticlesFilteredQuery : IRequest<IEnumerable<Article>>
    {
        public string SelectedCategory { get; set; }
        public string SearchString { get; set; }
        public Raiting SelectedRaiting { get; set; }
    }
}
