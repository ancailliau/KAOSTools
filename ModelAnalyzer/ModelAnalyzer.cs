using System;
using System.Linq;
using System.Collections.Generic;
using KAOSTools.Utils;
using KAOSTools.Parsing;
using System.Web.Script.Serialization;
using KAOSTools.Core;

namespace KAOSTools.ModelAnalyzer
{
    enum ExportFormat {
        Console, Json
    }

    class ModelAnalyzer : KAOSToolCLI
    {
        static bool show_formal = false;
        static int levensteinThreshold = 0;

        static ExportFormat format = ExportFormat.Console;
        static List<CheckResult> results;

        public static void Main (string[] args)
        {
            options.Add ("f|formal",  "Check formal attributes", v => show_formal = true);
            options.Add ("l|levenstein=",  "Set threshold for duplicate detection", 
                    v => { 
                        int.TryParse (v, out levensteinThreshold);
            });

            options.Add ("format=", 
                         "Output using the given format. Accepted values are 'console', 'json'.",
                         v => { 
                if (v.Equals("console")) { 
                    format = ExportFormat.Console;
                } else if (v.Equals ("json")) {
                    format = ExportFormat.Json;
                } else {
                    throw new ArgumentException (string.Format("'{0}' is not a supported output format.", v));
                }
            });

            Init (args);

            results = new List<CheckResult>();

            CheckImplicit (model);
            CheckUnassignedLeafGoals (model);
            CheckGoalWithSimilarNames (model, levensteinThreshold);
            CheckMissingDefinition (model);
            CheckIncompatibleSystems (model);

            if (show_formal)
                DisplayMissingFormalSpec (model);

            if (format == ExportFormat.Console)
                ConsoleWriter.Output (results);
            else if (format == ExportFormat.Json)
                JSONWriter.Output (results);
        }

        static void CheckImplicit (KAOSModel model) 
        {
            ForAllKAOSElement (x => {
                if (x.Implicit) {
                    AddWarning (string.Format ("{1} '{0}' is implicitely declared.", x.FriendlyName, x.GetType().Name));
                }
            });
        }

        static void CheckIncompatibleSystems (KAOSModel model)
        {
            ForAllGoals (x => {
                if (x.InSystems.Count() == 0) {
                    AddWarning (string.Format ("Goal '{0}' appears in no alternative system.", x.FriendlyName));
                }
            });

            ForAllGoals (g => {
                foreach (var ag in g.AgentAssignments()) {
                    if (ag.InSystems.Count() == 0) {
                        AddWarning (string.Format ("The assignment of goal '{0}' to agent(s) {1} is incompatible. Goal appears only in {2} alternative systems.", 
                                                g.FriendlyName, 
                                                string.Join (",", ag.Agents().Select (x => string.Format ("'{0}'", x.Name))),
                                                (g.InSystems.Count == 0 ? "no" : string.Join (",", g.InSystems.Select (x => string.Format ("'{0}'", x.Name))))
                                                ));
                    }
                }
            });
        }

        static void CheckMissingDefinition (KAOSModel model)
        {
            ForAllKAOSElement (x => {
                var propertyInfo = x.GetType ().GetProperty ("Definition");
                if (propertyInfo != null) {
                    if (string.IsNullOrWhiteSpace ((string) propertyInfo.GetValue (x, null))) {
                        AddWarning (string.Format ("{1} '{0}' has no definition.", x.FriendlyName, x.GetType().Name));
                    }
                }
            });
        }

        static void DisplayMissingFormalSpec (KAOSModel model)
        {
            ForAllKAOSElement (x => {
                var propertyInfo = x.GetType ().GetProperty ("FormalSpec");
                if (propertyInfo != null) {
                    if (string.IsNullOrWhiteSpace ((string) propertyInfo.GetValue (x, null))) {
                        AddWarning (string.Format ("{1} '{0}' has no formal specification.", x.FriendlyName, x.GetType().Name));
                    }
                }
            });
        }

        static void CheckUnassignedLeafGoals (KAOSModel model)
        {
            var unassignedLeafGoals = from g in model.Goals() 
                where g.AgentAssignments().Count() == 0 & g.Refinements().Count() == 0 select g;

            foreach (var item in unassignedLeafGoals) {
                AddWarning (string.Format ("Goal '{0}' is not refined or assigned.", item.FriendlyName));
            }
        }
        
        static void CheckGoalWithSimilarNames (KAOSModel model, int levensteinThreshold)
        {
            /* TODO
            var duplicateGoals() = from g1 in model.Goals() 
                where (from g2 in model.Goals() where g2 != g1 && g2.Name == g1.Name select g2).Count () > 0 
                select g1;

            foreach (var item in duplicateGoals()) {
                WriteKO (item.FriendlyName + " is a potential duplicated item.");
            }
            */
        }

        static void AddOK (string message) {
            results.Add (new CheckResult {
                Status = CheckResultStatus.OK,
                Message = message
            });
        }

        static void AddKO (string message) {
            results.Add (new CheckResult {
                Status = CheckResultStatus.KO,
                Message = message
            });
        }

        static void AddWarning (string message) {
            results.Add (new CheckResult {
                Status = CheckResultStatus.WARNING,
                Message = message
            });
        }

        /*

        private static string GetReferenceString (string name, KAOSCoreElement pred) 
        {
            if (!declarations.ContainsKey(pred)) {
                throw new Exception (pred.Identifier + ":" + pred.GetType() + " was not found in declarations. Please fill a bug with your model.");
            }

            var declaration = declarations[pred].First();
            var currentDirectory = new Uri(Directory.GetCurrentDirectory ()+"/");
            var uri = new Uri(Path.GetFullPath (Path.Combine (Path.GetDirectoryName (filename), declaration.Filename)));

            return (string.Format (name + " '{0}' ({1}:{2},{3})", 
                                   HasName(pred) && string.IsNullOrEmpty(GetName(pred)) ? pred.Identifier : GetName(pred), 
                                   currentDirectory.MakeRelativeUri (uri), 
                                   declaration.Line, 
                                   declaration.Col));
        }

        */

        #region ForAll... helpers 
        
        static void ForAllKAOSElement (Action<KAOSCoreElement> action) {
            ForAllGoals (action);
            ForAllObstacles (action);
            ForAllDomainProperties (action);
            ForAllDomainHypotheses (action);
            ForAllAgents (action);
            ForAllSystems (action);
            ForAllObjects (action);
            ForAllAssociations (action);
            ForAllPredicates (action);
            ForAllTypes (action);
        }

        static void ForAllGoals (Action<Goal> action) {
            foreach (var goal in model.Goals()) {
                action(goal);
            }
        }

        static void ForAllObstacles (Action<Obstacle> action) {
            foreach (var obstacle in model.Obstacles()) {
                action(obstacle);
            }
        }
        
        static void ForAllDomainProperties (Action<DomainProperty> action) {
            foreach (var domprop in model.DomainProperties()) {
                action(domprop);
            }
        }

        static void ForAllDomainHypotheses (Action<DomainHypothesis> action) {
            foreach (var domhyp in model.DomainHypotheses()) {
                action(domhyp);
            }
        }

        static void ForAllAgents (Action<Agent> action) {
            foreach (var agent in model.Agents()) {
                action(agent);
            }
        }

        static void ForAllSystems (Action<AlternativeSystem> action) {
            foreach (var system in model.AlternativeSystems()) {
                action(system);
            }
        }
        
        static void ForAllObjects (Action<Entity> action) {
            foreach (var entity in model.Entities()) {
                action(entity);
            }
        }

        static void ForAllAssociations (Action<Relation> action) {
            foreach (var relation in model.Relations()) {
                action(relation);
            }
        }

        static void ForAllPredicates (Action<Predicate> action) {
            foreach (var predicate in model.Predicates()) {
                action(predicate);
            }
        }

        static void ForAllTypes (Action<GivenType> action) {
            foreach (var goal in model.GivenTypes()) {
                action(goal);
            }
        }

        #endregion
    }

    enum CheckResultStatus {
        OK, KO, WARNING
    }

    class CheckResult {
        public CheckResultStatus Status { get; set; }
        public string Message { get; set; }
        public Declaration Location { get; set; }
    }

    class JSONWriter {

        public static void Output (IList<CheckResult> results)
        {
            var json = new JavaScriptSerializer().Serialize(from r in results select new {
                status = (r.Status == CheckResultStatus.OK ? "ok" : (r.Status == CheckResultStatus.KO ? "ko" : "warning" )),
                message = r.Message
            });
            Console.WriteLine(json);
        }
    }

    class ConsoleWriter {

        public static void Output (IList<CheckResult> results)
        {
            foreach (var item in results) {
                if (item.Status == CheckResultStatus.OK)
                    WriteOK (item);
                
                if (item.Status == CheckResultStatus.KO)
                    WriteKO (item);
                
                if (item.Status == CheckResultStatus.WARNING)
                    WriteWarning (item);
            }
        }

        static void WriteOK (CheckResult result) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.Write ("[ OK ]");
            Console.ResetColor ();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine (" {0}", result.Message);
            Console.ResetColor ();
        }

        static void WriteKO (CheckResult result) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Write ("[ KO ]");
            Console.ResetColor ();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine (" {0}", result.Message);
            Console.ResetColor ();
        }

        static void WriteWarning (CheckResult result) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.Write ("[WARN]");
            Console.ResetColor ();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine (" {0}", result.Message);
            Console.ResetColor ();
        }
    }
}
