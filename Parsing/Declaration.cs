using System;
using System.IO;

namespace KAOSTools.Parsing
{
    /// <summary>
    /// Represents the list of places where an element is declared. By places, we mean the filename (relative to
    /// the filename provided), the line number and the column.
    /// </summary>
    public class Declaration
    {
        /// <summary>
        /// Gets or sets the line number.
        /// </summary>
        /// <value>The line.</value>
        public int Line { get; set; }
        
        /// <summary>
        /// Gets or sets the column index.
        /// </summary>
        /// <value>The col.</value>
        public int Col { get; set; }
        
        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>The filename.</value>
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the declaration at that place is first declaration or an override.
        /// </summary>
        /// <value><c>true</c> if override; otherwise, <c>false</c>.</value>
        public bool Override { get; set; }

        /// <summary>
        /// Initializes a new declaration with the specified filename, line and column.
        /// </summary>
        /// <param name="line">Line.</param>
        /// <param name="column">Column.</param>
        /// <param name="filename">Filename.</param>
        public Declaration (int line, int column, string filename, Uri relativeURI)
        {
            this.Line = line;
            this.Col = column; 

            if (relativeURI != null && filename != null)
                this.Filename = relativeURI.MakeRelativeUri (new Uri(Path.GetFullPath (filename))).ToString ();
        }
    }

}

