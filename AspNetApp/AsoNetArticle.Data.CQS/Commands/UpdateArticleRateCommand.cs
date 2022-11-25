using MediatR;

namespace AsoNetArticle.Data.CQS.Commands
{
    public class UpdateArticleRateCommand : IRequest<int>
    {
        public int ArticleId { get; set; }
        public double Rate { get; set; }
    }
}
