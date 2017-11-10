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
    public class SamplesController : Controller
    {
        public IActionResult Clicker ()
        {
            var count = 0;
            var head = new Heading { Text = "Click away!" };
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
    }
}
