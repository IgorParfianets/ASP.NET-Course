using AspNetArticle.Core.DataTransferObjects;

namespace AspNetArticle.MvcApp.Models
{
    public class ArticleDetailViewModel
    {
        public ArticleModel Article { get; set; }
        public IEnumerable<CommentaryWithUserDto>? ExistComments { get; set; }
        public CreateCommentModel Comment { get; set; }
    }
}
