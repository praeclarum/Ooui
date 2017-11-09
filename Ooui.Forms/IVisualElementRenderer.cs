using System;
using Xamarin.Forms;

namespace Ooui.Forms
{
	public interface IVisualElementRenderer : IRegisterable, IDisposable
	{
		event EventHandler<VisualElementChangedEventArgs> ElementChanged;

		VisualElement Element { get; }

		Ooui.Element NativeView { get; }

		void SetElement (VisualElement element);

		void SetElementSize (Size size);
	}
}
