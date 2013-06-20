using System;
using System.Collections.Generic;
using System.Linq;

namespace KAOSTools.MetaModel
{
    public class KAOSModel
    {
        private Dictionary<string, KAOSMetaModelElement> _elements;

        public IEnumerable<KAOSMetaModelElement> Elements {
            get {
                return _elements.Values;
            }
        }

        public KAOSModel ()
        {
            this._elements = new Dictionary<string, KAOSMetaModelElement> ();
        }

        public void Add (KAOSMetaModelElement element)
        {
            var e = element;
            if (e.model != this)
                e = element.Copy ();

            if (e.model == null)
                e.model = this;

            if (this._elements.ContainsKey(e.Identifier))
                throw new InvalidOperationException ("Duplicated ID " + e.Identifier);

            this._elements.Add (e.Identifier, e);
        }
    }

    public class KAOSView : KAOSModel {
    
        public KAOSModel ParentModel { get; set; }

        public KAOSView (KAOSModel model) : base()
        {
            this.ParentModel = model;
        }

    }
}

