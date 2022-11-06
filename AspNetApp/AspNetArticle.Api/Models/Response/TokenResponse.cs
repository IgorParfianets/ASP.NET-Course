namespace AspNetArticle.Api.Models.Response
{
    public class TokenResponse
    {
        public string AccessToken { get; set; }
        public string Role { get; set; }
        public Guid UserId { get; set; }
        public DateTime TokenExpiration { get; set; }
    }
}
