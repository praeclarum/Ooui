
using Microsoft.Maui;

namespace Ooui.Maui.Handlers {
    public class OouiPropertyMapper<TVirtualView, TViewHandler> : PropertyMapper<TVirtualView, TViewHandler>, IOouiPropertyMapper
        where TVirtualView : IFrameworkElement
        where TViewHandler : IViewHandler
    {
        public OouiPropertyMapper()
        {
        }

        public OouiPropertyMapper(PropertyMapper chained) : base(chained)
        {
        }

        public void UpdateProperty(IViewHandler viewHandler, IFrameworkElement? virtualView, string property)
        {
            if (virtualView == null)
                return;

            UpdatePropertyCore(property, viewHandler, virtualView);
        }

        public void UpdateProperties(IViewHandler viewHandler, IFrameworkElement? virtualView)
        {
            if (virtualView == null)
                return;

            foreach (var key in UpdateKeys)
            {
                UpdatePropertyCore(key, viewHandler, virtualView);
            }
        }
    }

    public interface IOouiPropertyMapper
    {
        void UpdateProperty(IViewHandler viewHandler, IFrameworkElement? virtualView, string property);
        void UpdateProperties(IViewHandler viewHandler, IFrameworkElement? virtualView);
    }
}