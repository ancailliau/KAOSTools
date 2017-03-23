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
    }
}

