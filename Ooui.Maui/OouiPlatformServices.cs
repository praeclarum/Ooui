using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Internals;

namespace Ooui.Maui
{
    public class OouiPlatformServices : IPlatformServices, IPlatformInvalidate
    {
        public bool IsInvokeRequired => false;

        public OSAppTheme RequestedTheme => OSAppTheme.Unspecified;

        public string RuntimePlatform => "Ooui";

        public void BeginInvokeOnMainThread(Action action)
        {
            Task.Run(action);
        }

        public Ticker CreateTicker()
        {
            throw new NotImplementedException();
        }

        public Assembly[] GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        public string GetHash(string input)
        {
            throw new NotImplementedException();
        }

        public string GetMD5Hash(string input)
        {
            throw new NotImplementedException();
        }

        public Microsoft.Maui.Graphics.Color GetNamedColor(string name)
        {
            throw new NotImplementedException();
        }

        public double GetNamedSize(NamedSize size, Type targetElementType, bool useOldSizes)
        {
            return size switch {
                NamedSize.Body => 16.0,
                _ => 16.0
            };
        }

        public SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
        {
            throw new NotImplementedException();
        }

        public Task<Stream> GetStreamAsync(Uri uri, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IIsolatedStorageFile GetUserStoreForApplication()
        {
            throw new NotImplementedException();
        }

        public void Invalidate(VisualElement visualElement)
        {
            throw new NotImplementedException();
        }

        public void OpenUriAction(Uri uri)
        {
            throw new NotImplementedException();
        }

        public void QuitApplication()
        {
            throw new NotSupportedException();
        }

        public void StartTimer(TimeSpan interval, Func<bool> callback)
        {
            throw new NotImplementedException();
        }
    }
}
