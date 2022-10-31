using AspNetArticle.Core.DataTransferObjects;
using Microsoft.Build.Framework;
using System.Runtime.CompilerServices;

namespace AspNetArticle.MvcApp.Models;

public class ArticleModel 
{
    public Guid Id { get; set; }
    [Required]
    public string Title { get; set; }
    public string Category { get; set; }
    public string ShortDescription { get; set; }
    public string Text { get; set; }
    public DateTime PublicationDate { get; set; }



    //public List<CommentDto> Comments { get; set; }
    //public List<ViewDto> Views { get; set; }
}
