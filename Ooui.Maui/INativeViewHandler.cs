using Microsoft.Maui;

namespace Ooui.Maui
{
	interface INativeViewHandler : IViewHandler
	{
		new Element? NativeView { get; }
	}
}
