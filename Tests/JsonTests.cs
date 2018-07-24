using System;

#if NUNIT
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestMethodAttribute = NUnit.Framework.TestCaseAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

using Ooui;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using Ooui.Html;

namespace Tests
{
    [TestClass]
    public class JsonTests
    {
        static readonly Regex noid = new Regex ("⦙\\d+");
        static string NoId (string s)
        {
            return noid.Replace (s, "⦙");
        }

        [TestMethod]
        public void ButtonIndividualMessages ()
        {
            var b = new Button ();
            b.Text = "Hello";
            b.Click += (sender, e) => { };
            b.Title = "\"Quote\"";
            Assert.AreEqual ("{\"m\":\"create\",\"id\":\"⦙\",\"k\":\"button\"}", NoId (b.StateMessages[0].ToJson ()));
            Assert.AreEqual ("{\"m\":\"call\",\"id\":\"⦙\",\"k\":\"insertBefore\",\"v\":[\"⦙\",null]}", NoId (b.StateMessages[1].ToJson ()));
            Assert.AreEqual ("{\"m\":\"listen\",\"id\":\"⦙\",\"k\":\"click\"}", NoId (b.StateMessages[2].ToJson ()));
            Assert.AreEqual ("{\"m\":\"setAttr\",\"id\":\"⦙\",\"k\":\"title\",\"v\":\"\\\"Quote\\\"\"}", NoId (b.StateMessages[3].ToJson ()));
        }

        [TestMethod]
        public void ButtonWriteMessages ()
        {
            var b = new Button ();
            b.Text = "Hello";
            b.Click += (sender, e) => { };
            var sw = new StringWriter ();
            foreach (var m in b.StateMessages) {
                m.WriteJson (sw);
            }
            Assert.AreEqual ("{\"m\":\"create\",\"id\":\"⦙\",\"k\":\"button\"}" +
                             "{\"m\":\"call\",\"id\":\"⦙\",\"k\":\"insertBefore\",\"v\":[\"⦙\",null]}" +
                             "{\"m\":\"listen\",\"id\":\"⦙\",\"k\":\"click\"}", NoId (sw.ToString ()));
        }

        [TestMethod]
        public void JsonConvertValueNull ()
        {
            TextWriter w = new InMemoryTextWriter ();
            JsonConvert.WriteJsonValue (w, null);
            Assert.AreEqual ("null", w.ToString ());
        }

        [TestMethod]
        public void JsonConvertValueString ()
        {
            TextWriter w = new InMemoryTextWriter ();
            JsonConvert.WriteJsonValue (w, "string");
            Assert.AreEqual ("\"string\"", w.ToString ());
        }

        [TestMethod]
        public void JsonConvertValueArray ()
        {
            TextWriter w = new InMemoryTextWriter();
            JsonConvert.WriteJsonValue (w, new[] { 1, 2, 3 });
            Assert.AreEqual("[1,2,3]", w.ToString ());
        }

        [TestMethod]
        public void JsonConvertValueEventTarget ()
        {
            TextWriter w = new InMemoryTextWriter ();
            var textNode = new TextNode ();
            JsonConvert.WriteJsonValue (w, textNode);
            Assert.AreEqual ($"\"{textNode.Id}\"", w.ToString ());
        }

        [TestMethod]
        public void JsonConvertValueColor ()
        {
            TextWriter w = new InMemoryTextWriter ();
            var color = new Color (255, 255, 255, 0);
            JsonConvert.WriteJsonValue (w, color);
            Assert.AreEqual ("\"rgba(255,255,255,0)\"", w.ToString ());
        }

        [TestMethod]
        public void JsonConvertValueDouble ()
        {
            TextWriter w = new InMemoryTextWriter ();
            double d = 4.5;
            JsonConvert.WriteJsonValue (w, d);
            Assert.AreEqual ("4.5", w.ToString ());
        }

        [TestMethod]
        public void JsonConvertValueInt ()
        {
            TextWriter w = new InMemoryTextWriter ();
            int i = 45;
            JsonConvert.WriteJsonValue (w, i);
            Assert.AreEqual ("45", w.ToString ());
        }

        [TestMethod]
        public void JsonConvertValueFloat ()
        {
            TextWriter w = new InMemoryTextWriter ();
            float f = 45.5f;
            JsonConvert.WriteJsonValue (w, f);
            Assert.AreEqual ("45.5", w.ToString ());
        }

        [TestMethod]
        public void JsonConvertValueOther ()
        {
            TextWriter w = new InMemoryTextWriter ();
            JsonConvert.WriteJsonValue (w, new { foo = "bar" });
            Assert.AreEqual ("{\"foo\":\"bar\"}", w.ToString ());
        }

        class InMemoryTextWriter : TextWriter
        {
            private StringBuilder builder = new StringBuilder ();

            public InMemoryTextWriter() { }

            public override void Write(char value) => builder.Append(value);

            public override string ToString() => builder.ToString();

            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}
