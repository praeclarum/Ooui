using System;
using Ooui;

namespace Samples
{
    class Program
    {
        static void Main (string[] args)
        {
            new ButtonSample ().Publish ();
            new TodoSample ().Publish ();

            Console.ReadLine ();
        }
    }
}
