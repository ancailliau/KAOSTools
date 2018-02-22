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

		HashSet<Goal> _roots;

		BDDBasedPropagator _propagator;
		
		private static Logger logger = LogManager.GetCurrentClassLogger();

		public int MonitoredObstacleCount { 
			get {
				return obstacleMonitors.Count;
			}
		}

		public Dictionary<string,DoubleSatisfactionRate> RootSatisfactionRates { get; protected set; }

		public ModelMonitor(KAOSModel model, Goal root)
			: this (model, new[]{root})
		{
		}

		public ModelMonitor(KAOSModel model, IEnumerable<Goal> roots)
		{
			obstacleMonitors = new Dictionary<string, ObstacleMonitor>();
			_model_running = model;
			_roots = new HashSet<Goal>(roots);
			Initialize();
		}
		
		public void Stop()
		{
		}
		
		void Initialize ()
		{
			RootSatisfactionRates = new Dictionary<string, DoubleSatisfactionRate>();
			TimeSpan monitoringDelay = TimeSpan.FromMinutes(1);
		
			// Create the new obstacle monitors
			foreach (var obstacle in _model_running.LeafObstacles()
									      .Where(x => x.CustomData.ContainsKey("monitored") 
									                  && x.CustomData["monitored"].Equals("true"))) {

				IStateInformationStorage storage = new FiniteStateInformationStorage (60);
				var monitor = new ObstacleMonitor(obstacle, _model_running, storage, monitoringDelay);
				obstacleMonitors.Add(obstacle.Identifier, monitor);
			}

			// Initialize the propagator
			_propagator = new BDDBasedPropagator(_model_running);
			foreach (var root in _roots)
			{
				_propagator.PreBuildObstructionSet(root);
				RootSatisfactionRates.Add(root.Identifier, null);
			}
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
			foreach (var root in _roots)
			{
				RootSatisfactionRates[root.Identifier] = (DoubleSatisfactionRate)_propagator.GetESR(root);
			}

			if (!string.IsNullOrEmpty(outputFilename)) {
				var now = DateTime.Now;
				string str = string.Format("{0:yyyy-MM-dd HH:mm:ss.fffffff},{1},{2}", 
					now, 
					string.Join(",", RootSatisfactionRates.OrderBy(x => x.Key).Select(x => x.Value.SatisfactionRate)), 
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
				string root_names = string.Join(",", RootSatisfactionRates.OrderBy(x => x.Key).Select(x => x.Key));
				string obstacle_names = string.Join(",", obstacleMonitors.Select(x => x.Key + ",sd_" + x.Key));
				string headers = $"date,{root_names},{obstacle_names}";
				using (StreamWriter sw = File.CreateText(filename))
		        {
		            sw.WriteLine(headers);
		        }
			}
		}

		public void ModelChanged()
		{
			foreach (var root in _roots)
			{
				_propagator.PreBuildObstructionSet(root);
			}
		}
	}
}
