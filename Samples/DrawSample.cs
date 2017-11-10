using System;
using System.Collections.Generic;
using System.Linq;
using Ooui;

namespace Samples
{
    public class DrawSample : ISample
    {
        public string Title => "Collaborative Drawing";

        public void Publish ()
        {
            UI.Publish ("/draw", CreateElement ());
        }

        public Element CreateElement ()
        {
            var heading = new Heading ("Draw");
            var subtitle = new Paragraph ("Click to draw a collaborative masterpiece");
            var canvas = new Canvas {
                Width = 320,
                Height = 240,
            };
            var context = canvas.GetContext2D ();

            canvas.Clicked += (s, e) => {
                context.BeginPath ();
                context.Rect (e.OffsetX - 5, e.OffsetY - 5, 10, 10);
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
            clearbtn.Clicked += (s, e) => {
                context.ClearRect (0, 0, canvas.Width, canvas.Height);
            };
            clearbtn.Style.Display = "block";

            var app = new Div ();
            app.AppendChild (heading);
            app.AppendChild (subtitle);
            app.AppendChild (canvas);
            app.AppendChild (clearbtn);
            return app;
        }
    }
}
