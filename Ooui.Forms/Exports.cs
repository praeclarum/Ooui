using System;
using Ooui.Forms;
using Ooui.Forms.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

[assembly: Dependency (typeof (ResourcesProvider))]
[assembly: ExportRenderer (typeof (ActivityIndicator), typeof (ActivityIndicatorRenderer))]
[assembly: ExportRenderer (typeof (BoxView), typeof (BoxRenderer))]
[assembly: ExportRenderer (typeof (Button), typeof (ButtonRenderer))]
[assembly: ExportRenderer (typeof (DatePicker), typeof (DatePickerRenderer))]
[assembly: ExportRenderer (typeof (Editor), typeof (EditorRenderer))]
[assembly: ExportRenderer (typeof (Entry), typeof (EntryRenderer))]
[assembly: ExportRenderer (typeof (Label), typeof (LabelRenderer))]
[assembly: ExportRenderer (typeof (ProgressBar), typeof (ProgressBarRenderer))]

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
}
