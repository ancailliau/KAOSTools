using System;
using System.Linq;
using UCLouvain.ExpertOpinionSharp.Frameworks;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Core.SatisfactionRates;
using UCLouvain.ExpertOpinionSharp.Statistics;
namespace UCLouvain.KAOSTools.ExpertCombination
{
	public enum ExpertCombinationMethod
	{
		Cook, MendelSheridan
	}

	public class ExpertCombinator
	{
		KAOSModel _model;
		ExpertCombinationMethod _method;
		double[] quantileVector;
		ExpertOpinionFramework ef;

		public ExpertCombinator(KAOSModel model, ExpertCombinationMethod method)
		{
			_model = model;
			_method = method;
		}

		public void Combine()
		{
			InitQuantileVector();
		
            if (_method == ExpertCombinationMethod.MendelSheridan) {
                ef = new MendelSheridanFramework (quantileVector);
            } else if (_method == ExpertCombinationMethod.Cook) {
                ef = new CookFramework (quantileVector);
            } else {
                throw new NotImplementedException ();
            }

            ef.OvershootFactor = 10;

            AddEstimates ();

            AddCalibration ();

            SetupUncertaintySatisfaction ();
		}

		void InitQuantileVector()
		{
			if (!_model.Parameters.ContainsKey("experts.quantiles"))
				throw new InvalidProgramException("Please specify the quantiles.");

			// Get the string and removes first and last characters (parenthesis)
			string quantile_string = _model.Parameters["experts.quantiles"];
			quantile_string = quantile_string.Remove(quantile_string.Length - 1).Substring(1);

			// Build the list of quantiles
			var quantiles = quantile_string.Split(',').Select(x => {
				var y = x.Trim();
				if (y.EndsWith("%", StringComparison.Ordinal)) {
					return double.Parse(y.Remove(y.Length - 1));
				} else {
					return double.Parse(y);
				}
			}).ToArray();

			quantileVector = new double[quantiles.Length + 2];
			quantileVector[0] = 0;
			for (int i = 0; i < quantiles.Length; i++) {
				quantileVector[i + 1] = quantiles[i];
			}
			quantileVector[quantileVector.Length - 1] = 1;
		}

		void AddEstimates()
		{
			foreach (var o in _model.LeafObstacles()) {
				var satRates = _model.satisfactionRateRepository.GetObstacleSatisfactionRates(o.Identifier);
				if (satRates == null)
				{
					Console.WriteLine($"Obstacle '{o.Identifier}' not estimated.");
				}
				else
				{
					var estimates = satRates.Where(x => x.ExpertIdentifier != null);
					foreach (var estimate in estimates)
					{
						if (estimate is QuantileList qd)
						{
							try
							{
								ef.AddEstimate(estimate.ExpertIdentifier, o.Identifier, qd.Quantiles.ToArray());
							} catch (Exception e) {
								Console.WriteLine("---");
								Console.WriteLine($"An error occured while adding estimate for '{o.Identifier}' by expert '{estimate.ExpertIdentifier}'");
								Console.WriteLine(e.Message);
								Console.WriteLine(e.StackTrace);
								Console.WriteLine("---");
							
							}
						}
						else
						{
							throw new NotImplementedException("Distribution not supported");
						}
					}
				}
			}
		}

		void AddCalibration()
		{
			foreach (var calibrationVariable in _model.CalibrationVariables()) {
				var estimates = _model.satisfactionRateRepository
									.GetCalibrationSatisfactionRates(calibrationVariable.Identifier)
									.Where(x => x.ExpertIdentifier != null);
				foreach (var estimate in estimates) {
					if (estimate is QuantileList qd) {
						ef.AddEstimate(estimate.ExpertIdentifier,
										calibrationVariable.Identifier,
										qd.Quantiles.ToArray());
					} else {
						throw new NotImplementedException("Distribution not supported");
					}
				}
				var real_value = _model.satisfactionRateRepository
									   .GetCalibrationSatisfactionRates(calibrationVariable.Identifier)
									   .Where(x => x.ExpertIdentifier == null).Last();
									   
				if (real_value is DoubleSatisfactionRate dsr) {
					ef.SetValue(calibrationVariable.Identifier, dsr.SatisfactionRate);
				} else {
					throw new NotImplementedException("Real value for the calibration variable must be a double.");
				}
			}
		}

		public IExpertFrameworkStatistics GetStatistics()
		{
			return ef.GetStatistics();
		}

		void SetupUncertaintySatisfaction()
		{
			foreach (var o in _model.LeafObstacles()) {
				try {
					var satRates = _model.satisfactionRateRepository
									.GetObstacleSatisfactionRates(o.Identifier);
					if (satRates != null)
					{
						var estimates = satRates.Where(x => x.ExpertIdentifier != null);
						if (estimates.Count() > 0)
						{
							var dm = ef.Fit(o.Identifier);
							if (dm is ExpertOpinionSharp.Distributions.QuantileDistribution quantileD)
							{
								var distribution = new QuantileDistribution(quantileD.probabilities, quantileD.quantiles);
								_model.satisfactionRateRepository.AddObstacleSatisfactionRate(o.Identifier, distribution);
							}
							else if (dm is ExpertOpinionSharp.Distributions.MixtureDistribution mixtureD)
							{
								var distribution = new MixtureDistribution(mixtureD.cummulativeWeight, mixtureD.distributions.Select(x => new QuantileDistribution(x.probabilities, x.quantiles)).ToArray());
								_model.satisfactionRateRepository.AddObstacleSatisfactionRate(o.Identifier, distribution);
							}
						}
						else
						{
							Console.WriteLine($"Obstacle '{o.Identifier}' not estimated by experts. Using 'probability' value.");
						}
					} else {
						Console.WriteLine($"Obstacle '{o.Identifier}' not estimated. Using 0 as satisfaction rate.");
						_model.satisfactionRateRepository.AddObstacleSatisfactionRate(o.Identifier, new DoubleSatisfactionRate(0));
					}
				} catch (Exception e) {
					//throw (e);
					Console.WriteLine("Error with " + o.FriendlyName);
					Console.WriteLine(e.Message);
					Console.WriteLine("---");
					Console.WriteLine(e);
					return;
				}
			}
		}
	}
}
