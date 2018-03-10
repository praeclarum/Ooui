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

			UI.SetGlobalElement ("main", e);
			return e.ToString ();
		}
		catch (Exception e) {
			Console.WriteLine (e);
			return e.ToString ();
		}
	}
}
