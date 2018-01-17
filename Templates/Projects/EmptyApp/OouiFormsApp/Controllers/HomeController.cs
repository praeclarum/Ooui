using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ooui.AspNetCore;
using OouiFormsApp.Models;
using OouiFormsApp.Pages;
using Xamarin.Forms;

namespace OouiFormsApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var page = new MainPage();
            var element = page.GetOouiElement();
            return new ElementResult(element, "Ooui Forms");
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
