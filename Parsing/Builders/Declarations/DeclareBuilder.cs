using System;
using KAOSTools.Core;
using KAOSTools.Parsing.Parsers;

namespace KAOSTools.Parsing.Builders.Declarations
{
    public abstract class DeclareBuilder
    {
		public abstract bool IsBuildable(ParsedDeclare element);

		public abstract void BuildDeclare(ParsedDeclare parsedElement, KAOSModel model);

        public abstract KAOSCoreElement GetBuiltElement(ParsedDeclare parsedElement, KAOSModel model);
    }
}
