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
    public class EventTargetTests
    {
        [TestMethod]
        public void CreationState ()
        {
            var b = new Button ();
            Assert.AreEqual (1, b.StateMessages.Count);
        }

        [TestMethod]
        public void PersistentId ()
        {
            var div1 = new Div ();
            var div2 = new Div ();
            Assert.IsTrue (div2.PersistentId > div1.PersistentId);
        }
    }
}
