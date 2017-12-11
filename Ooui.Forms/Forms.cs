using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Ooui.Forms;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Ooui;

namespace Xamarin.Forms
{
    public static class Forms
    {
        public static bool IsInitialized { get; private set; }

        public static void Init ()
        {
            if (IsInitialized)
                return;
            IsInitialized = true;

            Log.Listeners.Add (new DelegateLogListener ((c, m) => Trace.WriteLine (m, c)));

            Device.SetIdiom (TargetIdiom.Desktop);
            Device.PlatformServices = new OouiPlatformServices ();
            Device.Info = new OouiDeviceInfo ();
            Color.SetAccent (Color.FromHex ("#0000EE")); // Safari Blue

            Registrar.RegisterAll (new[] {
                typeof(ExportRendererAttribute),
                //typeof(ExportCellAttribute),
                typeof(ExportImageSourceHandlerAttribute),
            });
        }

        public static event EventHandler<ViewInitializedEventArgs> ViewInitialized;

        public static void SendViewInitialized (this VisualElement self, Ooui.Element nativeView)
        {
            ViewInitialized?.Invoke (self, new ViewInitializedEventArgs { View = self, NativeView = nativeView });
        }

        class OouiDeviceInfo : DeviceInfo
        {
            public override Size PixelScreenSize => new Size (640, 480);

            public override Size ScaledScreenSize => PixelScreenSize;

            public override double ScalingFactor => 1;
        }

        class OouiPlatformServices : IPlatformServices
        {
            public bool IsInvokeRequired => false;

            public string RuntimePlatform => "Ooui";

            public void BeginInvokeOnMainThread (Action action)
            {
                Task.Run (action);
            }

            public Assembly[] GetAssemblies ()
            {
                return AppDomain.CurrentDomain.GetAssemblies ();
            }

            public string GetMD5Hash (string input)
            {
                throw new NotImplementedException ();
            }

            public double GetNamedSize (NamedSize size, Type targetElementType, bool useOldSizes)
            {
                switch (size) {
                    default:
                    case NamedSize.Default:
                        return 16;
                    case NamedSize.Micro:
                        return 9;
                    case NamedSize.Small:
                        return 12;
                    case NamedSize.Medium:
                        return 22;
                    case NamedSize.Large:
                        return 32;
                }
            }

            public Task<Stream> GetStreamAsync (Uri uri, CancellationToken cancellationToken)
            {
                throw new NotImplementedException ();
            }

            public IIsolatedStorageFile GetUserStoreForApplication ()
            {
                throw new NotImplementedException ();
            }

            public void OpenUriAction (Uri uri)
            {
                throw new NotImplementedException ();
            }

            public void StartTimer (TimeSpan interval, Func<bool> callback)
            {
                Timer timer = null;
                timer = new Timer ((_ => {
                    if (!callback ()) {
                        timer?.Dispose ();
                        timer = null;
                    }
                }), null, (int)interval.TotalMilliseconds, (int)interval.TotalMilliseconds);
            }

            public Ticker CreateTicker ()
            {
                return new OouiTicker ();
            }

            class OouiTicker : Ticker
            {
                Timer timer;
                protected override void DisableTimer ()
                {
                    var t = timer;
                    timer = null;
                    t?.Dispose ();
                }
                protected override void EnableTimer ()
                {
                    if (timer != null)
                        return;
                    var interval = TimeSpan.FromSeconds (1.0 / Ooui.UI.Session.MaxFps);
                    timer = new Timer ((_ => {
                        this.SendSignals ();
                    }), null, (int)interval.TotalMilliseconds, (int)interval.TotalMilliseconds);
                }
            }
        }

        public class ViewInitializedEventArgs
        {
            public VisualElement View { get; set; }
            public Ooui.Element NativeView { get; set; }
        }
    }
}
