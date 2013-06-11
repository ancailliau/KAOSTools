using System;
using KAOSTools.MetaModel;
using System.Collections.Generic;
using KAOSTools.Parsing;

namespace ModelWebBrowser.Models
{
    public class IndexModel
    {
        public string Code {
            get;
            set;
        }

        public KAOSModel Model {
            get;
            set;
        }

        public IDictionary<KAOSMetaModelElement, IList<Declaration>> Declarations {
            get;
            set;
        }
    }
}

