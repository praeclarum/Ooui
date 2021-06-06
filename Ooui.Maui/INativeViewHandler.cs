using Ooui;

namespace Microsoft.Maui
{
	public interface INativeViewHandler : IViewHandler
	{
		new Element? NativeView { get; }
		new Element? ContainerView { get; }
		Element? ViewController { get; }
	}
}
