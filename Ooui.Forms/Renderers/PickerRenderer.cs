using Ooui.Forms.Extensions;
using Ooui.Html;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Ooui.Forms.Renderers
{
    public class PickerRenderer : ViewRenderer<Picker, Select>
    {
        private bool _disposed;
        private Select _select;

        public PickerRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    _select = new Select();
                    _select.Change += _select_Change;

                    SetNativeControl(_select);
                }

                UpdateItems();
                UpdateBackgroundColor();
            }

            base.OnElementChanged(e);
        }

        private void _select_Change(object sender, TargetEventArgs e)
        {
            Element.SetValueFromRenderer(Picker.SelectedIndexProperty, Element.ItemsSource.IndexOf(_select.Value));
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Picker.ItemsSourceProperty.PropertyName)
            {
                UpdateItems();
            }

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && !_disposed)
            {
				_select.Change -= _select_Change;
                _disposed = true;
            }
        }

        private void UpdateBackgroundColor()
        {
            var backgroundColor = Element.BackgroundColor.ToOouiColor(Colors.Clear);

            _select.Style.BackgroundColor = backgroundColor;
        }

        private void UpdateItems()
        {
            var items = Element.ItemsSource;

            if (items != null)
            {
                foreach (var item in items)
                {
                    var s = item.ToString();
                    _select.AddOption(s, s);
                }
            }
        }

    }

}
