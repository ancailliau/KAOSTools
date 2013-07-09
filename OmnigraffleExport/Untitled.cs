using System;
using KAOSTools.MetaModel;
using KAOSTools.OmnigraffleExport.Omnigraffle;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace KAOSTools.OmnigraffleExport
{
    public class Untitled : ParentUntitled
    {
        public Untitled (Sheet sheet, IDictionary<string, IList<Graphic>> shapes)
            : base (sheet, shapes)
        {}

        public void Render (KAOSModel model) 
        {
            foreach (dynamic e in model.Elements) {
                Render (e);
            }
        }

        public void Render (Object element)
        {
            Console.WriteLine (element.GetType().Name + " is not supported.");
        }

        public void Render (Goal goal)
        {
            int lineWidth = 1;

            bool assignedToEnvAgents = (
                from a in goal.AgentAssignments().SelectMany (x => x.Agents())
                where a.Type != AgentType.Software select a).Count () > 0;

            if (goal.AgentAssignments().Count() > 0)
                lineWidth = 2;

            if (assignedToEnvAgents)
                AddParallelogram (goal.Identifier, goal.FriendlyName, 
                                  lineWidth, 1, 0.979841, 0.672223);
            else
                AddParallelogram (goal.Identifier, goal.FriendlyName, 
                                  lineWidth, 0.810871, 0.896814, 1);
        }

        public void Render (SoftGoal softGoal)
        {
            AddCloud (softGoal.Identifier, softGoal.FriendlyName, 
                      1, 1, 1, 1);
        }

        public void Render (AntiGoal antigoal)
        {
            int lineWidth = 1;
            if (antigoal.AgentAssignments().Count() > 0)
                lineWidth = 2;

            AddParallelogram (antigoal.Identifier, antigoal.FriendlyName, 
                              lineWidth, 1, 234.0/255, 192.0/255);
        }

        public void Render (Obstacle obstacle)
        {
            int lineWidth = 1;
            if (obstacle.AgentAssignments().Count() > 0)
                lineWidth = 2;

            AddInvertParallelogram (obstacle.Identifier, obstacle.FriendlyName, 
                                    lineWidth, 1, 0.590278, 0.611992);
        }

        public void Render (DomainProperty domProp)
        {
            AddHomeShape (domProp.Identifier, domProp.FriendlyName, 
                             1, 0.895214, 1, 0.72515);
        }

        public void Render (DomainHypothesis domHyp)
        {
            AddHomeShape (domHyp.Identifier, domHyp.FriendlyName, 
                          1, 1, 0.92156862745, 0.92156862745);
        }

        public void Render (Agent agent)
        {
            if (agent.Type == AgentType.Software)
                AddHexagon (agent.Identifier, agent.FriendlyName, 
                            1, 0.99607843137, 0.80392156862, 0.58039215686);
            else if (agent.Type == AgentType.Malicious)
                AddHexagon (agent.Identifier, agent.FriendlyName, 
                            1, 1, 0.590278, 0.611992);
            else
                AddHexagon (agent.Identifier, agent.FriendlyName, 
                            1, 0.824276, 0.670259, 1);
        }

        public void Render (Entity entity)
        {
            var graphic = GetRectangle ();
            AddText (graphic, entity.FriendlyName, true);

            if (entity.Type == EntityType.Environment)
                SetFillColor (graphic, 0.824276, 0.670259, 1);
            else if (entity.Type == EntityType.Software)
                SetFillColor (graphic, 0.99607843137, 0.80392156862, 0.58039215686);
            else if (entity.Type == EntityType.Shared)
                SetFillColor (graphic, 0.895214, 1, 0.72515);


            var attribute = GetRectangle ();
            attribute.Bounds.TopLeft.Y += 20;
            attribute.Style.Shadow.Draws = false;
            AddText (attribute, "", false, TextAlignement.Left);

            var grp = new Group (NextId);
            grp.Graphics.Add (graphic);
            grp.Graphics.Add (attribute);

            grp.Magnets.Add (new Point(1,.5));
            grp.Magnets.Add (new Point(1,-.5));

            grp.Magnets.Add (new Point(0.5,1));
            grp.Magnets.Add (new Point(0.5,.5));
            grp.Magnets.Add (new Point(0.5,0));
            grp.Magnets.Add (new Point(0.5,-.5));
            grp.Magnets.Add (new Point(0.5,-1));

            grp.Magnets.Add (new Point(0,.5));
            grp.Magnets.Add (new Point(0,-.5));

            grp.Magnets.Add (new Point(-0.5,1));
            grp.Magnets.Add (new Point(-0.5,.5));
            grp.Magnets.Add (new Point(-0.5,0));
            grp.Magnets.Add (new Point(-0.5,-.5));
            grp.Magnets.Add (new Point(-0.5,-1));

            grp.Magnets.Add (new Point(-1,.5));
            grp.Magnets.Add (new Point(-1,-.5));

            Add (entity.Identifier, grp);
        }
    }
}

