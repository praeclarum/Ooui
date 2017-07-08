using System.Reflection;

using Android.App;
using Android.OS;
using Android.Webkit;
using Xamarin.Android.NUnitLite;

namespace AndroidTests
{
	[Activity (Label = "AndroidTests", MainLauncher = true)]
	public class MainActivity : TestSuiteActivity
	{
		public static MainActivity Shared;

		public WebView Browser;

		protected override void OnCreate (Bundle bundle)
		{
			Browser = new WebView (this);

			Shared = this;

			// tests can be inside the main assembly
			AddTest (Assembly.GetExecutingAssembly ());
			// or in any reference assemblies
			// AddTest (typeof (Your.Library.TestClass).Assembly);

			// Once you called base.OnCreate(), you cannot add more assemblies.
			base.OnCreate (bundle);
		}
	}
}
