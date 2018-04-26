using System;

#if NUNIT
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestMethodAttribute = NUnit.Framework.TestCaseAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

using Ooui;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Tests
{
    [TestClass]
    public class EventTargetStressTests
    {
        [TestMethod]
        public void LotsOfThreads ()
        {
            var input = new Input ();
            Parallel.ForEach (Enumerable.Range (0, 100), _ => StressTestInput (input));
        }

        void StressTestInput (Input input)
        {
            var div = new Div (input);

            var changeCount = 0;
            var clickCount = 0;

            void Input_Change (object sender, TargetEventArgs e)
            {
                changeCount++;
            }
            void Div_MessageSent (Message m)
            {
                if (m.Key == "click")
                    clickCount++;
            }

            input.Change += Input_Change;
            div.MessageSent += Div_MessageSent;

            var sw = new Stopwatch ();
            sw.Start ();
            while (sw.ElapsedMilliseconds < 1000) {

                var t = sw.Elapsed.ToString ();
                input.Receive (Message.Event (input.Id, "change", t));

                var b = new Button ("Click");
                input.AppendChild (b);
                b.Send (Message.Event (b.Id, "click"));
                input.RemoveChild (b);
            }

            input.Change -= Input_Change;
            div.RemoveChild (input);
            div.MessageSent -= Div_MessageSent;

            Assert.IsTrue (changeCount > 0);
            Assert.IsTrue (clickCount > 0);
        }
    }
}
