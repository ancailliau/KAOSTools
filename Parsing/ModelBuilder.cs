using System;
using System.Linq;
using System.Collections.Generic;
using KAOSTools.Parsing;
using KAOSTools.MetaModel;
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
        /// The internal parser. This parser is generated from a grammar (see <c>GoalModelParsed.peg</c> for more 
        /// details)
        /// </summary>
        private GoalModelParser _parser = new GoalModelParser ();

        /// <summary>
        /// The model being built.
        /// </summary>
        private KAOSModel model;

        /// <summary>
        /// The declarations for each elemnt of the model.
        /// </summary>
        public IDictionary<KAOSMetaModelElement, IList<Declaration>> Declarations { get; set; }

        FirstStageBuilder FSB;
        SecondStageBuilder SSB;
        FormulaBuilder FB;

        Uri RelativePath = null;

        /// <summary>
        /// Initializes a new instance of the compiler.
        /// </summary>
        public ModelBuilder (){}

        [Obsolete("Do not provide a KAOS Model")]
        public KAOSModel Parse (string input, string filename, KAOSModel model)
        {
            Declarations = new Dictionary<KAOSMetaModelElement, IList<Declaration>> ();

            if (!string.IsNullOrEmpty (filename))
                RelativePath = new Uri(Path.GetFullPath (Path.GetDirectoryName(filename) + "/"));

            FSB = new FirstStageBuilder (model, Declarations, RelativePath);
            FB = new FormulaBuilder (model, Declarations, FSB);

            SSB = new SecondStageBuilder (model, Declarations, FSB, FB);

            try {
                var elements = _parser.Parse (input, filename) as ParsedElements;    

                this.model = model;

                FSB.BuildElementWithKeys (elements);
                SSB.BuildElement (elements);

                // SecondPass (elements);
                return model;

            } catch (Exception e) {
                throw new CompilationException (e.Message, e);
            }
        }

        public KAOSModel Parse (string input, string filename)
        {
            return Parse (input, filename, new KAOSModel());
        }

        public KAOSModel Parse (string input)
        {
            return Parse (input, null);
        }
    }
}

