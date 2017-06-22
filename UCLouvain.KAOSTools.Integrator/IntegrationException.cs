using System;
namespace UCLouvain.KAOSTools.Integrator
{
    public class IntegrationException : Exception
    {
        
        public const string SINGLE_REFINEMENT_ONLY = "The integration procedure does not support alternative refinements.";
        public const string ANCHOR_NO_REFINEMENT = "Anchor goal is not refined but 'Remove Obstructed Goal' integration schema was selected. "
        + "Make sure that anchor goal is the parent goal you guarantee, not the obstructed goal you want to remove.";
    
        public IntegrationException (string message) : base(message)
        {
        }
    }
}
