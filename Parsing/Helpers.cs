using System;
using KAOSTools.MetaModel;

namespace KAOSTools.Parsing
{
    public static class Helpers
    {
        public static KAOSMetaModelElement OverrideKeys (this KAOSMetaModelElement o1, KAOSMetaModelElement o2) {
            if (string.IsNullOrEmpty (o1.Identifier))
                o1.Identifier = o2.Identifier;

            if (o1.GetType().GetProperty ("Name") != null 
                && o2.GetType().GetProperty ("Name") != null
                && string.IsNullOrEmpty (o1.GetType ().GetProperty ("Name").GetValue (o1, null) as String)) 
                o1.GetType().GetProperty ("Name").SetValue (o1, o2.GetType().GetProperty ("Name").GetValue (o2, null), null);

            if (o1.GetType().GetProperty ("Signature") != null 
                && o2.GetType().GetProperty ("Signature") != null
                && string.IsNullOrEmpty (o1.GetType ().GetProperty ("Signature").GetValue (o1, null) as String)) 
                o1.GetType().GetProperty ("Signature").SetValue (o1, o2.GetType().GetProperty ("Signature").GetValue (o2, null), null);
            
            return o1;
        }

        public static void Merge (this KAOSTools.MetaModel.Predicate o1, KAOSTools.MetaModel.Predicate o2)
        {
            if (string.IsNullOrEmpty (o1.Identifier))
                o1.Identifier = o2.Identifier;
            
            if (string.IsNullOrEmpty (o1.Name))
                o1.Name = o2.Name;
            
            if (string.IsNullOrEmpty (o1.Definition))
                o1.Definition = o2.Definition;
            
            if (o1.FormalSpec == null)
                o1.FormalSpec = o2.FormalSpec;
            
            if (string.IsNullOrEmpty (o1.Signature))
                o1.Signature = o2.Signature;
        }

        public static void Merge (this KAOSTools.MetaModel.GivenType o1, KAOSTools.MetaModel.GivenType o2)
        {
            if (string.IsNullOrEmpty (o1.Identifier))
                o1.Identifier = o2.Identifier;
            
            if (string.IsNullOrEmpty (o1.Name))
                o1.Name = o2.Name;
            
            if (string.IsNullOrEmpty (o1.Definition))
                o1.Definition= o2.Definition;
        }

        public static KAOSTools.MetaModel.Entity Merge (this KAOSTools.MetaModel.Entity o1, KAOSTools.MetaModel.Entity o2)
        {
            if (string.IsNullOrEmpty (o1.Identifier))
                o1.Identifier = o2.Identifier;
            
            if (string.IsNullOrEmpty (o1.Name))
                o1.Name = o2.Name;
            
            if (string.IsNullOrEmpty (o1.Definition))
                o1.Definition= o2.Definition;
                        
            foreach (var r in o2.Attributes)
                if (!o1.Attributes.Contains (r))
                    o1.Attributes.Add (r);
            
            foreach (var r in o2.Parents)
                if (!o1.Parents.Contains (r))
                    o1.Parents.Add (r);

            return o1;
        }

        public static void Merge (this KAOSTools.MetaModel.Relation o1, KAOSTools.MetaModel.Relation o2)
        {
            if (string.IsNullOrEmpty (o1.Identifier))
                o1.Identifier = o2.Identifier;
            
            if (string.IsNullOrEmpty (o1.Name))
                o1.Name = o2.Name;
            
            if (string.IsNullOrEmpty (o1.Definition))
                o1.Definition= o2.Definition;
            
            foreach (var r in o2.Attributes)
                if (!o1.Attributes.Contains (r))
                    o1.Attributes.Add (r);
            
            foreach (var r in o2.Links)
                if (!o1.Links.Contains (r))
                    o1.Links.Add (r);
        }

        public static void Merge (this KAOSTools.MetaModel.AlternativeSystem o1, KAOSTools.MetaModel.AlternativeSystem o2)
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

            foreach (var r in g2.AgentAssignments)
                if (!g1.AgentAssignments.Contains (r))
                    g1.AgentAssignments.Add (r);
            
            foreach (var r in g2.Obstructions)
                if (!g1.Obstructions.Contains (r))
                    g1.Obstructions.Add (r);
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

