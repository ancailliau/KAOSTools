using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace KAOSTools.Core
{
    [DataContract]
    public abstract class KAOSCoreElement
    {
        [DataMember]
        public string Identifier { get; set; }

        public virtual string FriendlyName {
            get {
                return Identifier;
            }
        }

        [DataMember]
        public bool Implicit { get; set; }

        public KAOSModel model;

        public IDictionary<string,string> CustomData {
            get;
            set;
        }

        public KAOSCoreElement (KAOSModel model)
        {
            this.Identifier = "id_" + Guid.NewGuid ().ToString ();
            this.model = model;
            this.CustomData = new Dictionary<string,string> ();
		}

		public KAOSCoreElement(KAOSModel model, string identifier)
		{
            this.Identifier = identifier;
			this.model = model;
			this.CustomData = new Dictionary<string, string>();
		}

        public abstract KAOSCoreElement Copy ();

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(KAOSCoreElement))
                return false;
            KAOSCoreElement other = (KAOSCoreElement)obj;
            return Identifier == other.Identifier;
        }

        public override int GetHashCode ()
        {
            unchecked {
                return (Identifier != null ? Identifier.GetHashCode () : 0);
            }
        }
    }
}
