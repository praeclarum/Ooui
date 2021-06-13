using System;
using Microsoft.Maui;

namespace Ooui.Maui.Handlers
{
	public partial class PageHandler : ViewHandler<IPage, Ooui.Element>
	{
		protected override Ooui.Element CreateNativeView() => throw new NotImplementedException();

		public static void MapTitle(PageHandler handler, IPage page)
		{
		}
		public static void MapContent(PageHandler handler, IPage page)
		{
		}
	}
}
