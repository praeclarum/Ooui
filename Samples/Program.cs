using System;
using Ooui;

namespace Samples
{
    class Program
    {
        static int Main (string[] args)
        {
            var server = new Server ();
            var button = new Button() {
                Title = "The best button",
                Name = "TestButton",
                Value = "Click Me",
                Text = "I am a button, click me!"
            };
            server.Publish ("/button", button);
            server.RunAsync ("http://*:8080/").Wait ();
            return 0;
        }
    }
}
