using System;
using Ooui;

public class Program
{
	public static string Main (string a0, string a1)
	{
		try {
			var l = new Label { Text = "Hello" };
			var b = new Button ("Click Me");
			var e = new Div (new Div (l), b);
			var c = 0;
			b.Click += (s, ee) => {
				b.Text = $"Clicked {c} times";
			};

			UI.SetGlobalElement ("main", e);
			return e.ToString ();
		}
		catch (Exception e) {
			Console.WriteLine (e);
			return e.ToString ();
		}
	}
}
