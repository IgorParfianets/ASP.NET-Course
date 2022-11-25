namespace AspNetArticle.Core.DataTransferObjects
{
    public class CommentaryWithUserDto
    {
        public Guid CommentId { get; set; }
        public Guid UserId { get; set; }
        public string CommentDescription { get; set; }
        public DateTime PublishedDate { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime AccountCreated { get; set; }
        public bool IsEdited { get; set; }
    }
}
