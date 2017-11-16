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

        static readonly Lazy<List<Samples.ISample>> lazySamples =
            new Lazy<List<Samples.ISample>> ((() => {
                var sampleType = typeof (Samples.ISample);
                var asm = sampleType.Assembly;
                var sampleTypes = asm.GetTypes ().Where (x => x.Name.EndsWith ("Sample", StringComparison.Ordinal) && x != sampleType);
                var samples = from t in sampleTypes
                              let s = Activator.CreateInstance (t) as Samples.ISample
                              where s != null
                              orderby s.Title
                              select s;
                return samples.ToList ();
            }), true);

        public static List<Samples.ISample> Samples => lazySamples.Value;

        [Route("/Samples/Run/{name}")]
        public IActionResult Run (string name)
        {
            if (string.IsNullOrWhiteSpace (name) || name.Length > 32)
                return BadRequest ();
            
            var s = Samples.FirstOrDefault (x => x.Title == name);
            if (s == null)
                return NotFound ();

            return new ElementResult (s.CreateElement (), title: s.Title + " - Ooui Samples");
        }
    }
}
