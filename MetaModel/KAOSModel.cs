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
            if (element.model == null)
                element.model = this;

            if (element.model != this)
                throw new InvalidOperationException ("Cannot add element referencing a model in another model");

            if (this._elements.ContainsKey(element.Identifier))
                throw new InvalidOperationException ("Duplicated ID " + element.Identifier);

            this._elements.Add (element.Identifier, element);
        }
    }
}

