using System;
using Ooui;

public class Program
{
	static Element GetButtonElement ()
	{
		var l = new Label { Text = "Hello" };
		var b = new Button ("Click Me");
		var e = new Div (new Div (l), b);
		int count = 0;
		b.Click += (s, ee) => {
			count++;
			b.Text = $"Clicked {count} times";
		};
		return e;
	}

	static Element GetXamarinFormsElement ()
	{
		var s = new Samples.XamlPreviewPageSample ();
		return s.CreateElement ();
	}

	public static string Main (string a0, string a1)
	{
		Xamarin.Forms.Forms.Init ();
		try {
			var e = GetXamarinFormsElement ();
			UI.SetGlobalElement ("main", e);
			return e.ToString ();
		}
		catch (Exception e) {
			Console.WriteLine (e);
			return e.ToString ();
		}
	}
}
