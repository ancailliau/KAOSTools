using System;
using System.Text.RegularExpressions;
using KAOSTools.MetaModel;

namespace TestGenerator
{
    public static class CommonExtensions
    {
        public static string LTSAName (this Goal goal)
        {
            var name = goal.FriendlyName;
            Regex r = new Regex (@"[\[\]\(\) ]+");
            return r.Replace (name, "_");
        }
    }
}

