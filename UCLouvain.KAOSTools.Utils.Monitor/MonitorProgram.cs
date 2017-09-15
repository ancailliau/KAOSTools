using System;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Monitoring;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Linq;
using NLog;
using UCLouvain.KAOSTools.Optimizer;
using UCLouvain.KAOSTools.Propagators.BDD;
using MoreLinq;
using UCLouvain.KAOSTools.Integrators;

namespace UCLouvain.KAOSTools.Utils.Monitor
{
	class MonitorProgram : KAOSToolCLI
	{
		const string monitored_state_queue_name = "kaos_monitored_state_queue";
		const string kaos_cm_selection_queue_name = "kaos_cm_selection_queue";
		
		static ModelMonitor modelMonitor;
		
		private static Logger logger = LogManager.GetCurrentClassLogger();

		static bool stop;
		private static NaiveCountermeasureSelectionOptimizer optimizer;
		private static KAOSModel optimization_model;
		private static HashSet<Goal> roots;

		static SoftResolutionIntegrator _integrator_model;
		static IEnumerable<Resolution> _active_resolutions;

		static void Listen ()
		{
			var ts = new ThreadStart(ListenLoop);
			var t = new Thread(ts);
			t.Start();
            Console.WriteLine("Listening...");
		}

		static void ListenLoop ()
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };
	        using(var connection = factory.CreateConnection())
	        using(var channel = connection.CreateModel())
	        {
	            channel.QueueDeclare(queue: monitored_state_queue_name,
	                                 durable: false,
	                                 exclusive: false,
	                                 autoDelete: false,
	                                 arguments: null);
	
	            var consumer = new EventingBasicConsumer(channel);
	            consumer.Received += (model, ea) =>
	            {
	                var body = ea.Body;
	                var message = Encoding.UTF8.GetString(body);
	                
            		var dict = new JavaScriptSerializer().Deserialize<Dictionary<string, bool>>(message);
					
					var monitoredState = new LtlSharp.Monitoring.MonitoredState();
					foreach (var kv in dict) {
						monitoredState.Set(kv.Key, kv.Value);
					}
					
					try {
						modelMonitor.Update(monitoredState, DateTime.Now);
						
					} catch (Exception e) {
						logger.Error(e.Message);
						logger.Error(e.StackTrace);
					}
	                
	            };
	            
				channel.BasicConsume(queue: monitored_state_queue_name,
	                                 autoAck: true,
	                                 consumer: consumer);
	                                 
	            while(!stop) { Thread.Sleep(10); }
	        }
		}
		
		static void Optimize ()
		{
			var ts = new ThreadStart(OptimizeLoop);
			var t = new Thread(ts);
			t.Start();
            Console.WriteLine("Optimizing...");
		}
		
		static void OptimizeLoop ()
		{
			var factory = new ConnectionFactory() { HostName = "localhost" };
			var currentSelection = new HashSet<string> ();
			
	        using(var connection = factory.CreateConnection())
	        using(var channel = connection.CreateModel())
	        {
	            channel.QueueDeclare(queue: kaos_cm_selection_queue_name,
	                                 durable: false,
	                                 exclusive: false,
	                                 autoDelete: false,
	                                 arguments: null);
	
	            var _propagator_optimization = new BDDBasedPropagator(optimization_model);
				while (!stop) {
					System.Threading.Thread.Sleep(TimeSpan.FromMinutes(2));

					bool optimization_required = false;
					foreach (var root in roots)
					{
						var doubleSatisfactionRate = modelMonitor.RootSatisfactionRates[root.Identifier];
						if (doubleSatisfactionRate == null) {
							logger.Info($"Ignoring '{root.Identifier}', no satisfaction rate monitored.");
							continue;
						}
						if (doubleSatisfactionRate.SatisfactionRate >= root.RDS)
						{
							logger.Info("Current configuration is above RSR for "+root.FriendlyName+".");
						} else {
							logger.Info("Current configuration is below RSR for "+root.FriendlyName+".");
							optimization_required = true;
						}
					}

					if (!optimization_required)
						continue;

					var minimalcost = optimizer.GetMinimalCost(roots, _propagator_optimization);
					var optimalSelections = optimizer.GetOptimalSelections(minimalcost, roots, _propagator_optimization).FirstOrDefault();

					if (optimalSelections != null) {
						// Update the model
						var deployment_methods = new List<string>();
						foreach (var resolution in optimalSelections.Resolutions.Except(_active_resolutions)) {
							_integrator_model.Integrate(resolution);
							deployment_methods.Add(resolution.ResolvingGoal().CustomData["ondeploy"]);
						}
						foreach (var resolution in optimalSelections.Resolutions.Intersect(_active_resolutions)) {
							_integrator_model.Remove(resolution);
							deployment_methods.Add(resolution.ResolvingGoal().CustomData["onwithold"]);
						}
						modelMonitor.ModelChanged();
						
						_active_resolutions = optimalSelections.Resolutions.ToHashSet();
					
						// Deploy the countermeasures in the running system
						var json = new JavaScriptSerializer().Serialize(deployment_methods);
						logger.Info(json);
            		
						var body = Encoding.UTF8.GetBytes(json);
		        
						channel.BasicPublish(exchange: "",
		                             routingKey: kaos_cm_selection_queue_name,
		                             basicProperties: null,
		                             body: body);
						
					} else {
						logger.Info("No selection found!");
					}
				}
	        }
		}
	
		public static void Main(string[] args)
		{
            Console.WriteLine ("*** This is Monitor from KAOSTools. ***");
            Console.WriteLine ("*** For more information on KAOSTools see <https://github.com/ancailliau/KAOSTools> ***");
            Console.WriteLine ("*** Please report bugs to <https://github.com/ancailliau/KAOSTools/issues> ***");
            Console.WriteLine ();
            Console.WriteLine ("*** Copyright (c) 2017, Université catholique de Louvain ***");
            Console.WriteLine ("");

            string rootname = "root";
            options.Add ("roots=", "Specify the roots goal for which to compute the satisfaction rate. (Default: root)", v => rootname = v);

			string outfile = null;
            options.Add ("outfile=", "Specify the output file for the satisfaction rates", v => outfile = v);

			Init (args);
			optimization_model = BuildModel();
            
            roots = rootname.Split(',').Select(x => model.Goal (x)).ToHashSet();
            if (roots.Count() == 0 || roots.Any(x => x == null)) {
                PrintError ("A root goal among '"+rootname+"' was not found");
            }

            try {

				modelMonitor = new ModelMonitor(model, roots);
				modelMonitor.SetOutputFile(outfile);

				_active_resolutions = new HashSet<Resolution>();
				_integrator_model = new SoftResolutionIntegrator(model);
				optimizer = new NaiveCountermeasureSelectionOptimizer(optimization_model);
				
				var commands = new List<ICommand>();
				commands.Add(new GetSatisfactionRateCommand(model, roots, modelMonitor));
				commands.Add(new ExportModelCommand(model));

				stop = false;
				
				Listen();
				Optimize();

				while (!stop) {
					Console.Write("> ");
					var input = Console.ReadLine();
					if (input.Equals("quit") | input.Equals("exit")) {
						stop = true;
						continue;
					}
					
					foreach (var command in commands) {
						command.Execute(input);
					}
				}
				
				Console.WriteLine("Exiting...");
				modelMonitor.Stop();

			} catch (Exception e) {
                PrintError ("An error occured during the computation. ("+e.Message+").\n"
                +"Please report this error to <https://github.com/ancailliau/KAOSTools/issues>.\n"
                +"----------------------------\n"
                +e.StackTrace
                +"\n----------------------------\n");
                
            }
		}
	}
}
