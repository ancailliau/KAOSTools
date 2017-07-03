using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace UCLouvain.KAOSTools.Core
{
    public class Expert : KAOSCoreElement
    {
        public string Name { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public Expert  (KAOSModel model) : base (model)
        {
		}

        public Expert(KAOSModel model, string identifier) : base(model, identifier)
		{
		}

        public override KAOSCoreElement Copy ()
        {
            throw new NotImplementedException ();
        }
    }
    
}
