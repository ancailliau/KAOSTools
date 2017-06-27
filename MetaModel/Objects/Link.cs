using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace UCLouvain.KAOSTools.Core
{
    #region Object Model

    public class Link : KAOSCoreElement {
        public Entity Target { get; set; }
        public string Role { get; set; }
        public string Multiplicity { get; set; }
        public Link  (KAOSModel model) : base (model)
        {}

        public override KAOSCoreElement Copy ()
        {
            throw new NotImplementedException ();
        }
    }

    #endregion

}
