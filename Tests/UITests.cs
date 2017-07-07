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
    public class UITests
    {
        [TestMethod]
        public void UndefinedStylePropertyIsInherit ()
        {
            Assert.AreEqual ("inherit", UI.Styles["something random and made up"].BackgroundColor);
        }

        [TestMethod]
        public void SetStyleProperty ()
        {
            UI.Styles.Clear ();
            UI.Styles[".t1"].BackgroundColor = "red";
            Assert.AreEqual ("red", UI.Styles[".t1"].BackgroundColor);
        }

        [TestMethod]
        public void ClearWorks ()
        {
            UI.Styles[".t1"].BackgroundColor = "red";
            UI.Styles.Clear ();
            Assert.AreEqual ("inherit", UI.Styles[".t1"].BackgroundColor);
            Assert.AreEqual ("", UI.Styles.ToString ());
        }

        [TestMethod]
        public void SetStyle ()
        {
            UI.Styles.Clear ();
            UI.Styles[".t2"] = new Style {
                BackgroundColor = "red",
            };
            Assert.AreEqual ("red", UI.Styles[".t2"].BackgroundColor);
            Assert.AreEqual (".t2 {background-color:red}", UI.Styles.ToString ());
        }

        [TestMethod]
        public void SetNullStyle ()
        {
            UI.Styles.Clear ();
            UI.Styles[".t3"] = new Style {
                BackgroundColor = "red",
            };
            UI.Styles[".t3"] = null;
            Assert.AreEqual ("inherit", UI.Styles[".t3"].BackgroundColor);
            Assert.AreEqual ("", UI.Styles.ToString ());
        }
    }
}
