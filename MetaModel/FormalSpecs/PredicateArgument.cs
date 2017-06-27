using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace UCLouvain.KAOSTools.Core
{
    public class PredicateArgument {

        public string Name { get; set; }

        public Entity Type { get; set; }

		public PredicateArgument Copy()
		{
			return new PredicateArgument() {
				Name = Name,
				Type = (Entity)Type.Copy()
			};
		}

    }
}
