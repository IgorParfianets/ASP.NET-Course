using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace AspNetArticle.MvcApp.Helpers
{
    [HtmlTargetElement("article-text")]
    public class ArticleTextTagHelper : TagHelper
    {
        public string ArticleText { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "div";

            var sb = new StringBuilder();
            sb.Append(ArticleText);

            output.PreContent.SetHtmlContent(sb.ToString());
        }
    }
}
