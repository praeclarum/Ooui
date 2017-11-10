using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using AspNetCoreMvc.Models;
using Ooui;
using Ooui.AspNetCore;

namespace AspNetCoreMvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var count = 0;
            var head = new Heading { Text = "Ooui!" };
            var label = new Label { Text = "0" };
            var btn = new Button { Text = "Increase" };
            btn.Clicked += (sender, e) => {
                count++;
                label.Text = count.ToString ();
            };
            var div = new Div ();
            div.AppendChild (head);
            div.AppendChild (label);
            div.AppendChild (btn);
            return new ElementResult (div);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
