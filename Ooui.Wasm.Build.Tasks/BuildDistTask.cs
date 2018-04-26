using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Mono.Cecil;
using Mono.Linker;
using Mono.Linker.Steps;

namespace Ooui.Wasm.Build.Tasks
{
    public class BuildDistTask : Task
    {
        const string SdkUrl = "https://xamjenkinsartifact.azureedge.net/test-mono-mainline-webassembly/108/highsierra/sdks/wasm/mono-wasm-a14f41ca260.zip";

        const string AssemblyExtension = ".bin";

        [Required]
        public string Assembly { get; set; }
        [Required]
        public string OutputPath { get; set; }
        public string ReferencePath { get; set; }

        bool ok = false;

        public override bool Execute ()
        {
            try {
                ok = true;
                InstallSdk ();
                GetBcl ();
                CreateDist ();
                DeleteOldAssemblies ();
                CopyRuntime ();
                LinkAssemblies ();
                RenameAssemblies ();
                ExtractClientJs ();
                DiscoverEntryPoint ();
                GenerateHtml ();
                return ok;
            }
            catch (Exception ex) {
                //Console.WriteLine (ex);
                Log.LogErrorFromException (ex);
                return false;
            }
        }

        string sdkPath;

        void InstallSdk ()
        {
            var sdkName = Path.GetFileNameWithoutExtension (new Uri (SdkUrl).AbsolutePath.Replace ('/', Path.DirectorySeparatorChar));
            Log.LogMessage ("SDK: " + sdkName);
            string tmpDir = Path.GetTempPath ();
            sdkPath = Path.Combine (tmpDir, sdkName);
            Log.LogMessage ("SDK Path: " + sdkPath);
            if (Directory.Exists (sdkPath)
                && Directory.Exists (Path.Combine (sdkPath, "release")))
                return;

            var client = new WebClient ();
            var zipPath = sdkPath + ".zip";
            Log.LogMessage ($"Downloading {sdkName} to {zipPath}");
            if (File.Exists (zipPath))
                File.Delete (zipPath);
            client.DownloadFile (SdkUrl, zipPath);

            var sdkTempPath = Path.Combine (tmpDir, Guid.NewGuid ().ToString ());
            ZipFile.ExtractToDirectory (zipPath, sdkTempPath);
            if (Directory.Exists (sdkPath))
                Directory.Delete (sdkPath, true);
            Directory.Move (sdkTempPath, sdkPath);
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
            var outputPath = Path.GetFullPath (OutputPath);
            distPath = Path.Combine (outputPath, "dist");
            managedPath = Path.Combine (distPath, "managed");
            Directory.CreateDirectory (managedPath);
        }

        void DeleteOldAssemblies ()
        {
            foreach (var dll in Directory.GetFiles (managedPath, "*.dll")) {
                File.Delete (dll);
            }
            foreach (var dll in Directory.GetFiles (managedPath, "*" + AssemblyExtension)) {
                File.Delete (dll);
            }
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
            File.Copy (Path.Combine (sdkPath, "server.py"), Path.Combine (distPath, "server.py"), true);
        }

        List<string> linkedAsmPaths;
        List<string> refpaths;

        void LinkAssemblies ()
        {
            var references = ReferencePath.Split (';').Select (x => x.Trim ()).Where (x => x.Length > 0).ToList ();
            refpaths = new List<string> ();
            foreach (var r in references) {
                var name = Path.GetFileName (r);
                if (bclAssemblies.ContainsKey (name)) {
                    refpaths.Add (bclAssemblies[name]); 
                    //Console.WriteLine ($"+ {name}");
                }
                else {
                    refpaths.Add (r);
                    //Console.WriteLine ($"- {r}");
                }
            }

            var asmPath = Path.GetFullPath (Assembly);

            var pipeline = GetLinkerPipeline ();
            var resolver = new LinkerAssemblyResolver (this);
            var asmParameters = new ReaderParameters {
                AssemblyResolver = resolver,
                MetadataResolver = new LinkerMetadataResolver (resolver)
            };
            using (var context = new LinkContext (pipeline, resolver, asmParameters, new UnintializedContextFactory ())) {
                context.CoreAction = AssemblyAction.CopyUsed;
                context.UserAction = AssemblyAction.CopyUsed;
                context.OutputDirectory = managedPath;
                context.IgnoreUnresolved = false;

                pipeline.PrependStep (new ResolveFromAssemblyStep (asmPath, ResolveFromAssemblyStep.RootVisibility.Any));

                var refdirs = refpaths.Select (x => Path.GetDirectoryName (x)).Distinct ().ToList ();
                refdirs.Insert (0, Path.Combine (bclPath, "Facades"));
                refdirs.Insert (0, bclPath);
                foreach (var d in refdirs.Distinct ()) {
                    context.Resolver.AddSearchDirectory (d);
                }

                pipeline.AddStepAfter (typeof (LoadReferencesStep), new LoadI18nAssemblies (I18nAssemblies.None));
                DeleteOldAssemblies ();
                pipeline.Process (context);
            }

            linkedAsmPaths = Directory.GetFiles (managedPath, "*.dll").OrderBy (x => Path.GetFileName (x)).ToList ();
        }

        class PreserveUsingAttributesStep : ResolveStep
        {
            readonly HashSet<string> ignoreAsmNames;

            public PreserveUsingAttributesStep (IEnumerable<string> ignoreAsmNames)
            {
                this.ignoreAsmNames = new HashSet<string> (ignoreAsmNames);
            }

            protected override void Process ()
            {
                var asms = Context.GetAssemblies ();
                foreach (var a in asms.Where (x => !ignoreAsmNames.Contains (x.Name.Name))) {
                    foreach (var m in a.Modules) {
                        foreach (var t in m.Types) {
                            PreserveTypeIfRequested (t);
                        }
                    }
                }
            }

            void PreserveTypeIfRequested (TypeDefinition type)
            {
                var typePreserved = IsTypePreserved (type);
                if (IsTypePreserved (type)) {
                    MarkAndPreserveAll (type);
                }
                else {
                    foreach (var m in type.Methods.Where (IsMethodPreserved)) {
                        Annotations.AddPreservedMethod (type, m);
                    }
                    foreach (var t in type.NestedTypes) {
                        PreserveTypeIfRequested (t);
                    }
                }
            }

            static bool IsTypePreserved (TypeDefinition m)
            {
                return m.CustomAttributes.FirstOrDefault (x => x.AttributeType.Name.StartsWith ("Preserve", StringComparison.Ordinal)) != null;
            }

            static bool IsMethodPreserved (MethodDefinition m)
            {
                return m.CustomAttributes.FirstOrDefault (x => x.AttributeType.Name.StartsWith ("Preserve", StringComparison.Ordinal)) != null;
            }

            void MarkAndPreserveAll (TypeDefinition type)
            {
                Annotations.MarkAndPush (type);
                Annotations.SetPreserve (type, TypePreserve.All);
                if (!type.HasNestedTypes) {
                    Tracer.Pop ();
                    return;
                }
                foreach (TypeDefinition nested in type.NestedTypes)
                    MarkAndPreserveAll (nested);
                Tracer.Pop ();
            }
        }

        Pipeline GetLinkerPipeline ()
        {
            IEnumerable<string> bclNames = bclAssemblies.Values.Select (Path.GetFileNameWithoutExtension);

            var p = new Pipeline ();
            p.AppendStep (new DontLinkExeStep ());
            p.AppendStep (new LoadReferencesStep ());
            p.AppendStep (new PreserveUsingAttributesStep (bclNames));
            p.AppendStep (new BlacklistStep ());
            p.AppendStep (new LinkBclStep (bclNames));
            p.AppendStep (new TypeMapStep ());
            p.AppendStep (new MarkStepWithUnresolvedLogging (this));
            p.AppendStep (new SweepStep ());
            p.AppendStep (new CleanStep ());
            p.AppendStep (new RegenerateGuidStep ());
            p.AppendStep (new OutputStep ());
            return p;
        }

        class DontLinkExeStep : BaseStep
        {
            protected override void Process ()
            {
                foreach (var a in Context.GetAssemblies ()) {
                    Annotations.SetAction (a, AssemblyAction.Copy);
                }
            }
        }

        class LinkBclStep : BaseStep
        {
            HashSet<string> bclNames;
            List<Tuple<string, string>> preserveTypeNames;

            public LinkBclStep (IEnumerable<string> bclNames)
            {
                // CSLA cannot tolerate mscorlib being linked (uses reflection over types it doesn't reference)
                this.bclNames = new HashSet<string> (bclNames.Where(x => x != "mscorlib"));
                preserveTypeNames = new List<Tuple<string, string>> {
                    Tuple.Create ("System", "System.ComponentModel.IEditableObject"),
                    Tuple.Create ("System", "System.ComponentModel.IDataErrorInfo"),
                };
            }

            protected override void Process ()
            {
                var asms = Context.GetAssemblies ();

                foreach (var a in asms) {
                    if (bclNames.Contains (a.Name.Name)) {
                        Annotations.SetAction (a, AssemblyAction.Link);
                    }
                }

                foreach (var p in preserveTypeNames) {
                    var asm = asms.FirstOrDefault (x => x.Name.Name == p.Item1);
                    if (asm == null)
                        throw new Exception ($"Could not find assembly {p.Item1}");
                    var t = asm.MainModule.GetType (p.Item2);
                    if (t == null)
                        throw new Exception ($"Could not find type {p.Item2} in {p.Item1}");
                    Annotations.SetPreserve (t, TypePreserve.All);
                }

                //foreach (var a in asms) {
                //    var act = Annotations.GetAction (a);
                //    Console.WriteLine ($"{act} {a.Name.Name}");
                //}
            }
        }

        class MarkStepWithUnresolvedLogging : MarkStep
        {
            BuildDistTask task;

            public MarkStepWithUnresolvedLogging (BuildDistTask task)
            {
                this.task = task;
            }

            protected override void HandleUnresolvedType (TypeReference reference)
            {
                task.ok = false;
                task.Log.LogError ($"Linker failed to resolve type {reference} in {reference.Scope}");
            }

            protected override void HandleUnresolvedMethod (MethodReference reference)
            {
                task.ok = false;
                task.Log.LogError ($"Linker failed to resolve method {reference}");
            }
        }

        void RenameAssemblies ()
        {
            for (int i = 0; i < linkedAsmPaths.Count; i++) {
                var path = linkedAsmPaths[i];
                var newPath = Path.ChangeExtension(path, AssemblyExtension);
                File.Move (path, newPath);
                linkedAsmPaths[i] = newPath;
            }
        }

        void ExtractClientJs ()
        {
            var oouiPath = refpaths.FirstOrDefault (x => Path.GetFileName (x).Equals ("Ooui.dll", StringComparison.InvariantCultureIgnoreCase));
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
    <meta charset=""utf-8"" />
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
                foreach (var l in linkedAsmPaths.Select (x => Path.GetFileName (x))) {
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

        class LinkerAssemblyResolver : Mono.Linker.AssemblyResolver
        {
            BuildDistTask task;

            public LinkerAssemblyResolver (BuildDistTask buildDistTask)
            {
                task = buildDistTask;
            }

            public override AssemblyDefinition Resolve (AssemblyNameReference name, ReaderParameters parameters)
            {
                AssemblyDefinition asm = null;
                if (!AssemblyCache.TryGetValue (name.Name, out asm)) {
                    var path = task.refpaths.FirstOrDefault (x => {
                        var rname = Path.GetFileNameWithoutExtension (x);
                        var eq = rname.Equals (name.Name, StringComparison.InvariantCultureIgnoreCase);
                        return eq;
                    });
                    if (path != null) {
                        //Console.WriteLine ($"SUCCESS {path}");
                        asm = ModuleDefinition.ReadModule (path, parameters).Assembly;
                        CacheAssembly (asm);
                    }
                    return base.Resolve (name, parameters);
                }
                return asm;
            }
        }

        class LinkerMetadataResolver : MetadataResolver
        {
            readonly AssemblyNameReference mscorlibScope = new AssemblyNameReference ("mscorlib", new Version (1, 0));

            public LinkerMetadataResolver (LinkerAssemblyResolver asmResolver)
                : base (asmResolver)
            {
            }

            public override TypeDefinition Resolve (TypeReference type)
            {
                var def = base.Resolve (type);
                if (def != null) return def;

                var scope = type.Scope;
                if (scope == null) return null;

                switch (scope.MetadataScopeType) {
                    case MetadataScopeType.AssemblyNameReference: {
                            AssemblyNameReference asmRef = (AssemblyNameReference)scope;
                            if (asmRef.Name == "System.Runtime") {                                
                                return base.Resolve (new TypeReference (type.Namespace, type.Name, type.Module, mscorlibScope));
                            }
                        }
                        break;
                }

                return def;
            }
        }
    }
}
