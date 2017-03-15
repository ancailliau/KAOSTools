using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace KAOSTools.Core
{
    #region Object Model

    #endregion

    public class Predicate : KAOSCoreElement
    {
        public string Name { get; set; }

        public bool DefaultValue {
            get;
            set;
        }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public IList<PredicateArgument> Arguments { get; set; }

        public string Definition { get; set; }

        public Formula FormalSpec { get; set; }

        public Predicate  (KAOSModel model) : base (model)
        {
            Arguments = new List<PredicateArgument> ();
            DefaultValue = false;
        }

        public override KAOSCoreElement Copy ()
        {
			return new Predicate(model) {
				Identifier = Identifier,
				Implicit = Implicit,
				CustomData = CustomData,
				Name = Name,
				DefaultValue = DefaultValue,
				Arguments = new List<PredicateArgument>(Arguments.Select(x => x.Copy())),
				Definition = Definition,
				FormalSpec = FormalSpec
			};

        }
    }

}
