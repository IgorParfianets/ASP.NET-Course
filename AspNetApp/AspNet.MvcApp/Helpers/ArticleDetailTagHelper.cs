using AspNetArticle.Core.DataTransferObjects;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;
using AspNetArticle.MvcApp.Models.ArticleModels;

namespace AspNetArticle.MvcApp.Helpers
{
    [HtmlTargetElement("article-detail")]
    public class ArticleDetailTagHelper : TagHelper
    {
        public ArticleDto Article { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output) 
        {
            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "div";

            var sb = new StringBuilder();
            sb.Append(
                "<div class= \"px-3 text-center col-sm\">" +
                "<div class=\"my-3\">" +
                "<h3>");
            sb.Append(Article.Title);

            sb.Append("</h3><p>");
            sb.Append(Article.PublicationDate.ToString("g"));


            sb.Append("</p></div><div class = \"d-block\" style=width: 80%; height: 300px;>");
            sb.Append(Article.Text);
            sb.Append("</div></div>");

            output.PreContent.SetHtmlContent(sb.ToString());
        }
    }
}
