namespace AspNetArticle.Database.Entities;

public class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; }    
    public string Password { get; set; }
    public string Email { get; set; }   
    public bool Spam { get; set; }
    public DateTime AccountCreated { get; set; }
    public DateTime LastVisit { get;set; }

    public List<Comment> Comments { get; set; } 
    public List<View> Views { get; set; }
}
