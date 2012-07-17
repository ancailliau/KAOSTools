using System;
using System.Collections.Generic;


namespace Beaver.Domain
{
    public class View
    {
        public string Id { get; private set; }
        public GoalModel Model { get; set; }

        private List<string> elements;

        public View (string id)
        {
            this.Id = id;
            this.elements = new List<string> ();
        }

        public void Add (string id)
        {
            if (Model.Contains (id)) 
                elements.Add (id);
        }
        
        public void Remove (string id)
        {
            elements.Remove (id);
        }
        
        public bool Contains (string id)
        {
            return elements.Contains (id);
        }
    }
}

