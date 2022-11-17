using AspNetArticle.Core.DataTransferObjects;

namespace AspNetArticle.MvcApp.Models
{
    public class AdminPersonalAreaViewModel
    {
        public IEnumerable<UserDto> Users { get; set; }
        public IEnumerable<ArticleDto> Articles { get; set; }
        public IEnumerable<CommentDto> Comments { get; set; }
    }
}
