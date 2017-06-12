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
            server.RunAsync ("http://*:8080/").Wait ();
            return 0;
        }
    }
}
