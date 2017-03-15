using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace KAOSTools.Core
{
    #region Object Model

    public class Attribute : KAOSCoreElement {
        public bool Derived { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }

        public string EntityIdentifier {
            get;
            set;
        }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public string TypeIdentifier { get; set; }

        public Attribute (KAOSModel model) : base (model)
        {
            Derived = false;
        }
        
        public void SetEntity (Entity entity)
        {
            EntityIdentifier = entity.Identifier;
        }

        public void SetType (GivenType givenType)
        {
            TypeIdentifier = givenType.Identifier;
        }

        public override KAOSCoreElement Copy ()
        {
            return new Attribute (model) {
                Identifier = Identifier,
                Implicit = Implicit,
                Derived = Derived,
                Name = Name,
                Definition = Definition,
                TypeIdentifier = TypeIdentifier
            };
        }
    }

    #endregion

}
