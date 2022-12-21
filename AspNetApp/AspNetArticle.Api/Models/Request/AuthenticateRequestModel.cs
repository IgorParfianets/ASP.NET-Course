namespace AspNetArticle.Api.Models.Request
{
    /// <summary>
    /// Request model for logging (authenticate) user
    /// </summary>
    public class AuthenticateRequestModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
