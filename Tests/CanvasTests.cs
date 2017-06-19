using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ooui;

namespace Tests
{
    [TestClass]
    public class CanvasTests
    {
        [TestMethod]
        public void Context2dState ()
        {
            var c = new Canvas ();
            Assert.AreEqual (1, c.StateMessages.Count);
            var c2d = c.GetContext2d ();
            Assert.AreEqual (2, c.StateMessages.Count);
            var c2d2 = c.GetContext2d ();
            Assert.AreEqual (2, c.StateMessages.Count);
            Assert.AreEqual (c2d, c2d2);
        }
    }
}
