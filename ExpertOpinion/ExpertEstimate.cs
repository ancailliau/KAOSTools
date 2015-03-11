using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpertOpinionSharp
{
    /// <summary>
    /// Models the opinion of an expert. 
    /// </summary>
    interface ExpertEstimate {
        double GetQuantile (double d);
    }

    class TriangularEstimate : ExpertEstimate {

        public double Low { get; set; }
        public double Mid { get; set; }
        public double High { get; set; }

        public TriangularEstimate (double low, double mid, double high)
        {
            this.Low = low;
            this.Mid = mid;
            this.High = high;
        }

        public TriangularEstimate (double low, double mid, double high, double overshoot)
            : this (low, mid, high)
        {
            this.Low = low - overshoot * (high - low);
            this.High = high + overshoot * (high - low);
        }

        public double GetQuantile(double d)
        {
            return MathNet.Numerics.Distributions.Triangular.InvCDF(Low, High, Mid, d);
        }
    }

    class BetaEstimate : ExpertEstimate {

        public float Alpha { get; set; }
        public float Beta { get; set; }

        public BetaEstimate(float alpha, float beta)
        {
            this.Alpha = alpha;
            this.Beta = beta;
        }

        public double GetQuantile(double d)
        {
            return MathNet.Numerics.Distributions.Beta.InvCDF (Alpha, Beta, d);
        }
    }

    class PERTEstimate : ExpertEstimate {

        public double Low { get; private set; }
        public double Mid { get; private set; }
        public double High { get; private set; }

        double mean;
        double alpha;
        double beta;

        public PERTEstimate (double low, double mid, double high)
        {
            SetValues (low, mid, high);
        }

        public PERTEstimate (double low, double mid, double high, double overshoot)
            : this (low, mid, high)
        {
            this.SetValues (low - overshoot * (high - low), mid, high + overshoot * (high - low));
        }

        public void SetValues (double low, double mid, double high)
        {
            Low = low;
            Mid = mid;
            High = high;

            mean = (low + 4 * mid + high) / 6;
            alpha = 6 * ((mean - low) / (high - low));
            beta = 6 * ((high - mean) / (high - low));
        }

        public double GetQuantile(double d)
        {
            return (alglib.invincompletebeta (alpha, beta, d) * (High - Low)) + Low;
        }
    }
}

