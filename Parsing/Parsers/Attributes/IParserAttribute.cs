using System;
namespace UCLouvain.KAOSTools.Parsing.Parsers.Attributes
{
	public interface IParserAttribute
	{
		string GetIdentifier();

		ParsedElement ParsedAttribute(string identifier, NParsedAttributeValue parameters, NParsedAttributeValue value);
	}
}
