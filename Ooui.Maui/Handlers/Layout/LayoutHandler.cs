#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Maui;

namespace Ooui.Maui.Handlers
{
	public partial class LayoutHandler : ILayoutHandler
	{
		public static PropertyMapper<ILayout> LayoutMapper = new OouiPropertyMapper<ILayout>(ViewHandler.ViewMapper)
		{
		};

		public LayoutHandler() : base(LayoutMapper)
		{

		}

		public LayoutHandler(PropertyMapper? mapper = null) : base(mapper ?? LayoutMapper)
		{

		}
	}
}
