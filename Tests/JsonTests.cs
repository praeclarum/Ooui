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
    }
}
