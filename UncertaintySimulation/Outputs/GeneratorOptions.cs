using System;
using System.IO;
using KAOSTools.MetaModel;

namespace UncertaintySimulation.Outputs
{
    public class GeneratorOptions
	{
        public Goal RootGoal {
            get;
            set;
        }

        public string Filename {
            get;
            set;
        }

        public GeneratorOptions ()
        {
        }
	}

}

