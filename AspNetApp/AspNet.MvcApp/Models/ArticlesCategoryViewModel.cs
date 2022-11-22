using AspNetArticle.Core.DataTransferObjects;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AspNetArticle.MvcApp.Models
{
    public class ArticlesCategoryViewModel
    {
        public List<ArticleModel>? Articles { get; set; }
        public SelectList? Categories { get; set; }
        public string? ArticleCategory { get; set; }
        public string? SearchString { get; set; }
    }
}
