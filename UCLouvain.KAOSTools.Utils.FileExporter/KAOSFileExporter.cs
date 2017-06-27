using System;
using UCLouvain.KAOSTools.Core;
using System.Text;
using System.Linq;
using UCLouvain.KAOSTools.Core.SatisfactionRates;

namespace UCLouvain.KAOSTools.Utils.FileExporter
{
    public class KAOSFileExporter
    {
        KAOSModel _model;
    
        public KAOSFileExporter (KAOSModel model)
        {
            _model = model;
        }
        
        public string Export ()
        {
            StringBuilder builder = new StringBuilder ();
            ExportGoals (builder);
            ExportObstacles (builder);
            return builder.ToString ();
        }

        void ExportGoals (StringBuilder builder)
        {
            builder.AppendLine ("\n# Goals\n");
            foreach (var g in _model.Goals ()) {
                ExportGoal (builder, g);
            }
        }

        void ExportObstacles (StringBuilder builder)
        {
            builder.AppendLine ("\n# Obstacles\n");
            foreach (var g in _model.Obstacles ()) {
                ExportObstacle (builder, g);
            }
        }

        void ExportGoal (StringBuilder builder, Goal g)
        {
            builder.AppendLine (string.Format ("declare goal [ {0} ]", g.Identifier));
            if (!string.IsNullOrEmpty (g.Name))
                builder.AppendLine (string.Format ("\tname \"{0}\"", g.Name));
                
            if (g.FormalSpec != null) 
                builder.AppendLine (string.Format ("\tformalspec {0}", g.FormalSpec));
            
            foreach (var r in g.Refinements ()) {
                builder.AppendLine (string.Format ("\trefinedby {0}", string.Join (",", r.SubGoalIdentifiers.Select (x => x.Identifier + ToString (x.Parameters)))));
            }
            foreach (var r in g.AgentAssignments ()) {
                builder.AppendLine (string.Format ("\tassignedto {0}", string.Join (",", r.AgentIdentifiers)));
            }
            foreach (var r in g.Obstructions ()) {
                builder.AppendLine (string.Format ("\tobstructedby {0}", string.Join (",", r.ObstacleIdentifier)));
            }
            builder.AppendLine ("end");
        }

        void ExportObstacle (StringBuilder builder, Obstacle g)
        {
            builder.AppendLine (string.Format ("declare obstacle [ {0} ]", g.Identifier));
            
            if (!string.IsNullOrEmpty (g.Name))
                builder.AppendLine (string.Format ("\tname \"{0}\"", g.Name));
                
            if (g.FormalSpec != null) 
                builder.AppendLine (string.Format ("\tformalspec {0}", g.FormalSpec));
            
            foreach (var r in g.Refinements ()) {
                builder.AppendLine (string.Format ("\trefinedby {0}", string.Join (",", r.SubobstacleIdentifiers.Select (x => x.Identifier + ToString (x.Parameters)))));
            }
            foreach (var r in g.Resolutions ()) {
                builder.AppendLine (string.Format ("\tresolvedby{0} {1}", ToString (r.ResolutionPattern, r.AnchorIdentifier), string.Join (",", r.ResolvingGoalIdentifier)));
            }
            if (_model.satisfactionRateRepository.ObstacleSatisfactionRateExists (g.Identifier)) {
                Core.SatisfactionRates.ISatisfactionRate satisfactionRate = _model.satisfactionRateRepository.GetObstacleSatisfactionRate (g.Identifier);
                builder.AppendLine (string.Format ("\tprobability {0}", ToString(satisfactionRate)));
            }
            builder.AppendLine ("end");
        }
        
        string ToString (ISatisfactionRate satRate)
        {
            if (satRate is DoubleSatisfactionRate d) {
                return d.SatisfactionRate.ToString ();
            } else {
                throw new NotImplementedException ();
            }
        }

        string ToString (ResolutionPattern resolutionPattern, string anchorIdentifier)
        {
            if (resolutionPattern != ResolutionPattern.None) {
                string pattern = "";
                switch (resolutionPattern) {
                case ResolutionPattern.GoalRestoration: pattern = "restoration"; break;
                case ResolutionPattern.GoalSubstitution: pattern = "substitution"; break;
                case ResolutionPattern.GoalWeakening: pattern = "weakening"; break;
                case ResolutionPattern.ObstacleMitigation: pattern = "mitigation"; break;
                case ResolutionPattern.ObstaclePrevention: pattern = "prevention"; break;
                case ResolutionPattern.ObstacleReduction: pattern = "obstacle_reduction"; break;
                case ResolutionPattern.ObstacleStrongMitigation: pattern = "strong_mitigation"; break;
                case ResolutionPattern.ObstacleWeakMitigation: pattern = "weak_mitigation"; break;
                default:
                    break;
                }
                return string.Format (" [{0}:{1}]", pattern, anchorIdentifier);
            }
            return "";
        }

        string ToString (IRefineeParameter param) 
        {
            if (param != null)
                return "[" + param.ToString () + "]";
            else
                return "";
        }
    }
}
