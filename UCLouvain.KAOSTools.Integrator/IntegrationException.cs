using System;
namespace UCLouvain.KAOSTools.Integrators
{
    public class IntegrationException : Exception
    {
        
        public const string NO_PATTERN = "The integration procedure requires resolution tactic to be specified.";
        public const string PATTERN_NOT_KNOWN = "The tactic {0} is not known by the integration procedure.";
        public const string SINGLE_REFINEMENT_ONLY = "The integration procedure does not support alternative refinements.";
        public const string ANCHOR_NO_REFINEMENT = "Anchor goal is not refined but 'Remove Obstructed Goal' integration schema was selected. "
        + "Make sure that anchor goal is the parent goal you guarantee, not the obstructed goal you want to remove.";
    
        public IntegrationException (string message) : base(message)
        {
        }
    }
}
