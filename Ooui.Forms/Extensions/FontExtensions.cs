using System;
using Xamarin.Forms;

namespace Ooui.Forms.Extensions
{
    public static class FontExtensions
    {
        public static void SetStyleFont (this View view, string family, double size, FontAttributes attrs, Style style)
        {
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
            if (size == 14.0) {
                style.FontSize = null;
            }
            else {
                style.FontSize = size;
            }
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator

            if (string.IsNullOrEmpty (family)) {
                style.FontFamily = null;
            }
            else {
                style.FontFamily = family;
            }

            if (attrs.HasFlag (FontAttributes.Bold)) {
                style.FontWeight = "bold";
            }
            else {
                style.FontWeight = null;
            }

            if (attrs.HasFlag (FontAttributes.Italic)) {
                style.FontStyle = "italic";
            }
            else {
                style.FontStyle = null;
            }
        }

        public static Size MeasureSize (this string text, string fontFamily, double fontSize, FontAttributes fontAttrs)
        {
            if (string.IsNullOrEmpty (text))
                return Size.Zero;

            var fontHeight = fontSize;
            var charWidth = fontSize * 0.5;

            var width = text.Length * charWidth;

            return new Size (width, fontHeight);
        }

        public static Size MeasureSize (this string text, Style style)
        {
            return MeasureSize (text, "", 14, FontAttributes.None);
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
