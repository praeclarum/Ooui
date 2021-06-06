using Ooui;
using Microsoft.Maui;

namespace Ooui.Maui
{
	public interface INativeViewHandler : IViewHandler
	{
		new Element? NativeView { get; }
		new Element? ContainerView { get; }
		Element? ViewController { get; }
	}
}
