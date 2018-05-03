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
    public class ElementTests
    {
        [TestMethod]
        public void LocalAttributes ()
        {
            var div = new Div ();
            Assert.IsNull (div.GetLocalAttribute ("TestAttribute"));
            div.SetLocalAttribute ("TestAttribute", "TestValue");
            Assert.AreEqual ("TestValue", div.GetLocalAttribute ("TestAttribute"));
            Assert.IsNull (div.GetAttribute ("TestAttribute"));
        }
    }
}
