using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace UCLouvain.KAOSTools.Core
{
    #region Object Model

    public class Relation : Entity {

        public ISet<Link> Links { get; set; } 
        public Relation (KAOSModel model) : base (model)
        {
            Links = new HashSet<Link> ();
        }
        public Relation(KAOSModel model, string identifier) : base(model, identifier)
		{
			Links = new HashSet<Link>();
		}
    }

    #endregion

}
