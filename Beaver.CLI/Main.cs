using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using NDesk.Options;
using KAOSFormalTools.Domain;
using LtlSharp;
using KAOSFormalTools.RefinementChecker;

namespace Beaver.CLI
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            bool show_help = false;
            string nusmvModel      = "";
            string nusmvOutput     = "";
    
            var p = new OptionSet () {
                { "o|output=", "Generate a NuSMV model in provided file",
                    v => nusmvOutput = v },
                { "i|input=", "Read a NuSMV output and produce a report",
                    v => nusmvModel = v },
                { "h|help",  "show this message and exit", 
                    v => show_help = v != null },
            };

            try {
                p.Parse (args);
                
            } catch (OptionException e) {
                PrintError (e.Message);
                return;
            }
    
            if (string.IsNullOrEmpty (nusmvModel) & string.IsNullOrEmpty (nusmvOutput)) {
                PrintError ("`-i|--input` or `-o|--output` shall be given");
                return;
            }

            if (!string.IsNullOrEmpty (nusmvModel) & !string.IsNullOrEmpty (nusmvOutput)) {
                PrintError ("`-i|--input` and `-o|--output` are mutually exclusive");
                return;
            }

            if (show_help) {
                ShowHelp (p);
                return;
            }

            var model =  BuildModel ();

            if (!string.IsNullOrEmpty (nusmvModel)) {
                model.InterpretNuSMVOutput (nusmvModel);

            } else {
                model.WriteNuSMVModel (nusmvOutput);
            }
        }

        static GoalModel BuildModel ()
        {
            var model = new GoalModel ();

            var achieve_on_scene_when_reported   = new Goal () { Name = "Achieve [AmbulanceOnScene When IncidentReported]",                      FormalSpec = Parser.Parse ("G (incidentReported -> F ambulanceOnScene)")                      };
            var achieve_allocated_when_reported  = new Goal () { Name = "Achieve [AmbulanceAllocated When IncidentReported]",                    FormalSpec = Parser.Parse ("G (incidentReported -> F ambulanceAllocated)")                    };
            var achieve_on_scene_when_allocated  = new Goal () { Name = "Achieve [AmbulanceOnScene When AmbulanceAllocated]",                    FormalSpec = Parser.Parse ("G (ambulanceAllocated -> F ambulanceOnScene)")                    };
            var achieve_on_scene_when_mobilized  = new Goal () { Name = "Achieve [AmbulanceOnScene When AmbulanceMobilized]",                    FormalSpec = Parser.Parse ("G (ambulanceMobilized -> F ambulanceOnScene)")                    };
            var achieve_mobilized_when_allocated = new Goal () { Name = "Achieve [AmbulanceMobilized When AmbulanceAllocated]",                  FormalSpec = Parser.Parse ("G (ambulanceAllocated -> F ambulanceMobilized)")                  };
            var achieve_mobilized_when_onroad    = new Goal () { Name = "Achieve [AmbulanceMobilized When AmbulanceAllocatedOnRoad]",            FormalSpec = Parser.Parse ("G (ambulanceAllocated & onRoad -> F ambulanceMobilized)")         };
            var achieve_mobilized_when_atstation = new Goal () { Name = "Achieve [AmbulanceMobilized When AmbulanceAllocatedAtStation]",         FormalSpec = Parser.Parse ("G (ambulanceAllocated & !onRoad -> F ambulanceMobilized)")        };
            var achieve_mobilized_by_fax         = new Goal () { Name = "Achieve [AmbulanceMobilizedByFax When AmbulanceAllocatedAtStation]",    FormalSpec = Parser.Parse ("G (ambulanceAllocated & !onRoad -> F ambulanceMobilizedByFax)")   };
            var achieve_mobilized_by_phone       = new Goal () { Name = "Achieve [AmbulanceMobilizedByPhone When AmbulanceAllocatedAtStation]",  FormalSpec = Parser.Parse ("G (ambulanceAllocated & !onRoad -> F ambulanceMobilizedByPhone)") };

            achieve_on_scene_when_reported   .Connect (achieve_allocated_when_reported,  achieve_on_scene_when_allocated);
            achieve_on_scene_when_allocated  .Connect (achieve_mobilized_when_allocated, achieve_on_scene_when_mobilized);
            achieve_mobilized_when_allocated .Connect (achieve_mobilized_when_atstation, achieve_mobilized_when_onroad);
            achieve_mobilized_when_onroad    .Connect (achieve_mobilized_by_fax, achieve_mobilized_by_phone);

            model.RootGoals.Add (achieve_on_scene_when_reported);

            model.DomainProperties.Add (new DomainProperty () {
                Name = "Ambulance Mobilized By Fax or Phone", 
                FormalSpec = Parser.Parse ("G ((ambulanceMobilizedByFax | ambulanceMobilizedByPhone) <-> ambulanceMobilized)")
            });

            return model;
        }

        static void ShowHelp (OptionSet p)
        {
            Console.WriteLine ("Usage: ltlsharp [OPTIONS]+ formula");
            Console.WriteLine ();
            Console.WriteLine ("Options:");
            p.WriteOptionDescriptions (Console.Out);
        }
        
        static void PrintError (string error)
        {  
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.Write ("ltlsharp: ");
            Console.Error.WriteLine (error);
            Console.Error.WriteLine ("Try `ltlsharp --help' for more information.");
            Console.ResetColor ();
        }
    }
}
