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
using Ooui.Forms;
using Xamarin.Forms;

namespace Tests
{
    [TestClass]
    public class XamarinFormsTests
    {

        [TestMethod]
        public void BasicTest()
        {
            Xamarin.Forms.Forms.Init();

            var page = new Xaml.Basic();
            var q = page.GetOouiElement();

            var html = q.OuterHtml;
        }

    }
}
