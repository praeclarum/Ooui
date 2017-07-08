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
    public class ButtonTests
    {
        [TestMethod]
        public void DefaultCtor ()
        {
            var b = new Button ();
            Assert.AreEqual (0, b.Children.Count);
            Assert.AreEqual ("button", b.TagName);
            Assert.AreEqual ("", b.Text);
        }

        [TestMethod]
        public void TextCtor ()
        {
            var b = new Button ("Hello World!");
            Assert.AreEqual (1, b.Children.Count);
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
            b.Receive (Message.Event (b.Id, "click"));
            Assert.IsTrue (clicked);
        }

        [TestMethod]
        public void ChangeButtonType ()
        {
            var b = new Button ();
            Assert.AreEqual (1, b.StateMessages.Count);
            Assert.AreEqual (ButtonType.Submit, b.Type);
            b.Type = ButtonType.Button;
            Assert.AreEqual (2, b.StateMessages.Count);
            Assert.AreEqual (ButtonType.Button, b.StateMessages[1].Value);
        }
    }
}
