using AspNetArticle.Core.Abstractions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;
using AspNetArticle.Core.DataTransferObjects;

namespace AspNetArticle.MvcApp.Helpers
{
    [HtmlTargetElement("article-short-description")]
    public class ArticleShortDescriptionTagHelper : TagHelper
    {
        public ArticleDto Article { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "div";

            var sb = new StringBuilder();
            sb.Append(Article.Title);
            sb.Append(Article.ShortDescription);
            sb.Append(Article.PublicationDate.ToString("g"));

            output.PreContent.SetHtmlContent(sb.ToString());
        }
    }
}
