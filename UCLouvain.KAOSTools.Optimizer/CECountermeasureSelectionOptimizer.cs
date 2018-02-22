using System;
using System.Collections.Generic;
using UCLouvain.KAOSTools.Core;
using System.Linq;
using UCLouvain.KAOSTools.Core.SatisfactionRates;
using UCLouvain.KAOSTools.Propagators;
using System.Diagnostics;
using UCLouvain.KAOSTools.Integrators;

namespace UCLouvain.KAOSTools.Optimizer
{
    public class CECountermeasureSelectionOptimizer
    {
    	private class Candidate 
    	{
			public HashSet<string> cm;
			public double score;
			
			public Candidate(HashSet<string> cm, double score)
			{
				this.cm = cm;
				this.score = score;
			}
    	}
    
    	const int N = 50;
		const int MaxIterations = 50;
		const int StoppingCriterion = 5;

		double lastGammaT;
		int Iterations;
    
        KAOSModel _model;
        OptimizationStatistics stats;

        SoftResolutionIntegrator integrator;

        const double EPSILON = 1E-15;

        public CECountermeasureSelectionOptimizer (KAOSModel model)
        {
            _model = model;
            stats = new OptimizationStatistics ();
            integrator = new SoftResolutionIntegrator (model);
        }

        public double GetMinimalCost (Goal goal, IPropagator propagator)
        {
        	var countermeasure_goals = _model.Resolutions().Select(x => x.ResolvingGoalIdentifier).Distinct();

			// Initialize vector
			var mapping = Enumerable.Zip(Enumerable.Range(0, countermeasure_goals.Count()), countermeasure_goals, (arg1, arg2) => new { arg1, arg2 })
									.ToDictionary(item => item.arg1, item => item.arg2);

			double[] sampling = GetInitialSampling(countermeasure_goals, mapping);

			var random = new Random();
			var rho = .1;

			int i = 0;
			while (Iterations < StoppingCriterion & i < MaxIterations)
			{
				var gammaT = IterateCost(goal, propagator, sampling, mapping, random, rho);
				if (Math.Abs(gammaT - lastGammaT) <= EPSILON)
				{
					Iterations++;

				}
				else
				{
					Iterations = 0;
				}

				lastGammaT = gammaT;

				Console.WriteLine($"{gammaT:0.00} | " + string.Join(" ", sampling.Select(x => $"{x:0.00}")));
				i++;
			}

			return lastGammaT;
        }

		public IEnumerable<OptimalSelection> GetOptimalSelections(double minCost, Goal goal, IPropagator propagator)
		{
			var countermeasure_goals = _model.Resolutions().Select(x => x.ResolvingGoalIdentifier).Distinct();

			// Initialize vector
			var mapping = Enumerable.Zip(Enumerable.Range(0, countermeasure_goals.Count()), countermeasure_goals, (arg1, arg2) => new { arg1, arg2 })
									.ToDictionary(item => item.arg1, item => item.arg2);

			double[] sampling = GetInitialSampling(countermeasure_goals, mapping);

			var random = new Random();
			var rho = .1;

			int i = 0;
			while (Iterations < StoppingCriterion & i < MaxIterations)
			{
				var gammaT = Iterate(minCost, goal, propagator, sampling, mapping, random, rho);
				if (Math.Abs(gammaT - lastGammaT) <= EPSILON)
				{
					Iterations++;

				}
				else
				{
					Iterations = 0;
				}

				lastGammaT = gammaT;

				Console.WriteLine($"{gammaT:0.00} | " + string.Join(" ", sampling.Select(x => $"{x:0.00}")));
				i++;
			}

			return null;
		}

		private static double[] GetInitialSampling(IEnumerable<string> countermeasure_goals, Dictionary<int,string> mapping)
		{
			var sampling = new double[countermeasure_goals.Count()];

			Console.WriteLine("     | " + string.Join(" ", mapping.Select(x => $"{x.Value,-4}")));
			Console.WriteLine("-----+-" + string.Join("-", mapping.Select(x => "----")));

			for (int j = 0; j < sampling.Length; j++)
			{
				sampling[j] = .5;
			}
			Console.WriteLine("     | " + string.Join(" ", sampling.Select(x => $"{x:0.00}")));
			return sampling;
		}
		
		private double IterateCost(Goal goal, IPropagator propagator, double[] sampling, Dictionary<int,string> mapping, Random random, double rho)
		{
			var candidates = new List<Candidate>();
			
			for (int i = 0; i < N; i++)
			{
				HashSet<string> selection = GetRandomSelection(sampling, mapping, random);
				
					var maxSatRate = GetScore(goal, propagator, selection);
					if (maxSatRate > goal.RDS)
					{
						candidates.Add(new Candidate(selection, maxSatRate));
					} else {
						candidates.Add(new Candidate(selection, double.MaxValue));
					}
			}

			candidates = candidates.OrderByDescending(x => x.score).ToList();

			var gammaT = candidates[(int)Math.Ceiling((1 - rho) * N)].cm.Count();

			var normalizationFactor = 0;
			for (int i = 0; i < sampling.Length; i++)
			{
				int c = candidates.Count(x => x.score >= gammaT & x.cm.Contains(mapping[i]));
				sampling[i] = c;
				normalizationFactor += c;
				//Console.WriteLine($"sampling[{i}] = {c}");
			}

			for (int i = 0; i < sampling.Length; i++)
			{
				sampling[i] /= normalizationFactor;
			}

			return gammaT;
		}

		private double Iterate(double minCost, Goal goal, IPropagator propagator, double[] sampling, Dictionary<int,string> mapping, Random random, double rho)
		{
			var candidates = new List<Candidate>();
			
			for (int i = 0; i < N; i++)
			{
				HashSet<string> selection = GetRandomSelection(sampling, mapping, random);
				if (selection.Count() > minCost)
				{
					// Ignore solution
					candidates.Add(new Candidate(selection, 0));
				}
				else
				{
					var maxSatRate = GetScore(goal, propagator, selection);
					if (maxSatRate > goal.RDS)
					{
						candidates.Add(new Candidate(selection, maxSatRate));
					} else {
						candidates.Add(new Candidate(selection, 0));
					}
				}
			}

			candidates = candidates.OrderBy(x => x.score).ToList();

			var gammaT = candidates[(int)Math.Ceiling((1 - rho) * N)].score;

			var normalizationFactor = 0;
			for (int i = 0; i < sampling.Length; i++)
			{
				int c = candidates.Count(x => x.score >= gammaT & x.cm.Contains(mapping[i]));
				sampling[i] = c;
				normalizationFactor += c;
				//Console.WriteLine($"sampling[{i}] = {c}");
			}

			for (int i = 0; i < sampling.Length; i++)
			{
				sampling[i] /= normalizationFactor;
			}

			return gammaT;
		}

		private double GetScore(Goal goal, IPropagator propagator, HashSet<string> selection)
		{
			double maxSatRate;
			var r = _model.Resolutions().Where(x => selection.Contains(x.ResolvingGoalIdentifier));

			// Integrate the selection
			foreach (var resolution in r)
			{
				integrator.Integrate(resolution);
			}

			var satRate = (DoubleSatisfactionRate)propagator.GetESR(goal);
			//var all_goals_satisfied = (satRate.SatisfactionRate > goal.RDS);

			//if (all_goals_satisfied) {
			var cost = selection.Count();

			maxSatRate = satRate.SatisfactionRate;

			foreach (var resolution in r)
			{
				integrator.Remove(resolution);
			}

			return maxSatRate;
		}

		private static HashSet<string> GetRandomSelection(double[] sampling, Dictionary<int,string> mapping, Random random)
		{
			var selection = new HashSet<string>();

			// Randomly pick the countermeasure goals
			for (int j = 0; j < sampling.Length; j++)
			{
				if (random.NextDouble() <= sampling[j])
				{
					selection.Add(mapping[j]);
				}
			}

			return selection;
		}

		public IEnumerable<OptimalSelection> GetOptimalSelections (HashSet<Goal> goals, IPropagator propagator)
        {
			throw new NotImplementedException();
        }

        // Code from Stackoverflow
        // https://stackoverflow.com/questions/7802822/all-possible-combinations-of-a-list-of-values
        static List<List<T>> GetAllCombinations<T> (List<T> list)
        {
            List<List<T>> result = new List<List<T>> ();
            result.Add (new List<T> ());
            result.Last ().Add (list [0]);
            if (list.Count == 1)
                return result;
            List<List<T>> tailCombos = GetAllCombinations (list.Skip (1).ToList ());
            tailCombos.ForEach (combo => {
                result.Add (new List<T> (combo));
                combo.Add (list [0]);
                result.Add (new List<T> (combo));
            });
            return result;
        }

        public OptimizationStatistics GetStatistics ()
        {
            return stats;
        }
    }
}
