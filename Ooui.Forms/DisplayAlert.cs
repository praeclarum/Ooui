using System.Web;
using Xamarin.Forms.Internals;

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

            _closeButton.AppendChild(new Span(HttpUtility.HtmlDecode("&times;")));

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

                footer.AppendChild(_cancelButton);

                if (!string.IsNullOrEmpty(arguments.Accept))
                {
                    _acceptButton = new Button(arguments.Accept)
                    {
                        ClassName = "btn btn-default"
                    };
                    
                    footer.AppendChild(_acceptButton);
                }

                content.AppendChild(footer);
            }

            Element.AppendChild(content);
        }
        
        public event TargetEventHandler Clicked
        {
            add
            {
                _closeButton.Clicked += value;

                if(_cancelButton != null)
                    _cancelButton.Clicked += value;

                if(_acceptButton != null)
                    _acceptButton.Clicked += value;
            }
            remove
            {
                _closeButton.Clicked -= value;

                if (_cancelButton != null)
                    _cancelButton.Clicked -= value;

                if (_acceptButton != null)
                    _acceptButton.Clicked -= value;
            }
        }
        public Element Element { get; private set; } 
    }
}
