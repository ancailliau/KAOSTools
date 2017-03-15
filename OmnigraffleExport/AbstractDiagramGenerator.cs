using System;
using System.Collections.Generic;
using KAOSTools.OmnigraffleExport.Omnigraffle;
using KAOSTools.Core;
using System.Text;
using System.Linq;

namespace KAOSTools.OmnigraffleExport
{
    public abstract class AbstractDiagramGenerator
    {
        protected IDictionary<string, IList<Graphic>> shapes;
        protected Sheet sheet;

        protected static int _i = 1;
        protected static int NextId {
            get {
                return _i++;
            }
        }

        public AbstractDiagramGenerator (Sheet sheet, IDictionary<string, IList<Graphic>> shapes)
        {
            this.sheet = sheet;
            this.shapes = shapes;
        }

        protected void Add (string key, Graphic graphic)
        {
            if (!this.shapes.ContainsKey (key)) {
                this.shapes.Add (key, new List<Graphic> ());
            }
            this.shapes[key].Add (graphic);

            sheet.GraphicsList.Add (graphic);
        }

        protected static string GetRtfUnicodeEscapedString(string s)
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

        protected ShapedGraphic GetParallelogram ()
        {
            var graphic = new ShapedGraphic (NextId, 
                                             Omnigraffle.Shape.Bezier, 
                50, 50, 125, 70);

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

        protected ShapedGraphic GetInvertParallelogram ()
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

        protected ShapedGraphic GetHomeShape ()
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

        protected ShapedGraphic GetHexagon ()
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

        protected ShapedGraphic GetRectangle ()
        {
            var graphic = new ShapedGraphic (NextId, 
                                             Omnigraffle.Shape.Rectangle, 
                                             50, 50, 175, 70);
            return graphic;
        }

        protected ShapedGraphic GetCloud ()
        {
            var graphic = new ShapedGraphic (NextId, 
                                             Omnigraffle.Shape.Cloud, 
                                             50, 50, 175, 70);
            return graphic;
        }

        protected ShapedGraphic GetCircle ()
        {
            var circle = new ShapedGraphic (NextId, Omnigraffle.Shape.Circle, 50, 50, 10, 10);
            circle.Style.Shadow.Draws = false;

            circle.FitText = KAOSTools.OmnigraffleExport.Omnigraffle.FitText.Clip;
            circle.Flow = KAOSTools.OmnigraffleExport.Omnigraffle.Flow.Clip;

            return circle;
        }

        protected LineGraphic GetLine (Graphic @from, Graphic to, bool dashed = false)
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
            
            if (dashed)
                line.Style.Stroke.Pattern = StrokePattern.Dashed;

            return line;
        }

        protected LineGraphic GetArrow (Graphic @from, Graphic to, bool dashed = false)
        {
            var line = GetLine (@from, to, dashed);
            line.Style.Stroke.HeadArrow = KAOSTools.OmnigraffleExport.Omnigraffle.Arrow.Arrow;

            return line;
        }

        protected LineGraphic GetFilledArrow (Graphic @from, Graphic to, bool dashed = false)
        {
            var line = GetLine (@from, to, dashed);
            line.Style.Stroke.HeadArrow = KAOSTools.OmnigraffleExport.Omnigraffle.Arrow.FilledArrow;
            return line;
        }

        protected LineGraphic GetSharpBackCrossArrow (Graphic @from, Graphic to, bool dashed = false)
        {
            var line = GetLine (@from, to, dashed);
            line.Style.Stroke.TailArrow = KAOSTools.OmnigraffleExport.Omnigraffle.Arrow.SharpBackCross;
            return line;
        }

        protected void AddText (LineGraphic graphic, string text, float position = 0.5f)
        {
            var labelBox = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Rectangle, 50, 50, 100, 100);
            labelBox.Text = new Omnigraffle.TextInfo (text) {
                Alignement = KAOSTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center,
                SideMargin = 0, TopBottomMargin = 0
            };
            labelBox.FontInfo.Size = 8;
            labelBox.Style.Shadow.Draws = false;
            labelBox.FitText = KAOSTools.OmnigraffleExport.Omnigraffle.FitText.Yes;
            // labelBox.Wrap = false;
            labelBox.Flow = KAOSTools.OmnigraffleExport.Omnigraffle.Flow.Resize;
            labelBox.Style.Fill.Color = new KAOSTools.OmnigraffleExport.Omnigraffle.Color (1, 1, 1);
            labelBox.Style.Stroke.Draws = false;
            labelBox.Line = new LineInfo (graphic.ID) { Position = position };

            sheet.GraphicsList.Add (labelBox);
        }

        protected enum TextAlignement {
            Left, Center, Right
        }

        protected void AddText (ShapedGraphic graphic, string text, 
                                bool bold = false, TextAlignement align = TextAlignement.Center)
        {
            var str = (bold ? @"\b " : "");
            str += (align == TextAlignement.Left ? @"\ql " : "");
            str += (align == TextAlignement.Right ? @"\qr " : "");
            str += (align == TextAlignement.Center ? @"\qc " : "");
            str += GetRtfUnicodeEscapedString (text);

            graphic.Text = new Omnigraffle.TextInfo (str) {
                Alignement = KAOSTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center,
                SideMargin = 10,
                TopBottomMargin = 3
            };
            graphic.Style.Shadow.Draws = false;
            graphic.FitText = KAOSTools.OmnigraffleExport.Omnigraffle.FitText.Vertical;
            graphic.Flow = KAOSTools.OmnigraffleExport.Omnigraffle.Flow.Resize;
            graphic.FontInfo.Size = 10;
        }

        protected void SetFillColor (ShapedGraphic graphic, double red, double green, double blue)
        {
            graphic.Style.Fill.Color = new Color (red, green, blue);
        }

        protected void SetStrokeWidth (ShapedGraphic graphic, int lineWidth)
        {
            graphic.Style.Stroke.Width = lineWidth;
        }

        protected void AddParallelogram (string key,
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

        protected void AddInvertParallelogram (string key,
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

        protected void AddHomeShape (string key,
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

        protected ShapedGraphic AddHexagon (string key,
                                 string text,
                                 int lineWidth,
                                 double red, double green, double blue)
        {
            var graphic = GetHexagon ();
            AddText (graphic, text);
            SetFillColor (graphic, red, green, blue);
            SetStrokeWidth (graphic, lineWidth);

            graphic.Wrap = false;

            Add (key, graphic);

            return graphic;
        }

        protected void AddRectangle (string key,
                                     string text,
                                     int lineWidth,
                                     double red, double green, double blue)
        {
            var graphic = GetRectangle ();
            AddText (graphic, text);
            SetFillColor (graphic, red, green, blue);
            SetStrokeWidth (graphic, lineWidth);

            Add (key, graphic);
        }

        protected void AddCloud (string key,
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

        #region High-level abstraction

        protected void Render (Goal goal)
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

        protected void Render (SoftGoal softGoal)
        {
            AddCloud (softGoal.Identifier, softGoal.FriendlyName, 
                1, 1, 1, 1);
        }

        protected void Render (AntiGoal antigoal)
        {
            int lineWidth = 1;
            if (antigoal.AgentAssignments().Count() > 0)
                lineWidth = 2;

            AddParallelogram (antigoal.Identifier, antigoal.FriendlyName, 
                lineWidth, 1, 234.0/255, 192.0/255);
        }

        protected void Render (Obstacle obstacle)
        {
            int lineWidth = 1;
            if (obstacle.AgentAssignments().Count() > 0)
                lineWidth = 2;

            AddInvertParallelogram (obstacle.Identifier, obstacle.FriendlyName, 
                lineWidth, 1, 0.590278, 0.611992);
        }

        protected void Render (DomainProperty domProp)
        {
            AddHomeShape (domProp.Identifier, domProp.FriendlyName, 
                1, 0.895214, 1, 0.72515);
        }

        protected void Render (DomainHypothesis domHyp)
        {
            AddHomeShape (domHyp.Identifier, domHyp.FriendlyName, 
                1, 1, 0.92156862745, 0.92156862745);
        }

        protected ShapedGraphic Render (Agent agent)
        {
            if (agent.Type == AgentType.Software)
                return AddHexagon (agent.Identifier, agent.FriendlyName, 
                    1, 0.99607843137, 0.80392156862, 0.58039215686);
            if (agent.Type == AgentType.Malicious)
                return AddHexagon (agent.Identifier, agent.FriendlyName, 
                    1, 1, 0.590278, 0.611992);

            return AddHexagon (agent.Identifier, agent.FriendlyName, 
                1, 0.824276, 0.670259, 1);
        }

        protected void Render (Entity entity)
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


        protected void Render (GoalRefinement refinement)
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
            
            foreach (var child in refinement.DomainPropertyIdentifiers) {
                if (!shapes.ContainsKey(child))
                    continue;

                var childGraphic = shapes [child].First ();
                var line = GetLine (childGraphic, circle);
                sheet.GraphicsList.Add (line);
            }

            foreach (var child in refinement.PositiveSoftGoalsIdentifiers) {
                if (!shapes.ContainsKey(child))
                    continue;

                var childGraphic = shapes [child].First ();
                var line = GetFilledArrow (circle, childGraphic, true);
                AddText (line, @"Positive\par contribution");
                sheet.GraphicsList.Add (line);
            }

            foreach (var child in refinement.NegativeSoftGoalsIdentifiers) {
                if (!shapes.ContainsKey(child))
                    continue;

                var childGraphic = shapes [child].First ();
                var line = GetFilledArrow (circle, childGraphic, true);
                AddText (line, @"Negative\par contribution");
                sheet.GraphicsList.Add (line);
            }
        }

        protected void Render (AntiGoalRefinement refinement)
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

        protected void Render (ObstacleRefinement refinement)
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

            foreach (var child in refinement.DomainHypothesisIdentifiers) {
                if (!shapes.ContainsKey(child))
                    continue;

                var childGraphic = shapes [child].First ();
                var line = GetLine (childGraphic, circle);
                sheet.GraphicsList.Add (line);
            }
        }

        protected void Render (GoalAgentAssignment assignment, bool createAgentShapes = false)
        {
            var circle = GetCircle ();
            Add (assignment.Identifier, circle);

            if (shapes.ContainsKey(assignment.GoalIdentifier)) {
                var parentGraphic = shapes[assignment.GoalIdentifier].First ();
                var topArrow = GetFilledArrow (circle, parentGraphic);
                sheet.GraphicsList.Add (topArrow);
            }

            foreach (var child in assignment.AgentIdentifiers) {
                Graphic childGraphic;
                if (!createAgentShapes) {
                    if (!shapes.ContainsKey(child))
                        continue;
                    childGraphic = shapes [child].First ();
                } else {
                    childGraphic = Render (assignment.model.Agents ().Single (x => x.Identifier == child));
                }

                var line = GetLine (childGraphic, circle);
                sheet.GraphicsList.Add (line);
            }
        }

        protected void Render (AntiGoalAgentAssignment assignment)
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

        protected void Render (ObstacleAgentAssignment assignment)
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

        protected void Render (ObstacleAssumption exception)
        {
            if (!shapes.ContainsKey (exception.AnchorGoalIdentifier)) {
                return;
            }

            if (!shapes.ContainsKey (exception.ResolvedObstacleIdentifier)) {
                return;
            }

            var anchorGoalGraphic = shapes [exception.AnchorGoalIdentifier] [0];
            var resolvingGoalGraphic = shapes [exception.ResolvedObstacleIdentifier] [0];

            var topArrow = GetArrow (anchorGoalGraphic, resolvingGoalGraphic);
            topArrow.Style.Stroke.Pattern = StrokePattern.Dashed;
            AddText (topArrow, @"\b Provided not \b0");
            Add (exception.Identifier, topArrow);
        }

        protected void Render (GoalException exception)
        {
            if (!shapes.ContainsKey (exception.ResolvingGoalIdentifier)) {
                return;
            }

            if (!shapes.ContainsKey (exception.AnchorGoalIdentifier)) {
                return;
            }

            var anchorGoalGraphic = shapes [exception.AnchorGoalIdentifier].First ();
            var resolvingGoalGraphic = shapes [exception.ResolvingGoalIdentifier].First ();

            var topArrow = GetArrow (anchorGoalGraphic, resolvingGoalGraphic);
            topArrow.Style.Stroke.Pattern = StrokePattern.Dashed;
            AddText (topArrow, @"\b Except \b0" + exception.Obstacle().FriendlyName);
            Add (exception.Identifier, topArrow);
        }


        protected void Render (GoalReplacement exception)
        {
            if (!shapes.ContainsKey (exception.ResolvingGoalIdentifier)) {
                return;
            }

            if (!shapes.ContainsKey (exception.AnchorGoalIdentifier)) {
                return;
            }

            var anchorGoalGraphic = shapes [exception.AnchorGoalIdentifier].First ();
            var resolvingGoalGraphic = shapes [exception.ResolvingGoalIdentifier].First ();

            var topArrow = GetArrow (resolvingGoalGraphic, anchorGoalGraphic);
            topArrow.Style.Stroke.Pattern = StrokePattern.Dashed;
            AddText (topArrow, @"\b Replace \b0" + exception.Obstacle().FriendlyName);
            Add (exception.Identifier, topArrow);
        }

        protected void Render (Resolution resolution)
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

        protected void Render (Obstruction obstruction)
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

        protected void Render (KAOSTools.Core.Attribute attribute)
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

        #endregion
    }
}

