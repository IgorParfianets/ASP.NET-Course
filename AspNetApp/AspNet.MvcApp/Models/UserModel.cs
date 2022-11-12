namespace AspNetArticle.MvcApp.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime CountDays { get; set; }
    }


    internal enum CountDaysStatus
    {
        Newcomer,
        Local,
        Experienced,
        Veteran
    }
}
