using System;
using System.Text.RegularExpressions;
using UCLouvain.KAOSTools.Monitoring;
using UCLouvain.KAOSTools.Core;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace UCLouvain.KAOSTools.Utils.Monitor
{
	public class GetSatisfactionRateCommand : ICommand
	{
		ModelMonitor _modelMonitor;
		KAOSModel _model;
		HashSet<string> _root;

		public GetSatisfactionRateCommand(KAOSModel model, HashSet<Goal> root, ModelMonitor modelMonitor)
		{
			_modelMonitor = modelMonitor;
			_model = model;
			_root = root.Select(x => x.Identifier).ToHashSet();
		}

		public void Execute(string command)
		{
			Regex regex = new Regex (@"satrate ([a-zA-Z][a-zA-Z0-9_-]*)");
            Match match = regex.Match (command);
			if (match.Success) {
				var o_identifier = match.Groups[1].Value;

				if (_root.Contains(o_identifier))
					Console.WriteLine($"{_modelMonitor.RootSatisfactionRates[o_identifier]}");

				if (_model.Obstacle(o_identifier) != null) {
					var monitor = _modelMonitor.GetMonitor(o_identifier);

					if (monitor != null) {
						var state = monitor.MonitoredSatisfactionRate;
						if (state != null)
							Console.WriteLine($"{state.Mean} ({state.Negative}/{(state.Negative + state.Positive)})");
						else
							Console.WriteLine("No states observed so far.");
					} else {
						Console.WriteLine($"Obstacle '{o_identifier}' not found.");
					}
				}
			}
		}
	}
}
