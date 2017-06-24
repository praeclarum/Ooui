using System;
using System.Collections.Generic;
using Ooui;

namespace Samples
{
    public class TodoSample
    {

        Element MakeTodo ()
        {
            var items = new List ();
            var input = new Input ();
            var button = new Button ("Add the item");
            button.Clicked += (s, e) => {
                items.AppendChild (new ListItem {
                    Text = input.Value
                });
            };
            var app = new Div ();
            app.AppendChild (input);
            app.AppendChild (button);
            app.AppendChild (items);
            return app;
        }

        public void Publish ()
        {
            var b = MakeTodo ();

            UI.Publish ("/todo", MakeTodo);
        }
    }
}


