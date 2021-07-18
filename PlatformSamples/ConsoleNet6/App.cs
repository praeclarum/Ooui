using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;
using Application = Microsoft.Maui.Controls.Application;

namespace ConsoleNet6
{
	public partial class App : Application
	{
		public App()
		{
		}

		protected override IWindow CreateWindow(IActivationState activationState)
		{
			Console.WriteLine("CREATING A WINDOW, YO");
			// Microsoft.Maui.Controls.Compatibility.Forms.Init(activationState);

			this.On<Microsoft.Maui.Controls.PlatformConfiguration.Windows>()
				.SetImageDirectory("Assets");

			return new Microsoft.Maui.Controls.Window(new MainPage());
		}
	}
}
