using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Ooui.Forms.Renderers
{
    public class WebViewRenderer : ViewRenderer<WebView, Div>, IWebViewDelegate
    {
        private bool _disposed;
        private Iframe _iframe;

        void IWebViewDelegate.LoadHtml(string html, string baseUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(html))
                {
                    if (Element.Source is HtmlWebViewSource urlWebViewSource)
                    {
                        html = urlWebViewSource.Html;
                    }
                }

                if (_iframe != null)
                {
                    _iframe.Src = html;
                }
            }
            catch (Exception ex)
            {
                Log.Warning("WebView load string", $"WebView load string failed: {ex}");
            }
        }

        void IWebViewDelegate.LoadUrl(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    if (Element.Source is UrlWebViewSource urlWebViewSource)
                    {
                        url = urlWebViewSource.Url;
                    }
                }

                if (_iframe != null)
                {
                    _iframe.Src = url;
                }
            }
            catch (Exception ex)
            {
                Log.Warning("WebView load url", $"WebView load url failed: {ex}");
            }
        }

        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            var size = new Size(100, 100);
            return new SizeRequest(size, size);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var embed = new Div { ClassName = "embed-responsive" };
                    _iframe = new Iframe();
                    embed.AppendChild(_iframe);

                    SetNativeControl(embed);
                }
            }

            Load();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == WebView.SourceProperty.PropertyName)
                Load();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && !_disposed)
            {
                if (_iframe != null)
                {
                    _iframe = null;
                }

                _disposed = true;
            }
        }

        private void Load()
        {
            Element?.Source?.Load(this);
        }
    }
}
