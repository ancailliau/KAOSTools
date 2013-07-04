using System;
using KAOSTools.MetaModel;
using KAOSTools.OmnigraffleExport.Omnigraffle;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace KAOSTools.OmnigraffleExport
{
    public class Untitled
    {
        private IDictionary<string, IList<Graphic>> shapes;
        private Sheet sheet;

        private static int _i = 1;
        private static int NextId {
            get {
                return _i++;
            }
        }

        public Untitled (Sheet sheet)
        {
            this.sheet = sheet;
            this.shapes = new Dictionary<string, IList<Graphic>> ();
        }

        private void Add (string key, Graphic graphic)
        {
            if (!this.shapes.ContainsKey (key)) {
                this.shapes.Add (key, new List<Graphic> ());
            }
            this.shapes[key].Add (graphic);

            sheet.GraphicsList.Add (graphic);
        }
        
        private static string GetRtfUnicodeEscapedString(string s)
        {
            if (s == null)
                return null;

            var sb = new StringBuilder();
            foreach (var c in s)
            {
                if(c == '\\' || c == '{' || c == '}')
                    sb.Append(@"\" + c);
                else if (c <= 0x7f)
                    sb.Append(c);
                else
                    sb.Append("\\u" + Convert.ToUInt32(c) + "?");
            }
            return sb.ToString();
        }

        private ShapedGraphic GetParallelogram ()
        {
            var graphic = new ShapedGraphic (NextId, 
                                             Omnigraffle.Shape.Bezier, 
                                             50, 50, 175, 70);

            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.5, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.5, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.5, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.45, -0.5));

            return graphic;
        }

        private ShapedGraphic GetInvertParallelogram ()
        {
            var graphic = new ShapedGraphic (NextId, 
                                             Omnigraffle.Shape.Bezier,
                                             50, 50, 150, 70);

            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.5, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.5, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.5, -0.5));

            return graphic;
        }

        private ShapedGraphic GetHomeShape ()
        {
            var graphic = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Bezier, 50, 50, 150, 70);

            graphic.ShapeData.UnitPoints.Add(new KAOSTools.OmnigraffleExport.Omnigraffle.Point(-0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSTools.OmnigraffleExport.Omnigraffle.Point(-0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSTools.OmnigraffleExport.Omnigraffle.Point(0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSTools.OmnigraffleExport.Omnigraffle.Point(0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSTools.OmnigraffleExport.Omnigraffle.Point(0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSTools.OmnigraffleExport.Omnigraffle.Point(0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSTools.OmnigraffleExport.Omnigraffle.Point(0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSTools.OmnigraffleExport.Omnigraffle.Point(0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSTools.OmnigraffleExport.Omnigraffle.Point(-0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSTools.OmnigraffleExport.Omnigraffle.Point(-0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSTools.OmnigraffleExport.Omnigraffle.Point(-0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSTools.OmnigraffleExport.Omnigraffle.Point(-0.45, -0.5));

            return graphic;
        }

        private ShapedGraphic GetHexagon ()
        {
            var graphic = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Bezier, 50, 50, 150, 70);

            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.5, 0));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.5, 0));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.5, 0));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.5, 0));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.5, 0));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.5, 0));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.45, -0.5));

            return graphic;
        }

        private ShapedGraphic GetCloud ()
        {
            var graphic = new ShapedGraphic (NextId, 
                                             Omnigraffle.Shape.Cloud, 
                                             50, 50, 175, 70);
            return graphic;
        }
        
        private ShapedGraphic GetCircle ()
        {
            var circle = new ShapedGraphic (NextId, Omnigraffle.Shape.Circle, 50, 50, 10, 10);
            circle.Style.Shadow.Draws = false;

            circle.FitText = KAOSTools.OmnigraffleExport.Omnigraffle.FitText.Clip;
            circle.Flow = KAOSTools.OmnigraffleExport.Omnigraffle.Flow.Clip;

            return circle;
        }

        private LineGraphic GetLine (Graphic @from, Graphic to)
        {
            var line = new Omnigraffle.LineGraphic (NextId);
            line.Head = new Omnigraffle.LineEndInfo (to.ID);
            line.Tail = new Omnigraffle.LineEndInfo (@from.ID);

            if (to is ShapedGraphic) {
                line.Points.Add ((to as ShapedGraphic).Bounds.TopLeft);
            } else if (to is Omnigraffle.Group) {
                line.Points.Add (new Point(25,25));
            }

            if (@from is ShapedGraphic) {
                line.Points.Add ((@from as ShapedGraphic).Bounds.BottomRight);
            } else if (@from is Omnigraffle.Group) {
                line.Points.Add (new Point(30,30));
            }

            line.Style.Shadow.Draws = false;

            return line;
        }

        private LineGraphic GetArrow (Graphic @from, Graphic to)
        {
            var line = GetLine (@from, to);
            line.Style.Stroke.HeadArrow = KAOSTools.OmnigraffleExport.Omnigraffle.Arrow.Arrow;
            return line;
        }

        private LineGraphic GetFilledArrow (Graphic @from, Graphic to)
        {
            var line = GetLine (@from, to);
            line.Style.Stroke.HeadArrow = KAOSTools.OmnigraffleExport.Omnigraffle.Arrow.FilledArrow;
            return line;
        }

        private LineGraphic GetSharpBackCrossArrow (Graphic @from, Graphic to)
        {
            var line = GetLine (@from, to);
            line.Style.Stroke.TailArrow = KAOSTools.OmnigraffleExport.Omnigraffle.Arrow.SharpBackCross;
            return line;
        }

        private void AddText (LineGraphic graphic, string text, float position = 0.5f)
        {
            var labelBox = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Rectangle, 50, 50, 100, 100);
            labelBox.Text = new Omnigraffle.TextInfo (text) {
                Alignement = KAOSTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center,
                SideMargin = 0, TopBottomMargin = 0
            };
            labelBox.FontInfo.Size = 10;
            labelBox.Style.Shadow.Draws = false;
            labelBox.FitText = KAOSTools.OmnigraffleExport.Omnigraffle.FitText.Yes;
            labelBox.Wrap = false;
            labelBox.Flow = KAOSTools.OmnigraffleExport.Omnigraffle.Flow.Resize;
            labelBox.Style.Fill.Color = new KAOSTools.OmnigraffleExport.Omnigraffle.Color (1, 1, 1);
            labelBox.Style.Stroke.Draws = false;
            labelBox.Line = new LineInfo (graphic.ID) { Position = position };

            sheet.GraphicsList.Add (labelBox);
        }


        private void AddText (ShapedGraphic graphic, string text)
        {
            graphic.Text = new Omnigraffle.TextInfo (GetRtfUnicodeEscapedString(text)) {
                Alignement = KAOSTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center,
                SideMargin = 10,
                TopBottomMargin = 3
            };
            graphic.Style.Shadow.Draws = false;
            graphic.FitText = KAOSTools.OmnigraffleExport.Omnigraffle.FitText.Vertical;
            graphic.Flow = KAOSTools.OmnigraffleExport.Omnigraffle.Flow.Resize;

        }

        void SetFillColor (ShapedGraphic graphic, double red, double green, double blue)
        {
            graphic.Style.Fill.Color = new Color (red, green, blue);
        }

        void SetStrokeWidth (ShapedGraphic graphic, int lineWidth)
        {
            graphic.Style.Stroke.Width = lineWidth;
        }

        private void AddParallelogram (string key,
                                       string text,
                                       int lineWidth,
                                       double red, double green, double blue)
        {
            var graphic = GetParallelogram ();
            AddText (graphic, text);
            SetFillColor (graphic, red, green, blue);
            SetStrokeWidth (graphic, lineWidth);

            Add (key, graphic);
        }

        private void AddInvertParallelogram (string key,
                                             string text,
                                             int lineWidth,
                                             double red, double green, double blue)
        {
            var graphic = GetInvertParallelogram ();
            AddText (graphic, text);
            SetFillColor (graphic, red, green, blue);
            SetStrokeWidth (graphic, lineWidth);

            Add (key, graphic);
        }

        private void AddHomeShape (string key,
                                             string text,
                                             int lineWidth,
                                             double red, double green, double blue)
        {
            var graphic = GetHomeShape ();
            AddText (graphic, text);
            SetFillColor (graphic, red, green, blue);
            SetStrokeWidth (graphic, lineWidth);

            Add (key, graphic);
        }
        
        private void AddHexagon (string key,
                                   string text,
                                   int lineWidth,
                                   double red, double green, double blue)
        {
            var graphic = GetHexagon ();
            AddText (graphic, text);
            SetFillColor (graphic, red, green, blue);
            SetStrokeWidth (graphic, lineWidth);

            Add (key, graphic);
        }

        private void AddCloud (string key,
                               string text,
                               int lineWidth,
                               double red, double green, double blue)
        {
            var graphic = GetCloud ();
            AddText (graphic, text);
            SetFillColor (graphic, red, green, blue);
            SetStrokeWidth (graphic, lineWidth);

            Add (key, graphic);
        }

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

        public void Render (GoalRefinement refinement)
        {
            var circle = GetCircle ();
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
    }
}

