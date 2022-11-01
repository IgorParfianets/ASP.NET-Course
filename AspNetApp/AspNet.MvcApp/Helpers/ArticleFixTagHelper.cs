using AspNetArticle.Core.Abstractions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;
using AspNetArticle.Core.DataTransferObjects;

namespace AspNetArticle.MvcApp.Helpers
{
    [HtmlTargetElement("article-fix")]
    public class ArticleFixTagHelper : TagHelper
    {
        public ArticleDto Article { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "div";

            var sb = new StringBuilder();
            sb.Append(Article.Text);

            output.PreContent.SetHtmlContent(sb.ToString());
        }
    }
}
