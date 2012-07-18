using System;
using System.Linq;
using System.Collections.Generic;

namespace Beaver.Domain
{
    public class GoalModel
    {
        private List<KAOSElement> elements;
        private List<View> views;

        public bool Changed { get ; set ; }

        public GoalModel ()
        {
            elements = new List<KAOSElement> ();
            views = new List<View> ();
        }


        public void Add (KAOSElement element)
        {
            Changed = true;
            elements.Add (element);
        }
        
        public void Remove (string id)
        {
            Changed = true;
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
            return elements.Where (x => x is T && condition ((T)x)).Cast<T> ();
        }

        public void Connect (string id1, string id2)
        {
            var e1 = this.Get (id1);
            var e2 = this.Get (id2);

            if (e1 is Goal & e2 is Goal) {
                var refinement = new GoalRefinement ();
                refinement.Parent = id1;
                refinement.Children.Add (id2);
                Add (refinement);

            } else if (e1 is Goal & e2 is DomainProperty) {
                var refinement = new GoalRefinement ();
                refinement.Parent = id1;
                refinement.Children.Add (id2);
                Add (refinement);

            } else if (e1 is Goal & e2 is Obstacle) {
                var obstruction = new Obstruction ();
                obstruction.Goal = id1;
                obstruction.Obstacle = id2;
                Add (obstruction);

            } else if (e1 is Goal & e2 is Agent) {
                var responsibility = new Responsibility ();
                responsibility.Goal = id1;
                responsibility.Agent = id2;
                Add (responsibility);

            } else if (e1 is Goal & e2 is GoalRefinement) {
                (e2 as GoalRefinement).Children.Add (id1);

            } else if (e1 is Goal & e2 is ObstacleRefinement) {
                // Nothing change

            } else if (e1 is DomainProperty & e2 is Goal) {
                var refinement = new GoalRefinement ();
                refinement.Parent = id2;
                refinement.Children.Add (id1);
                Add (refinement);

            } else if (e1 is DomainProperty & e2 is DomainProperty) {
                // Nothing change

            } else if (e1 is DomainProperty & e2 is Obstacle) {
                // Nothing change

            } else if (e1 is DomainProperty & e2 is Agent) {
                // Nothing change

            } else if (e1 is DomainProperty & e2 is GoalRefinement) {
                (e2 as GoalRefinement).Children.Add (id1);

            } else if (e1 is DomainProperty & e2 is ObstacleRefinement) {
                (e2 as ObstacleRefinement).Children.Add (id1);
                
            } else if (e1 is Obstacle & e2 is Goal) {
            } else if (e1 is Obstacle & e2 is DomainProperty) {


            } else if (e1 is Obstacle & e2 is Obstacle) {
                var refinement = new ObstacleRefinement ();
                refinement.Parent = id1;
                refinement.Children.Add (id2);
                Add (refinement);

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
                var r1 = e1 as GoalRefinement;
                var r2 = e2 as GoalRefinement;

                if (r1.Parent == r2.Parent) {

                    var newRefinement = new GoalRefinement () { Parent = r1.Parent };
                    foreach (var child in r1.Children) {
                        newRefinement.Children.Add (child);
                    }
                    foreach (var child in r2.Children) {
                        if (!newRefinement.Children.Contains (child)) {
                            newRefinement.Children.Add (child);
                        }
                    }

                    elements.Add (newRefinement);
                    elements.Remove (r1);
                    elements.Remove (r2);
                }

            } else if (e1 is GoalRefinement & e2 is ObstacleRefinement) {
                
            } else if (e1 is ObstacleRefinement & e2 is Goal) {
            } else if (e1 is ObstacleRefinement & e2 is DomainProperty) {
            } else if (e1 is ObstacleRefinement & e2 is Obstacle) {
            } else if (e1 is ObstacleRefinement & e2 is Agent) {
            } else if (e1 is ObstacleRefinement & e2 is GoalRefinement) {
            } else if (e1 is ObstacleRefinement & e2 is ObstacleRefinement) {
                var o1 = e1 as ObstacleRefinement;
                var o2 = e2 as ObstacleRefinement;

                if (o1.Parent == o2.Parent) {

                    var newRefinement = new ObstacleRefinement () { Parent = o1.Parent };
                    foreach (var child in o1.Children) {
                        newRefinement.Children.Add (child);
                    }
                    foreach (var child in o2.Children) {
                        if (!newRefinement.Children.Contains (child)) {
                            newRefinement.Children.Add (child);
                        }
                    }

                    elements.Add (newRefinement);
                    elements.Remove (o1);
                    elements.Remove (o2);
                }

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

