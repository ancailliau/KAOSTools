using System;
using System.Linq;
using System.Collections.Generic;

namespace Beaver.Domain
{
    public class GoalModel
    {
        private List<KAOSElement> elements;
        private List<View> views;

        public GoalModel ()
        {
            elements = new List<KAOSElement> ();
            views = new List<View> ();
        }


        public void Add (KAOSElement element)
        {
            elements.Add (element);
        }
        
        public void Remove (string id)
        {
            elements.RemoveAt (elements.FindIndex (x => x.Id == id));

            foreach (var v in views)
                v.Remove (id);
        }

        public bool Contains (string id)
        {
            return elements.Where (x => x.Id == id).Count () > 0;
        }

        public object Get (string id)
        {
            return elements.Where (x => x.Id == id).Single ();
        }

        public IEnumerable<T> Find<T> (Func<T, bool> condition)
            where T : KAOSElement
        {
            return (IEnumerable<T>) elements.Where (x => x is T && condition((T) x));
        }

        public void Connect (string id1, string id2)
        {
            var e1 = this.Get (id1);
            var e2 = this.Get (id2);

            if (e1 is Goal & e2 is Goal) {
            } else if (e1 is Goal & e2 is DomainProperty) {
            } else if (e1 is Goal & e2 is Obstacle) {
            } else if (e1 is Goal & e2 is Agent) {
            } else if (e1 is Goal & e2 is GoalRefinement) {
            } else if (e1 is Goal & e2 is ObstacleRefinement) {

            } else if (e1 is DomainProperty & e2 is Goal) {
            } else if (e1 is DomainProperty & e2 is DomainProperty) {
            } else if (e1 is DomainProperty & e2 is Obstacle) {
            } else if (e1 is DomainProperty & e2 is Agent) {
            } else if (e1 is DomainProperty & e2 is GoalRefinement) {
            } else if (e1 is DomainProperty & e2 is ObstacleRefinement) {
                
            } else if (e1 is Obstacle & e2 is Goal) {
            } else if (e1 is Obstacle & e2 is DomainProperty) {
            } else if (e1 is Obstacle & e2 is Obstacle) {
            } else if (e1 is Obstacle & e2 is Agent) {
            } else if (e1 is Obstacle & e2 is GoalRefinement) {
            } else if (e1 is Obstacle & e2 is ObstacleRefinement) {
                
            } else if (e1 is Agent & e2 is Goal) {
            } else if (e1 is Agent & e2 is DomainProperty) {
            } else if (e1 is Agent & e2 is Obstacle) {
            } else if (e1 is Agent & e2 is Agent) {
            } else if (e1 is Agent & e2 is GoalRefinement) {
            } else if (e1 is Agent & e2 is ObstacleRefinement) {
                
            } else if (e1 is GoalRefinement & e2 is Goal) {
            } else if (e1 is GoalRefinement & e2 is DomainProperty) {
            } else if (e1 is GoalRefinement & e2 is Obstacle) {
            } else if (e1 is GoalRefinement & e2 is Agent) {
            } else if (e1 is GoalRefinement & e2 is GoalRefinement) {
            } else if (e1 is GoalRefinement & e2 is ObstacleRefinement) {
                
            } else if (e1 is ObstacleRefinement & e2 is Goal) {
            } else if (e1 is ObstacleRefinement & e2 is DomainProperty) {
            } else if (e1 is ObstacleRefinement & e2 is Obstacle) {
            } else if (e1 is ObstacleRefinement & e2 is Agent) {
            } else if (e1 is ObstacleRefinement & e2 is GoalRefinement) {
            } else if (e1 is ObstacleRefinement & e2 is ObstacleRefinement) {
            }
        }

        public void AddView (View view)
        {
            view.Model = this;
            views.Add (view);
        }

        public void RemoveView (string id)
        {
            views.RemoveAt (views.FindIndex (x => x.Id == id));
        }

        public bool ContainsView (string id)
        {
            return views.Where (x => x.Id == id).Count () > 0;
        }

        public View GetView (string id)
        {
            return views.Where (x => x.Id == id).Single ();
        }
    }
}

