using System;
using Ooui.Forms;
using Ooui.Forms.Cells;
using Ooui.Forms.Renderers;
using Xamarin.Forms;

[assembly: Dependency (typeof (ResourcesProvider))]
[assembly: ExportRenderer (typeof (ActivityIndicator), typeof (ActivityIndicatorRenderer))]
[assembly: ExportRenderer (typeof (BoxView), typeof (BoxRenderer))]
[assembly: ExportRenderer (typeof (Button), typeof (ButtonRenderer))]
[assembly: ExportRenderer (typeof (DatePicker), typeof (DatePickerRenderer))]
[assembly: ExportRenderer (typeof (Editor), typeof (EditorRenderer))]
[assembly: ExportRenderer (typeof (Entry), typeof (EntryRenderer))]
[assembly: ExportRenderer (typeof (Frame), typeof (FrameRenderer))]
[assembly: ExportRenderer (typeof (Image), typeof (ImageRenderer))]
[assembly: ExportRenderer (typeof (Label), typeof (LabelRenderer))]
[assembly: ExportRenderer (typeof (LinkLabel), typeof (LinkLabelRenderer))]
[assembly: ExportRenderer (typeof (LinkView), typeof (LinkViewRenderer))]
[assembly: ExportRenderer (typeof (Picker), typeof (PickerRenderer))]
[assembly: ExportRenderer (typeof (ListView), typeof (ListViewRenderer))]
[assembly: ExportRenderer (typeof (ProgressBar), typeof (ProgressBarRenderer))]
[assembly: ExportRenderer (typeof (ScrollView), typeof (ScrollViewRenderer))]
[assembly: ExportRenderer (typeof (SearchBar), typeof (SearchBarRenderer))]
[assembly: ExportRenderer (typeof (Slider), typeof (SliderRenderer))]
[assembly: ExportRenderer (typeof (Switch), typeof (SwitchRenderer))]
[assembly: ExportRenderer (typeof (TimePicker), typeof (TimePickerRenderer))]
[assembly: ExportRenderer (typeof (WebView), typeof (WebViewRenderer))]
[assembly: ExportRenderer(typeof(NavigationPage), typeof(NavigationPageRenderer))]
[assembly: ExportImageSourceHandler (typeof (FileImageSource), typeof (FileImageSourceHandler))]
[assembly: ExportImageSourceHandler (typeof (StreamImageSource), typeof (StreamImagesourceHandler))]
[assembly: ExportImageSourceHandler (typeof (UriImageSource), typeof (ImageLoaderSourceHandler))]
[assembly: ExportCell (typeof (Cell), typeof (CellRenderer))]
[assembly: ExportCell (typeof (EntryCell), typeof (EntryCellRenderer))]
[assembly: ExportCell (typeof (ImageCell), typeof (ImageCellRenderer))]
[assembly: ExportCell (typeof (SwitchCell), typeof (SwitchCellRenderer))]
[assembly: ExportCell (typeof (TextCell), typeof (TextCellRenderer))]
[assembly: ExportCell (typeof (ViewCell), typeof (ViewCellRenderer))]
namespace Ooui.Forms
{
    [AttributeUsage (AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class ExportRendererAttribute : HandlerAttribute
    {
        public ExportRendererAttribute (Type handler, Type target)
            : base (handler, target)
        {
        }
    }

    [AttributeUsage (AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class ExportCellAttribute : HandlerAttribute
    {
        public ExportCellAttribute (Type handler, Type target) : base (handler, target)
        {
        }
    }

    [AttributeUsage (AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class ExportImageSourceHandlerAttribute : HandlerAttribute
    {
        public ExportImageSourceHandlerAttribute (Type handler, Type target)
            : base (handler, target)
        {
        }
    }
}
