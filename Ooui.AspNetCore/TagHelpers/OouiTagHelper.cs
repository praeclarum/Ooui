using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ooui.AspNetCore.TagHelpers
{
    public class OouiTagHelper : TagHelper
    {
        public Ooui.Html.Element Element { get; set; }

        public override void Process (TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetHtmlContent (Element.OuterHtml);
        }
    }
}
