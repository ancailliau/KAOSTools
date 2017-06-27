using System;
using UCLouvain.KAOSTools.Parsing.Parsers;
using UCLouvain.KAOSTools.Core;

namespace UCLouvain.KAOSTools.Parsing
{
    public class BuilderException : Exception
	{
        public BuilderException(string message, ParsedElement element, Exception innerException = null)
			   : base(string.Format("{0}\nAt {1}:{2},{3}",
									  message,
									  element.Filename,
                                    element.Line,
									  element.Col), innerException)
		{ }

        public BuilderException (string message, string filename, int line, int col, Exception innerException = null)
            : base (string.Format ("{0}\nAt {1}:{2},{3}", 
                                   message, 
                                   filename, 
                                   line, 
                                   col), innerException)
        {}
    }

    public class EntityNotFound : BuilderException
    {
		public EntityNotFound(string identifier, ParsedElement element) 
            : base ("Entity '" + identifier +"' is not defined", element)
        {}
	}
	public class GivenTypeNotFound : BuilderException
	{
		public GivenTypeNotFound(string identifier, ParsedElement element)
			: base("Given type '" + identifier + "' is not defined", element)
		{ }
	}

    public class UnsupportedValue : BuilderException
    {
        public UnsupportedValue(KAOSCoreElement element, ParsedAttribute attribute, object value)
		    : base(string.Format("'{0}' is not supported in '{1}' on '{2}'",
																	  value?.GetType()?.Name,
																	  attribute?.GetType()?.Name,
							  element?.GetType()?.Name), attribute)
        {

        }


    }

    public class DuplicateDeclarationException : Exception
    {
        public DuplicateDeclarationException () : base ()
        {}
        
        public DuplicateDeclarationException (string id, string file1, int line1, int col1,
                                              string file2, int line2, int col2) 
            : base (string.Format ("{0} is declared multiple times. See following files:\n-> {1}, line {2}, column {3}\n-> {4}, line {5}, column {6}",
                                   id, file1, line1, col1, file2, line2, col2))
        {}
    }
}

