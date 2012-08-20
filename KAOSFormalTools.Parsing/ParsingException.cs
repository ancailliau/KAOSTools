using System;

namespace KAOSFormalTools.Parsing
{
    public class ParsingException : Exception
    {
        public ParsingException () : base ()
        {}
        
        public ParsingException (string message) : base (message)
        {}
    }
}

