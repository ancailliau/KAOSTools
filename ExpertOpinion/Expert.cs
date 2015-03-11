using System;
using System.Collections.Generic;

namespace ExpertOpinionSharp
{
    /// <summary>
    /// Models an expert.
    /// </summary>
    class Expert {

        /// <summary>
        /// Gets the name of the expert
        /// </summary>
        /// <value>The name.</value>
        public string Name {
            get;
            private set;
        }

        /// <summary>
        /// Gets the estimates for all variables
        /// </summary>
        /// <value>The estimates.</value>
        public IDictionary<ExpertVariable, ExpertEstimate> Estimates {
            get;
            private set;
        }

        public Expert(string name)
        {
            this.Name = name;
            this.Estimates = new Dictionary<ExpertVariable, ExpertEstimate> ();
        }

        /// <summary>
        /// Adds the estimate for the specified variable <c>variable</c>.
        /// </summary>
        /// <param name="variable">Variable.</param>
        /// <param name="estimates">Estimates.</param>
        public void AddEstimate (ExpertVariable variable, ExpertEstimate estimate)
        {
            Estimates.Add (variable, estimate);
        }

    }
}