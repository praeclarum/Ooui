using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Ooui.AspNetCore
{
    public class ElementResult : ActionResult
    {
        readonly Element element;

        public ElementResult (Element element)
        {
            this.element = element;
        }

        public override async Task ExecuteResultAsync (ActionContext context)
        {
            var response = context.HttpContext.Response;
            response.StatusCode = 200;
            response.ContentType = "text/html; charset=utf-8";
            var sessionId = WebSocketHandler.BeginSession (context.HttpContext, element);
            var html = Encoding.UTF8.GetBytes (UI.RenderTemplate (WebSocketHandler.WebSocketPath + "?id=" + sessionId));
            response.ContentLength = html.Length;
            using (var s = response.Body) {
                await s.WriteAsync (html, 0, html.Length).ConfigureAwait (false);
            }
        }
    }
}
