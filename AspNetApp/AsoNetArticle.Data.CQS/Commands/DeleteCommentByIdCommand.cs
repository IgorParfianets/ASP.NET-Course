using MediatR;

namespace AsoNetArticle.Data.CQS.Commands
{
    public class DeleteCommentByIdCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
