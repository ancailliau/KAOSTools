using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace KAOSTools.Core
{
    #region Object Model

    public class Entity : KAOSCoreElement {
        public string Name { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public string Definition { get; set; }
        public EntityType Type { get; set; }

        public ISet<string> ParentIdentifiers { get; set; }

        public Entity (KAOSModel model) : base (model)
        {
            ParentIdentifiers    = new HashSet<string> ();
		}

        public Entity(KAOSModel model, string identifier) : base(model, identifier)
		{
			ParentIdentifiers = new HashSet<string>();
		}

        public void AddParent (Entity parent)
        {
            ParentIdentifiers.Add (parent.Identifier);
        }

        public override KAOSCoreElement Copy ()
        {
            return new Entity (model) {
                Identifier = Identifier,
                Implicit = Implicit,
                Name = Name,
                Definition = Definition,
                Type = Type,
                ParentIdentifiers = new HashSet<string> (ParentIdentifiers)
            };
        }
    }

    #endregion

}
