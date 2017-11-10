using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Ooui.AspNetCore
{
    public class ElementResult : ActionResult
    {
        public ElementResult (Element element)
        {

        }

        public override async Task ExecuteResultAsync (ActionContext context)
        {
            var path = context.HttpContext.Request.Path;
            var response = context.HttpContext.Response;

            response.StatusCode = 200;
            response.ContentType = "text/html";
            var html = Encoding.UTF8.GetBytes (UI.RenderTemplate (path));
            response.ContentLength = html.Length;
            using (var s = response.Body) {
                await s.WriteAsync (html, 0, html.Length).ConfigureAwait (false);
            }
        }
    }
}
