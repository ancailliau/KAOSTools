using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using NDesk.Options;
using KAOSFormalTools.Domain;
using KAOSFormalTools.Executable;
using KAOSFormalTools.OmnigraffleExport.Omnigraffle;

namespace KAOSFormalTools.OmnigraffleExport
{
    class MainClass : KAOSFormalToolsCLI
    {
        static Dictionary<Omnigraffle.Sheet, Dictionary<string, Omnigraffle.ShapedGraphic>> mapping;

        private static int _i = 1;
        private static int NextId {
            get {
                return _i++;
            }
        }

        public static void Main (string[] args)
        {
            string filename = "";
            bool experimental = false;
            options.Add ("o|output=", "Export in specified filename", v => filename = v);
            options.Add ("experimental", "Export experimental diagrams", v => experimental = true);

            Init (args);

            mapping = new Dictionary<Omnigraffle.Sheet, Dictionary<string, Omnigraffle.ShapedGraphic>> ();

            var document   = new Omnigraffle.Document ();

            ExportIdealGoalModel (model, document);
            ExportObstacles (model, document);
            ExportResolutionsGoalModel (model, document);
            ExportResponsibilities (model, document);

            if (experimental)
                ExportExperimentalDiagrams (model, document);

            if (string.IsNullOrEmpty (filename)) 
                OmniGraffleGenerator.Export (document, Console.Out);
            else 
                OmniGraffleGenerator.Export (document, filename);
        }

        #region Export diagrams

        static void ExportExperimentalDiagrams (GoalModel model, KAOSFormalTools.OmnigraffleExport.Omnigraffle.Document document)
        {
            var t = model.GetResponsibilities ();
            var canvas = new Omnigraffle.Sheet (1, string.Format ("Alternative responsibilities"));
            ExportAlternativeResponsibilities (canvas, t, null);
            document.Canvas.Add (canvas);
        }

        private static void ExportAlternativeResponsibilities (Omnigraffle.Sheet canvas, ResponsibilityNode r, Graphic parent) 
        {
            var @group = new Omnigraffle.Group (NextId);

            var background = new Omnigraffle.ShapedGraphic (NextId, Shape.Rectangle, 0, 0, 300, 300);
            background.Flow = Flow.Clip;
            background.FitText = Omnigraffle.FitText.Clip;

            background.Style.Shadow.Draws = false;
            background.Style.Stroke.CornerRadius = 5;

            foreach (var kv in r.Responsibility) {
                var ag = RenderAgent (kv.Key);
                @group.Graphics.Add (ag);

                foreach (var goal in kv.Value) {
                    var goalGraphic = RenderGoal (goal);
                    @group.Graphics.Add (goalGraphic);
                    AddResponsibility (@group.Graphics, ag, goalGraphic);
                }

            }
            
            @group.Graphics.Add (background);

            canvas.GraphicsList.Add (@group);

            if (parent != null) {
                AddArrow (canvas.GraphicsList, background, parent);
            }
            
            foreach (var child in r.children) {
                ExportAlternativeResponsibilities (canvas, child, background);
            }
        }

        static void ExportResponsibilities (GoalModel model, KAOSFormalTools.OmnigraffleExport.Omnigraffle.Document document)
        {
            foreach (var agent in model.Agents) {
                var agentCanvas = new Omnigraffle.Sheet (1, string.Format ("Responsibilities for {0}", agent.Name));
                agentCanvas.LayoutInfo.LayoutEngine = KAOSFormalTools.OmnigraffleExport.Omnigraffle.LayoutEngine.Twopi;
                agentCanvas.LayoutInfo.TwopiOverlap = true;
                agentCanvas.LayoutInfo.TwopiRankSep = 100;
                mapping.Add (agentCanvas, new Dictionary<string, KAOSFormalTools.OmnigraffleExport.Omnigraffle.ShapedGraphic> ());

                var agentGraphic = AddAgent (agentCanvas, agent);
                foreach (var goal in model.Goals.Where (g => g.AssignedAgents.SelectMany(x => x.Agents).Contains (agent))) {
                    var goalGraphic = AddGoal (agentCanvas, goal);
                    AddResponsibility (agentCanvas.GraphicsList, agentGraphic, goalGraphic);
                }
                document.Canvas.Add (agentCanvas);
            }
        }

        static void ExportObstacles (GoalModel model, KAOSFormalTools.OmnigraffleExport.Omnigraffle.Document document)
        {
            foreach (var obstructedGoal in model.ObstructedGoals) {
                var obstacleCanvas = new Omnigraffle.Sheet (1, string.Format ("Obstacles to {0}", obstructedGoal.Name));
                obstacleCanvas.LayoutInfo.HierarchicalOrientation = Omnigraffle.HierarchicalOrientation.BottomTop;
                mapping.Add (obstacleCanvas, new Dictionary<string, KAOSFormalTools.OmnigraffleExport.Omnigraffle.ShapedGraphic> ());

                var goalGraphic = AddGoal (obstacleCanvas, obstructedGoal);
                foreach (var obstacle in obstructedGoal.Obstruction) {
                    RecursiveExportObstacle (obstacleCanvas, obstacle, true);
                    AddSharpBackCrossArrow (obstacleCanvas.GraphicsList, mapping [obstacleCanvas] [obstacle.Identifier], goalGraphic);
                }

                document.Canvas.Add (obstacleCanvas);
            }
        }

        static void ExportIdealGoalModel (GoalModel model, KAOSFormalTools.OmnigraffleExport.Omnigraffle.Document document)
        {
            var goalCanvas = new Omnigraffle.Sheet (1, "Ideal Goal Model");
            goalCanvas.LayoutInfo.HierarchicalOrientation = Omnigraffle.HierarchicalOrientation.BottomTop;
            mapping.Add (goalCanvas, new Dictionary<string, KAOSFormalTools.OmnigraffleExport.Omnigraffle.ShapedGraphic> ());
            foreach (var goal in model.RootGoals) {
                RecursiveExportGoal (goalCanvas, goal);
            }

            foreach (var goal in model.ObstructedGoals) {
                foreach (var obstacle in goal.Obstruction) {
                    AddObstacle (goalCanvas, obstacle);
                    AddSharpBackCrossArrow (goalCanvas.GraphicsList, mapping [goalCanvas] [obstacle.Identifier], mapping [goalCanvas] [goal.Identifier]);
                }
            }

            document.Canvas.Add (goalCanvas);
        }
        
        static void ExportResolutionsGoalModel (GoalModel model, KAOSFormalTools.OmnigraffleExport.Omnigraffle.Document document)
        {
            foreach (var obstacle in model.Obstacles.Where (o => o.Resolutions.Count > 0)) {
                foreach (var resolution in obstacle.Resolutions) {
                    var canvas = new Omnigraffle.Sheet (1, "Resolution for '" + obstacle.Name + "'");
                    canvas.LayoutInfo.HierarchicalOrientation = Omnigraffle.HierarchicalOrientation.BottomTop;
                    mapping.Add (canvas, new Dictionary<string, KAOSFormalTools.OmnigraffleExport.Omnigraffle.ShapedGraphic> ());

                    var obstacleGraphic = AddObstacle (canvas, obstacle);
                    RecursiveExportGoal (canvas, resolution);
                    AddSharpBackCrossArrow (canvas.GraphicsList, mapping [canvas] [resolution.Identifier], obstacleGraphic);

                    document.Canvas.Add (canvas);
                }
            }
        }

        #endregion

        #region Recursive export

        static void RecursiveExportGoal (Omnigraffle.Sheet canvas, Goal goal)
        {
            if (mapping[canvas].ContainsKey (goal.Identifier))
                return;
           
            var parentGraphic = AddGoal (canvas, goal);
            
            foreach (var refinement in goal.Refinements.Reverse ()) {
                var circle = AddCircle (canvas.GraphicsList);

                // We add the arrow to the canvas after the label, so that label is above the arrow
                var topArrow = AddFilledArrow (canvas.GraphicsList, circle, parentGraphic, false);

                if (!string.IsNullOrEmpty (refinement.AlternativeIdentifier)) {
                    var alternativeText = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Rectangle, 50, 50, 100, 100);
                    alternativeText.Text = new Omnigraffle.TextInfo (refinement.AlternativeIdentifier) {
                        Alignement = KAOSFormalTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center,
                        SideMargin = 0, TopBottomMargin = 0
                    };
                    alternativeText.FontInfo.Size = 10;
                    alternativeText.Style.Shadow.Draws = false;
                    alternativeText.FitText = KAOSFormalTools.OmnigraffleExport.Omnigraffle.FitText.Vertical;
                    alternativeText.Flow = KAOSFormalTools.OmnigraffleExport.Omnigraffle.Flow.Resize;
                    alternativeText.Style.Fill.Color = new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Color (1, 1, 1);
                    alternativeText.Style.Stroke.Draws = false;
                    alternativeText.Line = new LineInfo (topArrow.ID);
                    canvas.GraphicsList.Add (alternativeText);
                }

                // Ad the arrow
                canvas.GraphicsList.Add (topArrow);

                foreach (var child in refinement.Children.Reverse ()) {
                    RecursiveExportGoal (canvas, child);
                    var childGraphic = mapping[canvas][child.Identifier];
                    AddLine (canvas.GraphicsList, childGraphic, circle);
                }

                foreach (var domprop in refinement.DomainProperties.Reverse ()) {
                    var childGraphic = AddDomainProperty (canvas, domprop);
                    AddLine (canvas.GraphicsList, childGraphic, circle);
                }
            }
            
            foreach (var assignment in goal.AssignedAgents) {

                var circle = AddCircle (canvas.GraphicsList);
                
                // We add the arrow to the canvas after the label, so that label is above the arrow
                var topArrow = AddFilledArrow (canvas.GraphicsList, circle, parentGraphic, false);

                if (!string.IsNullOrEmpty (assignment.AlternativeIdentifier)) {
                    var alternativeText = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Rectangle, 50, 50, 100, 100);
                    alternativeText.Text = new Omnigraffle.TextInfo (assignment.AlternativeIdentifier) {
                        Alignement = KAOSFormalTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center,
                        SideMargin = 0, TopBottomMargin = 0
                    };
                    alternativeText.FontInfo.Size = 10;
                    alternativeText.Style.Shadow.Draws = false;
                    alternativeText.FitText = KAOSFormalTools.OmnigraffleExport.Omnigraffle.FitText.Vertical;
                    alternativeText.Flow = KAOSFormalTools.OmnigraffleExport.Omnigraffle.Flow.Resize;
                    alternativeText.Style.Fill.Color = new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Color (1, 1, 1);
                    alternativeText.Style.Stroke.Draws = false;
                    alternativeText.Line = new LineInfo (topArrow.ID);
                    canvas.GraphicsList.Add (alternativeText);
                }
                
                // Ad the arrow
                canvas.GraphicsList.Add (topArrow);

                foreach (var agent in assignment.Agents) {
                    var agentGraphic = AddAgent (canvas, agent);
                    AddLine (canvas.GraphicsList, agentGraphic, circle);
                    // AddResponsibility (canvas.GraphicsList, agentGraphic, parentGraphic);
                }
            }
        }
        
        static void RecursiveExportObstacle (Omnigraffle.Sheet canvas, Obstacle obstacle, bool export_resolution = false)
        {
            if (!mapping[canvas].ContainsKey (obstacle.Identifier))
                AddObstacle (canvas, obstacle);
            
            var parentGraphic = mapping[canvas][obstacle.Identifier];
            
            foreach (var refinement in obstacle.Refinements) {
                var circle = AddCircle (canvas.GraphicsList);
                
                AddFilledArrow (canvas.GraphicsList, circle, parentGraphic);
                
                foreach (var child in refinement.Children) {
                    RecursiveExportObstacle (canvas, child, export_resolution);
                    var childGraphic = mapping[canvas][child.Identifier];
                    
                    AddLine (canvas.GraphicsList, childGraphic, circle);
                }

                foreach (var domprop in refinement.DomainProperties) {
                    var childGraphic = AddDomainProperty (canvas, domprop);
                    AddLine (canvas.GraphicsList, childGraphic, circle);
                }
            }

            if (export_resolution) {
                foreach (var goal in obstacle.Resolutions) {
                    if (!mapping[canvas].ContainsKey (goal.Identifier))
                        AddGoal (canvas, goal);

                    var goalGraphic = mapping [canvas] [goal.Identifier];
                    AddSharpBackCrossArrow (canvas.GraphicsList, goalGraphic, parentGraphic);
                }
            }
        }

        #endregion

        static Omnigraffle.ShapedGraphic AddResponsibility (List<Graphic> canvas, Omnigraffle.ShapedGraphic agentGraphic, Omnigraffle.ShapedGraphic goalGraphic)
        {
            var circle = AddCircle (canvas);

            AddFilledArrow (canvas, circle, goalGraphic);
            AddLine (canvas, agentGraphic, circle);

            return circle;
        }

        static Omnigraffle.ShapedGraphic AddCircle (List<Graphic> canvas)
        {
            var circle = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Circle, 50, 50, 10, 10);
            circle.Style.Shadow.Draws = false;
            
            circle.FitText = KAOSFormalTools.OmnigraffleExport.Omnigraffle.FitText.Clip;
            circle.Flow = KAOSFormalTools.OmnigraffleExport.Omnigraffle.Flow.Clip;
            
            canvas.Add (circle);
            
            return circle;
        }

        static Omnigraffle.LineGraphic AddLine (List<Graphic> canvas, Omnigraffle.Graphic @from, Omnigraffle.Graphic to, bool add = true)
        {
            var line = new Omnigraffle.LineGraphic (NextId);
            line.Head = new Omnigraffle.LineEndInfo (to.ID);
            line.Tail = new Omnigraffle.LineEndInfo (@from.ID);

            if (to is ShapedGraphic) {
                line.Points.Add ((to as ShapedGraphic).Bounds.TopLeft);
            }

            if (@from is ShapedGraphic) {
                line.Points.Add ((@from as ShapedGraphic).Bounds.BottomRight);
            }

            line.Style.Shadow.Draws = false;

            if (add)
                canvas.Add (line);

            return line;
        }

        static Omnigraffle.LineGraphic AddArrow (List<Graphic> canvas, Omnigraffle.Graphic @from, Omnigraffle.Graphic to)
        {
            var line = AddLine (canvas, @from, to);
            line.Style.Stroke.HeadArrow = KAOSFormalTools.OmnigraffleExport.Omnigraffle.Arrow.Arrow;
            return line;
        }


        static Omnigraffle.LineGraphic AddFilledArrow (List<Graphic>  canvas, Omnigraffle.Graphic @from, Omnigraffle.Graphic to, bool add = true)
        {
            var line = AddLine (canvas, @from, to, add);
            line.Style.Stroke.HeadArrow = KAOSFormalTools.OmnigraffleExport.Omnigraffle.Arrow.FilledArrow;
            return line;
        }
        
        static Omnigraffle.LineGraphic AddSharpBackCrossArrow (List<Graphic> canvas, Omnigraffle.Graphic @from, Omnigraffle.Graphic to)
        {
            var line = AddLine (canvas, @from, to);
            line.Style.Stroke.TailArrow = KAOSFormalTools.OmnigraffleExport.Omnigraffle.Arrow.SharpBackCross;
            return line;
        }

        static Omnigraffle.ShapedGraphic AddObstacle (Omnigraffle.Sheet canvas, Obstacle obstacle)
        {
            var graphic = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Bezier, 50, 50, 150, 70);
           
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.5, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.5, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.5, -0.5));

            graphic.Text = new Omnigraffle.TextInfo (obstacle.Name) {
                Alignement = KAOSFormalTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center,
                SideMargin = 10, TopBottomMargin = 3
            };
            graphic.Style.Shadow.Draws = false;
            graphic.FitText = KAOSFormalTools.OmnigraffleExport.Omnigraffle.FitText.Vertical;
            graphic.Flow = KAOSFormalTools.OmnigraffleExport.Omnigraffle.Flow.Resize;

            graphic.Style.Fill.Color = new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Color (1, 0.590278, 0.611992);

            if (obstacle.Refinements.Count == 0)
                graphic.Style.Stroke.Width = 2;

            canvas.GraphicsList.Add (graphic);
            mapping[canvas].Add (obstacle.Identifier, graphic);

            return graphic;
        }

        static Omnigraffle.ShapedGraphic AddDomainProperty (Omnigraffle.Sheet canvas, DomainProperty domprop)
        {
            var graphic = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Bezier, 50, 50, 150, 70);
            
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.45, -0.5));
            
            graphic.Text = new Omnigraffle.TextInfo (domprop.Name) {
                Alignement = KAOSFormalTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center,
                SideMargin = 10, TopBottomMargin = 3
            };
            graphic.Style.Shadow.Draws = false;
            graphic.FitText = KAOSFormalTools.OmnigraffleExport.Omnigraffle.FitText.Vertical;
            graphic.Flow = KAOSFormalTools.OmnigraffleExport.Omnigraffle.Flow.Resize;
            
            graphic.Style.Fill.Color = new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Color (0.895214, 1, 0.72515);
            
            canvas.GraphicsList.Add (graphic);

            return graphic;
        }

        static ShapedGraphic RenderAgent (Agent agent)
        {
            var longestWord = (string.IsNullOrEmpty (agent.Name) ? agent.Identifier : agent.Name).Split (' ').Max (x => x.Length);
            var graphic = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Bezier, 50, 50, longestWord * 6 + 30, 30);
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (-0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (-0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (0.5, 0));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (0.5, 0));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (0.5, 0));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (-0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (-0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (-0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (-0.5, 0));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (-0.5, 0));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (-0.5, 0));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (-0.45, -0.5));
            graphic.Text = new Omnigraffle.TextInfo ((string.IsNullOrEmpty (agent.Name) ? agent.Identifier : agent.Name)) {
                Alignement = KAOSFormalTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center,
                SideMargin = 10, TopBottomMargin = 3
            };
            graphic.Style.Shadow.Draws = false;
            graphic.FitText = KAOSFormalTools.OmnigraffleExport.Omnigraffle.FitText.Vertical;
            graphic.Flow = KAOSFormalTools.OmnigraffleExport.Omnigraffle.Flow.Resize;
            if (agent.Type == AgentType.Software)
                graphic.Style.Fill.Color = new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Color (0.99607843137, 0.80392156862, 0.58039215686);
            else
                graphic.Style.Fill.Color = new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Color (0.824276, 0.670259, 1);
            return graphic;
        }

        static Omnigraffle.ShapedGraphic AddAgent (Omnigraffle.Sheet canvas,Agent agent)
        {
            var graphic = RenderAgent (agent);
            canvas.GraphicsList.Add (graphic);

            return graphic;
        }

        static ShapedGraphic RenderGoal (Goal goal)
        {
            var graphic = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Bezier, 50, 50, 175, 70);
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (-0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (-0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (0.5, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (0.5, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (0.5, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (-0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (-0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (-0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point (-0.45, -0.5));
            graphic.Text = new Omnigraffle.TextInfo (string.IsNullOrEmpty (goal.Name) ? goal.Identifier : goal.Name) {
                Alignement = KAOSFormalTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center,
                SideMargin = 10,
                TopBottomMargin = 3
            };
            graphic.Style.Shadow.Draws = false;
            graphic.FitText = KAOSFormalTools.OmnigraffleExport.Omnigraffle.FitText.Vertical;
            graphic.Flow = KAOSFormalTools.OmnigraffleExport.Omnigraffle.Flow.Resize;

            bool assignedToEnvAgents = (
                from a in goal.AssignedAgents.SelectMany (x => x.Agents)
                where a.Type != AgentType.Software select a).Count () > 0;

            if (assignedToEnvAgents)
                graphic.Style.Fill.Color = new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Color (1, 0.979841, 0.672223);
            else
                graphic.Style.Fill.Color = new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Color (0.810871, 0.896814, 1);

            if (goal.AssignedAgents.Count > 0)
                graphic.Style.Stroke.Width = 2;

            return graphic;
        }

        static Omnigraffle.ShapedGraphic AddGoal (Omnigraffle.Sheet canvas, Goal goal)
        {
            var graphic = RenderGoal (goal);

            canvas.GraphicsList.Add (graphic);
            mapping[canvas].Add (goal.Identifier, graphic);

            return graphic;
        }
    }
}
