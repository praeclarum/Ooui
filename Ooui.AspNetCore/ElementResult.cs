using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

            if (element.WantsFullScreen) {
                element.Style.Width = GetCookieDouble (context.HttpContext.Request.Cookies, "oouiWindowWidth", 32, 640, 10000);
                element.Style.Height = GetCookieDouble (context.HttpContext.Request.Cookies, "oouiWindowHeight", 24, 480, 10000);
            }

            var sessionId = WebSocketHandler.BeginSession (context.HttpContext, element);
            var initialHtml = element.OuterHtml;
            var html = UI.RenderTemplate (WebSocketHandler.WebSocketPath + "?id=" + sessionId, title: title, initialHtml: initialHtml);
            var htmlBytes = Encoding.UTF8.GetBytes (html);
            response.ContentLength = htmlBytes.Length;
            using (var s = response.Body) {
                await s.WriteAsync (htmlBytes, 0, htmlBytes.Length).ConfigureAwait (false);
            }
        }

        static double GetCookieDouble (IRequestCookieCollection cookies, string key, double min, double def, double max)
        {
            if (cookies.TryGetValue (key, out var s)) {
                if (double.TryParse (s, out var d)) {
                    if (d < min) return min;
                    if (d > max) return max;
                    return d;
                }
                return def;
            }
            else {
                return def;
            }
        }
    }
}
