using AspNetArticle.Core.DataTransferObjects;
using Microsoft.Build.Framework;

namespace AspNetArticle.MvcApp.Models;

public class ArticleModel
{
    public Guid Id { get; set; }
    [Required]
    public string Title { get; set; }
    public string ShortDescription { get; set; }
    public string FullText { get; set; }
    public DateTime PublicationDate { get; set; }



    //public List<CommentDto> Comments { get; set; }
    //public List<ViewDto> Views { get; set; }
}
