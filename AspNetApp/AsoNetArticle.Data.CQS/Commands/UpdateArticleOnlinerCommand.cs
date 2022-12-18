using MediatR;

namespace AsoNetArticle.Data.CQS.Commands
{
    public class UpdateArticleOnlinerCommand : IRequest
    {
        public Guid ArticleId { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }
    }
}
