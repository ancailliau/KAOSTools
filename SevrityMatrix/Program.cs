using System;
using KAOSTools.Utils;
using KAOSTools.MetaModel;
using UncertaintySimulation;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SevrityMatrix
{
    class MainClass: KAOSToolCLI
    {
        public static void Main(string[] args)
        {
            Init(args);

            Console.WriteLine("Hello World!\n");
            GenerateFigByCount();
            GenerateFigByCountOBlackSpot();

            foreach (var r in model.RootGoals())
            {
                var filename_csv = r.Identifier + "_risk_matrix.csv";
                var filename_d = r.Identifier + "_risk_matrix.d";
                var filename_gd = r.Identifier + "_risk_matrix_g";
                var filename_by_count = r.Identifier + "_by_count_g";

                var file_csv = new StreamWriter(filename_csv, false);
                var file_d = new StreamWriter(filename_d, false);
                StreamWriter file_gd = null;
                StreamWriter file_by_count = null;

                file_csv.WriteLine("Probability;Severity;Risks");

                var os = r.GetObstructionSuperset(false);
                var severities = new Dictionary<ISet<KAOSMetaModelElement>, double>();
                ComputeAllSeverities(os, os.mapping.Keys.ToList(), Enumerable.Empty<KAOSMetaModelElement>(), severities);

                int currentCount = -1;
                foreach (var s in severities.OrderBy(x => x.Key.Count))
                {
                    if (currentCount < s.Key.Count)
                    {
                        if (file_gd != null)
                        {
                            file_gd.Close();
                            file_gd.Dispose();
                        }
                        currentCount = s.Key.Count;
                        file_gd = new StreamWriter(filename_gd + currentCount + ".d", false);
                        file_by_count = new StreamWriter(filename_by_count + currentCount + ".d", false);
                    }

                    if (s.Key.Count > 0)
                    {
                        var prob = s.Key.Select(x =>
                        {
                            if (x is Obstacle)
                            {
                                var eps = ((Obstacle)x).EPS;
                                if (eps > 1)
                                    throw new ArgumentException(x.FriendlyName + " has an invalid estimated satisfaction rate.");
                                return eps;
                            }
                            else if (x is DomainProperty)
                            {
                                var eps = ((DomainProperty)x).EPS;
                                if (eps > 1)
                                    throw new ArgumentException(x.FriendlyName + " has an invalid estimated satisfaction rate.");
                                return eps;
                            }
                            throw new NotImplementedException();
                        }).Aggregate((arg1, arg2) => arg1 * arg2);

                        if (prob > 0)
                        {
                            file_csv.WriteLine("{0:0.#######};{1:0.#######};{2};{3}", prob, s.Value, string.Join(",", s.Key.Select(x => x.Identifier)), s.Key.Count);
                            file_d.WriteLine("{0:0.#######} {1:0.#######}", prob, s.Value);


                            file_gd.WriteLine("{0:0.#######} {1:0.#######}", prob, s.Value);
                            file_by_count.WriteLine("{0} {1:0.#######}", s.Key.Count, s.Value);
                        }
                    }
                }

                file_csv.Close();
                file_d.Close();
                file_csv.Dispose();
                file_d.Dispose();

                file_gd.Close();
                file_gd.Dispose();
            }
        }

        static void GenerateFigByCount()
        {
            var fd2 = new StreamWriter("fig49/data.d", false);
            foreach (var rootGoal in model.Goals(g => g.Identifier == "m_water_ph"))
            {
                var os = rootGoal.GetObstructionSuperset(false);
                var severities = new Dictionary<ISet<KAOSMetaModelElement>, double>();
                ComputeAllSeverities(os, os.mapping.Keys.ToList(), Enumerable.Empty<KAOSMetaModelElement>(), severities);

                var subsets = severities.OrderBy(kv => kv.Key.Count);
                var pendingSubset = new Queue<KeyValuePair<ISet<KAOSMetaModelElement>, double>>(subsets.Where(ss => ss.Key.Count == 1));
                var visitedSubset = new HashSet<KeyValuePair<ISet<KAOSMetaModelElement>, double>>(subsets.Where(ss => ss.Key.Count == 1));

                int i = 0;

                while (pendingSubset.Count > 0) {
                    var s = pendingSubset.Dequeue();

                    if (s.Key.Any(x => x.Identifier == "o_blackspot"))
                        continue;
                    fd2.WriteLine("{0} {1:0.#######}", s.Key.Count, s.Value);
                    
                    foreach (var s2 in subsets.Where(ss => ss.Key.IsSupersetOf(s.Key) & ss.Key.Count == s.Key.Count + 1))
                    {
                        if (s2.Key.Count > 5)
                            continue;

                        if (s2.Key.Any(x => x.Identifier == "o_blackspot"))
                            continue;
                        
                        if (!visitedSubset.Contains(s2))
                        {
                            visitedSubset.Add(s2);
                            pendingSubset.Enqueue(s2);
                        }

                        var fd = new StreamWriter("fig49/data" + (i++) + ".d", false);
                        fd.WriteLine("{0} {1:0.#######}", s.Key.Count, s.Value);
                        fd.WriteLine("{0} {1:0.#######}", s2.Key.Count, s2.Value);
                        fd.Close();
                        fd.Dispose();
                    }


                    //}
                }

                Console.WriteLine("Max data: " + i);
            }
            fd2.Close();
            fd2.Dispose();
        }

        static void GenerateFigByCountOBlackSpot()
        {
            var fd2 = new StreamWriter("fig49/data_b.d", false);
            foreach (var rootGoal in model.Goals(g => g.Identifier == "m_water_ph"))
            {
                var os = rootGoal.GetObstructionSuperset(false);
                var severities = new Dictionary<ISet<KAOSMetaModelElement>, double>();
                ComputeAllSeverities(os, os.mapping.Keys.ToList(), Enumerable.Empty<KAOSMetaModelElement>(), severities);

                var subsets = severities.OrderBy(kv => kv.Key.Count);
                var pendingSubset = new Queue<KeyValuePair<ISet<KAOSMetaModelElement>, double>>(subsets.Where(ss => ss.Key.Count == 1 && ss.Key.Contains(model.Obstacle(a => a.Identifier == "o_blackspot"))));
                                                                                                var inQueue = new HashSet<KeyValuePair<ISet<KAOSMetaModelElement>, double>>(pendingSubset);

                int i = 0;

                while (pendingSubset.Count > 0)
                {
                    var s = pendingSubset.Dequeue();
                    fd2.WriteLine("{0} {1:0.#######}", s.Key.Count, s.Value);
                    foreach (var s2 in subsets.Where(ss => ss.Key.IsSupersetOf(s.Key) & ss.Key.Count == s.Key.Count + 1))
                    {
                        if (s2.Key.Count > 5)
                            continue;

                        if (!inQueue.Contains(s2))
                        {
                            inQueue.Add(s2);
                            pendingSubset.Enqueue(s2);
                        }


                        var fd = new StreamWriter("fig49/data_b_" + (i++) + ".d", false);
                        fd.WriteLine("{0} {1:0.#######}", s.Key.Count, s.Value);
                        fd.WriteLine("{0} {1:0.#######}", s2.Key.Count, s2.Value);
                        fd.Close();
                        fd.Dispose();
                    }


                    //}
                }

                Console.WriteLine("Max data_b: " + i);
                fd2.Close();
                fd2.Dispose();
            }
        }

        static void ComputeAllSeverities(ObstructionSuperset os, IEnumerable<KAOSMetaModelElement> pending, IEnumerable<KAOSMetaModelElement> inSet, Dictionary<ISet<KAOSMetaModelElement>, double> severities)
        {
            if (pending.Count() == 0)
            {
                var samplingVector = new Dictionary<int, double>();
                foreach (var o in inSet)
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
                severities.Add(new HashSet<KAOSMetaModelElement>(inSet), 1 - os.GetProbability(samplingVector));
            }
            else
            {
                var first = pending.First();
                var tail = pending.Skip(1);

                ComputeAllSeverities(os, tail, inSet.Union(new[] { first }), severities);
                ComputeAllSeverities(os, tail, inSet, severities);
            }
        }
   }
}
