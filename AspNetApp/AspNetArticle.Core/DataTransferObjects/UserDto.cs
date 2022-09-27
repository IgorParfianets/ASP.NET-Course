using System.Text.Json;

namespace AspNetArticle.Core.DataTransferObjects;

public class UserDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public bool Spam { get; set; }
    public DateTime AccountCreated { get; set; }
    public DateTime LastVisit { get; set; }
    public List<CommentDto> Comments { get; set; }
    
}
