using System;

namespace KAOSFormalTools.Parsing
{
    public static class Helpers
    {
        public static void Merge (this KAOSFormalTools.Domain.Obstacle o1, KAOSFormalTools.Domain.Obstacle o2)
        {
            if (o1.Identifier == null)
                o1.Identifier = o2.Identifier;

            if (o1.Name == null)
                o1.Name = o2.Name;

            if (o1.FormalSpec == null)
                o1.FormalSpec = o2.FormalSpec;
            else if (o2.FormalSpec != null)
                Console.WriteLine ("Try to override 'FormalSpec' for obstacle '{0}'", o1.Name);

            foreach (var r in o2.Refinements)
                if (!o1.Refinements.Contains (r))
                    o1.Refinements.Add (r);
        }
    }
}

