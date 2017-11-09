using System;

namespace Ooui.Forms
{
	public static class PageExtensions
	{
		public static void Publish (this Xamarin.Forms.Page page, string path)
		{
			UI.Publish (path, () => page.CreateElement ());
		}

		public static void PublishShared (this Xamarin.Forms.Page page, string path)
		{
			var lazyPage = new Lazy<Element> ((() => page.CreateElement ()), true);
			UI.Publish (path, () => lazyPage.Value);
		}

		public static Element CreateElement (this Xamarin.Forms.Page page)
		{
			if (!Xamarin.Forms.Forms.IsInitialized)
				throw new InvalidOperationException ("call Forms.Init() before this");

			throw new NotImplementedException ();
		}
	}
}
