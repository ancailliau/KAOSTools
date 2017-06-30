using System;
using System.Text.RegularExpressions;
using UCLouvain.KAOSTools.Monitoring;
using UCLouvain.KAOSTools.Core;

namespace UCLouvain.KAOSTools.Utils.Monitor
{
	public class GetSatisfactionRateCommand : ICommand
	{
		ModelMonitor _modelMonitor;
		KAOSModel _model;
		Goal _root;

		public GetSatisfactionRateCommand(KAOSModel model, Goal root, ModelMonitor modelMonitor)
		{
			_modelMonitor = modelMonitor;
			_model = model;
			_root = root;
		}

		public void Execute(string command)
		{
			Regex regex = new Regex (@"satrate ([a-zA-Z][a-zA-Z0-9_-]*)");
            Match match = regex.Match (command);
			if (match.Success) {
				var o_identifier = match.Groups[1].Value;

				if (o_identifier.Equals(_root.Identifier))
					Console.WriteLine($"{_modelMonitor.RootSatisfactionRate}");

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
