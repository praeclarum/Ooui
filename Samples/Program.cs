using System;
using Ooui;

namespace Samples
{
    class Program
    {
        static int Main (string[] args)
        {
            Console.WriteLine ("Hello World!");
            var server = new Server ();
            var button = new Button();
            server.Publish ("/button", button);
            server.RunAsync ("http://*:8080/").Wait ();
            return 0;
        }
    }
}
