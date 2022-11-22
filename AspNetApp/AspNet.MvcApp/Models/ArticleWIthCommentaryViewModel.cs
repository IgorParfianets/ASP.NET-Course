using AspNetArticle.Core.DataTransferObjects;

namespace AspNetArticle.MvcApp.Models
{
    public class ArticleWIthCommentaryViewModel
    {
        public ArticleModel Article { get; set; }
        public IEnumerable<CommentaryWithUserDto>? ExistComments { get; set; }
        public CommentaryModel Comment { get; set; }
    }
}
