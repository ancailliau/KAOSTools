using System;
using System.Collections.Generic;
using UCLouvain.KAOSTools.Core;
using System.Linq;
using UCLouvain.KAOSTools.Core.SatisfactionRates;
using UCLouvain.KAOSTools.Propagators;
using System.Diagnostics;
using UCLouvain.KAOSTools.Integrators;
using MoreLinq;
using UCLouvain.KAOSTools.Propagators.BDD;

namespace UCLouvain.KAOSTools.Optimizer
{
    public class MOCECountermeasureSelectionOptimizer
    {
    	private class Candidate 
    	{
			public HashSet<string> cm;
			
			public double score;
			
			public int cost { get { return cm.Count();  }}

			public int nb_root_sat;
			
			internal IEnumerable<string> sat_root;

			public int Rank
			{
				get;
				set;
			}
			
			public Candidate(HashSet<string> cm, double score, IEnumerable<string> sat_root)
			{
				this.cm = cm;
				this.score = score;
				this.nb_root_sat = sat_root.Count();
				this.sat_root = sat_root;
			}
			
			public bool Dominate (Candidate x) {
				return (x.cost < this.cost & x.score >= this.score & x.nb_root_sat >= this.nb_root_sat)
				| (x.cost <= this.cost & x.score > this.score & x.nb_root_sat >= this.nb_root_sat)
				| (x.cost <= this.cost & x.score >= this.score & x.nb_root_sat > this.nb_root_sat);
			}
			
			public override bool Equals(object obj)
			{
				if (obj == null)
					return false;
				if (obj is Candidate c)
				{
					return c.cm.SetEquals(this.cm) & c.score == this.score & this.nb_root_sat == c.nb_root_sat;
				}
				else
				{
					return false;
				}
			}

			public override int GetHashCode()
			{
				int hash = 13;
				List<string> list = cm.ToList();
				list.Sort();
				foreach (var c in list)
				{
					hash = 27 * (hash) + c.GetHashCode();
				}
				return 27 * hash + 31 * (this.score.GetHashCode() + 31 * this.nb_root_sat);
			}
    	}
    
    	const int N = 75;
		const int MaxIterations = 400;
		const int StoppingCriterion = 6;
		const double rho = .1;
		const double alpha = .6;

		double lastNbSolutionRank1;
		int Iterations;
    
        KAOSModel _model;
        OptimizationStatistics stats;

        SoftResolutionIntegrator integrator;

        const double EPSILON = 1E-15;

        public MOCECountermeasureSelectionOptimizer (KAOSModel model)
        {
            _model = model;
            stats = new OptimizationStatistics ();
        }
        
        
		public IEnumerable<OptimalSelection> GetOptimalSelections(Goal goal, IPropagator propagator)
		{
			return GetOptimalSelections(new HashSet<Goal>(new[] { goal }), propagator);
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
		
		private IEnumerable<Candidate> Iterate(IEnumerable<Goal> goals, IPropagator propagator, double[] sampling, Dictionary<int,string> mapping, Random random, double rho)
		{
			var candidates = new List<Candidate>();
			
			for (int i = 0; i < N; i++)
			{
				HashSet<string> selection = GetRandomSelection(sampling, mapping, random);
				
				var satRate = GetScore(goals, propagator, selection, out IEnumerable<string> nb_root_sat);
				candidates.Add(new Candidate(selection, satRate, nb_root_sat));
			}

			for (int i = 0; i < N; i++)
			{
				candidates[i].Rank = 1 + candidates.Count(x => candidates[i].Dominate(x));
			}

			candidates = candidates.OrderBy(x => x.Rank).ToList();

			for (int i = 0; i < N; i++)
			{
				var c = candidates[i];
				Console.WriteLine($"{c.Rank:00} | {c.cost} {c.nb_root_sat} {c.score:0.00} | " + string.Join(",", c.cm));
			}
			Console.WriteLine();

			var gammaT = candidates[(int)Math.Ceiling((1 - rho) * N)].Rank;

			var new_sampling = new double[sampling.Length];
			var normalizationFactor = 0;
			for (int i = 0; i < sampling.Length; i++)
			{
				int c = candidates.Count(x => x.Rank <= gammaT & x.cm.Contains(mapping[i]));
				new_sampling[i] = c;
				normalizationFactor += c;
				//Console.WriteLine($"sampling[{i}] = {c}");
			}

			

			for (int i = 0; i < sampling.Length; i++)
			{
				sampling[i] = alpha * sampling[i] + (1 - alpha) * (new_sampling[i] / normalizationFactor);
			}

			Console.WriteLine($"{gammaT:0.00} | " + string.Join(" ", sampling.Select(x => $"{x:0.00}")));

			return candidates.Where(x => x.Rank == 1);
		}
		
		private double GetScore(IEnumerable<Goal> goals, IPropagator propagator, HashSet<string> selection, out IEnumerable<string> rootsSatisfied)
		{
			var r = _model.Resolutions().Where(x => selection.Contains(x.ResolvingGoalIdentifier));
			var rootGoalsSatisfied = new HashSet<string> ();

            integrator = new SoftResolutionIntegrator (_model);
			Console.WriteLine("---- Computing score for " + string.Join(",", selection) + ".");
			// Integrate the selection
			foreach (var resolution in r)
			{
				Console.WriteLine("Integrate " + resolution.ResolvingGoalIdentifier);
				integrator.Integrate(resolution);
			}
			
			var p2 = new BDDBasedPropagator (_model);

			//var all_goals_satisfied = true;
			var avg_sat_rate = 0.0;
			foreach (var goal in goals)
			{
				var satRate = ((DoubleSatisfactionRate)p2.GetESR(goal));
				avg_sat_rate += satRate.SatisfactionRate;
				if (satRate.SatisfactionRate >= goal.RDS) {
					rootGoalsSatisfied.Add(goal.Identifier);
				}
				
				Console.WriteLine("Cost " + selection.Count);
				Console.WriteLine("Selected cm: " + string.Join(",", selection));
				Console.WriteLine($"{goal.Identifier} : {satRate.SatisfactionRate} >= {goal.RDS}.");
			}
			
			//var all_goals_satisfied = (satRate.SatisfactionRate > goal.RDS);

			
			var cost = selection.Count();


			foreach (var resolution in r)
			{
				integrator.Remove(resolution);
			}

			//if (!all_goals_satisfied)
			//{
			//	Console.WriteLine("---- not all goal satisfied, returning score = 0");
			//	return 0;
			//}

			rootsSatisfied = rootGoalsSatisfied;
			
			//if (satRate > goal.RDS) {
			Console.WriteLine("---- returning score = " + (avg_sat_rate / goals.Count ()));
				return avg_sat_rate / goals.Count ();
			//} else {
			//	return 0;
			//}
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
			var countermeasure_goals = _model.Resolutions().Select(x => x.ResolvingGoalIdentifier).Distinct();

			// Initialize vector
			var mapping = Enumerable.Zip(Enumerable.Range(0, countermeasure_goals.Count()), countermeasure_goals, (arg1, arg2) => new { arg1, arg2 })
									.ToDictionary(item => item.arg1, item => item.arg2);

			double[] sampling = GetInitialSampling(countermeasure_goals, mapping);

			var random = new Random();
			
			int i = 0;
			var paretoFront = new HashSet<Candidate>();
			Iterations = 0;
			while (Iterations < StoppingCriterion & i < MaxIterations)
			{
				Console.WriteLine("iterate");
				var paretoFrontIteration = Iterate (goals, propagator, sampling, mapping, random, rho);
				
				var spf = paretoFront.Count;
				
				// Add all new non-dominated solutions
				foreach (var item in paretoFrontIteration)
				{
					paretoFront.Add(item);
				}
				
				//Console.WriteLine("Before");
				//Console.WriteLine(string.Join("\n", paretoFront.Select(x => x.score + " " + x.cost + " | " + string.Join(",", x.cm))));

				// Remove all dominated solutions
				paretoFront = paretoFront.Where(x => !paretoFront.Any(y => y.Dominate(x))).ToHashSet();
				
				//Console.WriteLine("After");
				//Console.WriteLine(string.Join("\n", paretoFront.Select(x => x.score + " " + x.cost + " | " + string.Join(",", x.cm))));

				
				if (spf == paretoFront.Count)
				{
					Iterations++;

				}
				else
				{
					Iterations = 0;
				}

				//Console.WriteLine($"{paretoFront.Count,-4} | " + string.Join(" ", sampling.Select(x => $"{x:0.00}")));
				i++;
			}
			
			Console.WriteLine("Pareto Front:");
			Console.WriteLine(string.Join("\n", paretoFront.Select(x => x.score + " " + x.nb_root_sat + " " + x.cost + " | " + string.Join(",", x.cm))));


			var solution = new List<OptimalSelection>();
			foreach (var candidate in paretoFront) {
				var activeResolutions = _model.Resolutions ().Where (x => candidate.cm.Contains (x.ResolvingGoalIdentifier));
				solution.Add(new OptimalSelection(activeResolutions, candidate.cost, candidate.score, candidate.sat_root));
			}
			return solution;
        }

        public OptimizationStatistics GetStatistics ()
        {
            return stats;
        }
    }
}
