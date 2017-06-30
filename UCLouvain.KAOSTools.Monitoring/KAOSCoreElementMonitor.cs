﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using UCLouvain.KAOSTools.Core;
using LtlSharp;
using LtlSharp.Monitoring;
using NLog;
using MoreLinq;

namespace UCLouvain.KAOSTools.Monitoring
{
    public abstract class KAOSCoreElementMonitor
    {

        protected ProbabilisticLTLMonitor monitor;
        
        protected TimeSpan MonitoringDelay = TimeSpan.FromMinutes(1);

        /// <summary>
        /// The list of all probabilistic monitors.
        /// </summary>
        public Dictionary<KAOSCoreElement, ProbabilisticLTLMonitor> kaosElementMonitor;

        /// <summary>
        /// The projection used for computing a hash.
        /// </summary>
        public HashSet<string> projection;

        /// <summary>
        /// The buffer for the observed states.
        /// </summary>
        protected BufferBlock<CachedHashMonitoredState> buffer;

        /// <summary>
        /// The block processing the observed state to create new monitors.
        /// </summary>
        protected TransformBlock<CachedHashMonitoredState,CachedHashMonitoredState> createMonitors;

        /// <summary>
        /// The block processing the observed state to update monitors
        /// </summary>
        protected TransformBlock<CachedHashMonitoredState, CachedHashMonitoredState> updateMonitors;

        /// <summary>
        /// The block ending the dataflow TPL pipeline. Required for completion of the transform blocks.
        /// </summary>
        protected ActionBlock<CachedHashMonitoredState> dummyEnd;

        /// <summary>
        /// The current state, only used when saving the transition relation.
        /// </summary>
        int currentState = -1;

        /// <summary>
        /// The monitored states. Key is the hash, value is the first monitored state observed with that specific hash.
        /// </summary>
        protected Dictionary<int, CachedHashMonitoredState> monitoredStates;

        public Dictionary<int, MonitoredState> MonitoredStates {
            get {
                if (monitoredStates == null)
                    return null;
                return monitoredStates.ToDictionary (kv => kv.Key, kv => (MonitoredState) kv.Value);
            }
        }

        static Logger logger = LogManager.GetCurrentClassLogger();

        public bool Ready { get; protected set; }

        #region Private classes

        /// <summary>
        /// Private class for storing the hash computed on the projection of the monitored state
        /// </summary>
        public class CachedHashMonitoredState : MonitoredState
        {
            public int StateHash;
            public DateTime Timestamp;

            public CachedHashMonitoredState (MonitoredState ms, DateTime now, HashSet<string> projection) : base (ms)
            {
                Timestamp = now;
                StateHash = GetProjectedHashCode (projection);
            }

            public int GetProjectedHashCode (HashSet<string> propositions)
            {
                unchecked {
                    var localHash = 17;
                    int factor = 23;
                    foreach (var kv in state.Where (x => propositions.Contains (x.Key.Name))) {
                        localHash += factor * (kv.Key.Name.GetHashCode () + kv.Value.GetHashCode ());
                        factor *= 23;
                    }

                    return localHash;
                }
            }
        }

        #endregion

        public KAOSModel model;

        #region Constructors

        /// <summary>
        /// Creates a new monitoring infrastructure for the specified formulas. 
        /// The projection is used to compute the hash of the observed states.
        /// </summary>
        /// <param name="elements">The goals to monitor.</param>
        /// <param name="storage">The storage.</param>
        /// <param name="defaultProjection">Projection.</param>
        public KAOSCoreElementMonitor (KAOSModel model,
                            		   KAOSCoreElement elements,
                            		   TimeSpan monitoringDelay)
        {
            Ready = false;
            
            this.model = model;
            this.MonitoringDelay = monitoringDelay;
        }

        #endregion

        #region Goal to LTLSharp

        protected ITLFormula TranslateToLtlSharp(Formula formalSpec)
        {
            if (formalSpec is StrongImply) {
                var casted = ((StrongImply)formalSpec);
                var left = TranslateToLtlSharp(casted.Left);
                var right = TranslateToLtlSharp(casted.Right);
                return new LtlSharp.Globally(new LtlSharp.Implication (left, right));

            } else if (formalSpec is Imply) {
                var casted = ((Imply)formalSpec);
                var left = TranslateToLtlSharp(casted.Left);
                var right = TranslateToLtlSharp(casted.Right);
                return new LtlSharp.Implication(left, right);

            } else if (formalSpec is KAOSTools.Core.Until) {
                var casted = ((KAOSTools.Core.Until)formalSpec);
                var left = TranslateToLtlSharp(casted.Left);
                var right = TranslateToLtlSharp(casted.Right);
                return new LtlSharp.Until(left, right);

            } else if (formalSpec is KAOSTools.Core.Release) {
                var casted = ((KAOSTools.Core.Release)formalSpec);
                var left = TranslateToLtlSharp(casted.Left);
                var right = TranslateToLtlSharp(casted.Right);
                return new LtlSharp.Release(left, right);

            } else if (formalSpec is KAOSTools.Core.Unless) {
                var casted = ((KAOSTools.Core.Unless)formalSpec);
                var left = TranslateToLtlSharp(casted.Left);
                var right = TranslateToLtlSharp(casted.Right);
                return new LtlSharp.Unless(left, right);

            } else if (formalSpec is And) {
                var casted = ((KAOSTools.Core.And)formalSpec);
                var left = TranslateToLtlSharp(casted.Left);
                var right = TranslateToLtlSharp(casted.Right);
                return new LtlSharp.Conjunction(left, right);

            } else if (formalSpec is Or) {
                var casted = ((KAOSTools.Core.Or)formalSpec);
                var left = TranslateToLtlSharp(casted.Left);
                var right = TranslateToLtlSharp(casted.Right);
                return new LtlSharp.Disjunction(left, right);

            } else if (formalSpec is Not) {
                var casted = ((KAOSTools.Core.Not)formalSpec);
                var enclosed = TranslateToLtlSharp(casted.Enclosed);
                return new LtlSharp.Negation(enclosed);

            } else if (formalSpec is KAOSTools.Core.Next) {
                var casted = ((KAOSTools.Core.Not)formalSpec);
                var enclosed = TranslateToLtlSharp(casted.Enclosed);
                return new LtlSharp.Negation(enclosed);

            } else if (formalSpec is Eventually) {
                var casted = ((KAOSTools.Core.Eventually)formalSpec);
                var enclosed = TranslateToLtlSharp(casted.Enclosed);
                if (casted.TimeBound == null) 
                    return new LtlSharp.Finally(enclosed);
                else
                    return new LtlSharp.BoundedFinally(enclosed, ConvertBound (casted.TimeBound));

            } else if (formalSpec is KAOSTools.Core.Globally) {
                var casted = ((KAOSTools.Core.Globally)formalSpec);
                if (casted.TimeBound != null) {
                    var enclosed = TranslateToLtlSharp(casted.Enclosed);
                    return new LtlSharp.BoundedGlobally(enclosed, ConvertBound (casted.TimeBound));
                    
                } else {
                    var enclosed = TranslateToLtlSharp(casted.Enclosed);
                    return new LtlSharp.Globally(enclosed);
                }

            } else if (formalSpec is PredicateReference) {
                var casted = ((KAOSTools.Core.PredicateReference)formalSpec);
                return new LtlSharp.Proposition(casted.PredicateIdentifier);
            }

            throw new NotImplementedException(string.Format ("Operator {0} is not translatable to LTLSharp framework.",
                                                             formalSpec.GetType ().FullName));
        }

        int ConvertBound(TimeBound span)
        {
            // todo not the safest...
            // todo issue warning if division is not an integer
            var value = (int) Math.Ceiling(span.Bound.TotalMilliseconds / MonitoringDelay.TotalMilliseconds);
            logger.Trace("Convertion of {0} to {1}", span.Bound.ToString(), value);
            logger.Trace("Monitoring Delay : {0}", MonitoringDelay);
            return value;
        }

        #endregion

        /// <summary>
        /// Run the monitors and save a summary every second in summaryFilename.
        /// </summary>
        /// <param name="saveTransitionRelation">Whether the transition relation shall be kept.</param>
        public void Run ()
        {
            buffer = new BufferBlock<CachedHashMonitoredState> ();
            createMonitors = new TransformBlock<CachedHashMonitoredState, CachedHashMonitoredState> ((ms) => CreateNewMonitors(ms));
            updateMonitors = new TransformBlock<CachedHashMonitoredState, CachedHashMonitoredState> ((ms) => UpdateMonitors (ms));
            dummyEnd = new ActionBlock<CachedHashMonitoredState> (_ => { });
            
            var opt = new DataflowLinkOptions ();
            opt.PropagateCompletion = true;

            buffer.LinkTo (createMonitors, opt);
            createMonitors.LinkTo (updateMonitors, opt);
            updateMonitors.LinkTo (dummyEnd, opt);

            Ready = true;
        }

        public void Stop ()
        {
            buffer.Complete ();
            createMonitors.Completion.Wait ();
            updateMonitors.Completion.Wait ();
        }

        /// <summary>
        /// Adds the monitored state to the processing pipeline.
        /// </summary>
        /// <param name="currentState">The monitored state.</param>
        public void MonitorStep (MonitoredState currentState, DateTime now)
        {
            var cachedMs = new CachedHashMonitoredState (currentState, now, projection);
            buffer.Post (cachedMs);
        }

        /// <summary>
        /// Starts a new monitors for the specified state.
        /// </summary>
        /// <returns>The monitored state.</returns>
        /// <param name="ms">The monitored state.</param>
        CachedHashMonitoredState CreateNewMonitors (CachedHashMonitoredState ms)
        {
        	try {
        		monitor.StartNew(ms.StateHash, ms);
			} catch (Exception e) {
				logger.Error($"Fail to update monitor ({e.Message})");
			}
            return ms;
        }

        /// <summary>
        /// Updates the monitors for the specified state.
        /// </summary>
        /// <returns>The monitored state.</returns>
        /// <param name="ms">The monitored state.</param>
        CachedHashMonitoredState UpdateMonitors (CachedHashMonitoredState ms)
        {
			try {
				monitor.Step(ms);
			} catch (Exception e) {
				logger.Error($"Fail to update monitor ({e.Message})");
			}
            return ms;
        }
    }
}
