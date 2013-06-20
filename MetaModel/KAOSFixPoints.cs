using System;
using System.Linq;
using KAOSTools.MetaModel;

namespace KAOSTools.MetaModel
{
    public static class KAOSFixPoints
    {
        public static void CompleteGoalAgentAssignments (this KAOSView view) {
            foreach (var aa in view.GoalAgentAssignments()) {
                view.Add (view.ParentModel.Elements.Single (x => x.Identifier == aa.GoalIdentifier));
                foreach (var agentIdentifier in aa.AgentIdentifiers) {
                    view.Add (view.ParentModel.Elements.Single (x => x.Identifier == agentIdentifier));
                }
            }
        }

        public static void CompleteObstacleAgentAssignments (this KAOSView view) {
            foreach (var aa in view.ObstacleAgentAssignments()) {
                view.Add (view.ParentModel.Elements.Single (x => x.Identifier == aa.ObstacleIdentifier));
                foreach (var agentIdentifier in aa.AgentIdentifiers) {
                    view.Add (view.ParentModel.Elements.Single (x => x.Identifier == agentIdentifier));
                }
            }
        }

        public static void CompleteAntiGoalAgentAssignments (this KAOSView view) {
            foreach (var aa in view.AntiGoalAgentAssignments()) {
                view.Add (view.ParentModel.Elements.Single (x => x.Identifier == aa.AntiGoalIdentifier));
                foreach (var agentIdentifier in aa.AgentIdentifiers) {
                    view.Add (view.ParentModel.Elements.Single (x => x.Identifier == agentIdentifier));
                }
            }
        }
    }
}

