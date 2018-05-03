using System;
using System.Text;

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
    public class NodeTests
    {
        [TestMethod]
        public void TextState ()
        {
            var b = new Button ();
            Assert.AreEqual (1, b.StateMessages.Count);
            b.Text = "Hello";
            Assert.AreEqual (2, b.StateMessages.Count);
            b.Text = "Bye";
            Assert.AreEqual (2, b.StateMessages.Count);
        }

        [TestMethod]
        public void InsertBeforeOfRemovedNodeStillWorks ()
        {
            var p = new Div ();
            var c0 = new Span ();
            var c1 = new Span ();
            var c2 = new Span ();
            p.InsertBefore (c2, null);
            p.InsertBefore (c1, c2);
            p.InsertBefore (c0, c1);
            p.RemoveChild (c1);
            var ms = p.StateMessages;
            Assert.AreEqual (3, ms.Count);
            var c0s = (Array)ms[2].Value;
            Assert.AreEqual (2, c0s.Length);
            Assert.AreEqual (c0, c0s.GetValue (0));
            Assert.AreEqual (c2, c0s.GetValue (1));
        }

        [TestMethod]
        public void EventReceptionBubblesDown ()
        {
            var p = new Div ();
            var b = new Button ();
            p.AppendChild (b);
            var clicked = false;
            b.Click += (s, e) => {
                clicked = true;
            };
            p.Receive (Message.Event (b.Id, "click"));
            Assert.IsTrue (clicked);
        }

        [TestMethod]
        public void ValueBubblesDown ()
        {
            var p = new Div ();
            var b = new Input ();
            p.AppendChild (b);
            p.Receive (Message.Event (b.Id, "change", "please work"));
            Assert.AreEqual ("please work", b.Value);
        }

        class TestDiv : Div
        {
            public TestDiv (string id)
            {
                Id = id;
            }
        }

        [TestMethod]
        public void ReplaceChildrenWith ()
        {
            // As ReplaceChildrenWith calls ReplaceChildren
            //  this test also tests ReplaceChildren.

            var parent = new TestDiv ("parent");
            var child1 = new TestDiv ("child1");
            var child2 = new TestDiv ("child2");
            var child3 = new TestDiv ("child3");

            // Log tracks all messages sent due to updates
            var log    = new StringBuilder ();
            log.AppendLine ();
            void Parent_MessageSent(Message obj)
            {
                log.AppendLine (obj.ToString ());
            }

            parent.MessageSent += Parent_MessageSent;

            Assert.AreEqual (parent.Children.Count, 0);

            log.AppendLine ("#1 - Inserts child1 last");
            parent.ReplaceChildrenWith (child1);
            Assert.AreEqual (parent.Children.Count, 1);
            Assert.AreEqual (child1.PersistentId, parent.Children[0].PersistentId);

            log.AppendLine ("#2 - Inserts child2 before child1");
            parent.ReplaceChildrenWith (child2, child1);
            Assert.AreEqual (parent.Children.Count, 2);
            Assert.AreEqual (child2.PersistentId, parent.Children[0].PersistentId);
            Assert.AreEqual (child1.PersistentId, parent.Children[1].PersistentId);

            log.AppendLine ("#3 - Inserts child1 before child2, inserts child 3 last");
            parent.ReplaceChildrenWith (child1, child2, child3);
            Assert.AreEqual (parent.Children.Count, 3);
            Assert.AreEqual (child1.PersistentId, parent.Children[0].PersistentId);
            Assert.AreEqual (child2.PersistentId, parent.Children[1].PersistentId);
            Assert.AreEqual (child3.PersistentId, parent.Children[2].PersistentId);

            log.AppendLine ("#4 - Inserts child2 before child1, removes child3 and child1");
            parent.ReplaceChildrenWith (child2);
            Assert.AreEqual (parent.Children.Count, 1);
            Assert.AreEqual (child2.PersistentId, parent.Children[0].PersistentId);

            log.AppendLine ("#5 - Removes child3");
            parent.ReplaceChildrenWith ();
            Assert.AreEqual (parent.Children.Count, 0);

            var expected = @"
#1 - Inserts child1 last
{""m"":""call"",""id"":""parent"",""k"":""insertBefore"",""v"":[""child1"",null]}
#2 - Inserts child2 before child1
{""m"":""call"",""id"":""parent"",""k"":""insertBefore"",""v"":[""child2"",""child1""]}
#3 - Inserts child1 before child2, inserts child 3 last
{""m"":""call"",""id"":""parent"",""k"":""insertBefore"",""v"":[""child1"",""child2""]}
{""m"":""call"",""id"":""parent"",""k"":""insertBefore"",""v"":[""child3"",null]}
#4 - Inserts child2 before child1, removes child3 and child1
{""m"":""call"",""id"":""parent"",""k"":""insertBefore"",""v"":[""child2"",""child1""]}
{""m"":""call"",""id"":""parent"",""k"":""removeChild"",""v"":[""child3""]}
{""m"":""call"",""id"":""parent"",""k"":""removeChild"",""v"":[""child1""]}
#5 - Removes child3
{""m"":""call"",""id"":""parent"",""k"":""removeChild"",""v"":[""child2""]}
".Replace ("\r\n", "\n");

            var actual = log.ToString ().Replace ("\r\n", "\n");
            Assert.AreEqual (expected, actual);
        }

    }
}
