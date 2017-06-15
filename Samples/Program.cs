using System;
using Ooui;

namespace Samples
{
    class Program
    {
        static int Main (string[] args)
        {
            new ButtonSample ().Publish ();

            Console.ReadLine ();
            return 0;
        }
    }
}
