using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Ooui;

namespace Samples
{
    public class FilesSample //: ISample
	{
        public string Title => "Upload files";

		public void Publish ()
		{
            var app = CreateElement ();

			UI.Publish ("/files", app);

			UI.PublishCustomResponse ("/files/upload", HandleUpload);
		}

		void HandleUpload (HttpListenerContext listenerContext, CancellationToken token)
		{
			var req = listenerContext.Request;
			var ct = req.ContentType;
			var bi = ct.IndexOf ("boundary=", StringComparison.InvariantCulture);
			var boundaryString = ct.Substring (bi + 9);
			var boundaryBytes = System.Text.Encoding.UTF8.GetBytes ("--" + boundaryString + "\r\n");
			var boundaryEndBytes = System.Text.Encoding.UTF8.GetBytes (boundaryString + "--");
			var headerEndBytes = System.Text.Encoding.UTF8.GetBytes ("\r\n\r\n");
			Console.WriteLine ("OMGGGGGG " + boundaryString);

			var state = 0;
			var buffer = new byte[1024];
			var bufferLen = 0;

			var needsRead = true;

			using (var s = req.InputStream) {
				while (state < 1000) {
					if (needsRead) {
						var r = buffer.Length - bufferLen;
						if (r <= 0) {
							Array.Resize (ref buffer, buffer.Length * 2);
							r = buffer.Length - bufferLen;
						}
						var n = s.Read (buffer, bufferLen, r);
						if (n > 0) {
							bufferLen += n;
						}
						else if (n == 0) {
							// End!
							state = 1000;
						}
						else {
							state = 1001;
						}
					}
					switch (state) {
						case 0:
							var i = FindIndex (buffer, 0, bufferLen, boundaryBytes);
							if (i >= 0) {
								var e = i + boundaryBytes.Length;
								var r = bufferLen - e;
								Buffer.BlockCopy (buffer, e, buffer, 0, r);
								bufferLen = r;
								state = 1;
							}
							else {
								needsRead = true;
							}
							break;
						case 1:
							i = FindIndex (buffer, 0, bufferLen, headerEndBytes);
							if (i >= 0) {
								var h = System.Text.Encoding.ASCII.GetString (buffer, 0, i);
								Console.WriteLine ("HEADERS {0}", h);
								var e = i + headerEndBytes.Length;
								var r = bufferLen - e;
								Buffer.BlockCopy (buffer, e, buffer, 0, r);
								bufferLen = r;
								state = 2;
							}
							else {
								needsRead = true;
							}
							break;
						case 2:
							i = FindIndex (buffer, 0, bufferLen, boundaryEndBytes);
							if (i >= 0) {
								Console.WriteLine ("DATA {0}", bufferLen);
								var data = new byte[bufferLen];
								var e = i + boundaryBytes.Length;
								var r = bufferLen - e;
								Buffer.BlockCopy (buffer, e, buffer, 0, r);
								bufferLen = r;
								state = 0;
							}
							else {
								needsRead = true;
							}
							break;
					}
				}
			}

			Console.WriteLine ("STATE " + state);
			listenerContext.Response.StatusCode = 200;
			listenerContext.Response.Close ();
		}

		static int FindIndex (byte[] buffer, int bufferStart, int bufferLen, byte[] pattern)
		{
			var n = pattern.Length;
			for (var i = bufferLen - n; i >= bufferStart; i--) {
				var all = true;
				for (var j = 0; all && j < n; j++) {
					all = buffer[i + j] == pattern[j];
				}
				if (all)
					return i;
			}
			return -1;
		}

        public Element CreateElement ()
        {
            var heading = new Heading ("Upload Files");
            var subtitle = new Paragraph ("Upload files to the app");

            var uploadForm = new Form ();
            uploadForm.Action = "/files/upload";
            uploadForm.Method = "POST";
            uploadForm.EncodingType = "multipart/form-data";
            uploadForm.AppendChild (new Input (InputType.File) { Name = "file" });
            uploadForm.AppendChild (new Input (InputType.Submit) { Value = "Upload" });

            var app = new Div ();
            app.AppendChild (heading);
            app.AppendChild (subtitle);
            app.AppendChild (uploadForm);

            return app;
        }
    }
}
