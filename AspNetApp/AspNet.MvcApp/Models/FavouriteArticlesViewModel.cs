namespace AspNetArticle.MvcApp.Models
{
    public class FavouriteArticlesViewModel
    {
        public IEnumerable<ArticleModel> Articles { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
}
