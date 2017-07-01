using System;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Utils;
using UCLouvain.KAOSTools.Core.SatisfactionRates;
using UCLouvain.KAOSTools.Propagators;
using UCLouvain.KAOSTools.Propagators.BDD;

namespace UCLouvain.KAOSTools.Utils.Propagator
{
    class PropagatorProgram : KAOSToolCLI
    {
        public static void Main (string [] args)
        {
            Console.WriteLine ("*** This is Propagator from KAOSTools. ***");
            Console.WriteLine ("*** For more information on KAOSTools see <https://github.com/ancailliau/KAOSTools> ***");
            Console.WriteLine ("*** Please report bugs to <https://github.com/ancailliau/KAOSTools/issues> ***");
            Console.WriteLine ();
            Console.WriteLine ("*** Copyright (c) 2017, Université catholique de Louvain ***");
            Console.WriteLine ("");

            string rootname = "root";
            options.Add ("root=", "Specify the root goal for which to compute the satisfaction rate. (Default: root)", v => rootname = v);

            bool bdd = true;
            options.Add ("bdd", "Uses the BDD-Based propagation (Default)", v => bdd = true);
            options.Add ("pattern", "Uses the Pattern-Based propagation", v => bdd = false);
            
            Init (args);
            
            var root = model.Goal (rootname);
            if (root == null) {
                PrintError ("The goal '"+rootname+"' was not found");
            }

            try {
                IPropagator _propagator;
                if (bdd) {
                    _propagator = new BDDBasedPropagator (model);
                } else {
                    _propagator = new PatternBasedPropagator (model);
                }

                ISatisfactionRate esr = _propagator.GetESR (root);
                DoubleSatisfactionRate esrd = (DoubleSatisfactionRate)esr;
            
                Console.WriteLine (root.FriendlyName + ": {0:P}", esrd.SatisfactionRate);
            
            } catch (Exception e) {
                PrintError ("An error occured during the computation. ("+e.Message+").\n"
                +"Please report this error to <https://github.com/ancailliau/KAOSTools/issues>.\n"
                +"----------------------------\n"
                +e.StackTrace
                +"\n----------------------------\n");
                
            }
        }
    }
}
