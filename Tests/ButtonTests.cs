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
            Assert.AreEqual ("button", b.TagName);
            Assert.AreEqual ("", b.Text);
        }

        [TestMethod]
        public void TextCtor ()
        {
            var b = new Button ("Hello World!");
            Assert.AreEqual ("Hello World!", b.Text);
        }

        [TestMethod]
        public void Clicked ()
        {
            var b = new Button ("Hello World!");
            var clicked = false;
            var listened = false;
            b.MessageSent += m => {
                listened = listened || (m.MessageType == MessageType.Listen);
            };
            Assert.IsFalse (listened);
            b.Clicked += (s, e) => {
                clicked = true;
            };
            Assert.IsTrue (listened);
            Assert.IsFalse (clicked);
            b.Receive (Message.Event (b.Id, "onclick"));
            Assert.IsTrue (clicked);
        }
    }
}
