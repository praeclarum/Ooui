#nullable enable
using Microsoft.Maui;

namespace Ooui.Maui.Handlers
{
	public partial class PageHandler : IViewHandler
	{
		public static PropertyMapper<IPage, PageHandler> PageMapper = new OouiPropertyMapper<IPage, PageHandler>(ViewHandler.ViewMapper)
		{
			[nameof(IPage.Title)] = MapTitle,
			[nameof(IPage.Content)] = MapContent,
		};

		public PageHandler() : base(PageMapper)
		{

		}

		// public PageHandler(PropertyMapper? mapper = null) : base(mapper ?? PageMapper)
		// {

		// }
	}
}
