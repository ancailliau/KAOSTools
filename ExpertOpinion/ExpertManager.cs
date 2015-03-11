using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;

namespace ExpertOpinionSharp
{
    class ExpertManager {

        /// <summary>
        /// Gets or sets the overshoot factor. (default: 10)
        /// </summary>
        /// <value>The k.</value>
        public int K {
            get;
            set;
        }

        /// <summary>
        /// Gets the experts.
        /// </summary>
        /// <value>The experts.</value>
        public ISet<Expert> Experts {
            get;
            private set;
        }

        /// <summary>
        /// Gets the variables.
        /// </summary>
        /// <value>The variables.</value>
        public ISet<ExpertVariable> Variables {
            get;
            private set;
        }

        public double[] Quantiles {
            get;
            private set;
        }

        public ExpertManager()
        {
            this.K = 10;
            this.Experts = new HashSet<Expert> ();
            this.Variables = new HashSet<ExpertVariable> ();
            this.Quantiles = new double[] { .05, .5, .95 };
        }

        public ExpertManager(IEnumerable<Expert> experts) : this()
        {
            foreach (var expert in experts) {
                AddExpert (expert);
            }
        }

        public void AddExpert (Expert e)
        {
            this.Experts.Add (e);
            foreach (var variable in e.Estimates.Keys) {
                this.Variables.Add (variable);
            }
        }

        public void SetQuantiles(double[] quantiles)
        {
            this.Quantiles = quantiles;
        }

        #region Private methods

        double[] GetInterquantileRanges (double[] quantiles)
        {
            var result = new List<double> ();
            var last = 0d;
            for (int i = 0; i < quantiles.Length; i++) {
                var qi = quantiles[i];
                result.Add (qi - last);
                last = qi;
            }
            result.Add (1 - last);

            return result.ToArray ();
        }


        int GetInterval (double value, ExpertEstimate estimates, double[] quantiles)
        {
            if (value <= estimates.GetQuantile(quantiles[0])) {
                return 0;
            }

            int i;
            for (i = 0; i < quantiles.Length - 1; i++) {
                if (estimates.GetQuantile(quantiles[i]) <= value
                    && value <= estimates.GetQuantile(quantiles[i + 1])) {
                    return i + 1;
                }
            }

            if (estimates.GetQuantile(quantiles[i]) <= value) {
                return i + 1;
            }

            throw new InvalidOperationException ();
        }

        void GetBounds (ExpertVariable var, out double vmin, out double vmax)
        {
            var min = Experts.Min (x => x.Estimates[var].GetQuantile(0));
            var max = Experts.Max (x => x.Estimates[var].GetQuantile(1));

            if (var is CalibrationVariable) {
                var cVar = (CalibrationVariable) var;
                min = Math.Min (min, cVar.TrueValue);
                max = Math.Max (max, cVar.TrueValue);
            }

            var overshoot = (max - min) * K / 100.0;

            vmin = min - overshoot;
            vmax = max + overshoot;
        }

        double[] GetInterpolatedDistribution (CalibrationVariable v, ExpertEstimate estimate, double[] quantiles)
        {
            var res = new List<double> ();
            double lowerBound, upperBound;
            GetBounds(v, out lowerBound, out upperBound);

            Func<double, double, double> interpolate = (x, y) => 1.0d * (y - x) / (upperBound - lowerBound);

            res.Add(interpolate(lowerBound, estimate.GetQuantile(quantiles[0])));

            var quantilesCount = quantiles.Count();
            for (int i = 1; i < quantilesCount; i++) {
                var l0 = estimate.GetQuantile (quantiles[i - 1]);
                var l1 = estimate.GetQuantile (quantiles[i]);
                res.Add (interpolate (l0, l1));
            }

            var lastQuantile = quantiles[quantilesCount - 1];
            res.Add (interpolate (estimate.GetQuantile(lastQuantile), upperBound));

            return res.ToArray ();
        }


        /// <summary>
        /// Gets the empirical distributions for the specified expert <c>e</c>.
        /// </summary>
        /// <returns>The empirical distributions.</returns>
        /// <param name="e">E.</param>
        double[] GetEmpiricalDistributions (Expert e, double[] quantiles)
        {
            var res = new double[quantiles.Length + 1];

            foreach (var v in Variables.OfType<CalibrationVariable> ()) {
                var trueValue = v.TrueValue;
                var estimates = e.Estimates[v];
                var i = GetInterval (trueValue, estimates, quantiles);
                res[i]++;
            }

            var nvar = Variables.Count();
            return res.Select (x => x / nvar).ToArray ();
        }

        #endregion

        /// <summary>
        /// Gets the information score for the specified variable <c>v</c> and specified expert <c>e</c>.
        /// </summary>
        /// <returns>The information score.</returns>
        /// <param name="v">The variable.</param>
        /// <param name="e">The expert.</param>
        public double GetInformationScore (CalibrationVariable v, Expert e)
        {
            var expertOpinion = e.Estimates[v];
            var p = GetInterquantileRanges (Quantiles);
            var r = GetInterpolatedDistribution(v, expertOpinion, Quantiles);

            var score = 0d;
            for (int i = 0; i < p.Length; i++) {
                var lscore = (p[i] * Math.Log(p[i] / r[i]));
                score += lscore;
            }
            
            return score;
        }

        /// <summary>
        /// Gets the information score for the specified expert <c>e</c>.
        /// </summary>
        /// <returns>The information score.</returns>
        /// <param name="e">The expert.</param>
        public double GetInformationScore (Expert e)
        {
            var score = 0d;
            foreach (var v in Variables.OfType<CalibrationVariable>()) {
                var lscore = GetInformationScore(v, e);
                score += lscore;
            }
            return score / Variables.Count ();
        }

        /// <summary>
        /// Gets the calibration score for the specified expert <c>e</c>.
        /// </summary>
        /// <returns>The calibration score.</returns>
        /// <param name="e">The expert.</param>
        public double GetCalibrationScore (Expert e)
        {
            var p = GetInterquantileRanges (Quantiles);
            var score = 0d;
            var s = GetEmpiricalDistributions (e, Quantiles);

            for (int i = 0; i < p.Length; i++) {
                double lscore = 0;
                var epsilon = Double.Parse("10e-5");
                if (Math.Abs(p[i]) < epsilon && Math.Abs(s[i]) > epsilon) {
                    throw new ArgumentException ("Specified quantiles are incompatible with datas. There" +
                        "is no absolute continuity between interpolated probabilities and empirical " +
                        "distribution. See http://en.wikipedia.org/wiki/Kullback%E2%80%93Leibler_divergence for " +
                        "more details.");
                }
                if (s[i] > 0 & p[i] > 0)
                    lscore = s[i] * Math.Log(s[i] / p[i]);
                score += lscore;
            }

            var nvar = Variables.Count();
            return 1 - ChiSquared.CDF (p.Length - 1, 2 * nvar * score);
        }

        /// <summary>
        /// Gets the calibration scores for all experts, sorted by score.
        /// </summary>
        /// <returns>The calibration scores.</returns>
        public List<Tuple<Expert, double>> GetCalibrationScores ()
        {
            var scores = Experts.Select (x => new Tuple<Expert, double> (x, GetCalibrationScore (x)))
                .ToList ();
            scores.Sort ((x, y) => x.Item2.CompareTo(y.Item2));
            return scores;
        }

        /// <summary>
        /// Gets the information scores for all experts, sorted by score.
        /// </summary>
        /// <returns>The information scores.</returns>
        public List<Tuple<Expert, double>> GetInformationScores ()
        {
            var scores = Experts.Select (x => new Tuple<Expert, double> (x, GetInformationScore (x)))
                .ToList ();
            scores.Sort ((x, y) => y.Item2.CompareTo(x.Item2));
            return scores;
        }

        /// <summary>
        /// Gets the weights for all experts, sorted.
        /// </summary>
        /// <returns>The weights.</returns>
        public IEnumerable<Tuple<Expert, double>> GetWeights ()
        {
            var scores = Experts.Select (x => new Tuple<Expert, double> (x, GetCalibrationScore (x) * GetInformationScore (x)))
                .ToList ();
            scores.Sort ((x, y) => y.Item2.CompareTo(x.Item2));
            var scaling = scores.Sum (x => x.Item2);
            return scores.Select (x => new Tuple<Expert, double> (x.Item1, x.Item2 / scaling));
        }
    }

}