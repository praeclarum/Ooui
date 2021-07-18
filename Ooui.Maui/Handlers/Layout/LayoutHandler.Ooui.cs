using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Maui;

namespace Ooui.Maui.Handlers
{
	public partial class LayoutHandler : ViewHandler<ILayout, Ooui.Div>
	{
		public void Add(IView view) => throw new NotImplementedException();
		public void Remove(IView view) => throw new NotImplementedException();

		protected override Ooui.Div CreateNativeView() {
			if (VirtualView == null)
			{
				throw new InvalidOperationException($"{nameof(VirtualView)} must be set to create a LayoutViewGroup");
			}

			var view = new Ooui.Div();

			return view;
		}
	}
}
