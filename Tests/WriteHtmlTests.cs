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
    public class WriteHtmlTests
    {
        System.Text.RegularExpressions.Regex idre = new System.Text.RegularExpressions.Regex ("\\sid=\"[^\"]*\"");

        string OuterHtmlWithoutIds (Element e)
        {
            return idre.Replace (e.OuterHtml, "");
        }

        [TestMethod]
        public void TextAreaWithTextStyled ()
        {
            var e = new TextArea {
                Value = "Hello World!",
            };
            e.Style.BackgroundColor = "#18f";
            Assert.AreEqual ("<textarea style=\"background-color:#18f\">Hello World!</textarea>", OuterHtmlWithoutIds (e));
        }

        [TestMethod]
        public void TextAreaEmptyStyled ()
        {
            var e = new TextArea ();
            e.Style.BackgroundColor = "#18f";
            Assert.AreEqual ("<textarea style=\"background-color:#18f\"></textarea>", OuterHtmlWithoutIds (e));
        }

        [TestMethod]
        public void Style ()
        {
            var e = new Div ();
            e.Style.BackgroundColor = "#18f";
            Assert.AreEqual ("<div style=\"background-color:#18f\"></div>", OuterHtmlWithoutIds (e));
        }

        [TestMethod]
        public void TwoGrandChildren ()
        {
            var e = new Div (new Div (new Anchor (), new Anchor ()), new Paragraph ());
            Assert.AreEqual ("<div><div><a /><a /></div><p /></div>", OuterHtmlWithoutIds (e));
        }

        [TestMethod]
        public void Child ()
        {
            var e = new Div (new Anchor ());
            Assert.AreEqual ("<div><a /></div>", OuterHtmlWithoutIds (e));
        }

        [TestMethod]
        public void TextChild ()
        {
            var e = new Paragraph ("Hello world!");
            Assert.AreEqual ("<p>Hello world!</p>", OuterHtmlWithoutIds (e));
        }

        [TestMethod]
        public void IdIsFirst ()
        {
            var e = new Anchor ();
            Assert.IsTrue (e.OuterHtml.StartsWith ("<a id=\""));
        }

        [TestMethod]
        public void EmptyElement ()
        {
            var e = new Anchor ();
            Assert.AreEqual ("<a />", OuterHtmlWithoutIds (e));
        }

        [TestMethod]
        public void AnchorHRef ()
        {
            var e = new Anchor {
                HRef = "http://google.com"
            };
            Assert.AreEqual ("<a href=\"http://google.com\" />", OuterHtmlWithoutIds (e));
        }
    }
}
