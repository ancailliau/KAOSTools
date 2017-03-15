using System;
using KAOSTools.Core;
using System.Collections.Generic;
using KAOSTools.Parsing;

namespace ModelWebBrowser.Models
{
    public class KAOSModelPage
    {
        public string Code {
            get;
            set;
        }

        public KAOSModel Model {
            get;
            set;
        }

        public IDictionary<KAOSCoreElement, IList<Declaration>> Declarations {
            get;
            set;
        }
    }
}

