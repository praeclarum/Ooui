using System;

namespace Ooui.Forms.Extensions
{
	public static class ColorExtensions
	{
		public static Color ToOouiColor (this Xamarin.Forms.Color color)
		{
			return new Color ((byte)(color.R * 255.0 + 0.5), (byte)(color.G * 255.0 + 0.5), (byte)(color.B * 255.0 + 0.5), (byte)(color.A * 255.0 + 0.5));
		}

        public static Color ToOouiColor (this Xamarin.Forms.Color color, Xamarin.Forms.Color defaultColor)
        {
            if (color == Xamarin.Forms.Color.Default)
                return defaultColor.ToOouiColor ();
            return color.ToOouiColor ();
        }
	}
}
