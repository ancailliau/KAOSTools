using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace KAOSTools.Core
{

    #region Goal Model

    #region Meta entities

    #endregion

    #region Assignements

    [DataContract]
    public class GoalAgentAssignment : AgentAssignment {
        
        [DataMember]
        public string GoalIdentifier { get; set ; }
        public GoalAgentAssignment  (KAOSModel model) : base (model) {}

        public override KAOSCoreElement Copy ()
        {
            return new GoalAgentAssignment (null) {
                Identifier = Identifier,
                Implicit = Implicit,
                GoalIdentifier = GoalIdentifier,
                AgentIdentifiers = new List<string> (AgentIdentifiers)
            };
        }
    }

    #endregion

    #region Refinements

    #endregion

    #region Obstructions and resolutions

    #endregion

    #region Exceptions and assumptions

    #endregion

    #endregion

    #region Object Model

    #endregion
    
}
