using AspNetArticle.Core.DataTransferObjects;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace AspNetArticle.MvcApp.Helpers
{
    [HtmlTargetElement("article-text")]
    public class ArticleTextTagHelper : TagHelper
    {
        public ArticleDto Article { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "div";

            var sb = new StringBuilder();
            sb.Append(Article.Title);
            sb.Append(Article.Text);
            sb.Append(Article.PublicationDate.ToString("g"));
            

            output.PreContent.SetHtmlContent(sb.ToString());
        }
    }
}
