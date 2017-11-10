using System;

using StyleValue = System.Object;

namespace Ooui
{
    public struct Color
    {
        public byte R, G, B, A;

        public Color (byte r, byte g, byte b, byte a)
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}

        public double Red {
            get => R / 255.0;
            set => R = value >= 1.0 ? (byte)255 : ((value <= 0.0) ? (byte)0 : (byte)(value * 255.0 + 0.5));
        }
        public double Green {
            get => G / 255.0;
            set => G = value >= 1.0 ? (byte)255 : ((value <= 0.0) ? (byte)0 : (byte)(value * 255.0 + 0.5));
        }
        public double Blue {
            get => B / 255.0;
            set => B = value >= 1.0 ? (byte)255 : ((value <= 0.0) ? (byte)0 : (byte)(value * 255.0 + 0.5));
        }
        public double Alpha {
            get => A / 255.0;
            set => A = value >= 1.0 ? (byte)255 : ((value <= 0.0) ? (byte)0 : (byte)(value * 255.0 + 0.5));
        }

		public static Color FromStyleValue (StyleValue styleColor)
		{
            return Colors.Clear;
		}
	}
}
