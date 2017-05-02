using System;
using KAOSTools.Core;
using System.IO;
using System.Linq;

namespace UCLouvain.KAOSTools.Parsing
{
    public class GoalModelExport
    {
        StreamWriter writer;
        KAOSModel _model;
    
        public GoalModelExport (KAOSModel model)
        {
            _model = model;
        }
        
        public void Export (string filename)
        {
            writer = new StreamWriter (filename);
            foreach (var g in _model.Goals ())
                ExportGoal (g);
                
            foreach (var g in _model.Obstacles ())
                Export (g);
                
            writer.Close ();
        }
        
        public void ExportGoal (Goal goal) 
        {
            writer.WriteLine ($"declare goal [ {goal.Identifier} ]");
            if (!string.IsNullOrWhiteSpace (goal.Name))
                writer.WriteLine ($"\tname \"{goal.Name}\"");
            foreach (var refinement in goal.Refinements()) {
                string refinees = string.Join (", ", refinement.SubGoalIdentifiers.Select(x => x.Identifier));
                writer.WriteLine ($"\trefinedby {refinees}");
            }
            foreach (var obstruction in goal.Obstructions()) {
                writer.WriteLine ($"\tobstructedby {obstruction.ObstacleIdentifier}");
            }
            writer.WriteLine ($"end\n");
        }
        
        public void Export (Obstacle obstacle)
        {
            writer.WriteLine ($"declare obstacle [ {obstacle.Identifier} ]");
            if (!string.IsNullOrWhiteSpace (obstacle.Name))
                writer.WriteLine ($"\tname \"{obstacle.Name}\"");
            foreach (var refinement in obstacle.Refinements()) {
                string refinees = string.Join (", ", refinement.SubobstacleIdentifiers.Select(x => x.Identifier));
                writer.WriteLine ($"\trefinedby {refinees}");
            }
            foreach (var resolution in obstacle.Resolutions()) {
                writer.WriteLine ($"\tresolvedby {resolution.ResolvingGoalIdentifier}");
            }
            writer.WriteLine ($"end\n");
        }
    }
}
