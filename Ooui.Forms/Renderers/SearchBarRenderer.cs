using Ooui.Forms.Extensions;
using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace Ooui.Forms.Renderers
{
    public class SearchBarRenderer : ViewRenderer<SearchBar, Div>
    {
        Input _searchBar;
        Button _searchButton;
        bool _disposed;

        IElementController ElementController => Element as IElementController;

        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            var text = Element.Text;
            if (text == null || text.Length == 0)
            {
                text = Element.Placeholder;
            }
            Size size;
            if (text == null || text.Length == 0)
            {
                size = new Size(Element.FontSize * 0.25, Element.FontSize);
            }
            else
            {
                size = text.MeasureSize(Element.FontFamily, Element.FontSize, Element.FontAttributes);
            }
            size = new Size(size.Width, size.Height * 1.428 + 14);
            return new SizeRequest(size, size);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;

            if (disposing)
            {
                if (Control != null && _searchBar != null && _searchButton != null)
                {
                    _searchBar.Change -= OnChange;
                    _searchButton.Click -= OnClick;
                }
            }

            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<SearchBar> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
                return;

            if (Control == null)
            {
                var p = new Div { ClassName = "input-group" };
                var pb = new Span { ClassName = "input-group-btn" };
                _searchButton = new Button { ClassName = "btn btn-secondary", Text = "Search" };
                pb.AppendChild(_searchButton);
                _searchBar = new Input
                {
                    ClassName = "form-control",
                    Type = InputType.Text
                };

                p.AppendChild(_searchBar);
                p.AppendChild(pb);
          
                _searchBar.Change += OnChange;
                _searchButton.Click += OnClick;

                SetNativeControl(p);
            }

            UpdateText();
            UpdateTextColor();
            UpdatePlaceholder();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == SearchBar.TextProperty.PropertyName)
                UpdateText();
            else if (e.PropertyName == SearchBar.PlaceholderProperty.PropertyName)
                UpdatePlaceholder();
            else if (e.PropertyName == SearchBar.PlaceholderColorProperty.PropertyName)
                UpdatePlaceholder();
            else if(e.PropertyName == SearchBar.TextColorProperty.PropertyName)
                UpdateTextColor();
        }

        void UpdateText()
        {
            _searchBar.Value = Element.Text;
        }

        void UpdateTextColor()
        {
            var textColor = (Xamarin.Forms.Color)Element.GetValue(TimePicker.TextColorProperty);

            Control.Style.Color = textColor.ToOouiColor(Xamarin.Forms.Color.Black);
        }

        void UpdatePlaceholder()
        {
            _searchBar.Placeholder = Element.Placeholder ?? string.Empty;
        }

        void OnChange(object sender, EventArgs eventArgs)
        {
            if (_searchBar.Value != Element.Text)
                ElementController.SetValueFromRenderer(SearchBar.TextProperty, _searchBar.Value);
        }

        void OnClick(object sender, TargetEventArgs e)
        {
            Element.OnSearchButtonPressed();
        }
    }
}
