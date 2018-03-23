using System;
using Xamarin.Forms;

namespace Xamarin.Forms
{
    public static class PageExtensions
    {
        public static void Publish (this Xamarin.Forms.Page page, string path)
        {
            Ooui.UI.Publish (path, () => page.CreateElement ());
        }

        public static void PublishShared (this Xamarin.Forms.Page page, string path)
        {
            var lazyPage = new Lazy<Ooui.Element> ((() => page.CreateElement ()), true);
            Ooui.UI.Publish (path, () => lazyPage.Value);
        }

        public static Ooui.Element GetOouiElement (this Xamarin.Forms.Page page)
        {
            if (!Xamarin.Forms.Forms.IsInitialized)
                throw new InvalidOperationException ("call Forms.Init() before this");
            
            var existingRenderer = Ooui.Forms.Platform.GetRenderer (page);
            if (existingRenderer != null)
                return existingRenderer.NativeView;

            ((IPageController)page).SendAppearing ();

            return CreateElement (page);
        }

        static Ooui.Element CreateElement (this Xamarin.Forms.Page page)
        {
            if (!(page.RealParent is Application)) {
                var app = new DefaultApplication ();
                app.MainPage = page;
            }
            var result = new Ooui.Forms.Platform ();
            result.SetPage (page);
            return result.Element;
        }

        class DefaultApplication : Application
        {
        }
    }
}
