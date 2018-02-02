using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Ooui.AspNetCore
{
    public class ElementResult : ActionResult
    {
        readonly Element element;
        readonly string title;

        public ElementResult (Element element, string title = "")
        {
            this.element = element;
            this.title = title;
        }

        public override async Task ExecuteResultAsync (ActionContext context)
        {
            var response = context.HttpContext.Response;
            response.StatusCode = 200;
            response.ContentType = "text/html; charset=utf-8";
            var sessionId = WebSocketHandler.BeginSession (context.HttpContext, element);
            var initialHtml = element.OuterHtml;
            Console.WriteLine(initialHtml);
            var html = UI.RenderTemplate (WebSocketHandler.WebSocketPath + "?id=" + sessionId, title: title, initialHtml: initialHtml);
            var htmlBytes = Encoding.UTF8.GetBytes (html);
            response.ContentLength = htmlBytes.Length;
            using (var s = response.Body) {
                await s.WriteAsync (htmlBytes, 0, htmlBytes.Length).ConfigureAwait (false);
            }
        }
    }
}
