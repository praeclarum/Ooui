using System;
using System.Collections.Generic;
using System.Linq;
using Ooui;

namespace Samples
{
    public class DrawSample : ISample
    {
        public string Title => "Drawing";

        public void Publish ()
        {
            UI.Publish ("/draw", CreateElement ());
        }

        public Element CreateElement ()
        {
            var heading = new Heading ("Draw");
            var subtitle = new Paragraph ("Click to draw a masterpiece");
            var toolSel = new Select ();
            toolSel.AppendChild (new Option { Label = "Boxes", Value = "box" });
            toolSel.AddOption ("Circles", "circle");
            var canvas = new Canvas {
                Width = 320,
                Height = 240,
            };
            var context = canvas.GetContext2D ();

            canvas.Click += (s, e) => {
                var radius = 10;
                context.BeginPath ();
                if (toolSel.Value == "box") {
                    context.Rect (e.OffsetX - radius, e.OffsetY - radius, 2*radius, 2*radius);
                }
                else {
                    context.Arc (e.OffsetX, e.OffsetY, radius, 0, 2 * Math.PI, true);
                }
                context.Fill ();
            };
            canvas.Style.Cursor = "pointer";
            canvas.Style.BorderColor = "#CCC";
            canvas.Style.BorderStyle = "solid";
            canvas.Style.BorderWidth = "1px";

            var clearbtn = new Button ("Clear") {
                Type = ButtonType.Submit,
                ClassName = "btn btn-danger",
            };
            clearbtn.Click += (s, e) => {
                context.ClearRect (0, 0, canvas.Width, canvas.Height);
            };
            clearbtn.Style.Display = "block";

            var app = new Div ();
            app.AppendChild (heading);
            app.AppendChild (subtitle);
            app.AppendChild (new Div (toolSel));
            app.AppendChild (canvas);
            app.AppendChild (clearbtn);
            return app;
        }
    }
}
