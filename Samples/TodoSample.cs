using System;
using System.Collections.Generic;
using Ooui;

namespace Samples
{
    public class TodoSample
    {
        List items = new List ();

        class Item : ListItem
        {
            public Item (string text)
            {
                var check = new Input {
                    Type = InputType.Checkbox,
                };
                var label = new Label {
                    Text = text,
                    For = check
                };
                check.Changed += (s,e) => {
                    label.Style.TextDecoration = 
                        check.IsChecked ? "line-through" : "none";
                };
                AppendChild (check);
                AppendChild (label);
            }
        }

        Element MakeTodo ()
        {            
            var input = new Input ();
            var button = new Button ("Add the item");
            button.Clicked += (s, e) => {
                if (string.IsNullOrWhiteSpace (input.Value))
                    return;
                var item = new Item (input.Value);
                items.AppendChild (item);
                input.Value = "";
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


