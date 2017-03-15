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
    public abstract class AgentAssignment : KAOSCoreElement
    {
        public AlternativeSystem SystemReference { get; set; }

        [DataMember]
        public IList<string> AgentIdentifiers { get; set; }

        public bool IsEmpty {
            get {
                return AgentIdentifiers.Count == 0;
            }
        }

        public AgentAssignment (KAOSModel model) : base (model)
        {
            AgentIdentifiers = new List<string> ();
        }

        public void Add (Agent agent)
        {
            this.AgentIdentifiers.Add (agent.Identifier);
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