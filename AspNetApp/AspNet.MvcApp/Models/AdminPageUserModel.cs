using AspNetArticle.Core.DataTransferObjects;

namespace AspNetArticle.MvcApp.Models
{
    public class AdminPageUserModel
    {
        public UserModel User { get; set; }
        public List<CommentDto> Comments { get; set; }
    }
}
