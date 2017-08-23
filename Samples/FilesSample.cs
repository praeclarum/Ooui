using System;
using System.Net;
using System.Threading;
using Ooui;

namespace Samples
{
	public class FilesSample
	{
		public void Publish ()
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

			UI.Publish ("/files", app);

			UI.PublishCustomResponse ("/files/upload", HandleUpload);
		}

		void HandleUpload (HttpListenerContext listenerContext, CancellationToken token)
		{
			var req = listenerContext.Request;
			var ct = req.ContentType;
			var bi = ct.IndexOf ("boundary=", StringComparison.InvariantCulture);
			var boundaryString = ct.Substring (bi + 9);
			var boundaryBytes = System.Text.Encoding.UTF8.GetBytes (boundaryString);
			var boundaryEndBytes = System.Text.Encoding.UTF8.GetBytes (boundaryString + "--");
			Console.WriteLine ("OMGGGGGG " + boundaryString);
			listenerContext.Response.StatusCode = 200;
			listenerContext.Response.Close ();
		}
	}
}
