using System;
using System.IO;
using KAOSFormalTools.Domain;
using System.Text.RegularExpressions;

namespace KAOSFormalTools.RefinementChecker
{
    public static class NuSMVExtensions
    {
        public static void WriteNuSMVModel (this GoalModel model, string filename)
        {
            var generator = new ProofObligationGenerator (model);

            var streamWriter = new StreamWriter (filename);

            streamWriter.WriteLine ("MODULE main");
            streamWriter.WriteLine ();
            streamWriter.WriteLine ("  VAR");
            foreach (var item in model.GetAlphabet ()) {
                streamWriter.WriteLine ("    {0} : boolean;", item);
            }

            streamWriter.WriteLine ();
            foreach (var proofObligation in generator.Obligations) {
                streamWriter.WriteLine ("  -- Expected result : {0} (otherwise, {1})", 
                                        proofObligation.ExpectedResult, 
                                        proofObligation.FailureMessage);
                streamWriter.WriteLine ("  LTLSPEC");
                streamWriter.WriteLine ("    {0}", proofObligation.Formula.ToString ());
                streamWriter.WriteLine ();
            }

            streamWriter.Close ();
        }

        public static void InterpretNuSMVOutput (this GoalModel model, string filename)
        {
            var generator = new ProofObligationGenerator (model);

            string[] lines = File.ReadAllLines(filename);

            var      proofObligations     = generator.Obligations;
            int      proofObligationIndex = 0; 
            
            foreach (var line in lines) {
                if (line.StartsWith ("-- specification")) {
                    var regex = new Regex (@"-- specification (.+) is (true|false)");
                    var match = regex.Match (line);

                    if (match.Success) {
                        var result = bool.Parse (match.Groups[2].Value);
                        var proofObligation = proofObligations [proofObligationIndex];

                        if (result == proofObligation.ExpectedResult) {
                            Console.BackgroundColor = ConsoleColor.Green;
                            Console.Write (" OK ");
                            Console.ResetColor ();
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write (" " + proofObligation.SuccessMessage);
                            Console.ResetColor ();
                            Console.WriteLine ();

                        } else {
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write (" KO ");
                            Console.ResetColor ();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write (" " + proofObligation.FailureMessage);
                            Console.ResetColor ();
                            Console.WriteLine ();
                        }
                    }
                    proofObligationIndex++;
                }
            }
        }
    }
}

