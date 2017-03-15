using System;
using System.Linq;
using System.Collections.Generic;
using KAOSTools.Parsing;
using KAOSTools.Core;
using System.Text.RegularExpressions;
using System.IO;

namespace KAOSTools.Parsing
{
    /// <summary>
    /// This is more a compiler than a parser. This class will parse and compile (in two-phases) a text-based 
    /// description of a KAOS model in an object-based representation of that model.
    /// </summary>
    public class ModelBuilder
    {
        /// <summary>
        /// The declarations for each elemnt of the model.
        /// </summary>
        public IDictionary<KAOSCoreElement, IList<Declaration>> Declarations { get; set; }

        public KAOSModel Parse (string input, string filename)
        {
            KAOSModel model = new KAOSModel();
            Declarations = new Dictionary<KAOSCoreElement, IList<Declaration>> ();
            GoalModelParser _parser = new GoalModelParser ();
            
            Uri RelativePath = null;
            if (!string.IsNullOrEmpty (filename))
                RelativePath = new Uri(Path.GetFullPath (Path.GetDirectoryName(filename) + "/"));

            FirstStageBuilder FSB = new FirstStageBuilder (model, Declarations, RelativePath);
            FormulaBuilder FB = new FormulaBuilder (model, Declarations, FSB, RelativePath);
            SecondStageBuilder SSB = new SecondStageBuilder (model, Declarations, FSB, FB, RelativePath);
            ThirdStageBuilder TSB = new ThirdStageBuilder (model, Declarations, FSB, SSB, FB, RelativePath);

            var elements = _parser.Parse (input, filename) as ParsedElements;    

            FSB.BuildElementWithKeys (elements);
            SSB.BuildElement (elements);
            TSB.BuildElement (elements);

            return model;
        }

        public KAOSModel Parse (string input)
        {
            return Parse (input, null);
        }
    }
}

