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
        public KAOSModel Parse (string input, string filename)
        {
            KAOSModel model = new KAOSModel();
            GoalModelParser _parser = new GoalModelParser ();
            
            Uri RelativePath = null;
            if (!string.IsNullOrEmpty (filename))
                RelativePath = new Uri(Path.GetFullPath (Path.GetDirectoryName(filename) + "/"));

            FirstStageBuilder FSB = new FirstStageBuilder (model, RelativePath);
            FormulaBuilder FB = new FormulaBuilder (model, FSB, RelativePath);
            SecondStageBuilder SSB = new SecondStageBuilder (model, RelativePath);
            ThirdStageBuilder TSB = new ThirdStageBuilder (model, FB, RelativePath);

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

