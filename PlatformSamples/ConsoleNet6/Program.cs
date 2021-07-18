using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Ooui.Maui;
using Microsoft.Maui;

namespace ConsoleNet6
{
	public class MainWindow : Microsoft.Maui.Controls.Window
	{
		public MainWindow() : base(new MainPage())
		{
		}
	}

	public class Program
	{
		static void Main(string[] args)
		{

			System.Console.WriteLine("Hello chat room from Maui on Ooui");

			var context = OouiMauiContext.FromStartup<Startup>();

			var Application = context.Services.GetRequiredService<IApplication>();
			
			var element = Application.ToOouiElement(context);

			Console.WriteLine($"Element = {element}");

			Ooui.UI.Publish("/", element);
			for (;;) {
				System.Threading.Thread.Sleep(1000);
			}
		}
	}
}