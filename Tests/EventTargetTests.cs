using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}
