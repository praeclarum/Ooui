using System;

namespace Ooui.Forms.Extensions
{
	public static class ColorExtensions
	{
		public static Color ToOouiColor (this Xamarin.Forms.Color color)
		{
            const byte defaultRed = 0;
            const byte defaultGreen = 0;
            const byte defaultBlue = 0;
            const byte defaultAlpha = 255;
            byte r = color.R < 0 ? defaultRed : (byte)(color.R * 255.0 + 0.5);
            byte g = color.G < 0 ? defaultGreen : (byte)(color.G * 255.0 + 0.5);
            byte b = color.B < 0 ? defaultBlue : (byte)(color.B * 255.0 + 0.5);
            byte a = color.A < 0 ? defaultAlpha : (byte)(color.A * 255.0 + 0.5);
            return new Color (r, g, b, a);
		}

        public static Color ToOouiColor (this Xamarin.Forms.Color color, Xamarin.Forms.Color defaultColor)
        {
            if (color == Xamarin.Forms.Color.Default)
                return defaultColor.ToOouiColor ();
            return color.ToOouiColor ();
        }

        public static Color ToOouiColor (this Xamarin.Forms.Color color, Ooui.Color defaultColor)
        {
            if (color == Xamarin.Forms.Color.Default)
                return defaultColor;
            return color.ToOouiColor ();
        }
	}
}
