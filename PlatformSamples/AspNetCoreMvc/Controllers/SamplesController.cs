using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using AspNetCoreMvc.Models;
using Ooui;
using Ooui.AspNetCore;
using Samples;
using System.Collections.Concurrent;

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
            btn.Click += (sender, e) => {
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

        static readonly ConcurrentDictionary<string, Element> sharedSamples =
            new ConcurrentDictionary<string, Element> ();

        public static List<Samples.ISample> Samples => lazySamples.Value;

        [Route ("/Samples/Run/{name}")]
        public IActionResult Run (string name, bool shared)
        {
            if (string.IsNullOrWhiteSpace (name) || name.Length > 32)
                return BadRequest ();

            var s = Samples.FirstOrDefault (x => x.Title == name);
            if (s == null)
                return NotFound ();

            var element = shared ? GetSharedSample (s) : s.CreateElement ();

            return new ElementResult (element, title: s.Title + " - Ooui Samples");
        }

        private Element GetSharedSample (ISample s)
        {
            if (sharedSamples.TryGetValue (s.Title, out var e))
                return e;
            e = s.CreateElement ();
            sharedSamples[s.Title] = e;
            return e;
        }

        [Route ("/shared-button")]
        public IActionResult SharedButton ()
        {
            return Run ("Button Counter", true);
        }
    }
}
