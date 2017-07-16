using System;
using System.Collections.Generic;
using System.Linq;
using UCLouvain.KAOSTools.Core;

namespace UCLouvain.KAOSTools.CriticalObstacles
{
	public abstract class AbstractCriticalObstacles
	{
		protected KAOSModel _model;

		protected Goal _root;
		
		public AbstractCriticalObstacles(KAOSModel model, Goal root)
		{
			this._model = model;
			this._root = root;
		}
	
        // Code from Stackoverflow
        // https://stackoverflow.com/questions/7802822/all-possible-combinations-of-a-list-of-values
        protected static List<List<T>> GetAllCombinations<T> (List<T> list, int size)
        {
            List<List<T>> result = new List<List<T>> ();
            result.Add (new List<T> ());
            result.Last ().Add (list [0]);
            if (list.Count == 1)
                return result;
            List<List<T>> tailCombos = GetAllCombinations (list.Skip (1).ToList (), size);
            tailCombos.ForEach (combo => {
                result.Add (new List<T> (combo));
				if (combo.Count < size) {
					combo.Add(list[0]);
					result.Add(new List<T>(combo));
				}
            });
            return result;
        }
	}
}
