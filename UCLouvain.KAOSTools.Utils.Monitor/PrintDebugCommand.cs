using System;
using System.Text.RegularExpressions;
using UCLouvain.KAOSTools.Monitoring;
using UCLouvain.KAOSTools.Core;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace UCLouvain.KAOSTools.Utils.Monitor
{
	public class PrintDebugCommand : ICommand
	{
		ModelMonitor _modelMonitor;
		KAOSModel _model;
		HashSet<string> _root;

		public PrintDebugCommand(KAOSModel model, HashSet<Goal> root, ModelMonitor modelMonitor)
		{
			_modelMonitor = modelMonitor;
			_model = model;
			_root = root.Select(x => x.Identifier).ToHashSet();
		}

		public void Execute(string command)
		{
			Regex regex = new Regex (@"print_debug ([a-zA-Z][a-zA-Z0-9_-]*)");
            Match match = regex.Match (command);
			if (match.Success) {
				var o_identifier = match.Groups[1].Value;

				if (_model.Obstacle(o_identifier) != null) {
					var monitor = _modelMonitor.GetMonitor(o_identifier);

					if (monitor != null) {
						Console.WriteLine("Debug:");
						Console.WriteLine(monitor.Storage.PrintDebug());
					
					} else {
						Console.WriteLine($"Obstacle '{o_identifier}' not found.");
					}
				}
			}
		}
	}
}
