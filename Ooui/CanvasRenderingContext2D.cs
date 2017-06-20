namespace Ooui
{
    public class CanvasRenderingContext2D : EventTarget
    {
        object fillStyle = "#000";
        public object FillStyle {
            get => fillStyle;
            set => SetProperty (ref fillStyle, value, "fillStyle");
        }

        double fontSize = 10;
        public double FontSize {
            get => fontSize;
            set {
                if (fontSize != value) {
                    fontSize = value;
                    SetProperty (ref font, GetFont (), "font", "Font");
                    OnPropertyChanged ("FontSize");
                }
            }
        }
        string fontFamily = "sans-serif";
        public string FontFamily {
            get => fontFamily;
            set {
                if (fontFamily != value) {
                    fontFamily = value ?? "sans-serif";
                    SetProperty (ref font, GetFont (), "font", "Font");
                    OnPropertyChanged ("FontFamily");
                }
            }
        }
        string font = "10px sans-serif";
        public string Font => font;
        string GetFont ()
        {
            var size = FontSize.ToString (System.Globalization.CultureInfo.InvariantCulture);
            return $"{size}px {FontFamily}";
        }

        double globalAlpha = 1;
        public double GlobalAlpha {
            get => globalAlpha;
            set => SetProperty (ref globalAlpha, value, "globalAlpha");
        }

        LineCap lineCap = LineCap.Butt;
        public LineCap LineCap {
            get => lineCap;
            set => SetProperty (ref lineCap, value, "lineCap");
        }

        LineJoin lineJoin = LineJoin.Miter;
        public LineJoin LineJoin {
            get => lineJoin;
            set => SetProperty (ref lineJoin, value, "lineJoin");
        }

        double lineWidth = 1;
        public double LineWidth {
            get => lineWidth;
            set => SetProperty (ref lineWidth, value, "lineWidth");
        }

        object strokeStyle = "#000";
        public object StrokeStyle {
            get => strokeStyle;
            set => SetProperty (ref strokeStyle, value, "strokeStyle");
        }

        public CanvasRenderingContext2D ()
            : base ("#canvasRenderingContext2D")
        {
            UpdateStateMessages (ms => ms.Clear ());
        }
    }

    public enum LineCap
    {
        Butt,
        Round,
        Square
    }

    public enum LineJoin
    {
        Bevel,
        Round,
        Miter
    }
}
