﻿using System;
using System.Linq;

namespace UCLouvain.KAOSTools.Core.SatisfactionRates
{
	public class SimulatedSatisfactionRate : UncertainSatisfactionRate
	{
		double[] _values;
		
		public double[] Values {
			get {
				return _values;
			}
		}
		
		public SimulatedSatisfactionRate(double[] values)
		{
			_values = values;
		}

		public override double Sample(Random r)
		{
			throw new NotImplementedException();
		}
		
		public double Mean {
			get {
				return _values.Sum() / _values.Count();
			}
		}
		
		public double ViolationUncertainty (double RSR) {
			int v = _values.Count(x => x < RSR);
			return v > 0 ? ((double)v) / _values.Count() : 0;
		}
		
		public double UncertaintySpread (double RSR) {
			int k = _values.Count(x => x < RSR);
			double s = _values.Where(x => x < RSR).Sum(x => (x - RSR) * (x - RSR));
			return k > 0 ? Math.Sqrt(s / k) : 0;
		}
	}
}
