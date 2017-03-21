using System;
using System.Text.RegularExpressions;
using System.Linq;
using KAOSTools.Core;
using System.Collections.Generic;

namespace KAOSTools.Parsing
{
    public class Builder
    {
        protected KAOSModel model;
        protected Uri relativePath;
    
        /// <summary>
        /// Initializes a new instance of the <see cref="T:KAOSTools.Parsing.Builder"/> class.
        /// </summary>
        /// <param name="model">The KAOS model.</param>
        /// <param name="relativePath">Relative path for the directory containing the source files.</param>
        public Builder (KAOSModel model, 
                        Uri relativePath)
        {
            this.model = model;
            this.relativePath = relativePath;
        }

        /// <summary>
        /// Replaces the spaces by single spaces. This is usefull for not taking tabs and/or line return into account
        /// in the names.
        /// </summary>
        /// <returns>A cleaned string.</returns>
        /// <param name="text">String to clean.</param>
        protected string Sanitize (string text) 
        {
            var t = Regex.Replace(text, @"\s+", " ", RegexOptions.Multiline).Trim ();
            t = Regex.Replace (t, "\"\"", "\"", RegexOptions.Multiline);
            return t;
        }
    }
}

