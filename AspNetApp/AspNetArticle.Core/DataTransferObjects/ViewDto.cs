namespace AspNetArticle.Core.DataTransferObjects;

public class ViewDto
{
    public Guid Id { get; set; }
    public string IpAddress { get; set; }
    public DateTime DateOfView { get; set; }
}
