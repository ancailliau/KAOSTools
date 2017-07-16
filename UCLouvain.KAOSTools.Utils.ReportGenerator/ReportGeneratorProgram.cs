using System;
using System.IO;
using RazorEngine;
using RazorEngine.Templating;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.SatisfactionRates;

namespace UCLouvain.KAOSTools.Utils.ReportGenerator
{
	class ReportGeneratorProgram : KAOSToolCLI
	{
		public static void Main(string[] args)
		{
			Console.WriteLine ("*** This is ReportGenerator from KAOSTools. ***");
            Console.WriteLine ("*** For more information on KAOSTools see <https://github.com/ancailliau/KAOSTools> ***");
            Console.WriteLine ("*** Please report bugs to <https://github.com/ancailliau/KAOSTools/issues> ***");
            Console.WriteLine ();
            Console.WriteLine ("*** Copyright (c) 2017, Université catholique de Louvain ***");
            Console.WriteLine ("");


			string template_name = null;
            options.Add ("template=", "Specify the template to use", v => template_name = v);
            
			string outfile = null;
            options.Add ("outfile=", "Specify the outpute file", v => outfile = v);
            
  			Init (args);

			if (string.IsNullOrEmpty(template_name))
			{
				PrintError("Please specify a template.");
				return;
			}

			var template_filename = template_name + ".template";
			if (File.Exists(template_filename)) {
				var template_content = File.ReadAllText(template_filename);
				var result = Engine.Razor.RunCompile(template_content, template_name + "Key", null, model);
				if (!string.IsNullOrEmpty(outfile)) {
					var out_filename = Environment.ExpandEnvironmentVariables(outfile);

					if (out_filename.StartsWith("~", StringComparison.Ordinal)) {
						var homepath = Environment.GetEnvironmentVariable("HOME");
						out_filename = out_filename.Replace("~", homepath);
					}
				
					File.WriteAllText(out_filename, result);
				}
				
				Console.WriteLine(result);
				
			} else {
				PrintError($"Template not found '{template_filename}'.");
				return;
			}
		}
	}
}
