using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using LtlSharp.Utils;

namespace KAOSFormalTools.Domain
{
    public class GoalModel
    {
        public IList<Goal>           RootGoals         { get; private set; }
        public IList<DomainProperty> DomainProperties  { get; private set; }

        public GoalModel ()
        {
            RootGoals = new List<Goal> ();
            DomainProperties = new List<DomainProperty> ();
        }
    }
}

