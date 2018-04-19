using System;

#if NUNIT
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestMethodAttribute = NUnit.Framework.TestCaseAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

using Ooui;

namespace Tests
{
    [TestClass]
    public class InputTests
    {
        [TestMethod]
        public void ValuePropertyChangedOnReceiver ()
        {
            var e = new Ooui.Input ();
            var count = 0;
            e.PropertyChanged += (s, ev) => {
                if (ev.PropertyName == "Value")
                    count++;
            };
            e.Receive (new Message {
                MessageType = MessageType.Event,
                TargetId = e.Id,
                Key = "change",
                Value = "woo",
            });
            Assert.AreEqual (1, count);
            Assert.AreEqual (e.Value, "woo");
        }
    }
}
