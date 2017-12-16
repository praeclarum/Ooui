using System;
using Ooui;

namespace Samples
{
    class Program
    {
        static void Main (string[] args)
        {
            Xamarin.Forms.Forms.Init ();

            for (var i = 0; i < args.Length; i++) {
                var a = args[i];
                switch (args[i]) {
                    case "-p" when i + 1 < args.Length:
                    case "--port" when i + 1 < args.Length:
                        {
                            int p;
                            if (int.TryParse (args[i + 1], out p)) {
                                UI.Port = p;
                            }
                            i++;
                        }
                        break;
                }
            }

            new ButtonSample ().Publish ();
            new TodoSample ().Publish ();
            new DrawSample ().Publish ();
            new FilesSample ().Publish ();
            new DisplayAlertSample ().Publish ();
            new DotMatrixClockSample().Publish();
            new EditorSample().Publish();
            new TimePickerSample().Publish();
            new TipCalcSample().Publish();
            new WeatherAppSample().Publish();
            new XuzzleSample().Publish();

            UI.Present ("/display-alert");

            Console.ReadLine ();
        }
    }
}
