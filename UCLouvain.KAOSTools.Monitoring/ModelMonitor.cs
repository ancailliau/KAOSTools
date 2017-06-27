using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp;
using LtlSharp.Monitoring;
using UCLouvain.KAOSTools.Core;
namespace UCLouvain.KAOSTools.Monitoring
{
	public class ModelMonitor
	{
		Dictionary<string, ObstacleMonitor> obstacleMonitors;

		KAOSModel model;

		public int MonitoredObstacleCount { 
			get {
				return obstacleMonitors.Count;
			}
		}

		public ModelMonitor(KAOSModel model)
		{
			obstacleMonitors = new Dictionary<string, ObstacleMonitor>();
			this.model = model;
			Initialize();
		}
		
		void Initialize ()
		{
			HashSet<string> projection = null;
			IStateInformationStorage storage = null;
			TimeSpan monitoringDelay = TimeSpan.FromSeconds(1);
		
			// Create the new obstacle monitors
			foreach (var obstacle in model.LeafObstacles()
									      .Where(x => x.CustomData.ContainsKey("monitored") 
									                  && x.CustomData["monitored"].Equals("true"))) {
				var monitor = new ObstacleMonitor(obstacle, model, projection, storage, monitoringDelay);
				obstacleMonitors.Add(obstacle.Identifier, monitor);
			}
		}
		
		public void Update (MonitoredState ms, DateTime datetime)
		{
			// Create the new monitors and update the existing ones accordingly
			foreach (var m in obstacleMonitors.Values) {
				m.MonitorStep(ms, datetime);
			}
		}
	}
}
