using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using NDesk.Options;
using KAOSTools.MetaModel;
using KAOSTools.Utils;
using KAOSTools.OmnigraffleExport.Omnigraffle;
using System.Text;

namespace KAOSTools.OmnigraffleExport
{
    class MainClass : KAOSToolCLI
    {
        static Dictionary<Omnigraffle.Sheet, Dictionary<string, Omnigraffle.ShapedGraphic>> mapping;
        static ExportOptions exportOptions;

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
            
            exportOptions = new ExportOptions ();

            options.Add ("o|output=", "Export in specified filename", v => filename = v);
            options.Add ("experimental", "Export experimental diagrams", v => experimental = true);
            options.Add ("i|identifier", "Display identifier in diagrams", v => exportOptions.DisplayIdentifiers = true);

            Init (args);
            if (model == null) 
                return;

            mapping = new Dictionary<Omnigraffle.Sheet, Dictionary<string, Omnigraffle.ShapedGraphic>> ();

            var document   = new Omnigraffle.Document ();

            ExportIdealGoalModel (model.GoalModel, document);
            ExportAntiGoalModel (model.GoalModel, document);
            ExportExceptions (model.GoalModel, document);
            ExportObstructedGoalModel (model.GoalModel, document);
            ExportObstacles (model.GoalModel, document);
            ExportResolutionsGoalModel (model.GoalModel, document);
            ExportResponsibilities (model.GoalModel, document);
            ExportObjectModel (model, document);

            if (experimental)
                ExportExperimentalDiagrams (model.GoalModel, document);

            if (string.IsNullOrEmpty (filename)) 
                OmniGraffleGenerator.Export (document, Console.Out);
            else 
                OmniGraffleGenerator.Export (document, filename);
        }

        #region Export diagrams

        static void ExportObjectModel (KAOSModel model, KAOSTools.OmnigraffleExport.Omnigraffle.Document document)
        {
            var canvas = new Omnigraffle.Sheet (1, string.Format ("Object Model"));
            var objMapping = new Dictionary<string, Omnigraffle.Graphic> ();

            foreach (var e in model.Entities.Where (x => x.GetType() != typeof(Relation))) {
                var shape = RenderEntity (e) as Omnigraffle.Group;
                canvas.GraphicsList.Add (shape);

                shape.GroupConnect = true;
                objMapping.Add (e.Identifier, shape);
            }

            foreach (var e in model.Entities.Where (x => x.GetType() != typeof(Relation))) {
                foreach (var p in e.Parents) {
                    var line = AddLine (canvas.GraphicsList, 
                                        objMapping[e.Identifier], 
                                        objMapping[p.Identifier]);
                    line.Style.Stroke.HeadArrow = KAOSTools.OmnigraffleExport.Omnigraffle.Arrow.Arrow;
                    line.Style.Stroke.LineType = LineType.Orthogonal;
                }
            }

            foreach (var e in model.Entities.Where (x => x.GetType() == typeof(Relation)).Cast<Relation>()) {
                if (e.Links.Count == 2) {
                    var t = e.Links.ToList();
                    var src = objMapping[t[0].Target.Identifier];
                    var trg = objMapping[t[1].Target.Identifier];

                    var line = AddLine (canvas.GraphicsList, src, trg, false);
                    line.Style.Stroke.LineType = LineType.Orthogonal;

                    var text = string.IsNullOrEmpty (e.Name) ? e.Identifier : e.Name;
                    var alternativeText = AddLineLabel(canvas.GraphicsList, line, text);
                    alternativeText.FontInfo.Size = 12;

                    if (!string.IsNullOrEmpty(t[0].Multiplicity)) {
                        var multiplicityA = AddLineLabel(canvas.GraphicsList, line, @"\i " + GetRtfUnicodeEscapedString(t[0].Multiplicity), 0.1f);
                        multiplicityA.FontInfo.Size = 10;
                        canvas.GraphicsList.Add (multiplicityA);
                    }

                    if (!string.IsNullOrEmpty(t[1].Multiplicity)) {
                        var multiplicityB = AddLineLabel(canvas.GraphicsList, line, @"\i " + GetRtfUnicodeEscapedString(t[1].Multiplicity), 0.9f);
                        multiplicityB.FontInfo.Size = 10;
                        canvas.GraphicsList.Add (multiplicityB);
                    }

                    canvas.GraphicsList.Add (alternativeText);
                    canvas.GraphicsList.Add (line);

                    if (e.Attributes.Count > 0) {
                        var attr = RenderAttributes (e);
                        var attr_line = AddLine (canvas.GraphicsList, alternativeText, attr, true);
                        attr_line.Style.Stroke.Pattern = StrokePattern.Dashed;
                        attr_line.Style.Stroke.LineType = LineType.Orthogonal;
                        canvas.GraphicsList.Add (attr);
                    }

                } else {
                    Console.WriteLine ("Oh... " + e.Links.Count);
                }
            }
            document.Canvas.Add (canvas);
        }

        static void ExportExperimentalDiagrams (GoalModel model, KAOSTools.OmnigraffleExport.Omnigraffle.Document document)
        {
            var t = model.GetResponsibilities ();
            var canvas = new Omnigraffle.Sheet (1, string.Format ("Alternative responsibilities"));
            ExportAlternativeResponsibilities (canvas, t, null);
            document.Canvas.Add (canvas);
        }

        private static void ExportAlternativeResponsibilities (Omnigraffle.Sheet canvas, ResponsibilityNode r, Graphic parent) 
        {
            /*
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
                    var circle = AddResponsibility (@group.Graphics, ag, goalGraphic);
                    AddLine (canvas, ag, circle);
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
            */
        }

        static void ExportResponsibilities (GoalModel model, KAOSTools.OmnigraffleExport.Omnigraffle.Document document)
        {
            foreach (var agent in model.Agents) {
                var agentCanvas = new Omnigraffle.Sheet (1, string.Format ("{0} responsibilities", agent.Name));
                agentCanvas.LayoutInfo.LayoutEngine = KAOSTools.OmnigraffleExport.Omnigraffle.LayoutEngine.Twopi;
                agentCanvas.LayoutInfo.TwopiOverlap = true;
                agentCanvas.LayoutInfo.TwopiRankSep = 100;
                mapping.Add (agentCanvas, new Dictionary<string, KAOSTools.OmnigraffleExport.Omnigraffle.ShapedGraphic> ());

                var agentGraphic = AddAgent (agentCanvas, agent);

                var assignmentsToAgent =
                    from g in model.Goals 
                        from a in g.AgentAssignments
                        where a.Agents.Contains (agent)
                        select a;

                foreach (var assignment in assignmentsToAgent) { // model.Goals.Where (g => g.AssignedAgents.SelectMany(x => x.Agents).Contains (agent))) {
                    var goal = model.Goals.Where (g => g.AgentAssignments.Contains (assignment)).Single ();
                    var goalGraphic = AddGoal (agentCanvas, goal);

                    string text = "";
                    var responsibilitiesInSystem = assignment.InSystems;

                    if (responsibilitiesInSystem.Count() == 0) {
                        text = "No compatible alternative found";
                    
                    } else if (!responsibilitiesInSystem.SetEquals (model.RootSystems)) {
                        text = string.Join (", ", responsibilitiesInSystem.Select (x => string.IsNullOrEmpty(x.Name) ? x.Identifier : x.Name));
                    }

                    var circle = AddResponsibility (agentCanvas.GraphicsList, agentGraphic, goalGraphic, text);
                    AddLine (agentCanvas.GraphicsList, agentGraphic, circle);
                }
                document.Canvas.Add (agentCanvas);
            }
        }

        static void ExportObstacles (GoalModel model, KAOSTools.OmnigraffleExport.Omnigraffle.Document document)
        {
            foreach (var obstructedGoal in model.ObstructedGoals) {
                var obstacleCanvas = new Omnigraffle.Sheet (1, string.Format ("Obstacles to {0}", obstructedGoal.Name));
                obstacleCanvas.LayoutInfo.HierarchicalOrientation = Omnigraffle.HierarchicalOrientation.BottomTop;
                mapping.Add (obstacleCanvas, new Dictionary<string, KAOSTools.OmnigraffleExport.Omnigraffle.ShapedGraphic> ());

                var goalGraphic = AddGoal (obstacleCanvas, obstructedGoal);
                foreach (var obstacle in obstructedGoal.Obstructions) {
                    RecursiveExportObstacle (obstacleCanvas, obstacle, true);
                    AddSharpBackCrossArrow (obstacleCanvas.GraphicsList, mapping [obstacleCanvas] [obstacle.Identifier], goalGraphic);
                }

                document.Canvas.Add (obstacleCanvas);
            }
        }

        static void ExportIdealGoalModel (GoalModel model, KAOSTools.OmnigraffleExport.Omnigraffle.Document document)
        {
            var goalCanvas = new Omnigraffle.Sheet (1, "Ideal Goal Model");
            goalCanvas.LayoutInfo.HierarchicalOrientation = Omnigraffle.HierarchicalOrientation.BottomTop;
            mapping.Add (goalCanvas, new Dictionary<string, KAOSTools.OmnigraffleExport.Omnigraffle.ShapedGraphic> ());
            foreach (var goal in model.RootGoals) {
                RecursiveExportGoal (goalCanvas, goal);
            }

            document.Canvas.Add (goalCanvas);
        }

        static void ExportAntiGoalModel (GoalModel model, KAOSTools.OmnigraffleExport.Omnigraffle.Document document)
        {
            var goalCanvas = new Omnigraffle.Sheet (1, "Anti Goal Model");
            goalCanvas.LayoutInfo.HierarchicalOrientation = Omnigraffle.HierarchicalOrientation.BottomTop;
            mapping.Add (goalCanvas, new Dictionary<string, KAOSTools.OmnigraffleExport.Omnigraffle.ShapedGraphic> ());
            foreach (var goal in model.RootAntiGoals) {
                RecursiveExportAntiGoal (goalCanvas, goal);
            }

            document.Canvas.Add (goalCanvas);
        }

        static void ExportObstructedGoalModel (GoalModel model, KAOSTools.OmnigraffleExport.Omnigraffle.Document document)
        {
            var goalCanvas = new Omnigraffle.Sheet (1, "Obstructed Goal Model");
            goalCanvas.LayoutInfo.HierarchicalOrientation = Omnigraffle.HierarchicalOrientation.BottomTop;
            mapping.Add (goalCanvas, new Dictionary<string, KAOSTools.OmnigraffleExport.Omnigraffle.ShapedGraphic> ());
            foreach (var goal in model.RootGoals) {
                RecursiveExportGoal (goalCanvas, goal);
            }

            foreach (var goal in model.ObstructedGoals) {
                foreach (var obstacle in goal.Obstructions) {
                    if (!mapping[goalCanvas].ContainsKey(obstacle.Identifier)) {
                        AddObstacle (goalCanvas, obstacle);
                    }
                    AddSharpBackCrossArrow (goalCanvas.GraphicsList, mapping [goalCanvas] [obstacle.Identifier], mapping [goalCanvas] [goal.Identifier]);
                }
            }

            document.Canvas.Add (goalCanvas);
        }

        static void ExportExceptions (GoalModel model, KAOSTools.OmnigraffleExport.Omnigraffle.Document document)
        {
            var canvas = new Omnigraffle.Sheet (1, "Exceptions");
            canvas.LayoutInfo.HierarchicalOrientation = Omnigraffle.HierarchicalOrientation.BottomTop;
            mapping.Add (canvas, new Dictionary<string, KAOSTools.OmnigraffleExport.Omnigraffle.ShapedGraphic> ());
            foreach (var goal in model.Goals.Where (x => x.Assumptions.Count + x.Exceptions.Count > 0)) {
                AddGoal (canvas, goal);
            }

            foreach (var goal in model.Goals.Where (x => x.Assumptions.Count + x.Exceptions.Count > 0)) {
                var parentGraphic = mapping [canvas] [goal.Identifier];

                foreach (var exception in goal.Exceptions) {
                    if (exception.ResolvingGoal == null)
                        continue;

                    ShapedGraphic exceptionGoal;
                    exceptionGoal = RenderGoal (exception.ResolvingGoal);
                    canvas.GraphicsList.Add (exceptionGoal);

                    var topArrow = AddFilledArrow (canvas.GraphicsList, exceptionGoal, parentGraphic, false);
                    topArrow.Style.Stroke.HeadArrow = Arrow.None;
                    topArrow.Style.Stroke.TailArrow = Arrow.Arrow;

                    var text = "Exception " + exception.ResolvedObstacle.FriendlyName;
                    var alternativeText = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Rectangle, 50, 50, 100, 100);
                    alternativeText.Text = new Omnigraffle.TextInfo (GetRtfUnicodeEscapedString(text)) {
                        Alignement = KAOSTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center,
                        SideMargin = 0, TopBottomMargin = 0
                    };
                    alternativeText.FontInfo.Size = 10;
                    alternativeText.Style.Shadow.Draws = false;
                    alternativeText.FitText = KAOSTools.OmnigraffleExport.Omnigraffle.FitText.Vertical;
                    alternativeText.Flow = KAOSTools.OmnigraffleExport.Omnigraffle.Flow.Resize;
                    alternativeText.Style.Fill.Color = new KAOSTools.OmnigraffleExport.Omnigraffle.Color (1, 1, 1);
                    alternativeText.Style.Stroke.Draws = false;
                    alternativeText.Line = new LineInfo (topArrow.ID);
                    canvas.GraphicsList.Add (alternativeText);

                    // Ad the arrow
                    canvas.GraphicsList.Add (topArrow);
                }

                foreach (var assumption in goal.Assumptions) {
                    ShapedGraphic assumeGoal;
                    if (assumption is GoalAssumption) {
                        assumeGoal = RenderGoal ((assumption as GoalAssumption).Assumed as Goal);
                    } else if (assumption is ObstacleNegativeAssumption) {
                        assumeGoal = RenderObstacle ((assumption as ObstacleNegativeAssumption).Assumed as Obstacle);
                    } else {
                        throw new NotImplementedException ();
                    }
                    canvas.GraphicsList.Add (assumeGoal);

                    var topArrow = AddFilledArrow (canvas.GraphicsList, assumeGoal, parentGraphic, false);
                    topArrow.Style.Stroke.HeadArrow = Arrow.None;
                    topArrow.Style.Stroke.TailArrow = Arrow.Arrow;

                    var text = GetRtfUnicodeEscapedString("Assume");
                    if (assumption is ObstacleNegativeAssumption)
                        text += " \\b "+GetRtfUnicodeEscapedString("not")+"\\b0 ";

                    var alternativeText = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Rectangle, 50, 50, 100, 100);
                    alternativeText.Text = new Omnigraffle.TextInfo (text) {
                        Alignement = KAOSTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center,
                        SideMargin = 0, TopBottomMargin = 0
                    };
                    alternativeText.FontInfo.Size = 10;
                    alternativeText.Style.Shadow.Draws = false;
                    alternativeText.FitText = KAOSTools.OmnigraffleExport.Omnigraffle.FitText.Vertical;
                    alternativeText.Flow = KAOSTools.OmnigraffleExport.Omnigraffle.Flow.Resize;
                    alternativeText.Style.Fill.Color = new KAOSTools.OmnigraffleExport.Omnigraffle.Color (1, 1, 1);
                    alternativeText.Style.Stroke.Draws = false;
                    alternativeText.Line = new LineInfo (topArrow.ID);
                    canvas.GraphicsList.Add (alternativeText);

                    // Ad the arrow
                    canvas.GraphicsList.Add (topArrow);
                }
            }

            document.Canvas.Add (canvas);
        }
        
        static void ExportResolutionsGoalModel (GoalModel model, KAOSTools.OmnigraffleExport.Omnigraffle.Document document)
        {
            foreach (var obstacle in model.Obstacles.Where (o => o.Resolutions.Count > 0)) {
                foreach (var resolution in obstacle.Resolutions) {
                    var canvas = new Omnigraffle.Sheet (1, "Resolution for '" + obstacle.Name + "'");
                    canvas.LayoutInfo.HierarchicalOrientation = Omnigraffle.HierarchicalOrientation.BottomTop;
                    mapping.Add (canvas, new Dictionary<string, KAOSTools.OmnigraffleExport.Omnigraffle.ShapedGraphic> ());

                    var obstacleGraphic = AddObstacle (canvas, obstacle);
                    RecursiveExportGoal (canvas, resolution.ResolvingGoal);
                    AddSharpBackCrossArrow (canvas.GraphicsList, mapping [canvas] [resolution.ResolvingGoal.Identifier], obstacleGraphic);

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

                if (refinement.SystemReference != null) {
                    var text = string.IsNullOrEmpty (refinement.SystemReference.Name) ? refinement.SystemReference.Identifier : refinement.SystemReference.Name;

                    var alternativeText = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Rectangle, 50, 50, 100, 100);
                    alternativeText.Text = new Omnigraffle.TextInfo (GetRtfUnicodeEscapedString(text)) {
                        Alignement = KAOSTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center,
                        SideMargin = 0, TopBottomMargin = 0
                    };
                    alternativeText.FontInfo.Size = 10;
                    alternativeText.Style.Shadow.Draws = false;
                    alternativeText.FitText = KAOSTools.OmnigraffleExport.Omnigraffle.FitText.Vertical;
                    alternativeText.Flow = KAOSTools.OmnigraffleExport.Omnigraffle.Flow.Resize;
                    alternativeText.Style.Fill.Color = new KAOSTools.OmnigraffleExport.Omnigraffle.Color (1, 1, 1);
                    alternativeText.Style.Stroke.Draws = false;
                    alternativeText.Line = new LineInfo (topArrow.ID);
                    canvas.GraphicsList.Add (alternativeText);
                }

                // Ad the arrow
                canvas.GraphicsList.Add (topArrow);

                foreach (var child in refinement.Subgoals.Reverse ()) {
                    RecursiveExportGoal (canvas, child);
                    var childGraphic = mapping[canvas][child.Identifier];
                    AddLine (canvas.GraphicsList, childGraphic, circle);
                }

                foreach (var domprop in refinement.DomainProperties.Reverse ()) {
                    var childGraphic = AddDomainProperty (canvas, domprop);
                    AddLine (canvas.GraphicsList, childGraphic, circle);
                }
                
                foreach (var domhyp in refinement.DomainHypotheses.Reverse ()) {
                    var childGraphic = AddDomainHypothesis (canvas, domhyp);
                    AddLine (canvas.GraphicsList, childGraphic, circle);
                }
            }
            
            foreach (var assignment in goal.AgentAssignments) {
                AddAgentAssignements (canvas, assignment, parentGraphic);
            }
        }

        static void RecursiveExportAntiGoal (Omnigraffle.Sheet canvas, AntiGoal goal)
        {
            if (mapping[canvas].ContainsKey (goal.Identifier))
                return;

            var parentGraphic = AddAntiGoal (canvas, goal);

            foreach (var refinement in goal.Refinements.Reverse ()) {
                var circle = AddCircle (canvas.GraphicsList);

                // We add the arrow to the canvas after the label, so that label is above the arrow
                var topArrow = AddFilledArrow (canvas.GraphicsList, circle, parentGraphic, false);

                if (refinement.SystemReference != null) {
                    var text = string.IsNullOrEmpty (refinement.SystemReference.Name) ? refinement.SystemReference.Identifier : refinement.SystemReference.Name;

                    var alternativeText = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Rectangle, 50, 50, 100, 100);
                    alternativeText.Text = new Omnigraffle.TextInfo (GetRtfUnicodeEscapedString(text)) {
                        Alignement = KAOSTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center,
                        SideMargin = 0, TopBottomMargin = 0
                    };
                    alternativeText.FontInfo.Size = 10;
                    alternativeText.Style.Shadow.Draws = false;
                    alternativeText.FitText = KAOSTools.OmnigraffleExport.Omnigraffle.FitText.Vertical;
                    alternativeText.Flow = KAOSTools.OmnigraffleExport.Omnigraffle.Flow.Resize;
                    alternativeText.Style.Fill.Color = new KAOSTools.OmnigraffleExport.Omnigraffle.Color (1, 1, 1);
                    alternativeText.Style.Stroke.Draws = false;
                    alternativeText.Line = new LineInfo (topArrow.ID);
                    canvas.GraphicsList.Add (alternativeText);
                }

                // Ad the arrow
                canvas.GraphicsList.Add (topArrow);

                foreach (var child in refinement.SubAntiGoals.Reverse ()) {
                    RecursiveExportAntiGoal (canvas, child);
                    var childGraphic = mapping[canvas][child.Identifier];
                    AddLine (canvas.GraphicsList, childGraphic, circle);
                }

                foreach (var domprop in refinement.DomainProperties.Reverse ()) {
                    var childGraphic = AddDomainProperty (canvas, domprop);
                    AddLine (canvas.GraphicsList, childGraphic, circle);
                }

                foreach (var domhyp in refinement.DomainHypotheses.Reverse ()) {
                    var childGraphic = AddDomainHypothesis (canvas, domhyp);
                    AddLine (canvas.GraphicsList, childGraphic, circle);
                }

                foreach (var obstacle in refinement.Obstacles.Reverse ()) {
                    var childGraphic = AddObstacle (canvas, obstacle);
                    AddLine (canvas.GraphicsList, childGraphic, circle);
                }
            }

            foreach (var assignment in goal.AgentAssignments) {
                AddAgentAssignements (canvas, assignment, parentGraphic);
            }
        }

        static void AddAgentAssignements (Sheet canvas, AgentAssignment assignment, ShapedGraphic parentGraphic)
        {
                var circle = AddCircle (canvas.GraphicsList);
                // We add the arrow to the canvas after the label, so that label is above the arrow
                var topArrow = AddFilledArrow (canvas.GraphicsList, circle, parentGraphic, false);
                if (assignment.SystemReference != null) {
                    var text = string.IsNullOrEmpty (assignment.SystemReference.Name) ? assignment.SystemReference.Identifier : assignment.SystemReference.Name;
                    var alternativeText = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Rectangle, 50, 50, 100, 100);
                    alternativeText.Text = new Omnigraffle.TextInfo (GetRtfUnicodeEscapedString (text)) {
                        Alignement = KAOSTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center,
                        SideMargin = 0,
                        TopBottomMargin = 0
                    };
                    alternativeText.FontInfo.Size = 10;
                    alternativeText.Style.Shadow.Draws = false;
                    alternativeText.FitText = KAOSTools.OmnigraffleExport.Omnigraffle.FitText.Vertical;
                    alternativeText.Flow = KAOSTools.OmnigraffleExport.Omnigraffle.Flow.Resize;
                    alternativeText.Style.Fill.Color = new KAOSTools.OmnigraffleExport.Omnigraffle.Color (1, 1, 1);
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
        
        static void RecursiveExportObstacle (Omnigraffle.Sheet canvas, Obstacle obstacle, bool export_resolution = false)
        {
            if (!mapping[canvas].ContainsKey (obstacle.Identifier))
                AddObstacle (canvas, obstacle);
            
            var parentGraphic = mapping[canvas][obstacle.Identifier];
            
            foreach (var refinement in obstacle.Refinements) {
                var circle = AddCircle (canvas.GraphicsList);
                
                AddFilledArrow (canvas.GraphicsList, circle, parentGraphic);
                
                foreach (var child in refinement.Subobstacles) {
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
                foreach (var resolution in obstacle.Resolutions) {
                    var goal = resolution.ResolvingGoal;
                    if (!mapping[canvas].ContainsKey (goal.Identifier))
                        AddGoal (canvas, goal);

                    var goalGraphic = mapping [canvas] [goal.Identifier];
                    AddSharpBackCrossArrow (canvas.GraphicsList, goalGraphic, parentGraphic);
                }
            }
        
            foreach (var assignment in obstacle.AgentAssignments) {
                AddAgentAssignements (canvas, assignment, parentGraphic);
            }
        }

        #endregion

        static Omnigraffle.ShapedGraphic AddLineLabel (List<Graphic> canvas, Omnigraffle.LineGraphic line, string text, float position = 0.5f)
        {
            var label = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Rectangle, 50, 50, 100, 100);
            label.Text = new Omnigraffle.TextInfo (text) {
                Alignement = KAOSTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center,
                SideMargin = 0, TopBottomMargin = 0
            };
            label.FontInfo.Size = 10;
            label.Style.Shadow.Draws = false;
            label.FitText = KAOSTools.OmnigraffleExport.Omnigraffle.FitText.Yes;
            label.Wrap = false;
            label.Flow = KAOSTools.OmnigraffleExport.Omnigraffle.Flow.Resize;
            label.Style.Fill.Color = new KAOSTools.OmnigraffleExport.Omnigraffle.Color (1, 1, 1);
            label.Style.Stroke.Draws = false;
            label.Line = new LineInfo (line.ID) { Position = position };

            return label;
        }

        static Omnigraffle.ShapedGraphic AddResponsibility (List<Graphic> canvas, Omnigraffle.ShapedGraphic agentGraphic, Omnigraffle.ShapedGraphic goalGraphic, string text)
        {
            var circle = AddCircle (canvas);
            
            // We add the arrow to the canvas after the label, so that label is above the arrow
            var topArrow = AddFilledArrow (canvas, circle, goalGraphic, false);
            
            if (!string.IsNullOrWhiteSpace (text)) {
                var alternativeText = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Rectangle, 50, 50, 100, 100);
                alternativeText.Text = new Omnigraffle.TextInfo (GetRtfUnicodeEscapedString(text)) {
                    Alignement = KAOSTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center,
                    SideMargin = 0, TopBottomMargin = 0
                };
                alternativeText.FontInfo.Size = 10;
                alternativeText.Style.Shadow.Draws = false;
                alternativeText.FitText = KAOSTools.OmnigraffleExport.Omnigraffle.FitText.Vertical;
                alternativeText.Flow = KAOSTools.OmnigraffleExport.Omnigraffle.Flow.Resize;
                alternativeText.Style.Fill.Color = new KAOSTools.OmnigraffleExport.Omnigraffle.Color (1, 1, 1);
                alternativeText.Style.Stroke.Draws = false;
                alternativeText.Line = new LineInfo (topArrow.ID);
                canvas.Add (alternativeText);
            }
            
            // Ad the arrow
            canvas.Add (topArrow);

            return circle;
        }

        static Omnigraffle.ShapedGraphic AddCircle (List<Graphic> canvas)
        {
            var circle = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Circle, 50, 50, 10, 10);
            circle.Style.Shadow.Draws = false;
            
            circle.FitText = KAOSTools.OmnigraffleExport.Omnigraffle.FitText.Clip;
            circle.Flow = KAOSTools.OmnigraffleExport.Omnigraffle.Flow.Clip;
            
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
            } else if (to is Omnigraffle.Group) {
                line.Points.Add (new Point(25,25));
            }

            if (@from is ShapedGraphic) {
                line.Points.Add ((@from as ShapedGraphic).Bounds.BottomRight);
            } else if (@from is Omnigraffle.Group) {
                line.Points.Add (new Point(30,30));
            }

            line.Style.Shadow.Draws = false;

            if (add)
                canvas.Add (line);

            return line;
        }

        static Omnigraffle.LineGraphic AddArrow (List<Graphic> canvas, Omnigraffle.Graphic @from, Omnigraffle.Graphic to)
        {
            var line = AddLine (canvas, @from, to);
            line.Style.Stroke.HeadArrow = KAOSTools.OmnigraffleExport.Omnigraffle.Arrow.Arrow;
            return line;
        }
       
        static Omnigraffle.LineGraphic AddFilledArrow (List<Graphic>  canvas, Omnigraffle.Graphic @from, Omnigraffle.Graphic to, bool add = true)
        {
            var line = AddLine (canvas, @from, to, add);
            line.Style.Stroke.HeadArrow = KAOSTools.OmnigraffleExport.Omnigraffle.Arrow.FilledArrow;
            return line;
        }
        
        static Omnigraffle.LineGraphic AddSharpBackCrossArrow (List<Graphic> canvas, Omnigraffle.Graphic @from, Omnigraffle.Graphic to)
        {
            var line = AddLine (canvas, @from, to);
            line.Style.Stroke.TailArrow = KAOSTools.OmnigraffleExport.Omnigraffle.Arrow.SharpBackCross;
            return line;
        }

        static ShapedGraphic RenderAntiGoal (AntiGoal antigoal)
        {
            var graphic = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Bezier, 50, 50, 150, 70);
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
            graphic.Text = new Omnigraffle.TextInfo (GetRtfUnicodeEscapedString (antigoal.FriendlyName)) {
                Alignement = KAOSTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center,
                SideMargin = 10,
                TopBottomMargin = 3
            };
            graphic.Style.Shadow.Draws = false;
            graphic.FitText = KAOSTools.OmnigraffleExport.Omnigraffle.FitText.Vertical;
            graphic.Flow = KAOSTools.OmnigraffleExport.Omnigraffle.Flow.Resize;
            graphic.Style.Fill.Color = new KAOSTools.OmnigraffleExport.Omnigraffle.Color (0.810871, 0.896814, 1);
            if (antigoal.Refinements.Count == 0)
                graphic.Style.Stroke.Width = 2;
            return graphic;
        }

        static ShapedGraphic RenderObstacle (Obstacle obstacle)
        {
            var graphic = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Bezier, 50, 50, 150, 70);
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
            graphic.Text = new Omnigraffle.TextInfo (GetRtfUnicodeEscapedString (obstacle.FriendlyName)) {
                Alignement = KAOSTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center,
                SideMargin = 10,
                TopBottomMargin = 3
            };
            graphic.Style.Shadow.Draws = false;
            graphic.FitText = KAOSTools.OmnigraffleExport.Omnigraffle.FitText.Vertical;
            graphic.Flow = KAOSTools.OmnigraffleExport.Omnigraffle.Flow.Resize;
            graphic.Style.Fill.Color = new KAOSTools.OmnigraffleExport.Omnigraffle.Color (1, 0.590278, 0.611992);
            if (obstacle.Refinements.Count == 0)
                graphic.Style.Stroke.Width = 2;
            return graphic;
        }

        static Omnigraffle.ShapedGraphic AddObstacle (Omnigraffle.Sheet canvas, Obstacle obstacle)
        {
            var graphic = RenderObstacle (obstacle);
           
            canvas.GraphicsList.Add (graphic);
            mapping[canvas].Add (obstacle.Identifier, graphic);

            return graphic;
        }

        static Omnigraffle.ShapedGraphic AddDomainProperty (Omnigraffle.Sheet canvas, DomainProperty domprop)
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
            
            graphic.Text = new Omnigraffle.TextInfo (GetRtfUnicodeEscapedString(domprop.Name)) {
                Alignement = KAOSTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center,
                SideMargin = 10, TopBottomMargin = 3
            };
            graphic.Style.Shadow.Draws = false;
            graphic.FitText = KAOSTools.OmnigraffleExport.Omnigraffle.FitText.Vertical;
            graphic.Flow = KAOSTools.OmnigraffleExport.Omnigraffle.Flow.Resize;
            
            graphic.Style.Fill.Color = new KAOSTools.OmnigraffleExport.Omnigraffle.Color (0.895214, 1, 0.72515);
            
            canvas.GraphicsList.Add (graphic);

            return graphic;
        }

        static Omnigraffle.ShapedGraphic AddDomainHypothesis (Omnigraffle.Sheet canvas, DomainHypothesis domhyp)
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
            
            graphic.Text = new Omnigraffle.TextInfo (GetRtfUnicodeEscapedString(domhyp.Name)) {
                Alignement = KAOSTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center,
                SideMargin = 10, TopBottomMargin = 3
            };
            graphic.Style.Shadow.Draws = false;
            graphic.FitText = KAOSTools.OmnigraffleExport.Omnigraffle.FitText.Vertical;
            graphic.Flow = KAOSTools.OmnigraffleExport.Omnigraffle.Flow.Resize;
            
            graphic.Style.Fill.Color = new KAOSTools.OmnigraffleExport.Omnigraffle.Color (1, 0.92156862745, 0.92156862745);
            
            canvas.GraphicsList.Add (graphic);
            
            return graphic;
        }

        static ShapedGraphic RenderAgent (Agent agent)
        {
            var longestWord = (string.IsNullOrEmpty (agent.Name) ? agent.Identifier : agent.Name).Split (' ').Max (x => x.Length);
            var graphic = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Bezier, 50, 50, longestWord * 6 + 30, 30);
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
            graphic.Text = new Omnigraffle.TextInfo (GetRtfUnicodeEscapedString(string.IsNullOrEmpty (agent.Name) ? agent.Identifier : agent.Name)) {
                Alignement = KAOSTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center,
                SideMargin = 10, TopBottomMargin = 3
            };
            graphic.Style.Shadow.Draws = false;
            graphic.FitText = KAOSTools.OmnigraffleExport.Omnigraffle.FitText.Vertical;
            graphic.Flow = KAOSTools.OmnigraffleExport.Omnigraffle.Flow.Resize;
            if (agent.Type == AgentType.Software)
                graphic.Style.Fill.Color = new KAOSTools.OmnigraffleExport.Omnigraffle.Color (0.99607843137, 0.80392156862, 0.58039215686);
            else
                graphic.Style.Fill.Color = new KAOSTools.OmnigraffleExport.Omnigraffle.Color (0.824276, 0.670259, 1);
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

            string text = goal.FriendlyName;

            graphic.Text = new Omnigraffle.TextInfo (GetRtfUnicodeEscapedString(text)) {
                Alignement = KAOSTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center,
                SideMargin = 10,
                TopBottomMargin = 3
            };
            graphic.Style.Shadow.Draws = false;
            graphic.FitText = KAOSTools.OmnigraffleExport.Omnigraffle.FitText.Vertical;
            graphic.Flow = KAOSTools.OmnigraffleExport.Omnigraffle.Flow.Resize;

            bool assignedToEnvAgents = (
                from a in goal.AgentAssignments.SelectMany (x => x.Agents)
                where a.Type != AgentType.Software select a).Count () > 0;

            if (assignedToEnvAgents)
                graphic.Style.Fill.Color = new KAOSTools.OmnigraffleExport.Omnigraffle.Color (1, 0.979841, 0.672223);
            else
                graphic.Style.Fill.Color = new KAOSTools.OmnigraffleExport.Omnigraffle.Color (0.810871, 0.896814, 1);

            if (goal.AgentAssignments.Count > 0)
                graphic.Style.Stroke.Width = 2;

            return graphic;
        }
        
        static Omnigraffle.ShapedGraphic AddAntiGoal (Omnigraffle.Sheet canvas, AntiGoal goal)
        {
            var graphic = RenderAntiGoal (goal);

            canvas.GraphicsList.Add (graphic);
            mapping[canvas].Add (goal.Identifier, graphic);

            return graphic;
        }

        static Omnigraffle.ShapedGraphic AddGoal (Omnigraffle.Sheet canvas, Goal goal)
        {
            var graphic = RenderGoal (goal);

            canvas.GraphicsList.Add (graphic);
            mapping[canvas].Add (goal.Identifier, graphic);

            return graphic;
        }

        static ShapedGraphic RenderAttributes (Entity entity) 
        {
            var graphic = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Rectangle, 50, 50, 175, 70);
            
            string text = @"";

            text = string.Join (@"\par ", 
                                entity.Attributes.Select(attr => 
                                     GetRtfUnicodeEscapedString((attr.Derived ? "/" : "-") 
                                       + " " + attr.FriendlyName 
                                       + (attr.Type != null ? ": " + (!string.IsNullOrEmpty(attr.Type.Name) ? attr.Type.Name : attr.Type.Identifier) : ""))));

            graphic.Text = new Omnigraffle.TextInfo (text) {
                Alignement = KAOSTools.OmnigraffleExport.Omnigraffle.TextAlignement.Left,
                SideMargin = 3,
                TopBottomMargin = 3
            };
            graphic.Style.Shadow.Draws = false;
            graphic.FitText = KAOSTools.OmnigraffleExport.Omnigraffle.FitText.Vertical;
            graphic.Flow = KAOSTools.OmnigraffleExport.Omnigraffle.Flow.Resize;

            return graphic;
        }
    
        static Graphic RenderEntity (Entity entity) 
        {
            var grp = new Omnigraffle.Group (NextId);

            var header = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Rectangle, 50, 30, 175, 70);
            
            string text = @"\b\qc ";
            text += GetRtfUnicodeEscapedString(entity.FriendlyName);

            header.Text = new Omnigraffle.TextInfo (text) {
                Alignement = KAOSTools.OmnigraffleExport.Omnigraffle.TextAlignement.Left,
                SideMargin = 3,
                TopBottomMargin = 3
            };
            header.Style.Shadow.Draws = false;
            header.FitText = KAOSTools.OmnigraffleExport.Omnigraffle.FitText.Vertical;
            header.Flow = KAOSTools.OmnigraffleExport.Omnigraffle.Flow.Resize;

            if (entity.Type == EntityType.Environment)
                header.Style.Fill.Color = new KAOSTools.OmnigraffleExport.Omnigraffle.Color (0.824276, 0.670259, 1);
            else if (entity.Type == EntityType.Software)
                header.Style.Fill.Color = new KAOSTools.OmnigraffleExport.Omnigraffle.Color (0.99607843137, 0.80392156862, 0.58039215686);
            else if (entity.Type == EntityType.Shared)
                header.Style.Fill.Color = new KAOSTools.OmnigraffleExport.Omnigraffle.Color (0.895214, 1, 0.72515);

            grp.Graphics.Add (header);
            grp.Graphics.Add (RenderAttributes (entity));

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

            return grp;
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
    }
}
