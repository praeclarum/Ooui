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

        CompositeOperation compositeOperation = CompositeOperation.SourceOver;
        public CompositeOperation GlobalCompositeOperation
        {
            get => compositeOperation;
            set
            {
                if (value != compositeOperation)
                {
                    compositeOperation = value;
                    switch (value)
                    {
                        case CompositeOperation.SourceOver:
                            SendSet("globalCompositeOperation", "source-over");
                            break;
                        case CompositeOperation.SourceIn:
                            SendSet("globalCompositeOperation", "source-in");
                            break;
                        case CompositeOperation.SourceOut:
                            SendSet("globalCompositeOperation", "source-out");
                            break;
                        case CompositeOperation.SourceAtop:
                            SendSet("globalCompositeOperation", "source-atop");
                            break;
                        case CompositeOperation.DestinationOver:
                            SendSet("globalCompositeOperation", "destination-over");
                            break;
                        case CompositeOperation.DestinationIn:
                            SendSet("globalCompositeOperation", "destination-in");
                            break;
                        case CompositeOperation.DestinationOut:
                            SendSet("globalCompositeOperation", "destination-out");
                            break;
                        case CompositeOperation.DestinationAtop:
                            SendSet("globalCompositeOperation", "destination-atop");
                            break;
                        case CompositeOperation.Lighter:
                            SendSet("globalCompositeOperation", "lighter");
                            break;
                        case CompositeOperation.Copy:
                            SendSet("globalCompositeOperation", "copy");
                            break;
                        case CompositeOperation.Xor:
                            SendSet("globalCompositeOperation", "xor");
                            break;
                        case CompositeOperation.Multiply:
                            SendSet("globalCompositeOperation", "multiply");
                            break;
                        case CompositeOperation.Screen:
                            SendSet("globalCompositeOperation", "screen");
                            break;
                        case CompositeOperation.Overlay:
                            SendSet("globalCompositeOperation", "overlay");
                            break;
                        case CompositeOperation.Darken:
                            SendSet("globalCompositeOperation", "darken");
                            break;
                        case CompositeOperation.Lighten:
                            SendSet("globalCompositeOperation", "lighten");
                            break;
                        case CompositeOperation.ColorDodge:
                            SendSet("globalCompositeOperation", "color-dodge");
                            break;
                        case CompositeOperation.ColorBurn:
                            SendSet("globalCompositeOperation", "color-burn");
                            break;
                        case CompositeOperation.HardLight:
                            SendSet("globalCompositeOperation", "hard-light");
                            break;
                        case CompositeOperation.SoftLight:
                            SendSet("globalCompositeOperation", "soft-light");
                            break;
                        case CompositeOperation.Difference:
                            SendSet("globalCompositeOperation", "difference");
                            break;
                        case CompositeOperation.Exclusion:
                            SendSet("globalCompositeOperation", "exclusion");
                            break;
                        case CompositeOperation.Hue:
                            SendSet("globalCompositeOperation", "hue");
                            break;
                        case CompositeOperation.Saturation:
                            SendSet("globalCompositeOperation", "saturation");
                            break;
                        case CompositeOperation.Color:
                            SendSet("globalCompositeOperation", "color");
                            break;
                        case CompositeOperation.Luminosity:
                            SendSet("globalCompositeOperation", "luminosity");
                            break;
                    }
                }
            }
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

        public void Ellipse(double x, double y, double radiusX, double radiusY, double rotation, double startAngle, double endAngle, bool counterclockwise = false)
        {
            Call("ellipse", x, y, radiusX, radiusY, rotation, startAngle, endAngle, counterclockwise);
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

        public void Rotate(double radians)
        {
            Call("rotate", radians);
        }

        public void Scale(double x, double y)
        {
            Call("scale", x, y);
        }

        public void Translate(double x, double y)
        {
            Call("translate", x, y);
        }

        public void SetTransform(double a, double b, double c, double d, double e, double f)
        {
            Call("setTransform", a, b, c, d, e, f);
        }

        public void ResetTransform()
        {
            Call("resetTransform");
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

    public enum CompositeOperation
    {
        SourceOver,
        SourceIn,
        SourceOut,
        SourceAtop,
        DestinationOver,
        DestinationIn,
        DestinationOut,
        DestinationAtop,
        Lighter,
        Copy,
        Xor,
        Multiply,
        Screen,
        Overlay,
        Darken,
        Lighten,
        ColorDodge,
        ColorBurn,
        HardLight,
        SoftLight,
        Difference,
        Exclusion,
        Hue,
        Saturation,
        Color,
        Luminosity
    }
}
