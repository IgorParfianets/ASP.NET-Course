using AspNetArticle.Core.DataTransferObjects;
using Microsoft.Build.Framework;
using System.Runtime.CompilerServices;

namespace AspNetArticle.MvcApp.Models;

public class ArticleModel
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Category { get; set; }
    public string ShortDescription { get; set; }
    public string Text { get; set; }
    public string? ImageUrl { get; set; }
    public double? Rate { get; set; }
    public DateTime PublicationDate { get; set; }
}
