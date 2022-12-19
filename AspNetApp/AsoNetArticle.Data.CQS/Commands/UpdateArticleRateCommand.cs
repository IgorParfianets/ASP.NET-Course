using MediatR;

namespace AsoNetArticle.Data.CQS.Commands
{
    public class UpdateArticleRateCommand : IRequest
    {
        public Guid ArticleId { get; set; }
        public double Rate { get; set; }
    }
}
