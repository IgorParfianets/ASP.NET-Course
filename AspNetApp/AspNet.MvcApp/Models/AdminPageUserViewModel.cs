using AspNetArticle.Core.DataTransferObjects;

namespace AspNetArticle.MvcApp.Models
{
    public class AdminPageUserViewModel
    {
        public UserModel User { get; set; }
        public List<CommentDto> Comments { get; set; }
    }
}
