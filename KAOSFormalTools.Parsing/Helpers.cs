using System;

namespace KAOSFormalTools.Parsing
{
    public static class Helpers
    {
        public static void Merge (this KAOSFormalTools.Domain.Obstacle o1, KAOSFormalTools.Domain.Obstacle o2)
        {
            if (string.IsNullOrEmpty (o1.Identifier))
                o1.Identifier = o2.Identifier;

            if (string.IsNullOrEmpty (o1.Name))
                o1.Name = o2.Name;
            
            if (string.IsNullOrEmpty (o1.Definition))
                o1.Definition= o2.Definition;

            if (o1.FormalSpec == null)
                o1.FormalSpec = o2.FormalSpec;

            foreach (var r in o2.Refinements)
                if (!o1.Refinements.Contains (r))
                    o1.Refinements.Add (r);
            
            foreach (var r in o2.Resolutions)
                if (!o1.Resolutions.Contains (r))
                    o1.Resolutions.Add (r);
        }

        public static void Merge (this KAOSFormalTools.Domain.Goal g1, KAOSFormalTools.Domain.Goal g2)
        {
            if (string.IsNullOrEmpty (g1.Identifier))
                g1.Identifier = g2.Identifier;
            
            if (string.IsNullOrEmpty (g1.Name))
                g1.Name = g2.Name;
            
            if (string.IsNullOrEmpty (g1.Definition))
                g1.Definition = g2.Definition;

            if (g1.FormalSpec == null)
                g1.FormalSpec = g2.FormalSpec;

            foreach (var r in g2.Refinements)
                if (!g1.Refinements.Contains (r))
                    g1.Refinements.Add (r);

            foreach (var r in g2.AssignedAgents)
                if (!g1.AssignedAgents.Contains (r))
                    g1.AssignedAgents.Add (r);
            
            foreach (var r in g2.Obstruction)
                if (!g1.Obstruction.Contains (r))
                    g1.Obstruction.Add (r);
        }

        public static void Merge (this KAOSFormalTools.Domain.DomainProperty d1, KAOSFormalTools.Domain.DomainProperty d2)
        {
            if (string.IsNullOrEmpty (d1.Identifier))
                d1.Identifier = d2.Identifier;
            
            if (string.IsNullOrEmpty (d1.Name))
                d1.Name = d2.Name;
            
            if (string.IsNullOrEmpty (d1.Definition))
                d1.Definition= d2.Definition;
            
            if (d1.FormalSpec == null)
                d1.FormalSpec = d2.FormalSpec;
        }

    }
}

