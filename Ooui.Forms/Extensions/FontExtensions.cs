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

        public static Size MeasureSize (this string text, string fontFamily, double fontSize, FontAttributes fontAttrs, double widthConstraint, double heightConstraint)
        {
            if (string.IsNullOrEmpty (text))
                return Size.Zero;

            var fontHeight = fontSize;
            var lineHeight = fontHeight * 1.4;

            var isBold = fontAttrs.HasFlag (FontAttributes.Bold);

            var props = isBold ? BoldCharacterProportions : CharacterProportions;
            var avgp = isBold ? BoldAverageCharProportion : AverageCharProportion;

            var px = 0.0;
            var lines = 1;
            var maxPWidth = 0.0;
            var pwidthConstraint = double.IsPositiveInfinity (widthConstraint) ? double.PositiveInfinity : widthConstraint / fontSize;
            var lastSpaceWidth = -1.0;

            // Tiny little padding to account for sampling errors
            var pwidthHack = 1.0e-6;
            var plineHack = 0.333;

            var n = text != null ? text.Length : 0;

            for (var i = 0; i < n; i++) {
                var c = (int)text[i];
                var pw = (c < 128) ? props[c] : avgp;
                // Should we wrap?
                if (px + pw + plineHack > pwidthConstraint) {
                    lines++;
                    if (lastSpaceWidth > 0) {
                        maxPWidth = Math.Max (maxPWidth, lastSpaceWidth + pwidthHack);
                        px = pw - lastSpaceWidth;
                        lastSpaceWidth = -1;
                    }
                    else {
                        maxPWidth = Math.Max (maxPWidth, px + pwidthHack);
                        px = 0;
                    }
                }
                if (c == ' ') {
                    lastSpaceWidth = pw;
                }
                px += pw;
            }
            maxPWidth = Math.Max (maxPWidth, px + pwidthHack);
            var width = fontSize * maxPWidth;
            var height = lines * lineHeight;

            // Console.WriteLine ($"MEASURE TEXT SIZE {widthConstraint}x{heightConstraint} \"{text}\" == {width}x{height}");

            return new Size (width, height);
        }

        public static Size MeasureSize (this string text, Style style, double widthConstraint, double heightConstraint)
        {
            // System.Console.WriteLine("!!! MEASURE STYLED TEXT SIZE: " + style);
            return MeasureSize (text, "", 14, FontAttributes.None, widthConstraint, heightConstraint);
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

        public static string ToOouiVerticalAlign (this TextAlignment align)
        {
            switch (align) {
                case TextAlignment.Start:
                default:
                    return "top";
                case TextAlignment.Center:
                    return "middle";
                case TextAlignment.End:
                    return "bottom";
            }
        }

        static readonly double[] CharacterProportions = {
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0.27799999713897705, 0.27799999713897705, 0.27799999713897705, 0.27799999713897705, 0.27799999713897705, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            0.27799999713897705, 0.25899994373321533, 0.4259999990463257, 0.5560001134872437, 0.5560001134872437, 1.0000001192092896, 0.6299999952316284, 0.27799999713897705,
            0.25899994373321533, 0.25899994373321533, 0.3520001173019409, 0.6000000238418579, 0.27799999713897705, 0.3890000581741333, 0.27799999713897705, 0.3330000638961792,
            0.5560001134872437, 0.5560001134872437, 0.5560001134872437, 0.5560001134872437, 0.5560001134872437, 0.5560001134872437, 0.5560001134872437, 0.5560001134872437,
            0.5560001134872437, 0.5560001134872437, 0.27799999713897705, 0.27799999713897705, 0.6000000238418579, 0.6000000238418579, 0.6000000238418579, 0.5560001134872437,
            0.8000000715255737, 0.6480001211166382, 0.6850000619888306, 0.722000002861023, 0.7040001153945923, 0.6110001802444458, 0.5740000009536743, 0.7589999437332153,
            0.722000002861023, 0.25899994373321533, 0.5190001726150513, 0.6669999361038208, 0.5560001134872437, 0.8709999322891235, 0.722000002861023, 0.7600001096725464,
            0.6480001211166382, 0.7600001096725464, 0.6850000619888306, 0.6480001211166382, 0.5740000009536743, 0.722000002861023, 0.6110001802444458, 0.9259999990463257,
            0.6110001802444458, 0.6480001211166382, 0.6110001802444458, 0.25899994373321533, 0.3330000638961792, 0.25899994373321533, 0.6000000238418579, 0.5000001192092896,
            0.22200000286102295, 0.5370000600814819, 0.593000054359436, 0.5370000600814819, 0.593000054359436, 0.5370000600814819, 0.2960001230239868, 0.5740000009536743,
            0.5560001134872437, 0.22200000286102295, 0.22200000286102295, 0.5190001726150513, 0.22200000286102295, 0.8530000448226929, 0.5560001134872437, 0.5740000009536743,
            0.593000054359436, 0.593000054359436, 0.3330000638961792, 0.5000001192092896, 0.31500017642974854, 0.5560001134872437, 0.5000001192092896, 0.7580000162124634,
            0.5180000066757202, 0.5000001192092896, 0.4800001382827759, 0.3330000638961792, 0.22200000286102295, 0.3330000638961792, 0.6000000238418579, 0
        };
        const double AverageCharProportion = 0.5131400561332703;

        static readonly double[] BoldCharacterProportions = {
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0.27799999713897705, 0.27799999713897705, 0.27799999713897705, 0.27799999713897705, 0.27799999713897705, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
            0.27799999713897705, 0.27799999713897705, 0.46299993991851807, 0.5560001134872437, 0.5560001134872437, 1.0000001192092896, 0.6850000619888306, 0.27799999713897705,
            0.2960001230239868, 0.2960001230239868, 0.40700018405914307, 0.6000000238418579, 0.27799999713897705, 0.40700018405914307, 0.27799999713897705, 0.37099993228912354,
            0.5560001134872437, 0.5560001134872437, 0.5560001134872437, 0.5560001134872437, 0.5560001134872437, 0.5560001134872437, 0.5560001134872437, 0.5560001134872437,
            0.5560001134872437, 0.5560001134872437, 0.27799999713897705, 0.27799999713897705, 0.6000000238418579, 0.6000000238418579, 0.6000000238418579, 0.5560001134872437,
            0.8000000715255737, 0.6850000619888306, 0.7040001153945923, 0.7410000562667847, 0.7410000562667847, 0.6480001211166382, 0.593000054359436, 0.7589999437332153,
            0.7410000562667847, 0.29499995708465576, 0.5560001134872437, 0.722000002861023, 0.593000054359436, 0.9070001840591431, 0.7410000562667847, 0.777999997138977,
            0.6669999361038208, 0.777999997138977, 0.722000002861023, 0.6490000486373901, 0.6110001802444458, 0.7410000562667847, 0.6299999952316284, 0.9440001249313354,
            0.6669999361038208, 0.6669999361038208, 0.6480001211166382, 0.3330000638961792, 0.37099993228912354, 0.3330000638961792, 0.6000000238418579, 0.5000001192092896,
            0.25899994373321533, 0.5740000009536743, 0.6110001802444458, 0.5740000009536743, 0.6110001802444458, 0.5740000009536743, 0.3330000638961792, 0.6110001802444458,
            0.593000054359436, 0.2580000162124634, 0.27799999713897705, 0.5740000009536743, 0.2580000162124634, 0.906000018119812, 0.593000054359436, 0.6110001802444458,
            0.6110001802444458, 0.6110001802444458, 0.3890000581741333, 0.5370000600814819, 0.3520001173019409, 0.593000054359436, 0.5200001001358032, 0.8140000104904175,
            0.5370000600814819, 0.5190001726150513, 0.5190001726150513, 0.3330000638961792, 0.223000168800354, 0.3330000638961792, 0.6000000238418579, 0
        };
        const double BoldAverageCharProportion = 0.5346300601959229;
    }
}
