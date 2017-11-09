using System;
using System.Collections.Generic;
using System.ComponentModel;
using Ooui.Forms.Extensions;
using Xamarin.Forms;

namespace Ooui.Forms
{
	public class VisualElementRenderer<TElement> : Ooui.Element, IVisualElementRenderer where TElement : VisualElement
	{
		readonly Color _defaultColor = Color.Clear;

		readonly PropertyChangedEventHandler _propertyChangedHandler;

		public TElement Element { get; private set; }

		VisualElement IVisualElementRenderer.Element => Element;

		public Element NativeView => this;

		event EventHandler<VisualElementChangedEventArgs> IVisualElementRenderer.ElementChanged {
			add { _elementChangedHandlers.Add (value); }
			remove { _elementChangedHandlers.Remove (value); }
		}

		readonly List<EventHandler<VisualElementChangedEventArgs>> _elementChangedHandlers =
			new List<EventHandler<VisualElementChangedEventArgs>> ();

		public VisualElementRenderer () : base ("div")
		{
			_propertyChangedHandler = OnElementPropertyChanged;
		}

		protected virtual void OnElementChanged (ElementChangedEventArgs<TElement> e)
		{
			var args = new VisualElementChangedEventArgs (e.OldElement, e.NewElement);
			for (int i = 0; i < _elementChangedHandlers.Count; i++) {
				_elementChangedHandlers[i] (this, args);
			}

			var changed = ElementChanged;
			if (changed != null)
				changed (this, e);
		}

		public event EventHandler<ElementChangedEventArgs<TElement>> ElementChanged;

		void IVisualElementRenderer.SetElement (VisualElement element)
		{
			SetElement ((TElement)element);
		}

		public void SetElement (TElement element)
		{
			var oldElement = Element;
			Element = element;

			if (oldElement != null)
				oldElement.PropertyChanged -= _propertyChangedHandler;

			if (element != null) {
				if (element.BackgroundColor != Xamarin.Forms.Color.Default || (oldElement != null && element.BackgroundColor != oldElement.BackgroundColor))
					SetBackgroundColor (element.BackgroundColor);

				//if (_tracker == null) {
				//	_tracker = new VisualElementTracker (this);
				//	_tracker.NativeControlUpdated += (sender, e) => UpdateNativeWidget ();
				//}

				//if (AutoPackage && _packager == null) {
				//	_packager = new VisualElementPackager (this);
				//	_packager.Load ();
				//}

				//if (AutoTrack && _events == null) {
				//	_events = new EventTracker (this);
				//	_events.LoadEvents (this);
				//}

				element.PropertyChanged += _propertyChangedHandler;
			}

			OnElementChanged (new ElementChangedEventArgs<TElement> (oldElement, element));

			if (element != null)
				SendVisualElementInitialized (element, this);

			if (Element != null && !string.IsNullOrEmpty (Element.AutomationId))
				SetAutomationId (Element.AutomationId);
		}

		public void SetElementSize (Size size)
		{
			Xamarin.Forms.Layout.LayoutChildIntoBoundingRegion (Element, new Rectangle (Element.X, Element.Y, size.Width, size.Height));
		}

		protected virtual void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName) {
				SetBackgroundColor (Element.BackgroundColor);
			}
			else if (e.PropertyName == Layout.IsClippedToBoundsProperty.PropertyName) {
				//UpdateClipToBounds ();
			}
		}

		protected virtual void OnRegisterEffect (PlatformEffect effect)
		{
			//effect.Container = this;
		}

		protected virtual void SetAutomationId (string id)
		{
		}

		protected virtual void SetBackgroundColor (Xamarin.Forms.Color color)
		{
			if (color == Xamarin.Forms.Color.Default)
				Style.BackgroundColor = _defaultColor;
			else
				Style.BackgroundColor = color.ToOouiColor ();
		}

		protected virtual void UpdateNativeWidget ()
		{
		}

		protected virtual void SendVisualElementInitialized (VisualElement element, Element nativeView)
		{
			element.SendViewInitialized (nativeView);
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose (bool disposing)
		{
			if (!disposedValue) {
				if (disposing) {
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~VisualElementRenderer() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose ()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose (true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion
	}
}
