using System;
using Microsoft.Maui;

namespace Ooui.Maui.Handlers
{
    public partial class PageHandler : ViewHandler<IPage, Ooui.Element>
    {
        protected override Ooui.Element CreateNativeView() => new Ooui.Div();

        public static void MapTitle(PageHandler handler, IPage page)
        {
            // TODO: fak: Map Page.Title
        }

        public static void MapContent(PageHandler handler, IPage page)
        {
        }
    }
}
