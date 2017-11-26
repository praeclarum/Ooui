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
        public IActionResult Index ()
        {
            return View ();
        }

        public IActionResult About ()
        {
            ViewData["Message"] = "Ooui is a mini web framework to make programming interactive UIs easy.";

            return View ();
        }

        public IActionResult Contact ()
        {
            ViewData["Message"] = "Find us on github.";

            return View ();
        }

        public IActionResult Error ()
        {
            return View (new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
