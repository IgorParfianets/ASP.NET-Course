using AspNetArticle.Database.Entities;
using MediatR;


namespace AsoNetArticle.Data.CQS.Handers
{
    public class AddCommentCommand : IRequest
    {
        public Comment Comment { get; set; }
    }
}
