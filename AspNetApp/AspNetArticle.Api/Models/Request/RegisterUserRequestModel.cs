namespace AspNetArticle.Api.Models.Request
{
    public class RegisterUserRequestModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
    }
}
