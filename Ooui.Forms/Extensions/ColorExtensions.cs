using System;

namespace Ooui.Forms.Extensions
{
	public static class ColorExtensions
	{
		static Color ToOouiColor (ref Xamarin.Forms.Color color)
		{
            byte r = (byte)(color.R * 255.0 + 0.5);
            byte g = (byte)(color.G * 255.0 + 0.5);
            byte b = (byte)(color.B * 255.0 + 0.5);
            byte a = (byte)(color.A * 255.0 + 0.5);
            return new Color (r, g, b, a);
		}

        public static Color ToOouiColor (this Xamarin.Forms.Color color, Xamarin.Forms.Color defaultColor)
        {
            if (color == Xamarin.Forms.Color.Default)
                return ToOouiColor (ref defaultColor);
            return ToOouiColor (ref color);
        }

        public static Color ToOouiColor (this Xamarin.Forms.Color color, Ooui.Color defaultColor)
        {
            if (color == Xamarin.Forms.Color.Default)
                return defaultColor;
            return ToOouiColor (ref color);
        }
	}
}
