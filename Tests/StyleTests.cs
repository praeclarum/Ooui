using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ooui;

namespace Tests
{
    [TestClass]
    public class StyleTests
    {
        [TestMethod]
        public void DefaultIsInherit ()
        {
            var s = new Style ();
            Assert.AreEqual ("inherit", s.BackgroundColor);
        }

        [TestMethod]
        public void NullMakesInherit ()
        {
            var s = new Style ();
            s.BackgroundColor = "red";
            Assert.AreEqual ("red", s.BackgroundColor);
            s.BackgroundColor = null;
            Assert.AreEqual ("inherit", s.BackgroundColor);
        }

        [TestMethod]
        public void ChangedWhen ()
        {
            var s = new Style ();
            var changeCount = 0;
            s.PropertyChanged += (_, e) => changeCount++;
            s.BackgroundColor = "red";
            Assert.AreEqual (1, changeCount);
            s.BackgroundColor = "blue";
            Assert.AreEqual (2, changeCount);
            s.BackgroundColor = "blue";
            Assert.AreEqual (2, changeCount);
        }
    }
}
