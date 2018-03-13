using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Text;
using Mono.Cecil;

namespace Ooui.Wasm.Build.Tasks
{
    public class BuildDistTask : Task
    {
        const string SdkUrl = "https://jenkins.mono-project.com/job/test-mono-mainline-webassembly/62/label=highsierra/Azure/processDownloadRequest/62/highsierra/sdks/wasm/mono-wasm-ddf4e7be31b.zip";

        [Required]
        public string Assembly { get; set; }
        public string ReferencePath { get; set; }

        public override bool Execute ()
        {
            try {
                InstallSdk ();
                GetBcl ();
                CreateDist ();
                CopyRuntime ();
                LinkAssemblies ();
                ExtractClientJs ();
                DiscoverEntryPoint ();
                GenerateHtml ();
                GenerateServer ();
                return true;
            }
            catch (Exception ex) {
                Log.LogErrorFromException (ex);
                return false;
            }
        }

        string sdkPath;

        void InstallSdk ()
        {
            var sdkName = Path.GetFileNameWithoutExtension (new Uri (SdkUrl).AbsolutePath.Replace ('/', Path.DirectorySeparatorChar));
            Log.LogMessage ("SDK: " + sdkName);
            sdkPath = Path.Combine (Path.GetTempPath (), sdkName);
            Log.LogMessage ("SDK Path: " + sdkPath);
            if (Directory.Exists (sdkPath))
                return;

            var client = new WebClient ();
            var zipPath = sdkPath + ".zip";
            Log.LogMessage ($"Downloading {sdkName} to {zipPath}");
            client.DownloadFile (SdkUrl, zipPath);

            ZipFile.ExtractToDirectory (zipPath, sdkPath);
            Log.LogMessage ($"Extracted {sdkName} to {sdkPath}");
        }

        string bclPath;
        Dictionary<string, string> bclAssemblies;

        void GetBcl ()
        {
            bclPath = Path.Combine (sdkPath, "bcl");
            var reals = Directory.GetFiles (bclPath, "*.dll");
            var facades = Directory.GetFiles (Path.Combine (bclPath, "Facades"), "*.dll");
            var allFiles = reals.Concat (facades);
            bclAssemblies = allFiles.ToDictionary (x => Path.GetFileName (x));
        }

        string distPath;
        string managedPath;

        void CreateDist ()
        {
            var fullAsmPath = Path.GetFullPath (Assembly);
            var outputPath = Path.GetDirectoryName (fullAsmPath);
            distPath = Path.Combine (outputPath, "dist");
            managedPath = Path.Combine (distPath, "managed");
            Directory.CreateDirectory (managedPath);
            //foreach (var dll in Directory.GetFiles (managedPath, "*.dll")) {
            //    File.Delete (dll);
            //}
        }

        void CopyRuntime ()
        {
            var rtPath = Path.Combine (sdkPath, "release");
            var files = new[] { "mono.wasm", "mono.js" };
            foreach (var f in files) {
                var src = Path.Combine (rtPath, f);
                var dest = Path.Combine (distPath, f);
                Log.LogMessage ($"Runtime {src} -> {dest}");
                File.Copy (src, dest, true);
            }
        }

        List<string> linkedAsmNames;
        List<string> linkedAsmPaths;

        void LinkAssemblies ()
        {
            var references = ReferencePath.Split (';').Select (x => x.Trim ()).Where (x => x.Length > 0).ToList ();
            references.Add (Path.GetFullPath (Assembly));

            linkedAsmPaths = new List<string> ();
            foreach (var r in references) {
                var name = Path.GetFileName (r);
                if (bclAssemblies.ContainsKey (name)) {
                    linkedAsmPaths.Add (bclAssemblies[name]);
                }
                else {
                    linkedAsmPaths.Add (r);
                }
            }

            linkedAsmNames = new List<string> ();
            foreach (var l in linkedAsmPaths) {
                var name = Path.GetFileName (l);
                linkedAsmNames.Add (name);
                var dest = Path.Combine (managedPath, name);
                if (bclAssemblies.ContainsKey (name) && File.Exists (dest))
                    continue;
                File.Copy (l, dest, true);
                Log.LogMessage ($"Managed {l} -> {dest}");
            }
        }

        void ExtractClientJs ()
        {
            var oouiPath = linkedAsmPaths.FirstOrDefault (x => Path.GetFileName (x).Equals ("Ooui.dll", StringComparison.InvariantCultureIgnoreCase));
            if (oouiPath == null) {
                Log.LogError ("Ooui.dll not included in the project");
                return;
            }
            var oouiAsm = AssemblyDefinition.ReadAssembly (oouiPath);
            var clientJs = oouiAsm.MainModule.Resources.FirstOrDefault (x => x.Name.EndsWith ("Client.js", StringComparison.InvariantCultureIgnoreCase)) as EmbeddedResource;
            if (clientJs == null) {
                Log.LogError ("Ooui.dll missing client javascript");
                return;
            }
            var dest = Path.Combine (distPath, "ooui.js");
            using (var srcs = clientJs.GetResourceStream ()) {
                using (var dests = new FileStream (dest, FileMode.Create, FileAccess.Write)) {
                    srcs.CopyTo (dests);
                }
            }
            Log.LogMessage ($"Client JS {dest}");
        }

        MethodDefinition entryPoint;

        void DiscoverEntryPoint ()
        {
            var asm = AssemblyDefinition.ReadAssembly (Assembly);
            entryPoint = asm.EntryPoint;
            if (entryPoint == null) {
                throw new Exception ($"{Path.GetFileName (Assembly)} is missing an entry point");
            }
        }

        void GenerateHtml ()
        {
            var htmlPath = Path.Combine (distPath, "index.html");
            using (var w = new StreamWriter (htmlPath, false, new UTF8Encoding (false))) {
                w.Write (@"<!DOCTYPE html>
<html>
<head>
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
    <link rel=""stylesheet"" href=""https://ajax.aspnetcdn.com/ajax/bootstrap/3.3.7/css/bootstrap.min.css"" />
    <link rel=""stylesheet"" href=""https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css"">
</head>
<body>
    <div id=""ooui-body"" class=""container-fluid"">
        <p id=""loading""><i class=""fa fa-refresh fa-spin"" style=""font-size:14px;margin-right:0.5em;""></i> Loading...</p>
    </div>
    <script defer type=""text/javascript"" src=""ooui.js""></script>
    <script type=""text/javascript"">
        var assemblies = [");
                var head = "";
                foreach (var l in linkedAsmNames) {
                    w.Write (head);
                    w.Write ('\"');
                    w.Write (l);
                    w.Write ('\"');
                    head = ",";
                }
                w.WriteLine ($@"];
        document.addEventListener(""DOMContentLoaded"", function(event) {{
            oouiWasm(""{entryPoint.DeclaringType.Module.Assembly.Name.Name}"", ""{entryPoint.DeclaringType.Namespace}"", ""{entryPoint.DeclaringType.Name}"", ""{entryPoint.Name}"", assemblies);
        }});
    </script>
    <script defer type=""text/javascript"" src=""mono.js""></script>
</body>
</html>");
            }
            Log.LogMessage ($"HTML {htmlPath}");
        }

        void GenerateServer ()
        {
            var server = @"import SimpleHTTPServer
import SocketServer
PORT = 8000
class Handler(SimpleHTTPServer.SimpleHTTPRequestHandler):
    pass
Handler.extensions_map["".wasm""] = ""application/wasm""
httpd = SocketServer.TCPServer(("""", PORT), Handler)
print ""serving at port"", PORT
httpd.serve_forever()";
            var serverPath = Path.Combine (distPath, "server.py");
            using (var w = new StreamWriter (serverPath, false, new UTF8Encoding (false))) {
                w.WriteLine (server);
            }
            Log.LogMessage ($"Server {serverPath}");
        }
    }
}
