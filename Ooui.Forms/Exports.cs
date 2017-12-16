using System;
using Ooui.Forms;
using Ooui.Forms.Renderers;
using Xamarin.Forms;

[assembly: Dependency(typeof(ResourcesProvider))]
[assembly: ExportRenderer(typeof(ActivityIndicator), typeof(ActivityIndicatorRenderer))]
[assembly: ExportRenderer(typeof(BoxView), typeof(BoxRenderer))]
[assembly: ExportRenderer(typeof(Button), typeof(ButtonRenderer))]
[assembly: ExportRenderer(typeof(DatePicker), typeof(DatePickerRenderer))]
[assembly: ExportRenderer(typeof(Editor), typeof(EditorRenderer))]
[assembly: ExportRenderer(typeof(Entry), typeof(EntryRenderer))]
[assembly: ExportRenderer(typeof(Frame), typeof(FrameRenderer))]
[assembly: ExportRenderer(typeof(Image), typeof(ImageRenderer))]
[assembly: ExportRenderer(typeof(Label), typeof(LabelRenderer))]
[assembly: ExportRenderer(typeof(ProgressBar), typeof(ProgressBarRenderer))]
[assembly: ExportRenderer(typeof(TimePicker), typeof(TimePickerRenderer))]
[assembly: ExportRenderer(typeof(Switch), typeof(SwitchRenderer))]
[assembly: ExportImageSourceHandler(typeof(FileImageSource), typeof(FileImageSourceHandler))]
[assembly: ExportImageSourceHandler(typeof(StreamImageSource), typeof(StreamImagesourceHandler))]
[assembly: ExportImageSourceHandler(typeof(UriImageSource), typeof(ImageLoaderSourceHandler))]

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
    public sealed class ExportImageSourceHandlerAttribute : HandlerAttribute
    {
        public ExportImageSourceHandlerAttribute (Type handler, Type target)
            : base (handler, target)
        {
        }
    }
}
