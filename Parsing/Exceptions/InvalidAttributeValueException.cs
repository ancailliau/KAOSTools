using System;
namespace UCLouvain.KAOSTools.Parsing.Parsers.Exceptions
{
    public class InvalidAttributeValueException : Exception
    {
        public const string ATOMIC_ONLY = "Attribute '{0}' only accept atomic value.";
        public const string BRACKET_ONLY = "Attribute '{0}' only accept value with parameters (between brackets).";
        public const string COLON_ONLY = "Attribute '{0}' only accept pair of value (separated with a ':').";
        public const string LIST_ONLY = "Attribute '{0}' only accept list of values.";

        public const string IDENTIFIER = "Attribute '{0}' only accept an identifier expression as a value.";
        public const string FLOAT_INTEGER_PERCENTAGE_ONLY = "Attribute '{0}' only accept a float, an integer, or a percentage as a value.";
        public const string PROBABILITY_EXPECTED = "Attribute '{0}' only accept a value between 0 and 1.";
        public const string INVALID_VALUE = "Attribute '{0}' only accept a valid value. See online documentation for more details.";

        public InvalidAttributeValueException(string identifier, string message)
            : base(string.Format(message, identifier))
        {
        }
    }
}
