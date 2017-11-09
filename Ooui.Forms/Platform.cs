using System;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Ooui.Forms
{
	public class Platform : BindableObject, IPlatform, IDisposable
	{
		bool _disposed;

		void IDisposable.Dispose()
		{
			if (_disposed)
				return;
			_disposed = true;

			MessagingCenter.Unsubscribe<Page, ActionSheetArguments>(this, Page.ActionSheetSignalName);
			MessagingCenter.Unsubscribe<Page, AlertArguments>(this, Page.AlertSignalName);
			MessagingCenter.Unsubscribe<Page, bool>(this, Page.BusySetSignalName);
		}

		public SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
		{
			return new SizeRequest(new Size(100, 100));
		}
	}
}
