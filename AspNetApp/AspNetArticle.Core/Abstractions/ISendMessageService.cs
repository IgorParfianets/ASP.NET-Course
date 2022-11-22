namespace AspNetArticle.Core.Abstractions
{
    public interface ISendMessageService
    {
        Task GetArticlesAndUsersForMessage();
    }
}
