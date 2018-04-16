using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ooui.AspNetCore
{
    public class ElementResult : ActionResult
    {
        readonly Element element;
        readonly string title;
        readonly bool disposeAfterSession;
        readonly ILogger logger;

        public ElementResult (Element element, string title = "", bool disposeAfterSession = true, ILogger logger = null)
        {
            this.logger = logger;
            this.element = element;
            this.title = title;
            this.disposeAfterSession = disposeAfterSession;
        }

        public override async Task ExecuteResultAsync (ActionContext context)
        {
            var response = context.HttpContext.Response;
            response.StatusCode = 200;
            response.ContentType = "text/html; charset=utf-8";
            response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";

            if (element.WantsFullScreen) {
                element.Style.Width = GetCookieDouble (context.HttpContext.Request.Cookies, "oouiWindowWidth", 32, 640, 10000);
                element.Style.Height = GetCookieDouble (context.HttpContext.Request.Cookies, "oouiWindowHeight", 24, 480, 10000);
            }

            var sessionId = WebSocketHandler.BeginSession (context.HttpContext, element, disposeAfterSession, logger);
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
