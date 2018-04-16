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
    public class WindowTests
    {
        [TestMethod]
        public void ElementDocumentNotNull ()
        {
            var b = new Button ();
            Assert.IsNotNull (b.Document);
        }

        [TestMethod]
        public void DocumentWindowNotNull ()
        {
            var d = new Document ();
            Assert.IsNotNull (d.Window);
        }

        [TestMethod]
        public void DocumentBodyNotNull ()
        {
            var d = new Document ();
            Assert.IsNotNull (d.Body);
        }

        [TestMethod]
        public void WindowIdIsWindow ()
        {
            Assert.AreEqual ("window", new Window ().Id);
        }

        [TestMethod]
        public void DocumentIdIsDocument ()
        {
            Assert.AreEqual ("document", new Document ().Id);
        }

        [TestMethod]
        public void BodyIdIsDocumentBody ()
        {
            Assert.AreEqual ("document.body", new Body ().Id);
        }

        [TestMethod]
        public void DocumentGetsWindowMessages ()
        {
            var d = new Document ();
            var received = false;
            d.MessageSent += m => {
                received = m.TargetId == "window";
            };
            d.Window.Location = "http://google.com";
            Assert.IsTrue (received);
        }

        [TestMethod]
        public void ElementGetsWindowMessages ()
        {
            var b = new Button ();
            var received = false;
            b.MessageSent += m => {
                received = m.TargetId == "window";
            };
            b.Document.Window.Location = "http://google.com";
            Assert.IsTrue (received);
        }

        [TestMethod]
        public void ElementGetsBodyMessages ()
        {
            var b = new Button ();
            var received = false;
            b.MessageSent += m => {
                received = m.TargetId == "document.body";
            };
            b.Document.Body.Call ("foo");
            Assert.IsTrue (received);
        }
    }
}
