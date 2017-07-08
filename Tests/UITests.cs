using System;
using System.Net.Http;
using System.Threading.Tasks;

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
    public class UITests
    {
        [TestMethod]
        public void UndefinedStylePropertyIsInherit ()
        {
            Assert.AreEqual ("inherit", UI.Styles["something random and made up"].BackgroundColor);
        }

        [TestMethod]
        public void SetStyleProperty ()
        {
            UI.Styles.Clear ();
            UI.Styles[".t1"].BackgroundColor = "red";
            Assert.AreEqual ("red", UI.Styles[".t1"].BackgroundColor);
        }

        [TestMethod]
        public void ClearWorks ()
        {
            UI.Styles[".t1"].BackgroundColor = "red";
            UI.Styles.Clear ();
            Assert.AreEqual ("inherit", UI.Styles[".t1"].BackgroundColor);
            Assert.AreEqual ("", UI.Styles.ToString ());
        }

        [TestMethod]
        public void SetStyle ()
        {
            UI.Styles.Clear ();
            UI.Styles[".t2"] = new Style {
                BackgroundColor = "red",
            };
            Assert.AreEqual ("red", UI.Styles[".t2"].BackgroundColor);
            Assert.AreEqual (".t2 {background-color:red}", UI.Styles.ToString ());
        }

        [TestMethod]
        public void SetNullStyle ()
        {
            UI.Styles.Clear ();
            UI.Styles[".t3"] = new Style {
                BackgroundColor = "red",
            };
            UI.Styles[".t3"] = null;
            Assert.AreEqual ("inherit", UI.Styles[".t3"].BackgroundColor);
            Assert.AreEqual ("", UI.Styles.ToString ());
        }

        [TestMethod]
        public void PublishMissingFileFails ()
        {
            try {
                UI.PublishFile ("/file", "a file that doesn't exist");
                Assert.Fail ("Publishing not existing file should fail");
            }
            catch (System.IO.FileNotFoundException) {
            }
        }

        [TestMethod]
        public void PublishEmptyFile ()
        {
            var f = System.IO.Path.GetTempFileName ();
            UI.PublishFile ("/file", f);
            UI.WaitUntilStarted ();
            var c = new System.Net.WebClient ();
            var r = c.DownloadString (UI.GetUrl ("/file"));
            Assert.AreEqual ("", r);
        }

        [TestMethod]
        public void PublishTextFile ()
        {
            var f = System.IO.Path.GetTempFileName ();
            System.IO.File.WriteAllText (f, "Test Ooui Text File", System.Text.Encoding.UTF8);
            UI.PublishFile ("/text-file", f, "text/plain; charset=utf-8");
            UI.WaitUntilStarted ();
            var c = new System.Net.WebClient ();
            var r = c.DownloadString (UI.GetUrl ("/text-file"));
            Assert.AreEqual ("Test Ooui Text File", r);
        }

        [TestMethod]
        public void PublishFileWithoutPath ()
        {
            var f = System.IO.Path.GetTempFileName ();
            System.IO.File.WriteAllText (f, "Test Ooui Text File 2", System.Text.Encoding.UTF8);
            UI.PublishFile (f);
            UI.WaitUntilStarted ();
            var c = new System.Net.WebClient ();
            var r = c.DownloadString (UI.GetUrl ("/" + System.IO.Path.GetFileName (f)));
            Assert.AreEqual ("Test Ooui Text File 2", r);
        }

        [TestMethod]
        public void PublishJsonObject ()
        {
            UI.PublishJson ("/json", new JsonTestObject ());
            UI.WaitUntilStarted ();
            var c = new System.Net.WebClient ();
            var r = c.DownloadString (UI.GetUrl ("/json"));
            Assert.AreEqual ("{\"Name\":\"X\",\"Value\":null}", r);
        }


        [TestMethod]
        public void PublishJsonCtor ()
        {
            var i = 1;
            UI.PublishJson ("/jsond", () => new JsonTestObject { Value = i++ });
            UI.WaitUntilStarted ();
            var c = new System.Net.WebClient ();
            var r1 = c.DownloadString (UI.GetUrl ("/jsond"));
            var r2 = c.DownloadString (UI.GetUrl ("/jsond"));
            Assert.AreEqual ("{\"Name\":\"X\",\"Value\":1}", r1);
            Assert.AreEqual ("{\"Name\":\"X\",\"Value\":2}", r2);
        }

        class JsonTestObject
        {
            public string Name = "X";
            public object Value;
        }
    }
}
