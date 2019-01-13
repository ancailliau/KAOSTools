using System;
namespace UCLouvain.KAOSTools.Core.Model
{
	public class Context : KAOSCoreElement
    {
        public string Name { get; set; }

        public override string FriendlyName {
            get {
                return string.IsNullOrEmpty(Name) ? Identifier : Name;
            }
        }

        public Context (KAOSModel model) : base (model)
        {
		}

		public Context (KAOSModel model, string identifier) : base(model, identifier)
		{
		}

        public override KAOSCoreElement Copy ()
        {
            throw new NotImplementedException ();
        }
    }
}
