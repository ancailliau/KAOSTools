using System;
using KAOSTools.MetaModel;
using KAOSTools.OmnigraffleExport.Omnigraffle;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace KAOSTools.OmnigraffleExport
{
    public class Untitled2 : ParentUntitled
    {
        public Untitled2 (Sheet sheet, IDictionary<string, IList<Graphic>> shapes)
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

        public void Render (GoalRefinement refinement)
        {
            var circle = GetCircle ();
            if (refinement.IsComplete)
                SetFillColor (circle, 0, 0, 0);

            Add (refinement.Identifier, circle);

            if (shapes.ContainsKey(refinement.ParentGoalIdentifier)) {
                var parentGraphic = shapes[refinement.ParentGoalIdentifier].First ();
                var topArrow = GetFilledArrow (circle, parentGraphic);
                if (refinement.SystemReferenceIdentifier != null)
                    AddText (topArrow, refinement.SystemReference().FriendlyName);
                sheet.GraphicsList.Add (topArrow);
            }

            foreach (var child in refinement.SubGoalIdentifiers) {
                if (!shapes.ContainsKey(child))
                    continue;

                var childGraphic = shapes [child].First ();
                var line = GetLine (childGraphic, circle);
                sheet.GraphicsList.Add (line);
            }
        }

        public void Render (AntiGoalRefinement refinement)
        {
            var circle = GetCircle ();
            Add (refinement.Identifier, circle);
            
            if (shapes.ContainsKey(refinement.ParentAntiGoalIdentifier)) {
                var parentGraphic = shapes[refinement.ParentAntiGoalIdentifier].First ();
                var topArrow = GetFilledArrow (circle, parentGraphic);
                if (refinement.SystemReferenceIdentifier != null)
                    AddText (topArrow, refinement.SystemReference().FriendlyName);
                sheet.GraphicsList.Add (topArrow);
            }

            foreach (var child in refinement.SubAntiGoalIdentifiers) {
                if (!shapes.ContainsKey(child))
                    continue;

                var childGraphic = shapes [child].First ();
                var line = GetLine (childGraphic, circle);
                sheet.GraphicsList.Add (line);
            }
        }

        public void Render (ObstacleRefinement refinement)
        {
            var circle = GetCircle ();
            Add (refinement.Identifier, circle);
            
            if (shapes.ContainsKey(refinement.ParentObstacleIdentifier)) {
                var parentGraphic = shapes[refinement.ParentObstacleIdentifier].First ();
                var topArrow = GetFilledArrow (circle, parentGraphic);
                sheet.GraphicsList.Add (topArrow);
            }

            foreach (var child in refinement.SubobstacleIdentifiers) {
                if (!shapes.ContainsKey(child))
                    continue;

                var childGraphic = shapes [child].First ();
                var line = GetLine (childGraphic, circle);
                sheet.GraphicsList.Add (line);
            }
        }

        public void Render (GoalAgentAssignment assignment)
        {
            var circle = GetCircle ();
            Add (assignment.Identifier, circle);

            if (shapes.ContainsKey(assignment.GoalIdentifier)) {
                var parentGraphic = shapes[assignment.GoalIdentifier].First ();
                var topArrow = GetFilledArrow (circle, parentGraphic);
                sheet.GraphicsList.Add (topArrow);
            }

            foreach (var child in assignment.AgentIdentifiers) {
                if (!shapes.ContainsKey(child))
                    continue;

                var childGraphic = shapes [child].First ();
                var line = GetLine (childGraphic, circle);
                sheet.GraphicsList.Add (line);
            }
        }

        public void Render (AntiGoalAgentAssignment assignment)
        {
            var circle = GetCircle ();
            Add (assignment.Identifier, circle);
            
            if (shapes.ContainsKey(assignment.AntiGoalIdentifier)) {
                var parentGraphic = shapes[assignment.AntiGoalIdentifier].First ();
                var topArrow = GetFilledArrow (circle, parentGraphic);
                sheet.GraphicsList.Add (topArrow);
            }

            foreach (var child in assignment.AgentIdentifiers) {
                if (!shapes.ContainsKey(child))
                    continue;

                var childGraphic = shapes [child].First ();
                var line = GetLine (childGraphic, circle);
                sheet.GraphicsList.Add (line);
            }
        }

        public void Render (ObstacleAgentAssignment assignment)
        {
            var circle = GetCircle ();
            Add (assignment.Identifier, circle);
            
            if (shapes.ContainsKey(assignment.ObstacleIdentifier)) {
                var parentGraphic = shapes[assignment.ObstacleIdentifier].First ();
                var topArrow = GetFilledArrow (circle, parentGraphic);
                sheet.GraphicsList.Add (topArrow);
            }

            foreach (var child in assignment.AgentIdentifiers) {
                if (!shapes.ContainsKey(child))
                    continue;

                var childGraphic = shapes [child].First ();
                var line = GetLine (childGraphic, circle);
                sheet.GraphicsList.Add (line);
            }
        }

        public void Render (Resolution resolution)
        {
            if (!shapes.ContainsKey (resolution.ObstacleIdentifier))
                return;

            if (!shapes.ContainsKey (resolution.ResolvingGoalIdentifier)) 
                return;

            var obstacleGraphic = shapes [resolution.ObstacleIdentifier].First ();
            var goalGraphic = shapes [resolution.ResolvingGoalIdentifier].First ();

            var topArrow = GetSharpBackCrossArrow (goalGraphic, obstacleGraphic);
            Add (resolution.Identifier, topArrow);
        }

        public void Render (Obstruction obstruction)
        {
            if (!shapes.ContainsKey (obstruction.ObstacleIdentifier))
                return;

            if (!shapes.ContainsKey (obstruction.ObstructedGoalIdentifier)) 
                return;

            var obstacleGraphic = shapes [obstruction.ObstacleIdentifier].First ();
            var goalGraphic = shapes [obstruction.ObstructedGoalIdentifier].First ();

            var topArrow = GetSharpBackCrossArrow (obstacleGraphic, goalGraphic);
            Add (obstruction.Identifier, topArrow);
        }

        public void Render (KAOSTools.MetaModel.Attribute attribute)
        {
            if (!shapes.ContainsKey (attribute.EntityIdentifier)) 
                return;

            foreach (var entity in shapes[attribute.EntityIdentifier].Cast<Group>()) {
                var text = @"";

                if ((entity.Graphics[1] as ShapedGraphic).Text.Text.Length > 4)
                    text+= @"\ql\par ";

                text += GetRtfUnicodeEscapedString((attribute.Derived ? "/ " : "- "));
                text += GetRtfUnicodeEscapedString(attribute.FriendlyName);

                Console.WriteLine ("***" + string.IsNullOrEmpty(attribute.TypeIdentifier) + "***");

                if (!string.IsNullOrEmpty(attribute.TypeIdentifier))
                    text += " : " + attribute.Type().FriendlyName;

                Console.WriteLine ("*" + entity.Graphics[1].GetType() + "*");


                (entity.Graphics[1] as ShapedGraphic).Text.Text += text;

                Console.WriteLine (text + "**");
            }
        }

    }
}

