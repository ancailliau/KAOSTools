using System;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Parsing.Parsers;

namespace UCLouvain.KAOSTools.Parsing.Builders.Declarations
{
    public abstract class DeclareBuilder
    {
		public abstract bool IsBuildable(ParsedDeclare element);

		public abstract void BuildDeclare(ParsedDeclare parsedElement, KAOSModel model);

        public abstract KAOSCoreElement GetBuiltElement(ParsedDeclare parsedElement, KAOSModel model);
    }
}
