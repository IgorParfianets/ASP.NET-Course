using MediatR;

namespace AsoNetArticle.Data.CQS.Commands
{
    public class UpdateArticleTextAndShortDescriptionOnlinerCommand : IRequest
    {
        public Guid ArticleId { get; set; }
        public string ShortDescription { get; set; }
        public string Text { get; set; }
    }
}
