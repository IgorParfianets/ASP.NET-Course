using System.Text.Json;

namespace AspNetArticle.Core.DataTransferObjects;

public class UserDto   // Можно отказаться от некоторых полей
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public bool Spam { get; set; }
    
}
