using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.Database.Entities;
using MediatR;

namespace AsoNetArticle.Data.CQS.Commands
{
    public class AddArticleDataFromRssFeedCommand : IRequest
    {
        public IEnumerable<Article> Articles { get; set; }
    }
}
