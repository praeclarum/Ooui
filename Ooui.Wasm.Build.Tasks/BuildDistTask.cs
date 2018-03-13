using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Ooui.Wasm.Build.Tasks
{
    public class BuildDistTask : MarshalByRefObject, ITask
    {
        [Required]
		public string Assembly { get; set; }
		public string ReferencePath { get; set; }

        public bool Execute ()
		{
            var fullAsmPath = Path.GetFullPath (Assembly);
            Console.WriteLine ("YO " + fullAsmPath);
            return true;
        }

        public IBuildEngine BuildEngine { get; set; }
		public ITaskHost HostObject { get; set; }

    }

}
