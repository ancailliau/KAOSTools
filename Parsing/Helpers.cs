using System;
using KAOSTools.Core;

namespace KAOSTools.Parsing
{
    public static class Helpers
    {
        public static KAOSCoreElement OverrideKeys (this KAOSCoreElement o1, KAOSCoreElement o2) {
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
    }
}

