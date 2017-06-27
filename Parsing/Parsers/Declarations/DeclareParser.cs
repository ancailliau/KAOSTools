using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UCLouvain.KAOSTools.Parsing.Parsers.Attributes;
using UCLouvain.KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Parsers.Declarations
{
    public abstract class DeclareParser
    {
        protected List<IParserAttribute> attributeParsers = new List<IParserAttribute>();

		public abstract ParsedElement ParsedDeclare(string identifier, List<dynamic> attributes, bool @override);


        public ParsedElement ParsedAttribute(string identifier, NParsedAttributeValue parameters, NParsedAttributeValue value)
        {
            foreach (var parser in attributeParsers)
            {
                var regexString = @"^" + parser.GetIdentifier() + "$";
                var regex = new Regex(regexString);
                var match = regex.Match(identifier);
                if (match.Success)
                {
                    return parser.ParsedAttribute(identifier, parameters, value);
                }
            }

            throw new NotImplementedException("The attribute " + identifier + " is not valid for " + GetIdentifier() + ".");
        }

        public abstract string GetIdentifier();

		protected void Add(IParserAttribute parser)
		{
			attributeParsers.Add(parser);
		}
	}
    
}
