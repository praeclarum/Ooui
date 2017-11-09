using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Ooui.Forms;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

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

			Registrar.RegisterAll (new[] {
				typeof(ExportRendererAttribute),
				//typeof(ExportCellAttribute),
				//typeof(ExportImageSourceHandlerAttribute),
			});
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

			public Ticker CreateTicker ()
			{
				throw new NotImplementedException ();
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
				throw new NotImplementedException ();
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
				throw new NotImplementedException ();
			}
		}
	}
}
