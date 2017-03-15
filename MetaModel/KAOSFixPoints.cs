using System;
using System.Linq;
using KAOSTools.Core;

namespace KAOSTools.Core
{
    public static class KAOSFixPoints
    {
        #region Agent Assignments 

        public static void CompleteGoalAgentAssignments (this KAOSView view) {
            foreach (var aa in view.GoalAgentAssignments().ToArray()) {
                view.Add (view.ParentModel.Elements.Single (x => x.Identifier == aa.GoalIdentifier));
                foreach (var agentIdentifier in aa.AgentIdentifiers) {
                    view.Add (view.ParentModel.Elements.Single (x => x.Identifier == agentIdentifier));
                }
            }
        }

        public static void CompleteObstacleAgentAssignments (this KAOSView view) {
            foreach (var aa in view.ObstacleAgentAssignments().ToArray()) {
                view.Add (view.ParentModel.Elements.Single (x => x.Identifier == aa.ObstacleIdentifier));
                foreach (var agentIdentifier in aa.AgentIdentifiers) {
                    view.Add (view.ParentModel.Elements.Single (x => x.Identifier == agentIdentifier));
                }
            }
        }

        public static void CompleteAntiGoalAgentAssignments (this KAOSView view) {
            foreach (var aa in view.AntiGoalAgentAssignments().ToArray()) {
                view.Add (view.ParentModel.Elements.Single (x => x.Identifier == aa.AntiGoalIdentifier));
                foreach (var agentIdentifier in aa.AgentIdentifiers) {
                    view.Add (view.ParentModel.Elements.Single (x => x.Identifier == agentIdentifier));
                }
            }
        }

        #endregion

        #region Refinements 

        public static void CompleteGoalRefinements (this KAOSView view) {
            foreach (var refinement in view.GoalRefinements().ToArray()) {
                view.Add (view.ParentModel.Elements.Single (x => x.Identifier == refinement.ParentGoalIdentifier));

                if (refinement.SystemReferenceIdentifier != null)
                    view.Add (view.ParentModel.Elements.Single (x => x.Identifier == refinement.SystemReferenceIdentifier));

                foreach (var subgoal in refinement.SubGoalIdentifiers) {
                    view.Add (view.ParentModel.Elements.Single (x => x.Identifier == subgoal));
                }

                foreach (var domprop in refinement.DomainPropertyIdentifiers) {
                    view.Add (view.ParentModel.Elements.Single (x => x.Identifier == domprop));
                }
                
                foreach (var domhyp in refinement.DomainHypothesisIdentifiers) {
                    view.Add (view.ParentModel.Elements.Single (x => x.Identifier == domhyp));
                }
            }
        }

        #endregion

        #region Resolution
        
        public static void CompleteResolution (this KAOSView view) {
            foreach (var resolution in view.Resolutions().ToArray()) {
                view.Add (view.ParentModel.Elements.Single (x => x.Identifier == resolution.ResolvingGoalIdentifier));
                view.Add (view.ParentModel.Elements.Single (x => x.Identifier == resolution.ObstacleIdentifier));
            }
        }

        #endregion

        #region Obstruction
        
        public static void CompleteObstruction (this KAOSView view) {
            foreach (var resolution in view.Obstructions().ToArray()) {
                view.Add (view.ParentModel.Elements.Single (x => x.Identifier == resolution.ObstructedGoalIdentifier));
                view.Add (view.ParentModel.Elements.Single (x => x.Identifier == resolution.ObstacleIdentifier));
            }
        }

        #endregion
    }
}

