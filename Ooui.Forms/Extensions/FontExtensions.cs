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

        public static Size MeasureSize (this string text, Style style)
        {
            if (string.IsNullOrEmpty (text))
                return Size.Zero;

            var fontHeight = 16.0;
            var charWidth = 8.0;

            var width = text.Length * charWidth;

            return new Size (width, fontHeight);
        }
    }
}
