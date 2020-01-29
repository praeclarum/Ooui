using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Runtime.InteropServices;

namespace Ooui
{
    public static class UI
    {
        public const int MaxFps = 30;

        static readonly ManualResetEvent started = new ManualResetEvent (false);

        static CancellationTokenSource serverCts;

        static readonly Dictionary<string, RequestHandler> publishedPaths =
            new Dictionary<string, RequestHandler> ();

        static readonly byte[] clientJsBytes;
        static readonly string clientJsEtag;

        public static byte[] ClientJsBytes => clientJsBytes;
        public static string ClientJsEtag => clientJsEtag;

        public static string HeadHtml { get; set; } = @"<link rel=""stylesheet"" href=""https://ajax.aspnetcdn.com/ajax/bootstrap/3.3.7/css/bootstrap.min.css"" />";
        public static string BodyHeaderHtml { get; set; } = @"";
        public static string BodyFooterHtml { get; set; } = @"";

        static string host = "*";
        public static string Host {
            get => host == "*" ? "localhost" : host;
            set {
                if (!string.IsNullOrWhiteSpace (value) && host != value) {
                    host = value;
                    Restart ();
                }
            }
        }
        static int port = 8080;
        public static int Port {
            get => port;
            set {
                if (port != value) {
                    port = value;
                    Restart ();
                }
            }
        }
        static bool serverEnabled = true;
        public static bool ServerEnabled {
            get => serverEnabled;
            set {
                if (serverEnabled != value) {
                    serverEnabled = value;
                    if (serverEnabled)
                        Restart ();
                    else
                        Stop ();
                }
            }
        }

        [Preserve]
        static void DisableServer ()
        {
            ServerEnabled = false;
        }

        static UI ()
        {
            var asm = typeof(UI).Assembly;
            // System.Console.WriteLine("ASM = {0}", asm);
            // foreach (var n in asm.GetManifestResourceNames()) {
            //     System.Console.WriteLine("  {0}", n);
            // }
            using (var s = asm.GetManifestResourceStream ("Ooui.Client.js")) {
				if (s == null)
					throw new Exception ("Missing Client.js");
                using (var r = new StreamReader (s)) {
                    clientJsBytes = Encoding.UTF8.GetBytes (r.ReadToEnd ());
                }
            }
            clientJsEtag = "\"" + Utilities.GetShaHash (clientJsBytes) + "\"";
        }

        static void Publish (string path, RequestHandler handler)
        {
            //Console.WriteLine ($"PUBLISH {path} {handler}");
            lock (publishedPaths) publishedPaths[path] = handler;
            Start ();
        }

        public static void Publish (string path, Func<Element> elementCtor, bool disposeElementWhenDone = true)
        {
            Publish (path, new ElementHandler (elementCtor, disposeElementWhenDone));
        }

        public static void Publish (string path, Element element, bool disposeElementWhenDone = true)
        {
            Publish (path, () => element, disposeElementWhenDone);
        }

        public static void PublishFile (string filePath)
        {
            var path = "/" + System.IO.Path.GetFileName (filePath);
            PublishFile (path, filePath);
        }

        public static void PublishFile (string path, string filePath, string contentType = null)
        {
            var data = System.IO.File.ReadAllBytes (filePath);
            if (contentType == null) {
                contentType = GuessContentType (path, filePath);
            }
            var etag = "\"" + Utilities.GetShaHash (data) + "\"";
            Publish (path, new DataHandler (data, etag, contentType));
        }

        public static void PublishFile (string path, byte[] data, string contentType)
        {
            var etag = "\"" + Utilities.GetShaHash (data) + "\"";
            Publish (path, new DataHandler (data, etag, contentType));
        }

        public static void PublishFile (string path, byte[] data, string etag, string contentType)
        {            
            Publish (path, new DataHandler (data, etag, contentType));
        }

        public static bool TryGetFileContentAtPath (string path, out FileContent file)
        {
            RequestHandler handler;
            lock (publishedPaths) {
                if (!publishedPaths.TryGetValue (path, out handler)) {
                    file = null;
                    return false;
                }
            }
            if (handler is DataHandler dh) {
                file = new FileContent {
                    Etag = dh.Etag,
                    Content = dh.Data,
                    ContentType = dh.ContentType,
                };
                return true;
            }
            file = null;
            return false;
        }

        public class FileContent
        {
            public string ContentType { get; set; }
            public string Etag { get; set; }
            public byte[] Content { get; set; }
        }

        public static void PublishJson (string path, Func<object> ctor)
        {
            Publish (path, new JsonHandler (ctor));
        }

        public static void PublishJson (string path, object value)
        {
            var data = JsonHandler.GetData (value);
            var etag = "\"" + Utilities.GetShaHash (data) + "\"";
            Publish (path, new DataHandler (data, etag, JsonHandler.ContentType));
        }

		public static void PublishCustomResponse (string path, Action<HttpListenerContext, CancellationToken> responder)
		{
			Publish (path, new CustomHandler (responder));
		}

        static string GuessContentType (string path, string filePath)
        {
            return null;
        }

        public static void Present (string path, object presenter = null)
        {
            WaitUntilStarted ();
            var url = GetUrl (path);
            Console.WriteLine ($"PRESENT {url}");
			Platform.OpenBrowser (url, presenter);
		}

        public static string GetUrl (string path)
        {            
            var url = $"http://{Host}:{port}{path}";
            return url;
        }

        public static void WaitUntilStarted () => started.WaitOne ();

        static void Start ()
        {
            if (!serverEnabled) return;
            if (serverCts != null) return;
            serverCts = new CancellationTokenSource ();
            var token = serverCts.Token;
            var listenerPrefix = $"http://{Host}:{port}/";
            Task.Run (() => RunAsync (listenerPrefix, token), token);
        }

        static void Stop ()
        {
            var scts = serverCts;
            if (scts == null) return;
            serverCts = null;
            started.Reset ();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine ($"Stopping...");
            Console.ResetColor ();

            scts.Cancel ();
        }

        static void Restart ()
        {
            if (serverCts == null) return;
            Stop ();
            Start ();
        }

        static async Task RunAsync (string listenerPrefix, CancellationToken token)
        {
            HttpListener listener = null;
            var wait = 5;

            started.Reset ();
            while (!started.WaitOne(0) && !token.IsCancellationRequested) {
                try {
                    listener = new HttpListener ();
                    listener.Prefixes.Add (listenerPrefix);
                    listener.Start ();
                    started.Set ();
                }
                catch (System.Net.Sockets.SocketException ex) {
                    Console.WriteLine ($"{listenerPrefix} error: {ex.Message}. Trying again in {wait} seconds...");
                    await Task.Delay (wait * 1000).ConfigureAwait (false);
                }
                catch (System.Net.HttpListenerException ex) {
                    Console.WriteLine ($"{listenerPrefix} error: {ex.Message}. Trying again in {wait} seconds...");
                    await Task.Delay (wait * 1000).ConfigureAwait (false);
                }
                catch (Exception ex) {
                    Error ("Error listening", ex);
                    return;
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine ($"Listening at {listenerPrefix}...");
            Console.ResetColor ();

            while (!token.IsCancellationRequested) {
                var listenerContext = await listener.GetContextAsync ().ConfigureAwait (false);
                if (listenerContext.Request.IsWebSocketRequest) {
                    ProcessWebSocketRequest (listenerContext, token);
                }
                else {
                    ProcessRequest (listenerContext, token);
                }
            }
        }

        static void ProcessRequest (HttpListenerContext listenerContext, CancellationToken token)
        {
            var url = listenerContext.Request.Url;
            var path = url.LocalPath;

            Console.WriteLine ($"{listenerContext.Request.HttpMethod} {url.LocalPath}");

            var response = listenerContext.Response;

            if (path == "/ooui.js") {
                var inm = listenerContext.Request.Headers.Get ("If-None-Match");
                if (string.IsNullOrEmpty (inm) || inm != clientJsEtag) {
                    response.StatusCode = 200;
                    response.ContentLength64 = clientJsBytes.LongLength;
                    response.ContentType = "application/javascript";
                    response.ContentEncoding = Encoding.UTF8;
                    response.AddHeader ("Cache-Control", "public, max-age=60");
                    response.AddHeader ("Etag", clientJsEtag);
                    using (var s = response.OutputStream) {
                        s.Write (clientJsBytes, 0, clientJsBytes.Length);
                    }
                    response.Close ();
                }
                else {
                    response.StatusCode = 304;
                    response.Close ();
                }
            }
            else {
                var found = false;
                RequestHandler handler;
                lock (publishedPaths) found = publishedPaths.TryGetValue (path, out handler);
                if (found) {
                    try {
                        handler.Respond (listenerContext, token);
                    }
                    catch (Exception ex) {
                        Error ("Handler failed to respond", ex);
                        try {
                            response.StatusCode = 500;
                            response.Close ();
                        }
                        catch {
                            // Ignore ending the response errors
                        }
                    }
                }
                else {
                    response.StatusCode = 404;
                    response.Close ();
                }
            }
        }

        abstract class RequestHandler
        {
            public abstract void Respond (HttpListenerContext listenerContext, CancellationToken token);
        }

        class ElementHandler : RequestHandler
        {
            readonly Lazy<Element> element;

            public bool DisposeElementWhenDone { get; }

            public ElementHandler (Func<Element> ctor, bool disposeElementWhenDone)
            {
                element = new Lazy<Element> (ctor);
                DisposeElementWhenDone = disposeElementWhenDone;
            }

            public Element GetElement () => element.Value;

            public override void Respond (HttpListenerContext listenerContext, CancellationToken token)
            {
                var url = listenerContext.Request.Url;
                var path = url.LocalPath;
                var response = listenerContext.Response;

                response.StatusCode = 200;
                response.ContentType = "text/html";
                response.ContentEncoding = Encoding.UTF8;
                var html = Encoding.UTF8.GetBytes (RenderTemplate (path));
                response.ContentLength64 = html.LongLength;
                using (var s = response.OutputStream) {
                    s.Write (html, 0, html.Length);
                }
                response.Close ();
            }
        }

        public static string RenderTemplate (string webSocketPath, string title = "", string initialHtml = "")
        {
            using (var w = new System.IO.StringWriter ()) {
                RenderTemplate (w, webSocketPath, title, initialHtml);
                return w.ToString ();
            }
        }

        static string EscapeHtml (string text)
        {
            return text.Replace ("&", "&amp;").Replace ("<", "&lt;");
        }

        public static void RenderTemplate (TextWriter writer, string webSocketPath, string title, string initialHtml)
        {
            writer.Write (@"<!DOCTYPE html>
<html>
<head>
  <title>");
            writer.Write (EscapeHtml (title));
            writer.Write (@"</title>
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
  ");
            writer.WriteLine (HeadHtml);
            writer.WriteLine (@"  <style>");
            writer.WriteLine (rules.ToString ());
            writer.WriteLine (@"  </style>
</head>
<body>");
            writer.WriteLine (BodyHeaderHtml);
            writer.WriteLine (@"<div id=""ooui-body"" class=""container-fluid"" style=""padding:0;margin:0"">");
            writer.WriteLine (initialHtml);
            writer.Write (@"</div>

<script src=""/ooui.js""></script>
<script>ooui(""");
            writer.Write (webSocketPath);
            writer.WriteLine (@""");</script>");
            writer.WriteLine (BodyFooterHtml);
            writer.WriteLine (@"</body>
</html>");
        }

        class DataHandler : RequestHandler
        {
            readonly byte[] data;
            readonly string etag;
            readonly string contentType;

            public byte[] Data => data;
            public string Etag => etag;
            public string ContentType => contentType;

            public DataHandler (byte[] data, string etag, string contentType = null)
            {
                this.data = data;
                this.etag = etag;
                this.contentType = contentType;
            }

            public override void Respond (HttpListenerContext listenerContext, CancellationToken token)
            {
                var url = listenerContext.Request.Url;
                var path = url.LocalPath;
                var response = listenerContext.Response;

                var inm = listenerContext.Request.Headers.Get ("If-None-Match");
                if (!string.IsNullOrEmpty (inm) && inm == etag) {
                    response.StatusCode = 304;
                }
                else {
                    response.StatusCode = 200;
                    response.AddHeader ("Etag", etag);
                    if (!string.IsNullOrEmpty (contentType))
                        response.ContentType = contentType;
                    response.ContentLength64 = data.LongLength;

                    using (var s = response.OutputStream) {
                        s.Write (data, 0, data.Length);
                    }
                }
                response.Close ();
            }
        }

        class JsonHandler : RequestHandler
        {
            public const string ContentType = "application/json; charset=utf-8";

            readonly Func<object> ctor;

            public JsonHandler (Func<object> ctor)
            {
                this.ctor = ctor;
            }

            public static byte[] GetData (object obj)
            {
                var r = Ooui.JsonConvert.SerializeObject (obj);
                var e = new UTF8Encoding (false);
                return e.GetBytes (r);
            }

            public override void Respond (HttpListenerContext listenerContext, CancellationToken token)
            {
                var response = listenerContext.Response;

                var data = GetData (ctor ());

                response.StatusCode = 200;
                response.ContentType = ContentType;
                response.ContentLength64 = data.LongLength;

                using (var s = response.OutputStream) {
                    s.Write (data, 0, data.Length);
                }
                response.Close ();
            }
        }

		class CustomHandler : RequestHandler
		{
			readonly Action<HttpListenerContext, CancellationToken> responder;

			public CustomHandler (Action<HttpListenerContext, CancellationToken> responder)
			{
				this.responder = responder;
			}

			public override void Respond (HttpListenerContext listenerContext, CancellationToken token)
			{
				responder (listenerContext, token);
			}
		}

        static async void ProcessWebSocketRequest (HttpListenerContext listenerContext, CancellationToken serverToken)
        {
            //
            // Find the element
            //
            var url = listenerContext.Request.Url;
            var path = url.LocalPath;

            RequestHandler handler;
            var found = false;
            lock (publishedPaths) found = publishedPaths.TryGetValue (path, out handler);
            var elementHandler = handler as ElementHandler;
            if (!found || elementHandler == null) {
                listenerContext.Response.StatusCode = 404;
                listenerContext.Response.Close ();
                return;
            }

            Element element = null;
            bool disposeElementWhenDone = true;
            try {
                element = elementHandler.GetElement ();
                disposeElementWhenDone = elementHandler.DisposeElementWhenDone;

				if (element == null)
					throw new Exception ("Handler returned a null element");
            }
            catch (Exception ex) {
                listenerContext.Response.StatusCode = 500;
                listenerContext.Response.Close();
                Error ("Failed to create element", ex);
                return;
            }

            //
            // Connect the web socket
            //
            System.Net.WebSockets.WebSocketContext webSocketContext = null;
            System.Net.WebSockets.WebSocket webSocket = null;
            try {
                webSocketContext = await listenerContext.AcceptWebSocketAsync (subProtocol: "ooui").ConfigureAwait (false);
                webSocket = webSocketContext.WebSocket;
                Console.WriteLine ("WEBSOCKET {0}", listenerContext.Request.Url.LocalPath);
            }
            catch (Exception ex) {
                listenerContext.Response.StatusCode = 500;
                listenerContext.Response.Close();
                Error ("Failed to accept WebSocket", ex);
                return;
            }

            //
            // Set the element's dimensions
            //
            var query =
                (from part in listenerContext.Request.Url.Query.Split (new[] { '?', '&' })
                 where part.Length > 0
                 let kvs = part.Split ('=')
                 where kvs.Length == 2
                 select kvs).ToDictionary (x => Uri.UnescapeDataString (x[0]), x => Uri.UnescapeDataString (x[1]));
            if (!query.TryGetValue ("w", out var wValue) || string.IsNullOrEmpty (wValue)) {
                wValue = "640";
            }
            if (!query.TryGetValue ("h", out var hValue) || string.IsNullOrEmpty (hValue)) {
                hValue = "480";
            }
            var icult = System.Globalization.CultureInfo.InvariantCulture;
            if (!double.TryParse (wValue, System.Globalization.NumberStyles.Any, icult, out var w))
                w = 640;
            if (!double.TryParse (hValue, System.Globalization.NumberStyles.Any, icult, out var h))
                h = 480;

            //
            // Create a new session and let it handle everything from here
            //
            try {
                var session = new WebSocketSession (webSocket, element, disposeElementWhenDone, w, h, Error, serverToken);
                await session.RunAsync ().ConfigureAwait (false);
            }
            catch (System.Net.WebSockets.WebSocketException ex) when (ex.WebSocketErrorCode == System.Net.WebSockets.WebSocketError.ConnectionClosedPrematurely) {
                // The remote party closed the WebSocket connection without completing the close handshake.
            }
            catch (Exception ex) {
                Error ("Web socket failed", ex);
            }
            finally {
                webSocket?.Dispose ();
            }
        }

        static void Error (string message, Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine ("{0}: {1}", message, ex);
            Console.ResetColor ();
        }

        static readonly Dictionary<string, WebAssemblySession> globalElementSessions = new Dictionary<string, WebAssemblySession> ();

        [Preserve]
        public static void StartWebAssemblySession (string sessionId, string elementPath, string initialSize)
        {
            Element element;
            RequestHandler handler;
            lock (publishedPaths) {
                publishedPaths.TryGetValue (elementPath, out handler);
            }
            var disposeElementWhenDone = true;
            if (handler is ElementHandler eh) {
                element = eh.GetElement ();
                disposeElementWhenDone = eh.DisposeElementWhenDone;
            }
            else {
                element = new Div ();
            }

            var ops = initialSize.Split (' ');
            var initialWidth = double.Parse (ops[0]);
            var initialHeight = double.Parse (ops[1]);
            var g = new WebAssemblySession (sessionId, element, disposeElementWhenDone, initialWidth, initialHeight, Error);
            lock (globalElementSessions) {
                globalElementSessions[sessionId] = g;
            }
            g.StartSession ();
        }

        [Preserve]
        public static void ReceiveWebAssemblySessionMessageJson (string sessionId, string json)
        {
            WebAssemblySession g;
            lock (globalElementSessions) {
                if (!globalElementSessions.TryGetValue (sessionId, out g))
                    return;
            }
            g.ReceiveMessageJson (json);
        }


        static readonly Dictionary<string, Style> styles =
            new Dictionary<string, Style> ();
        static readonly StyleSelectors rules = new StyleSelectors ();

        public static StyleSelectors Styles => rules;

        public class StyleSelectors
        {
            public Style this[string selector] {
                get {
                    var key = selector ?? "";
                    lock (styles) {
                        if (!styles.TryGetValue (key, out Style r)) {
                            r = new Style ();
                            styles.Add (key, r);
                        }
                        return r;
                    }
                }
                set {
                    var key = selector ?? "";
                    lock (styles) {
                        if (value == null) {
                            styles.Remove (key);
                        }
                        else {
                            styles[key] = value;
                        }
                    }
                }
            }

            public void Clear ()
            {
                lock (styles) {
                    styles.Clear ();
                }
            }

            public override string ToString()
            {
                lock (styles) {
                    var q =
                        from s in styles
                        let v = s.Value.ToString ()
                        where v.Length > 0
                        select s.Key + " {" + s.Value.ToString () + "}";
                    return String.Join ("\n", q);
                }
            }
        }
    }
}
