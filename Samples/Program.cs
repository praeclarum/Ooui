using System;
using Ooui;

namespace Samples
{
    class Program
    {
        static int Main (string[] args)
        {
            var server = new Server ();
            server.RunAsync ("http://*:8080/");

            new ButtonSample ().Publish ();

            Console.ReadLine ();
            return 0;
        }
    }
}
