using System;

namespace Ooui.Forms.Extensions
{
	public static class ColorExtensions
	{
		public static Color ToOouiColor (this Xamarin.Forms.Color color)
		{
			return new Color ((byte)(color.R * 255.0 + 0.5), (byte)(color.G * 255.0 + 0.5), (byte)(color.B * 255.0 + 0.5), (byte)(color.A * 255.0 + 0.5));
		}
	}
}
