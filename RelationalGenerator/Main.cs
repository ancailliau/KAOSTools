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

        public static Object ObstacleRefinements() {
          var tuples = from g in model.Obstacles
                         from r in g.Refinements
                           select new { id = r.Identifier,
                                        parent = g.Identifier };
          return tuples;
        }

        public static Object ObstacleRefinementChildren(){
          var grgc = from g in model.Obstacles
                       from r in g.Refinements
                         from sg in r.Subobstacles
                           select new { refinement = r.Identifier,
                                        child = sg.Identifier };
          var grpc = from g in model.Obstacles
                       from r in g.Refinements
                         from d in r.DomainProperties
                           select new { refinement = r.Identifier,
                                        child = d.Identifier };
          var grhc = from g in model.Obstacles
                       from r in g.Refinements
                         from d in r.DomainHypotheses
                           select new { refinement = r.Identifier,
                                        child = d.Identifier };
          return grgc.Union(grpc).Union(grhc);
        }

        public static Object GoalRefinements() {
          var tuples = from g in model.Goals()
                         from r in g.Refinements
                           select new { id = r.Identifier,
                                        parent = g.Identifier };
          return tuples;
        }

        public static Object GoalRefinementChildren(){
          var grgc = from g in model.Goals()
                       from r in g.Refinements
                         from sg in r.Subgoals
                           select new { refinement = r.Identifier,
                                        child = sg.Identifier };
          var grpc = from g in model.Goals()
                       from r in g.Refinements
                         from d in r.DomainProperties
                           select new { refinement = r.Identifier,
                                        child = d.Identifier };
          var grhc = from g in model.Goals()
                       from r in g.Refinements
                         from d in r.DomainHypotheses
                           select new { refinement = r.Identifier,
                                        child = d.Identifier };
          return grgc.Union(grpc).Union(grhc);
        }

        public static void Main (string[] args)
        {
            Init (args);

            var dbvalue = new
            {
              // IDEAL GOAL MODEL
              goals = from g in model.Goals().OrderBy (x => x.Name)
                      select new { id = g.Identifier,
                                   name = g.Name,
                                   definition = g.Definition,
                                   formalspec = FormalSpecHtmlExporter.ToHtmlString(g.FormalSpec) },

              domain_properties = from d in model.DomainProperties.OrderBy (x => x.Name)
                                  select new { id = d.Identifier,
                                               name = d.Name,
                                               definition = d.Definition,
                                               formalspec = FormalSpecHtmlExporter.ToHtmlString(d.FormalSpec) },

              domain_hypotheses = from g in model.DomainHypotheses.OrderBy (x => x.Name)
                                  select new { id = g.Identifier,
                                               name = g.Name,
                                               definition = g.Definition,
                                               formalspec = FormalSpecHtmlExporter.ToHtmlString(g.FormalSpec) },

              goal_refinements = GoalRefinements(),

              goal_refinement_children = GoalRefinementChildren(),

              // OBSTACLE ANALYSIS

              obstacles = from o in model.Obstacles.OrderBy (x => x.Name)
                          select new { id = o.Identifier,
                                       name = o.Name,
                                       definition = o.Definition,
                                       formalspec = FormalSpecHtmlExporter.ToHtmlString(o.FormalSpec) },

              obstacle_refinements = ObstacleRefinements(),

              obstacle_refinement_children = ObstacleRefinementChildren(),

              obstructions = from g in model.Goals().OrderBy (x => x.Name)
                             from o in g.Obstructions.OrderBy (x => x.ObstructingObstacle.Name)
                               select new { goal     = g.Identifier,
                                            obstacle = o.ObstructingObstacle.Identifier },

              resolutions = from o in model.Obstacles.OrderBy (x => x.Name)
                              from r in o.Resolutions.OrderBy (x => x.ResolvingGoal.Name)
                                select new { obstacle = o.Identifier,
                                                 goal = r.ResolvingGoal.Identifier },

              // AGENT MODEL

              agents = from a in model.Agents.OrderBy (x => x.Name)
                       select new { id = a.Identifier,
                                    name = a.Name,
                                    definition = a.Definition,
                                    type = Enum.GetName(typeof(MetaModel.AgentType), a.Type).ToLower() },

              assignments = from g in model.Goals()
                              from aa in g.AgentAssignments
                                from a in aa.Agents
                                  select new {     id = aa.Identifier,
                                                 goal = g.Identifier,
                                                agent = a.Identifier },

              // OTHER ORTHOGONAL FEATURES

              locations = from pair in declarations
                          where HasIdentifier(pair.Key)
                          select new { object_id = HandleIdentifier(pair.Key),
                                       locations = HandleLocations(pair.Value) },

              predicates = from p in model.Predicates.OrderBy (x => x.Name)
                           select new { id = p.Identifier,
                                        name = p.Name,
                                        signature = "", // TODO
                                        definition = p.Definition }

            };

            var json = new JavaScriptSerializer().Serialize(dbvalue);
            Console.WriteLine(json);

        }
    }
}
