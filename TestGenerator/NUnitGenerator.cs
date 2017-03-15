using System;
using System.Text;
using System.Linq;

namespace TestGenerator
{
    public class NUnitGenerator
    {
        public NUnitGenerator ()
        {
        }

        public static string Generate (string name, CounterExample cx)
        {
            var sb = new StringBuilder ();

            sb.AppendLine ("[Test ()]");
            sb.AppendLine (string.Format ("public void {0} ()", name));
            sb.AppendLine ("{");
            sb.AppendLine ("    var simulator = new Simulator ();");
            foreach (var a in cx.Items) {
                if (a is CXInput) {
                    var value = a.Variables.Single (x => x.Name == "Label").Value;
                    if (value == "tick") {
                        sb.AppendLine ("    simulator.tick();");
                    } else {
                        sb.AppendLine ("    simulator." + value + "();");
                    }
                }
            }
            sb.AppendLine ("}");

            return sb.ToString ();
        }
    }
}

