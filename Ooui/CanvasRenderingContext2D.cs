using System;

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

        // https://www.w3.org/TR/2dcontext/#canvaspathmethods

        public void Save ()
        {
            Call ("save");
        }

        public void Restore ()
        {
            Call ("restore");
        }

        public void ClearRect (double x, double y, double w, double h)
        {
            Call ("clearRect", x, y, w, h);
        }

        public void FillRect (double x, double y, double w, double h)
        {
            Call ("fillRect", x, y, w, h);
        }

        public void StrokeRect (double x, double y, double w, double h)
        {
            Call ("strokeRect", x, y, w, h);
        }

        public void BeginPath ()
        {
            Call ("beginPath");
        }

        public void ClosePath ()
        {
            Call ("closePath");
        }

        public void MoveTo (double x, double y)
        {
            Call ("moveTo", x, y);
        }

        public void LineTo (double x, double y)
        {
            Call ("lineTo", x, y);
        }

        public void QuadraticCurveTo (double cpx, double cpy, double x, double y)
        {
            Call ("quadraticCurveTo", cpx, cpy, x, y);
        }

        public void BezierCurveTo (double cp1x, double cp1y, double cp2x, double cp2y, double x, double y)
        {
            Call ("bezierCurveTo", cp1x, cp1y, cp2x, cp2y, x, y);
        }

        public void ArcTo (double x1, double y1, double x2, double y2, double radius)
        {
            Call ("arcTo", x1, y1, x2, y2, radius);
        }

        public void Rect (double x, double y, double w, double h)
        {
            Call ("rect", x, y, w, h);
        }

        public void Arc (double x, double y, double radius, double startAngle, double endAngle, bool counterclockwise)
        {
            Call ("arc", x, y, radius, startAngle, endAngle, counterclockwise);
        }

        public void Fill ()
        {
            Call ("fill");
        }

        public void Stroke ()
        {
            Call ("stroke");
        }

        public void Clip ()
        {
            Call ("clip");
        }

        public void FillText (string text, double x, double y, double? maxWidth)
        {
            Call ("fillText", text, x, y, maxWidth);
        }

        public void StrokeText (string text, double x, double y, double? maxWidth)
        {
            Call ("strokeText", text, x, y, maxWidth);
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
