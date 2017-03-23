using System;
using KAOSTools.Core;

namespace KAOSTools.Parsing.Builders.Declarations
{
    public abstract class DeclareBuilder
    {
		public abstract bool IsBuildable(ParsedDeclare element);

		public abstract void BuildDeclare(ParsedDeclare parsedElement, KAOSModel model);

        public abstract KAOSCoreElement GetBuiltElement(ParsedDeclare parsedElement, KAOSModel model);
    }
}
