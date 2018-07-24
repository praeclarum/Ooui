using System;
using Xamarin.Forms.Internals;
using Ooui.Html;

namespace Ooui.Forms
{
    public class DisplayAlert
    {
        private readonly Button _closeButton;
        private readonly Button _acceptButton;
        private readonly Button _cancelButton;

        public DisplayAlert(AlertArguments arguments)
        {
            Element = new Div
            {
                ClassName = "modal-dialog"
            };

            var content = new Div
            {
                ClassName = "modal-content"
            };

            var header = new Div
            {
                ClassName = "modal-header"
            };

            _closeButton = new Button
            {
                ClassName = "close"
            };

            _closeButton.AppendChild(new Span("×"));

            var h4 = new Heading(4)
            {
                Text = arguments.Title
            };

            header.AppendChild(_closeButton);
            header.AppendChild(h4);

            content.AppendChild(header);
            content.AppendChild(new Div()
            {
                ClassName = "modal-body",
                Text = arguments.Message
            });

            if (!string.IsNullOrEmpty(arguments.Cancel))
            {
                var footer = new Div()
                {
                    ClassName = "modal-footer"
                };

                _cancelButton = new Button(arguments.Cancel)
                {
                    ClassName = "btn btn-default"
                };
                _cancelButton.Click += (s, e) => SetResult(false);

                footer.AppendChild(_cancelButton);

                if (!string.IsNullOrEmpty(arguments.Accept))
                {
                    _acceptButton = new Button(arguments.Accept)
                    {
                        ClassName = "btn btn-default"
                    };

                    _acceptButton.Click += (s, e) => SetResult(true);
                    footer.AppendChild(_acceptButton);
                }

                content.AppendChild(footer);
            }

            
            Element.AppendChild(content);

            void SetResult(bool result)
            {
                arguments.SetResult(result);
            }
        }
        
        public event TargetEventHandler Clicked
        {
            add
            {
                _closeButton.Click += value;

                if(_cancelButton != null)
                    _cancelButton.Click += value;

                if(_acceptButton != null)
                    _acceptButton.Click += value;
            }
            remove
            {
                _closeButton.Click -= value;

                if (_cancelButton != null)
                    _cancelButton.Click -= value;

                if (_acceptButton != null)
                    _acceptButton.Click -= value;
            }
        }
        public Element Element { get; private set; } 
    }
}
