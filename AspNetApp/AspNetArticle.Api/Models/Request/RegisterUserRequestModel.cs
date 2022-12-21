namespace AspNetArticle.Api.Models.Request
{
    /// <summary>
    /// Request model for registration user
    /// </summary>
    public class RegisterUserRequestModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
        public bool Spam { get; set; }
    }
}
