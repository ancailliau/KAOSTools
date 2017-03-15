using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace KAOSTools.Core
{
    #region Object Model

    public class GivenType : KAOSCoreElement {
        public string Name { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public string Definition { get; set; }

        public GivenType  (KAOSModel model) : base (model)
        {
            
        }

        public override KAOSCoreElement Copy ()
        {
            throw new NotImplementedException ();
        }
    }

    #endregion

}
