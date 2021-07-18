#nullable enable
using System;
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

            Console.WriteLine("PageHandler created");
		}

		// public PageHandler(PropertyMapper? mapper = null) : base(mapper ?? PageMapper)
		// {

		// }
	}
}
