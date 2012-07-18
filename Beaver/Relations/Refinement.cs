using System;
using System.Collections.Generic;


namespace Beaver.Domain
{
    public abstract class Refinement : KAOSElement
    {
        public string Parent {
            get;
            set;
        }
        
        public List<string> Children {
            get;
            private set;
        }

        public Refinement ()
            : base ()
        {
            Children = new List<string> ();
        }

        public Refinement (string id) : base (id)
        {
            Children = new List<string> ();
        }
    }
}

