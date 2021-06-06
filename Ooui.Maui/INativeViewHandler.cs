using Microsoft.Maui;

namespace Ooui.Maui
{
	public interface INativeViewHandler : IViewHandler
	{
		new Element? NativeView { get; }
	}
}
