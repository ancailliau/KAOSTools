﻿using System;
namespace UCLouvain.KAOSTools.Propagators
{
    public class PropagationException : Exception
    {
        public const string MULTIPLE_REFINEMENTS
        = "Multiple refinements are not supported in propagation.";

        public const string PATTERN_NOT_SUPPORTED
        = "The refinement pattern is not supported in propagation.";

        public const string MISSING_PARAMETER
        = "The by-case refinement pattern requiers parameters.";

        public PropagationException (string message) : base (message)
        {
        }
    }
}
