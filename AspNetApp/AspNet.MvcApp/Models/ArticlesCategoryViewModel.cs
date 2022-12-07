using AspNetArticle.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AspNetArticle.MvcApp.Models
{
    public class ArticlesCategoryViewModel
    {
        public List<ArticleModel>? Articles { get; set; }
        public SelectList? Categories { get; set; }
        public SelectList? Raiting { get; set; }
        public Raiting SelectedRaiting { get; set; }
        public string? SelectedCategory { get; set; } 
        public string? SearchString { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
}
