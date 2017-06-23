using System;
namespace UCLouvain.KAOSTools.Integrators
{
    public class PropagationException : Exception
    {
        public const string COULD_NOT_EXTRACT_ANTECEDANT = "Could not extract the antecedant in {0}.";
        public const string COULD_NOT_EXTRACT_CONSEQUENT = "Could not extract the consequent in {0}.";
    
        public PropagationException (string message) : base(message)
        {
        }
    }
}
