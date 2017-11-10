using System;
using Newtonsoft.Json;
using StyleValue = System.Object;

namespace Ooui
{
    [Newtonsoft.Json.JsonConverter (typeof (ColorJsonConverter))]
    public struct Color : IEquatable<Color>
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

        public override bool Equals (object obj)
        {
            if (obj is Color other)
                return R == other.R && G == other.G && B == other.B && A == other.A;
            return false;
        }

        public bool Equals (Color other) => R == other.R && G == other.G && B == other.B && A == other.A;

        public override int GetHashCode () => R.GetHashCode () + G.GetHashCode () * 2 + B.GetHashCode () * 3 + A.GetHashCode () * 5;

        public static Color FromStyleValue (StyleValue styleColor)
        {
            if (styleColor is Color c)
                return c;
            if (styleColor is string s)
                return Parse (s);
            return Colors.Clear;
        }

        public static Color Parse (string styleValue)
        {
            if (string.IsNullOrWhiteSpace (styleValue) || styleValue.Length < 4)
                throw new ArgumentException ("Cannot parse empty strings", nameof (styleValue));
            
            if (styleValue.Length > 32)
                throw new ArgumentException ("Color string is too long", nameof (styleValue));

            if (styleValue == "inherit")
                return Colors.Clear;
            
            //if (styleValue[0] == '#' && styleValue.Length == 4) {
            //}

            //if (styleValue[0] == '#' && styleValue.Length == 7) {
            //}

            throw new ArgumentException ($"Cannot parse color string `{styleValue}`", nameof (styleValue));
        }

        public override string ToString ()
        {
            if (A == 255)
                return string.Format ("#{0:x2}{1:x2}{2:x2}", R, G, B);
            return string.Format ("rgba({0},{1},{2},{3})", R, G, B, A / 255.0);
        }
    }

    class ColorJsonConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert (Type objectType)
        {
            return objectType == typeof (Color);
        }

        public override object ReadJson (JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var str = reader.ReadAsString ();
            return Color.Parse (str);
        }

        public override void WriteJson (JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue (value.ToString ());
        }
    }
}
