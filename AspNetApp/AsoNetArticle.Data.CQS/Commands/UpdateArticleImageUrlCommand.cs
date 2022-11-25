
using MediatR;

namespace AsoNetArticle.Data.CQS.Commands
{
    public class UpdateArticleImageUrlOnlinerCommand : IRequest
    {
        public Guid ArticleId { get; set; }
        public string ImageUrl { get; set; }
    }
}
