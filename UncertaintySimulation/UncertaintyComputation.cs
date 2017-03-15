using System;
using BDDSharp;
using System.Collections.Generic;
using KAOSTools.Core;
using System.Linq;
using ExpertOpinionSharp.Frameworks;

namespace UncertaintySimulation
{
    public class UncertaintyComputation
    {
        int n_sample = 10000;
        public int NSamples {
            get { return n_sample; }
            set { n_sample = value; }
        }

        double overshoot_factor = .1;
        public double OverShoot {
            get { return overshoot_factor; }
            set { overshoot_factor = value; }
        }

        Random r = new Random ();
        public Random RandomGenerator {
            get { return r; }
            set { r = value; }
        }

        KAOSModel model;

        ObstructionSuperset ComputeObstructionSets (Goal root)
        {
            return root.GetObstructionSuperset ();
        }

        Dictionary<KAOSCoreElement, double[]> InitProbabilityVectors (Goal root, ObstructionSuperset os)
        {
            Dictionary<KAOSCoreElement, double[]> probabilityVectors;
            probabilityVectors = new Dictionary<KAOSCoreElement, double[]> ();
            foreach (var n in os.reverse_mapping) {
                probabilityVectors.Add (n.Value, new double[n_sample]);
            }
            probabilityVectors.Add (root, new double[n_sample]);
            return probabilityVectors;
        }

        double[] quantileVector;

        void InitQuantileVector (KAOSModel model)
        {
            var quantiles = model.Parameters ["experts.quantiles"].Remove (model.Parameters ["experts.quantiles"].Length - 1).Substring (1).Split (',').Select (x =>  {
                var y = x.Trim ();
                if (y.EndsWith ("%")) {
                    return double.Parse (y.Remove (y.Length - 1));
                }
                else {
                    return double.Parse (y);
                }
            });
            var enumerable = quantiles.ToArray ();
            quantileVector = new double[enumerable.Count () + 2];
            quantileVector [0] = 0;
            for (int i = 0; i < enumerable.Count (); i++) {
                quantileVector [i + 1] = enumerable [i];
            }
            quantileVector [quantileVector.Length - 1] = 1;
        }

        static void AddEstimates (KAOSModel model, ExpertOpinionFramework ef)
        {
            foreach (var o in model.LeafObstacles ()) {
                foreach (var estimate in o.ExpertEstimates) {
                    ef.AddEstimate (estimate.Key.Identifier, o.Identifier, estimate.Value.Quantiles.ToArray ());
                }
            }
        }

        static void AddCalibration (KAOSModel model, ExpertOpinionFramework ef)
        {
            foreach (var o in model.CalibrationVariables ()) {
                foreach (var estimate in o.ExpertEstimates) {
                    ef.AddEstimate (estimate.Key.Identifier, o.Identifier, estimate.Value.Quantiles.ToArray ());
                }
                ef.SetValue (o.Identifier, o.EPS);
            }
        }

        static void SetupUncertaintySatisfaction (KAOSModel model, ExpertOpinionFramework ef)
        {
            foreach (var o in model.LeafObstacles ()) {
                try {
                    if (o.ExpertEstimates.Count > 0) {
                        var dm = ef.Fit (o.Identifier);
                        if (dm is ExpertOpinionSharp.Distributions.QuantileDistribution) {
                            var ddm = (ExpertOpinionSharp.Distributions.QuantileDistribution)dm;
                            o.SatisfactionUncertainty = new QuantileDistribution (ddm.probabilities, ddm.quantiles);
                        }
                        else
                            if (dm is ExpertOpinionSharp.Distributions.MixtureDistribution) {
                                var ddm = (ExpertOpinionSharp.Distributions.MixtureDistribution)dm;
                                o.SatisfactionUncertainty = new MixtureDistribution (ddm.cummulativeWeight, ddm.distributions.Select (x => new QuantileDistribution (x.probabilities, x.quantiles)).ToArray ());
                            }
                    }
                    else {
                        throw new NotImplementedException ("'" + o.FriendlyName + "' is not estimated.");
                    }
                }
                catch (Exception e) {
                    throw(e);
                    Console.WriteLine ("Error with " + o.FriendlyName);
                    Console.WriteLine (e.Message);
                    Console.WriteLine ("---");
                    Console.WriteLine (e);
                    return;
                }
            }
        }

        Goal root;

        public UncertaintyComputation (KAOSModel model, Goal root, UncertaintySimulation.MainClass.COMBINATION_METHOD combination_method)
        {
            this.model = model;
            this.root = root;

            InitQuantileVector (model);

            ExpertOpinionFramework ef;
            if (combination_method == UncertaintySimulation.MainClass.COMBINATION_METHOD.MS) {
                ef = new MendelSheridanFramework (quantileVector);
            } else if (combination_method == UncertaintySimulation.MainClass.COMBINATION_METHOD.COOK) {
                ef = new CookFramework (quantileVector);
            } else {
                throw new NotImplementedException ();
            }

            ef.OvershootFactor = overshoot_factor;

            AddEstimates (model, ef);

            AddCalibration (model, ef);

            SetupUncertaintySatisfaction (model, ef);
        }

        public Dictionary<KAOSCoreElement, double[]> Simulate ()
        {
            return Simulate (root, null);
        }

        public Dictionary<KAOSCoreElement, double[]> Simulate (Goal goal)
        {
            return Simulate (goal, null);
        }

        public Dictionary<KAOSCoreElement, double[]> Simulate (Goal goal, IEnumerable<Obstacle> obstacles)
        {
            var obstructionSuperset = ComputeObstructionSets (goal);
            var probabilityVectors = InitProbabilityVectors (goal, obstructionSuperset);

            for (int i = 0; i < n_sample; i++) {
                // Fill sample vector
                var sampleVector = new Dictionary <int, double> ();
                foreach (var n in obstructionSuperset.reverse_mapping) {
                    double sample;
                    if (n.Value is Obstacle && (obstacles == null || obstacles.Contains ((Obstacle) n.Value))) {
                        var obstacle = ((Obstacle)n.Value);

                        if (obstacle.SatisfactionUncertainty == null)
                            throw new ArgumentNullException (string.Format ("Obstacle '{0}' has a null UEPS", 
                                obstacle.FriendlyName));

                        sample = obstacle.SatisfactionUncertainty.Sample(r);

                    } else if (n.Value is DomainHypothesis) {
                        var domainHypothesis = ((DomainHypothesis)n.Value);
                        if (domainHypothesis.SatisfactionUncertainty == null)
                            throw new ArgumentNullException (string.Format ("Domain hypothesis '{0}' has a null UEPS", 
                                domainHypothesis.FriendlyName));

                        sample = domainHypothesis.SatisfactionUncertainty.Sample(r);

                    } else {
                        throw new NotImplementedException (string.Format ("'{0}' is not supported", n.GetType ()));
                    }
                    sampleVector.Add(n.Key, sample);
                    probabilityVectors[n.Value][i] = sample;
                }

                probabilityVectors[goal][i] = 1.0 - obstructionSuperset.GetProbability (sampleVector);
            }

            return probabilityVectors;
        }
    }
}

