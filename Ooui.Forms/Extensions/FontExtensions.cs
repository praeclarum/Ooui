using System;
using Xamarin.Forms;

namespace Ooui.Forms.Extensions
{
    public static class FontExtensions
    {
        public static void SetStyleFont (this View view, Style style)
        {
        }

        public static string ToOouiTextAlign (this TextAlignment align)
        {
            switch (align) {
                case TextAlignment.Start:
                default:
                    return "start";
                case TextAlignment.Center:
                    return "center";
                case TextAlignment.End:
                    return "end";
            }
        }
    }
}
