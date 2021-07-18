using Microsoft.Maui;

namespace Ooui.Maui
{
	public interface ILayoutHandler : IViewHandler
	{
		void Add(IView view);
		void Remove(IView view);
	}
}
