using AspNetArticle.Core.DataTransferObjects;
using AspNetArticle.MvcApp.Models.ArticleModels;

namespace AspNetArticle.MvcApp.Models
{
    public class ArticleWIthCommentaryViewModel
    {
        public ArticleDto Article { get; set; }
        public IEnumerable<CommentaryWithUserDto>? ExistComments { get; set; }
        public CommentaryModel Comment { get; set; }
    }
}
