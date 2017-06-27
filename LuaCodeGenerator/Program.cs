using System;
using System.Collections.Generic;
using System.IO;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Utils;
using UncertaintySimulation;

namespace LuaCodeGenerator
{
    class MainClass : KAOSToolCLI
    {
        public static void Main(string[] args)
        {
            Init(args);

            Console.WriteLine("Hello World!\n");

            var filename = "satisfaction_rate_lua.tex";
            var file = new StreamWriter(filename, false);

            file.WriteLine("\\startluacode");
            foreach (var r in model.Goals())
            {   
                var os = r.GetObstructionSuperset(false);
                var severities = new Dictionary<ISet<KAOSCoreElement>, double>();

                var samplingVector = new Dictionary<int, double>();
                foreach (var o in os.mapping.Keys)
                {
                    if (o is Obstacle)
                    {
                        samplingVector.Add(os.mapping[o], ((Obstacle)o).EPS);
                    }
                    else if (o is DomainProperty)
                    {
                        samplingVector.Add(os.mapping[o], ((DomainProperty)o).EPS);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }

                file.WriteLine("sr_{0} = {1:0.########}", r.Identifier, 1 - os.GetProbability(samplingVector));
            }

            file.WriteLine();

            foreach (var r in model.Obstacles())
            {
                var os = r.GetObstructionSuperset();
                var severities = new Dictionary<ISet<KAOSCoreElement>, double>();

                var samplingVector = new Dictionary<int, double>();
                foreach (var o in os.mapping.Keys)
                {
                    if (o is Obstacle)
                    {
                        samplingVector.Add(os.mapping[o], ((Obstacle)o).EPS);
                    }
                    else if (o is DomainProperty)
                    {
                        samplingVector.Add(os.mapping[o], ((DomainProperty)o).EPS);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }

                file.WriteLine("sr_{0} = {1:0.########}", r.Identifier, os.GetProbability(samplingVector));
            }

            file.WriteLine("\\stopluacode");
            file.Close();
            file.Dispose();
        }
    }
}
