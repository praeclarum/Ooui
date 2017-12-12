using System;
using Ooui;

namespace Samples
{
    public class ButtonSample : ISample
    {
        public string Title => "Button Counter";

        Button MakeButton ()
        {
            var button = new Button ("Click me!") {
                ClassName = "btn btn-primary", // Some bootstrap styling
            };
            button.Style.MarginTop = "2em";
            var count = 0;
            button.Click += (s, e) => {
                count++;
                button.Text = $"Clicked {count} times";
            };
            return button;
        }

        public void Publish ()
        {
            var b = MakeButton ();

            UI.Publish ("/shared-button", b);
            UI.Publish ("/button", MakeButton);
        }

        public Element CreateElement ()
        {
            return MakeButton ();
        }
    }
}


