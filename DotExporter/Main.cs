using System;
using System.Linq;
using System.IO;
using KAOSTools.Parsing;
using KAOSTools.Core;
using NDesk.Options;
using System.Collections.Generic;
using KAOSTools.Utils;

namespace KAOSTools.DotExporter
{
    class MainClass : KAOSToolCLI
    {
        public static void Main (string[] args)
        {
            string modelFilename     = "";

            options.Add ("o|output=", "File to use to store dot model",
                         v => modelFilename = v );

            Init (args);

            var exporter = new DotExport (model, 
                                          !string.IsNullOrEmpty (modelFilename) ? new StreamWriter (modelFilename) : Console.Out);

            exporter.ExportModel ();
            exporter.Close ();
        }
    }
}
