using AspNetArticle.Core;

namespace AspNetArticle.MvcApp.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool Spam { get; set; }
        public DateTime AccountCreated { get; set; }
        public DateTime LastVisit { get; set; }
        public MembershipStatus Status { get; set; }
        public string? Avatar { get; set; }
    }
}