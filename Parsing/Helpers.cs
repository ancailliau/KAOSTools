using System;

namespace KAOSTools.Parsing
{
    public static class Helpers
    {
        public static void Merge (this KAOSTools.MetaModel.System o1, KAOSTools.MetaModel.System o2)
        {
            if (string.IsNullOrEmpty (o1.Identifier))
                o1.Identifier = o2.Identifier;
            
            if (string.IsNullOrEmpty (o1.Name))
                o1.Name = o2.Name;
            
            if (string.IsNullOrEmpty (o1.Description))
                o1.Description= o2.Description;

            foreach (var r in o2.Alternatives)
                if (!o1.Alternatives.Contains (r))
                    o1.Alternatives.Add (r);
        }

        public static void Merge (this KAOSTools.MetaModel.Obstacle o1, KAOSTools.MetaModel.Obstacle o2)
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

        public static void Merge (this KAOSTools.MetaModel.Goal g1, KAOSTools.MetaModel.Goal g2)
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

        public static void Merge (this KAOSTools.MetaModel.DomainProperty d1, KAOSTools.MetaModel.DomainProperty d2)
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
        
        public static void Merge (this KAOSTools.MetaModel.DomainHypothesis d1, KAOSTools.MetaModel.DomainHypothesis d2)
        {
            if (string.IsNullOrEmpty (d1.Identifier))
                d1.Identifier = d2.Identifier;
            
            if (string.IsNullOrEmpty (d1.Name))
                d1.Name = d2.Name;
            
            if (string.IsNullOrEmpty (d1.Definition))
                d1.Definition= d2.Definition;
        }
        
        public static void Merge (this KAOSTools.MetaModel.Agent a1, KAOSTools.MetaModel.Agent a2)
        {
            if (string.IsNullOrEmpty (a1.Identifier))
                a1.Identifier = a2.Identifier;
            
            if (string.IsNullOrEmpty (a1.Name))
                a1.Name = a2.Name;
            
            if (string.IsNullOrEmpty (a1.Description))
                a1.Description = a2.Description;
        }
    }
}

