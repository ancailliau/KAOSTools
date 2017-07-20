﻿using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace UCLouvain.KAOSTools.Monitoring
{
    public class MonitoringClient : IDisposable
    {
        string monitored_state_queue_name;
		TimeSpan _monitoring_delay;

        IConnection connection;
        IModel channel;

        Func<Dictionary<string, bool>> GetState;
    
        public MonitoringClient(Func<Dictionary<string, bool>> getState,
        						string queue_name, 
        						TimeSpan monitoring_delay)
        {
			monitored_state_queue_name = queue_name;
			_monitoring_delay = monitoring_delay;	
        
            Setup();
            GetState = getState;
        }

        public void Dispose()
        {
            channel.Dispose();
            connection.Dispose();
        }

        void Setup ()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            
            channel.QueueDeclare(queue: monitored_state_queue_name,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }
        
        public void Run ()
        {
            var ts = new ThreadStart(ExecuteStep);
            var t = new Thread(ts);
            t.Start();
        }
        
        void ExecuteStep ()
        {
            while (!stop)
            {
                Send();
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }            
        }
        
        public void Send ()
        {
            var message = GetState();        
            var json = new JavaScriptSerializer().Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(exchange: "",
                                 routingKey: monitored_state_queue_name,
                                 basicProperties: null,
                                 body: body);
        }

        bool stop = false;

        internal void Stop()
        {
            stop = true;
            Dispose();
        }
    }
}
