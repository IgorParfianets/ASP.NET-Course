namespace AspNetArticle.Api.Models.Request
{
    /// <summary>
    /// Request model for updating user
    /// </summary>
    public class UpdateUserRequestModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string PasswordConfirmation { get; set; }
        public string NewPassword { get; set; }
        public bool Spam { get; set; }
        //public string? Avatar { get; set; }
    }
}
