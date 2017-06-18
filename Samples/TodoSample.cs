using System;
using Ooui;

namespace Samples
{
    public class TodoSample
    {
        Button MakeTodo ()
        {
            var button = new Button ("Click me!");
            button.Style.FontSize = 100;
            var count = 0;
            button.Clicked += (s, e) => {
                button.Style.FontSize = (int)button.Style.FontSize + 1;
                count++;
                button.Text = $"Clicked {count} times";
            };
            return button;
        }

        public void Publish ()
        {
            var b = MakeTodo ();

            UI.Publish ("/todo", MakeTodo);
        }
    }
}


