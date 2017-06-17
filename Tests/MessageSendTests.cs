using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ooui;

namespace Tests
{
    [TestClass]
    public class MessageSendTests
    {
        [TestMethod]
        public void SendTriggers ()
        {
            var b = new Button ();
            var sendCount = 0;
            b.MessageSent += m => {
                if (m.Key == "test") sendCount++;
            };
            b.Send (Message.Event (b.Id, "test"));
            Assert.AreEqual (1, sendCount);
        }

        [TestMethod]
        public void ChildSendTriggers ()
        {
            var p = new Div ();
            var b = new Button ();
            p.AppendChild (b);
            var sendCount = 0;
            void HandleMessage (Message m) {
                if (m.Key == "test") sendCount++;
            };
            p.MessageSent += HandleMessage;
            b.Send (Message.Event (b.Id, "test"));
            Assert.AreEqual (1, sendCount);
        }

        [TestMethod]
        public void OldChildSendDoesntTrigger ()
        {
            var p = new Div ();
            var b = new Button ();
            p.AppendChild (b);
            var sendCount = 0;
            void HandleMessage (Message m) {
                if (m.Key == "test") sendCount++;
            };
            p.MessageSent += HandleMessage;
            b.Send (Message.Event (b.Id, "test"));
            p.RemoveChild (b);
            b.Send (Message.Event (b.Id, "test"));
            Assert.AreEqual (1, sendCount);
        }
    }
}
