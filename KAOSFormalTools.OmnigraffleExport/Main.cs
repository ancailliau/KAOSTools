using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using NDesk.Options;
using KAOSFormalTools.Domain;
using KAOSFormalTools.Executable;

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
            options.Add ("o|output=", "Export in specified filename", v => filename = v);

            Init (args);

            mapping = new Dictionary<Omnigraffle.Sheet, Dictionary<string, Omnigraffle.ShapedGraphic>> ();

            var document   = new Omnigraffle.Document ();

            ExportIdealGoalModel (model, document);
            ExportObstacles (model, document);
            ExportResolutionsGoalModel (model, document);
            ExportResponsibilities (model, document);

            if (string.IsNullOrEmpty (filename)) 
                OmniGraffleGenerator.Export (document, Console.Out);
            else 
                OmniGraffleGenerator.Export (document, filename);
        }

        #region Export diagrams

        static void ExportResponsibilities (GoalModel model, KAOSFormalTools.OmnigraffleExport.Omnigraffle.Document document)
        {
            foreach (var agent in model.Agents) {
                var agentCanvas = new Omnigraffle.Sheet (1, string.Format ("Responsibilities for {0}", agent.Name));
                agentCanvas.LayoutInfo.LayoutEngine = KAOSFormalTools.OmnigraffleExport.Omnigraffle.LayoutEngine.Twopi;
                agentCanvas.LayoutInfo.TwopiOverlap = true;
                agentCanvas.LayoutInfo.TwopiRankSep = 100;
                mapping.Add (agentCanvas, new Dictionary<string, KAOSFormalTools.OmnigraffleExport.Omnigraffle.ShapedGraphic> ());
                var agentGraphic = AddAgent (agentCanvas, agent);
                foreach (var goal in model.Goals.Where (g => g.AssignedAgents.Contains (agent))) {
                    var goalGraphic = AddGoal (agentCanvas, goal);
                    AddResponsibility (agentCanvas, agentGraphic, goalGraphic);
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
                    AddSharpBackCrossArrow (obstacleCanvas, mapping [obstacleCanvas] [obstacle.Identifier], goalGraphic);
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
                    AddSharpBackCrossArrow (goalCanvas, mapping [goalCanvas] [obstacle.Identifier], mapping [goalCanvas] [goal.Identifier]);
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
                    AddSharpBackCrossArrow (canvas, mapping [canvas] [resolution.Identifier], obstacleGraphic);

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
                var circle = AddCircle (canvas);

                AddFilledArrow (canvas, circle, parentGraphic);
                
                foreach (var child in refinement.Children) {
                    RecursiveExportGoal (canvas, child);
                    var childGraphic = mapping[canvas][child.Identifier];
                    AddLine (canvas, childGraphic, circle);
                }

                foreach (var domprop in refinement.DomainProperties) {
                    var childGraphic = AddDomainProperty (canvas, domprop);
                    AddLine (canvas, childGraphic, circle);
                }
            }
            
            foreach (var agent in goal.AssignedAgents) {
                var agentGraphic = AddAgent (canvas, agent);
                AddResponsibility (canvas, agentGraphic, parentGraphic);
            }
        }
        
        static void RecursiveExportObstacle (Omnigraffle.Sheet canvas, Obstacle obstacle, bool export_resolution = false)
        {
            if (!mapping[canvas].ContainsKey (obstacle.Identifier))
                AddObstacle (canvas, obstacle);
            
            var parentGraphic = mapping[canvas][obstacle.Identifier];
            
            foreach (var refinement in obstacle.Refinements) {
                var circle = AddCircle (canvas);
                
                AddFilledArrow (canvas, circle, parentGraphic);
                
                foreach (var child in refinement.Children) {
                    RecursiveExportObstacle (canvas, child, export_resolution);
                    var childGraphic = mapping[canvas][child.Identifier];
                    
                    AddLine (canvas, childGraphic, circle);
                }

                foreach (var domprop in refinement.DomainProperties) {
                    var childGraphic = AddDomainProperty (canvas, domprop);
                    AddLine (canvas, childGraphic, circle);
                }
            }

            if (export_resolution) {
                foreach (var goal in obstacle.Resolutions) {
                    if (!mapping[canvas].ContainsKey (goal.Identifier))
                        AddGoal (canvas, goal);

                    var goalGraphic = mapping [canvas] [goal.Identifier];
                    AddSharpBackCrossArrow (canvas, goalGraphic, parentGraphic);
                }
            }
        }

        #endregion

        static Omnigraffle.ShapedGraphic AddResponsibility (Omnigraffle.Sheet canvas, Omnigraffle.ShapedGraphic agentGraphic, Omnigraffle.ShapedGraphic goalGraphic)
        {
            var circle = AddCircle (canvas);

            AddFilledArrow (canvas, circle, goalGraphic);
            AddLine (canvas, agentGraphic, circle);

            return circle;
        }

        static Omnigraffle.ShapedGraphic AddCircle (Omnigraffle.Sheet canvas)
        {
            var circle = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Circle, 50, 50, 10, 10);
            circle.Style.Shadow.Draws = false;
            
            circle.FitText = KAOSFormalTools.OmnigraffleExport.Omnigraffle.FitText.Clip;
            circle.Flow = KAOSFormalTools.OmnigraffleExport.Omnigraffle.Flow.Clip;
            
            canvas.GraphicsList.Add (circle);
            
            return circle;
        }

        static Omnigraffle.LineGraphic AddLine (Omnigraffle.Sheet canvas, Omnigraffle.ShapedGraphic @from, Omnigraffle.ShapedGraphic to)
        {
            var line = new Omnigraffle.LineGraphic (NextId);
            line.Head = new Omnigraffle.LineEndInfo (to.ID);
            line.Tail = new Omnigraffle.LineEndInfo (@from.ID);
            
            line.Points.Add (to.Bounds.TopLeft);
            line.Points.Add (@from.Bounds.BottomRight);
            
            line.Style.Shadow.Draws = false;

            canvas.GraphicsList.Add (line);

            return line;
        }

        static Omnigraffle.LineGraphic AddFilledArrow (Omnigraffle.Sheet canvas, Omnigraffle.ShapedGraphic @from, Omnigraffle.ShapedGraphic to)
        {
            var line = AddLine (canvas, @from, to);
            line.Style.Stroke.HeadArrow = KAOSFormalTools.OmnigraffleExport.Omnigraffle.Arrow.FilledArrow;
            return line;
        }
        
        static Omnigraffle.LineGraphic AddSharpBackCrossArrow (Omnigraffle.Sheet canvas, Omnigraffle.ShapedGraphic @from, Omnigraffle.ShapedGraphic to)
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

        static Omnigraffle.ShapedGraphic AddAgent (Omnigraffle.Sheet canvas,Agent agent)
        {
            var longestWord = agent.Name.Split (' ').Max (x => x.Length);

            var graphic = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Bezier, 50, 50, longestWord * 6 + 30, 30);

            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.5, 0));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.5, 0));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.5, 0));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.5, 0));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.5, 0));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.5, 0));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.45, -0.5));


            graphic.Text = new Omnigraffle.TextInfo (agent.Name) {
                Alignement = KAOSFormalTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center
            };
            graphic.Style.Shadow.Draws = false;
            
            graphic.FitText = KAOSFormalTools.OmnigraffleExport.Omnigraffle.FitText.Vertical;
            graphic.Flow = KAOSFormalTools.OmnigraffleExport.Omnigraffle.Flow.Resize;

            if (agent.Software) 
                graphic.Style.Fill.Color = new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Color (0.99607843137, 0.80392156862, 0.58039215686);
            else
                graphic.Style.Fill.Color = new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Color (0.824276, 0.670259, 1);
            
            canvas.GraphicsList.Add (graphic);

            return graphic;
        }

        static Omnigraffle.ShapedGraphic AddGoal (Omnigraffle.Sheet canvas, Goal goal)
        {
            var graphic = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Bezier, 50, 50, 200, 70);

            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.5, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.5, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.5, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Point(-0.45, -0.5));

            graphic.Text = new Omnigraffle.TextInfo (goal.Name) {
                Alignement = KAOSFormalTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center,
                SideMargin = 10, TopBottomMargin = 3
            };
            graphic.Style.Shadow.Draws = false;
            graphic.FitText = KAOSFormalTools.OmnigraffleExport.Omnigraffle.FitText.Vertical;
            graphic.Flow = KAOSFormalTools.OmnigraffleExport.Omnigraffle.Flow.Resize;

            bool assignedToSoftwareAgents = (from a in goal.AssignedAgents select a.Software == true).Count () > 0;
            if (assignedToSoftwareAgents) 
                graphic.Style.Fill.Color = new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Color (1, 0.979841, 0.672223);
            else 
                graphic.Style.Fill.Color = new KAOSFormalTools.OmnigraffleExport.Omnigraffle.Color (0.810871, 0.896814, 1);

            if (goal.AssignedAgents.Count > 0)
                graphic.Style.Stroke.Width = 2;
            
            canvas.GraphicsList.Add (graphic);
            mapping[canvas].Add (goal.Identifier, graphic);

            return graphic;
        }
    }
}
