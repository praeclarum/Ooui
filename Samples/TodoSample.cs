using System;
using System.Collections.Generic;
using System.Linq;
using Ooui;
using Ooui.Html;

namespace Samples
{
    public class TodoSample : ISample
    {
        public string Title => "Todo List";

        class Item : ListItem
        {
            Element label = new Div ();

            bool isDone;
            public bool IsDone {
                get => isDone;
                set {
                    isDone = value;
                    label.Style.TextDecoration = 
                        isDone ? "line-through" : "none";
                    label.Style.FontWeight = 
                        isDone ? "normal" : "bold";
                    label.Style.Color = 
                        isDone ? "#999" : "#000";
                }
            }

            public Item (string text)
            {
                ClassName = "list-group-item";
                Style.Cursor = "pointer";
                label.Text = text;
                label.Style.FontWeight = "bold";
                AppendChild (label);
            }
        }

        Element MakeTodo ()
        {
            List items = new List () {
                ClassName = "list-group",
            };
            items.Style.MarginTop = "1em";

            var heading = new Heading ("Todo List");
            var subtitle = new Paragraph ("This is the shared todo list of the world.");
            var count = new Paragraph ("0 chars");
            var inputForm = new Form {
                ClassName = "form-inline"
            };
            var input = new Input {
                ClassName = "form-control"
            };
            var addbtn = new Button ("Add") {
                Type = ButtonType.Submit,
                ClassName = "btn btn-primary",
            };
            addbtn.Style.MarginLeft = "1em";
            var clearbtn = new Button ("Clear Completed") {
                Type = ButtonType.Submit,
                ClassName = "btn btn-danger",
            };
            void UpdateCount ()
            {
                count.Text = $"{input.Value.Length} chars";
            }
            void AddItem ()
            {
                if (string.IsNullOrWhiteSpace (input.Value))
                    return;
                var item = new Item (input.Value);
                item.Click += (s, e) => {
                    item.IsDone = !item.IsDone;
                };
                items.InsertBefore (item, items.FirstChild);
                input.Value = "";
                UpdateCount ();
            }
            addbtn.Click += (s, e) => {
                AddItem ();
            };
            inputForm.Submit += (s, e) => {
                AddItem ();
            };
            input.KeyUp += (s, e) => {
                UpdateCount ();
            };
            clearbtn.Click += (s, e) => {
                var toremove = new List<Node> ();
                foreach (Item i in items.Children) {
                    if (i.IsDone) toremove.Add (i);
                }
                foreach (var i in toremove) {
                    items.RemoveChild (i);
                }
            };
            var app = new Div ();
            app.AppendChild (heading);
            app.AppendChild (subtitle);
            inputForm.AppendChild (input);
            inputForm.AppendChild (addbtn);
            inputForm.AppendChild (count);
            app.AppendChild (inputForm);
            app.AppendChild (items);
            app.AppendChild (clearbtn);
            return app;
        }

        public void Publish ()
        {
            var b = MakeTodo ();

            UI.Publish ("/todo", MakeTodo);
        }

        public Element CreateElement ()
        {
            return MakeTodo ();
        }
    }
}


