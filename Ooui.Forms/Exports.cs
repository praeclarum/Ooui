using System;
using Ooui.Forms;
using Ooui.Forms.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

[assembly: Dependency (typeof (ResourcesProvider))]
[assembly: ExportRenderer (typeof (Button), typeof (ButtonRenderer))]
[assembly: ExportRenderer (typeof (Label), typeof (LabelRenderer))]

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
