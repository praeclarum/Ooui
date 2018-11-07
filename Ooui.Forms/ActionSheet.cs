using System;
using System.Collections.Generic;
using Xamarin.Forms.Internals;

namespace Ooui.Forms
{
    public class ActionSheet
    {
        private readonly Button _cancelButton;
        private readonly List<Button> _buttons;

        public Element Element { get; private set; }

        public ActionSheet(ActionSheetArguments arguments)
        {
            Element = new Div();
            Element.ClassName = "modal-dialog";

            var content = new Div();
            content.ClassName = "modal-content";

            var header = new Div();
            header.ClassName = "modal-header";

            var h4 = new Heading(4)
            {
                Text = arguments.Title
            };

            header.AppendChild(h4);

            content.AppendChild(header);

            var body = new Div();
            body.ClassName = "modal-body";

            content.AppendChild(body);

            _buttons = new List<Button>();

            foreach (var button in arguments.Buttons)
            {
                var btnBody = new Div();
                btnBody.Style.MarginBottom = 5;

                var btn = new Button(button);
                btn.Style.Width = "100%";
                btn.ClassName = "btn";
                btn.Click += (s,e) =>  SetResult(button);

                _buttons.Add(btn);

                btnBody.AppendChild(btn);
                body.AppendChild(btnBody);
            }

            if (!string.IsNullOrEmpty(arguments.Cancel))
            {
                var footer = new Div();
                footer.ClassName = "modal-footer";

                _cancelButton = new Button(arguments.Cancel);
                _cancelButton.ClassName = "btn -btn-default";
                _cancelButton.Click += (s, e) => SetResult(arguments.Cancel);

                footer.AppendChild(_cancelButton);

                content.AppendChild(footer);
            }

            Element.AppendChild(content);

            void SetResult(string result)
            {
                arguments.SetResult(result);
            }
        }

        public event TargetEventHandler Clicked
        {
            add
            {
                _cancelButton.Click += value;
                foreach (var btn in _buttons)
                {
                    btn.Click += value;
                }

            }
            remove
            {
                _cancelButton.Click -= value;
                foreach (var btn in _buttons)
                {
                    btn.Click -= value;
                }
            }
        }
    }
}
