using System;

namespace KAOSTools.Parsing
{
    [Obsolete("Use BuilderException instead")]
    public class CompilationException : Exception
    {
        public CompilationException () : base ()
        {}
        
        public CompilationException (string message) : base (message)
        {}

        public CompilationException (string message, Exception e) : base (message, e)
        {}
    }

    public class BuilderException : Exception
    {
        public BuilderException (string message, Declaration declaration, Exception innerException = null)
            : this (message, 
                    declaration.Filename, 
                    declaration.Line, 
                    declaration.Col, 
                    innerException)
        {}

        public BuilderException (string message, string filename, int line, int col, Exception innerException = null)
            : base (string.Format ("{0}\nAt {1}:{2},{3}", 
                                   message, 
                                   filename, 
                                   line, 
                                   col), innerException)
        {}
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

