using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using NDesk.Options;
using KAOSTools.MetaModel;
using System.Collections;
using LtlSharp.Utils;
using KAOSTools.Utils;
    
namespace KAOSTools.ReportGenerator
{
    class MainClass : KAOSToolCLI
    {
        public static void Main (string[] args)
        {
            Init (args);

            Console.WriteLine ("<!doctype html>");
            Console.WriteLine ("<html lang=en>");
            Console.WriteLine ("<head>");
            Console.WriteLine ("<meta charset=utf-8>");
            Console.WriteLine ("<title>Predicates definition</title>");
            Console.WriteLine ("<link href=\"http://netdna.bootstrapcdn.com/twitter-bootstrap/2.3.1/css/bootstrap-combined.min.css\" rel=\"stylesheet\">");
            Console.WriteLine ("</head>");
            Console.WriteLine ("<body>");

            Console.WriteLine ("<div class=\"container\">");

            Console.WriteLine ("<h2>Goals definitions</h2>");
            
            foreach (var goal in model.GoalModel.Goals.OrderBy (x => x.Name)) {
                Console.WriteLine ("<dt>{0} (<code>{1}</code>)</dt>", goal.Name, goal.Identifier);
                Console.WriteLine ("<dd>");
                Console.WriteLine ("<p><strong>Definition</strong> {0}<p>", goal.Definition);
                Console.WriteLine ("</dd>");
            }

            Console.WriteLine ("<h2>Predicates definitions</h2>");

            foreach (var predicate in model.Predicates.Values.OrderBy (x => x.Name)) {
                Console.WriteLine ("<dt>{0} (<code>{1}</code>)</dt>", predicate.Name, predicate.Signature);
                Console.WriteLine ("<dd>");
                Console.WriteLine ("<p><strong>Definition</strong> {0}<p>", predicate.Definition);
                if (!string.IsNullOrEmpty (predicate.FormalSpec)) 
                    Console.WriteLine ("<p><strong>FormalSpec</strong><code>{0}</code></p>", predicate.FormalSpec);
                Console.WriteLine ("</dd>");
            }
            
            Console.WriteLine ("</div>");

            Console.WriteLine ("</body>");
            Console.WriteLine ("</html>");

        }
    }
}
