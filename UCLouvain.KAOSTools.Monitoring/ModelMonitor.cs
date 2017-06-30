using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp;
using LtlSharp.Monitoring;
using NLog;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.SatisfactionRates;
using UCLouvain.KAOSTools.Propagators;
using UCLouvain.KAOSTools.Propagators.BDD;
using System.IO;
using UCLouvain.KAOSTools.Optimizer;
namespace UCLouvain.KAOSTools.Monitoring
{
	public class ModelMonitor
	{
		Dictionary<string, ObstacleMonitor> obstacleMonitors;

		KAOSModel _model_running;

		Goal _root;

		BDDBasedPropagator _propagator;
		
		private static Logger logger = LogManager.GetCurrentClassLogger();

		public int MonitoredObstacleCount { 
			get {
				return obstacleMonitors.Count;
			}
		}

		public DoubleSatisfactionRate RootSatisfactionRate { get; protected set; }

		public ModelMonitor(KAOSModel model, Goal root)
		{
			obstacleMonitors = new Dictionary<string, ObstacleMonitor>();
			_model_running = model;
			_root = root;
			Initialize();
		}
		
		public void Stop()
		{
		}
		
		void Initialize ()
		{
			TimeSpan monitoringDelay = TimeSpan.FromSeconds(1);
		
			// Create the new obstacle monitors
			foreach (var obstacle in _model_running.LeafObstacles()
									      .Where(x => x.CustomData.ContainsKey("monitored") 
									                  && x.CustomData["monitored"].Equals("true"))) {

				IStateInformationStorage storage = new FiniteStateInformationStorage (400);
				var monitor = new ObstacleMonitor(obstacle, _model_running, storage, monitoringDelay);
				obstacleMonitors.Add(obstacle.Identifier, monitor);
			}

			// Initialize the propagator
			_propagator = new BDDBasedPropagator(_model_running);
			_propagator.PreBuildObstructionSet(_root);
		}
		
		public void Update (MonitoredState ms, DateTime datetime)
		{
			// Create the new monitors and update the existing ones accordingly
			foreach (var m in obstacleMonitors.Values) {
				try {
					m.MonitorStep(ms, datetime);

					if (m.MonitoredSatisfactionRate != null) {
						// Update the satisfaction rate for the obstacle
						_model_running.satisfactionRateRepository.AddObstacleSatisfactionRate(
							m.Obstacle.Identifier,
							new DoubleSatisfactionRate(m.MonitoredSatisfactionRate.Mean));
					}
					
				} catch (Exception e) {
					logger.Error($"Fail to update monitor for '{m.Obstacle.FriendlyName}' ({e.Message})");
				}
			}

			// Compute the satisfaction rate for the root goal
			RootSatisfactionRate = (DoubleSatisfactionRate) _propagator.GetESR(_root);

			if (!string.IsNullOrEmpty(outputFilename)) {
				var now = DateTime.Now;
				string str = string.Format("{0:yyyy-MM-dd HH:mm:ss.fffffff},{1},{2}", 
					now, 
					RootSatisfactionRate.SatisfactionRate, 
					string.Join(",", obstacleMonitors.Select(x => (x.Value.MonitoredSatisfactionRate?.Mean ?? 0) + "," + (x.Value.MonitoredSatisfactionRate?.StdDev ?? 0)))
				);
				using (StreamWriter sw = File.AppendText(outputFilename))
		        {
		            sw.WriteLine(str);
		        }
			}
		}
		
		public ObstacleMonitor GetMonitor (string identifier)
		{
			if (obstacleMonitors.ContainsKey(identifier)) {
				return obstacleMonitors[identifier];
			} else {
				return null;
			}
		}

		string outputFilename;
		
		public void SetOutputFile (string filename)
		{
			outputFilename = filename;
			
			if (!File.Exists(filename)) {
				string headers = $"date,{_root.Identifier},{string.Join(",", obstacleMonitors.Select(x => x.Key + ",sd_" + x.Key))}";
				using (StreamWriter sw = File.CreateText(filename))
		        {
		            sw.WriteLine(headers);
		        }
			}
		}

		public void ModelChanged()
		{
			_propagator.PreBuildObstructionSet(_root);
		}
	}
}
