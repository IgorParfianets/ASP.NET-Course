using AspNetArticle.Core.DataTransferObjects;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace AspNetArticle.MvcApp.Helpers
{
    [HtmlTargetElement("test-index")]
    public class TestTagHelper : TagHelper
    {
        public ArticleDto Article { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output) // px-3 pt-md-5 px-md-5
        {
            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "div";

            var sb = new StringBuilder();
            sb.Append(
                "<div class= \"px-3 text-center overflow-hidden col-sm\">" +
                "<div class=\"my-3\">" +
                "<h3>");
            sb.Append(Article.Title);

            sb.Append("</h3><p>");
            sb.Append(Article.PublicationDate.ToString("g"));
            

            sb.Append("</p></div><div class = \"d-block overflow-hidden text-truncate\" style=width: 80%; height: 300px;>");
            sb.Append(Article.ShortDescription);
            sb.Append("</div></div>");
            output.PreContent.SetHtmlContent(sb.ToString());
        }
    }
}

/*
            <div class="text-bg-dark me-md-3 pt-3 px-3 pt-md-5 px-md-5 text-center overflow-hidden col-sm">
                <div class="my-3 py-3">

                    <h2 class="display-5">Another headline</h2> <!--Title-->
                    <p class=lead>And an even wittier subheading.</p> <!--Publication Date-->
                </div>
                <div class=bg-light shadow-sm mx-auto style=width: 80%; height: 300px; border-radius: 21px 21px 0 0;>
                    <!--Short Description-->
                </div>
            </div>
*/