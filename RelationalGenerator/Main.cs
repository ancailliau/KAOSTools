using System;
using System.Linq;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using NDesk.Options;
using KAOSTools.MetaModel;
using KAOSTools.Utils;
using KAOSTools.Parsing;
    
namespace KAOSTools.ReportGenerator
{

    class MainClass : KAOSToolCLI
    {

        public static bool HasIdentifier(KAOSMetaModelElement elm)
        {
          return elm.GetType().GetProperty("Identifier") != null;
        }

        public static Object HandleIdentifier(KAOSMetaModelElement elm)
        {
          if (elm == null){
            return null;
          }
          return elm.GetType().GetProperty("Identifier").GetValue(elm, null);
        }

        public static Object HandleLocations(IList<Declaration> locations)
        {
          var tuples = from d in locations
                       select new { filename = d.Filename,
                                    line = d.Line,
                                    col = d.Col,
                                    index = locations.IndexOf(d) };
          return tuples;
        }

        public static void Main (string[] args)
        {
            Init (args);

            var dbvalue = new
            {
              //
              agents = from a in model.GoalModel.Agents.OrderBy (x => x.Name)
                       select new { id = a.Identifier,
                                    name = a.Name, 
                                    description = a.Description,
                                    type = Enum.GetName(typeof(MetaModel.AgentType), a.Type) },

              //
              goals = from g in model.GoalModel.Goals.OrderBy (x => x.Name)
                      select new { id = g.Identifier,
                                   name = g.Name,
                                   definition = g.Definition },

              //
              obstacles = from o in model.GoalModel.Obstacles.OrderBy (x => x.Name)
                          select new { id = o.Identifier,
                                       name = o.Name,
                                       definition = o.Definition },

              //
              resolutions = from o in model.GoalModel.Obstacles.OrderBy (x => x.Name)
                              from g in o.Resolutions.OrderBy (x => x.Name)
                                select new { obstacle = o.Identifier,
                                                 goal = g.Identifier },

              //
              predicates = from p in model.Predicates.Values.OrderBy (x => x.Name)
                           select new { id = p.Identifier,
                                        name = p.Name,
                                        signature = p.Signature,
                                        definition = p.Definition },

              //
              refinements = from g in model.GoalModel.Goals
                              from r in g.Refinements
                                from c in r.Subgoals
                                  select new {          id = r.Identifier,
                                                    sysref = HandleIdentifier(r.SystemReference),
                                                    parent = g.Identifier,
                                                     child = c.Identifier },

              //
              assignments = from g in model.GoalModel.Goals
                              from aa in g.AgentAssignments
                                from a in aa.Agents
                                  select new {     id = aa.Identifier,
                                                 goal = g.Identifier,
                                                agent = a.Identifier,
                                               sysref = HandleIdentifier(aa.SystemReference) },

              //
              insystem = from g in model.GoalModel.Goals.OrderBy (x => x.Name)
                           from s in g.InSystems
                             select new { goal   = g.Identifier,
                                          system = s.Identifier },

              //
              locations = from pair in declarations
                          where HasIdentifier(pair.Key)
                          select new { object_id = HandleIdentifier(pair.Key),
                                       locations = HandleLocations(pair.Value) }

            };

            var json = new JavaScriptSerializer().Serialize(dbvalue);
            Console.WriteLine(json);
            
        }
    }
}
