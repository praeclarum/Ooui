using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ooui;

namespace Tests
{
    [TestClass]
    public class CanvasTests
    {
        [TestMethod]
        public void Context2DState ()
        {
            var c = new Canvas ();
            Assert.AreEqual (1, c.StateMessages.Count);
            var c2d = c.GetContext2D ();
            Assert.AreEqual (2, c.StateMessages.Count);
            var c2d2 = c.GetContext2D ();
            Assert.AreEqual (2, c.StateMessages.Count);
            Assert.AreEqual (c2d, c2d2);
        }

        [TestMethod]
        public void DefaultWidthAndHeight ()
        {
            var c = new Canvas ();
            Assert.AreEqual (150, c.Width);
            Assert.AreEqual (150, c.Height);
        }

        [TestMethod]
        public void WidthAndHeight ()
        {
            var c = new Canvas {
                Width = 640,
                Height = 480,
            };
            Assert.AreEqual (640, c.Width);
            Assert.AreEqual (480, c.Height);
        }

        [TestMethod]
        public void CantBeNegativeOrZero ()
        {
            var c = new Canvas {
                Width = 640,
                Height = 480,
            };
            Assert.AreEqual (640, c.Width);
            Assert.AreEqual (480, c.Height);
            c.Width = 0;
            c.Height = -100;
            Assert.AreEqual (150, c.Width);
            Assert.AreEqual (150, c.Height);
        }
    }
}
