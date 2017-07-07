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

        [TestMethod]
        public void EmptyString ()
        {
            var s = new Style ();
            Assert.AreEqual ("", s.ToString ());
        }

        [TestMethod]
        public void SingleString ()
        {
            var s = new Style ();
            s.BackgroundColor = "red";
            Assert.AreEqual ("background-color:red", s.ToString ());
        }

        [TestMethod]
        public void NullString ()
        {
            var s = new Style ();
            s.BackgroundColor = "red";
            s.BackgroundColor = null;
            Assert.AreEqual ("", s.ToString ());
        }

        [TestMethod]
        public void FloatString ()
        {
            var s = new Style ();
            s.BorderLeftWidth = 3.142;
            Assert.AreEqual ("border-left-width:3.142", s.ToString ());
        }

        [TestMethod]
        public void JsName ()
        {
            Assert.AreEqual ("borderLeftWidth", Style.GetJsName ("border-left-width"));
        }
    }
}
