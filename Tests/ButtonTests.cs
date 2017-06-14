using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ooui;

namespace Tests
{
    [TestClass]
    public class ButtonTests
    {
        [TestMethod]
        public void DefaultCtor ()
        {
            var b = new Button ();
            Assert.AreEqual ("", b.Text);
        }

        [TestMethod]
        public void TextCtor ()
        {
            var b = new Button ("Hello World!");
            Assert.AreEqual ("Hello World!", b.Text);
        }
    }
}
