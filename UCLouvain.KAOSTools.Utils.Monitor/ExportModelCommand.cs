using System;
using System.Text.RegularExpressions;
using UCLouvain.KAOSTools.Monitoring;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Utils.FileExporter;
using System.IO;

namespace UCLouvain.KAOSTools.Utils.Monitor
{
	public class ExportModelCommand : ICommand
	{
		KAOSModel _model;

		public ExportModelCommand(KAOSModel model)
		{
			_model = model;
		}

		public void Execute(string command)
		{
			Regex regex = new Regex (@"export ([a-zA-Z\.][a-zA-Z0-9_\.-]*)");
            Match match = regex.Match (command);
			if (match.Success) {

				var exporter = new KAOSFileExporter(_model);
				var e = exporter.Export();
				string filename = match.Groups[1].Value;
				if (File.Exists(filename)) {
					Console.WriteLine("File exits. Do you want to overwrite? (yes/no)");
					var a = Console.ReadLine();
					if (!a.Equals("yes"))
						return;
				}
				File.WriteAllText(filename, e);
				Console.WriteLine($"Model exported to '{filename}'.");
			}
		}
	}
}
