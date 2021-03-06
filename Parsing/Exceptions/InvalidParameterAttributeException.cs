﻿using System;
namespace UCLouvain.KAOSTools.Parsing.Parsers.Exceptions
{
    public class InvalidParameterAttributeException : Exception
    {
        public const string ATOMIC_ONLY = "Attribute '{0}' only accept atomic parameter.";
        public const string BRACKET_ONLY = "Attribute '{0}' only accept parameter with parameters (between brackets).";
        public const string COLON_ONLY = "Attribute '{0}' only accept pair of value (separated with a ':') as a parameter.";
        public const string LIST_ONLY = "Attribute '{0}' only accept list of parameters.";

        public const string ATOMIC_OR_LIST = "Attribute '{0}' only accept an atomic parameter or a list of parameters.";

        public const string IDENTIFIER = "Attribute '{0}' only accept identifier expression parameter.";

        public const string NO_PARAM = "Attribute '{0}' does not accept parameters.";
        public const string INVALID_VALUE = "Attribute '{0}' only accept a valid parameters. See online documentation for more details.";

        public InvalidParameterAttributeException(string identifier, string message)
            : base(string.Format(message, identifier))
        {
        }
    }
}
