using System;

#if NUNIT
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestMethodAttribute = NUnit.Framework.TestCaseAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

using Ooui;
using Ooui.Html;

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
            Assert.AreEqual (0, c2d.StateMessages.Count);
        }

        [TestMethod]
        public void DefaultWidthAndHeight ()
        {
            var c = new Canvas ();
            Assert.AreEqual (300, c.Width);
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
            Assert.AreEqual (0, c.Width);
            Assert.AreEqual (0, c.Height);
        }
    }
}
