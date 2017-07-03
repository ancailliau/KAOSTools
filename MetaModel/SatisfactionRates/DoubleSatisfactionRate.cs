using System;
namespace UCLouvain.KAOSTools.Core.SatisfactionRates
{
    public class DoubleSatisfactionRate : ISatisfactionRate
    {
        public static readonly DoubleSatisfactionRate ONE = new DoubleSatisfactionRate (1);
        public static readonly DoubleSatisfactionRate ZERO = new DoubleSatisfactionRate (0);

        public double SatisfactionRate {
            get;
            private set;
        }

        public string ExpertIdentifier { 
            get; 
            set;
        }

        public DoubleSatisfactionRate(double sr)
        {
            SatisfactionRate = sr;
			ExpertIdentifier = null;
        }

        public ISatisfactionRate OneMinus ()
        {
            return new DoubleSatisfactionRate (1 - SatisfactionRate);
        }

        public ISatisfactionRate Product (ISatisfactionRate x)
        {
            if (x is DoubleSatisfactionRate)
                return new DoubleSatisfactionRate (SatisfactionRate * ((DoubleSatisfactionRate)x).SatisfactionRate);
            else
                throw new NotSupportedException ();
        }

        public ISatisfactionRate Product (double x)
        {
            return new DoubleSatisfactionRate (SatisfactionRate * x);
        }

        public ISatisfactionRate Sum (ISatisfactionRate x)
        {
            if (x is DoubleSatisfactionRate)
                return new DoubleSatisfactionRate (SatisfactionRate + ((DoubleSatisfactionRate)x).SatisfactionRate);
            else
                throw new NotSupportedException ();
        }
        
        public override string ToString ()
        {
            return string.Format ("[DoubleSatisfactionRate: Value={0}]", SatisfactionRate);
        }

        public double Sample ()
        {
            return SatisfactionRate;
        }

        public double Sample (Random r)
        {
            return SatisfactionRate;
        }
    }
}
