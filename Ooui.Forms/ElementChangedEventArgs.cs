using System;
using Xamarin.Forms;

namespace Ooui.Forms
{
	public class VisualElementChangedEventArgs : ElementChangedEventArgs<VisualElement>
	{
		public VisualElementChangedEventArgs (VisualElement oldElement, VisualElement newElement)
			: base (oldElement, newElement)
		{
		}
	}

	public class ElementChangedEventArgs<TElement> : EventArgs where TElement : Xamarin.Forms.Element
	{
		public ElementChangedEventArgs (TElement oldElement, TElement newElement)
		{
			OldElement = oldElement;
			NewElement = newElement;
		}

		public TElement NewElement { get; private set; }

		public TElement OldElement { get; private set; }
	}
}
