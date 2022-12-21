using AspBetSample.DataBase.Entities;

namespace AspNetArticle.Database.Entities;

public class User : IBaseEntity
{
    public Guid Id { get; set; }
    public string UserName { get; set; }    
    public string PasswordHash { get; set; }
    public string Email { get; set; }   
    public bool Spam { get; set; }
    public byte[]? Avatar { get; set; }

    public Guid RoleId { get; set; }
    public Role Role { get; set; }

    public DateTime AccountCreated { get; set; }
    public DateTime LastVisit { get;set; }

    public List<Comment> Comments { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; }
}
