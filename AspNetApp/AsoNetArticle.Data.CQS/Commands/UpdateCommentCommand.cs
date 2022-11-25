using AspNetArticle.Database.Entities;
using MediatR;

namespace AsoNetArticle.Data.CQS.Commands
{
    public class UpdateCommentCommand : IRequest<int>
    {
        public Comment Comment { get; set; }
    }
}
