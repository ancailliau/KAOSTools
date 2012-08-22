using System;
using System.Linq;
using System.Collections.Generic;
using KAOSFormalTools.Parsing;
using KAOSFormalTools.Domain;

internal sealed partial class GoalModelParser
{   
    private KAOSFormalTools.Parsing.Element BuildElements (List<Result> results)
    {
        var attrs = new Elements();
        foreach (var result in results) {
            attrs.Values.Add (result.Value);
        }
        return attrs;
    }

    private KAOSFormalTools.Parsing.Element BuildGoal (List<Result> results)
    {
        var goal = new KAOSFormalTools.Parsing.Goal ();

        for (int i = 1; i < results.Count; i++) {
            goal.Attributes.Add (results[i].Value as KAOSFormalTools.Parsing.Attribute);
        }

        return goal;
    }

    private KAOSFormalTools.Parsing.Element BuildDomainProperty (List<Result> results)
    {
        var domprop = new KAOSFormalTools.Parsing.DomainProperty ();

        for (int i = 1; i < results.Count; i++) {
            domprop.Attributes.Add (results[i].Value as KAOSFormalTools.Parsing.Attribute);
        }

        return domprop;
    }

    private KAOSFormalTools.Parsing.Element BuildObstacle (List<Result> results)
    {
        var obstacle = new KAOSFormalTools.Parsing.Obstacle ();

        for (int i = 1; i < results.Count; i++) {
            obstacle.Attributes.Add (results[i].Value as KAOSFormalTools.Parsing.Attribute);
        }

        return obstacle;
    }

    private KAOSFormalTools.Parsing.Element BuildRefinedBy (List<Result> results)
    {
        var list = new RefinedByList ();
        for (int i = 1; i < results.Count; i = i + 2) {
            list.Values.Add (results[i].Value as IdentifierOrName);
        }

        return list;
    }

    private KAOSFormalTools.Parsing.Element BuildObstructedBy (List<Result> results)
    {
        var list = new ObstructedByList ();
        for (int i = 1; i < results.Count; i = i + 2) {
            list.Values.Add (results[i].Value as IdentifierOrName);
        }

        return list;
    }

    private KAOSFormalTools.Parsing.Element BuildId (List<Result> results)
    {
        return new KAOSFormalTools.Parsing.Identifier(results[1].Text);
    }

    private KAOSFormalTools.Parsing.Element BuildName (List<Result> results)
    {
        return new KAOSFormalTools.Parsing.Name(results[2].Text);
    }

    private KAOSFormalTools.Parsing.Element BuildFormalSpec (List<Result> results)
    {
        var formalSpec = new FormalSpec (results [2].Text);
        return formalSpec;
    }
    
    private KAOSFormalTools.Parsing.Element BuildIdOrName (List<Result> results)
    {
        if (results.Count == 3) {
            return new Name (results[1].Text);
        } else if (results.Count == 1) {
            return new Identifier (results[0].Text);
        }
        return null;
    }
}

namespace KAOSFormalTools.Parsing
{
    public class Parser
    {
        private GoalModelParser _parser = new GoalModelParser ();

        public Parser (){}

        public GoalModel Parse (string input)
        {
            var elements = _parser.Parse (input);
            var goals = elements as Elements;

            var goalmodel = new GoalModel ();
            FirstPass (goalmodel, goals);
            SecondPass (goalmodel, goals);

            return goalmodel;
        }

        private void FirstPass (GoalModel model, Elements elements)
        {
            var temporaryGoals = new Dictionary<string, KAOSFormalTools.Domain.Goal> ();
            foreach (var element in elements.Values) {
                if (element is Goal) {
                    var g = new KAOSFormalTools.Domain.Goal ();
                    var parsedGoal = element as Goal;

                    foreach (var attr in parsedGoal.Attributes) {
                        if (attr is Identifier) {
                            g.Identifier = (attr as Identifier).Value;
                        } else if (attr is Name) {
                            g.Name = (attr as Name).Value;
                        } else if (attr is FormalSpec) {
                            g.FormalSpec = (attr as FormalSpec).Value;
                        }
                    }

                    if (string.IsNullOrEmpty (g.Identifier) & string.IsNullOrEmpty(g.Name))
                        throw new ParsingException ("Missing identifier or name for goal");

                    if (g.Identifier != null && temporaryGoals.ContainsKey(g.Identifier))
                        throw new ParsingException (string.Format ("Identifier '{0}' is not unique", g.Identifier));

                    if (g.Identifier == null)
                        g.Identifier = Guid.NewGuid ().ToString ();

                    temporaryGoals.Add (g.Identifier, g);

                } else if (element is DomainProperty) {
                    var domprop = new KAOSFormalTools.Domain.DomainProperty();
                    var parsedDomProp = element as DomainProperty;

                    foreach (var attr in parsedDomProp.Attributes) {
                        if (attr is Identifier) {
                            domprop.Identifier = (attr as Identifier).Value;
                        } else if (attr is Name) {
                            domprop.Name = (attr as Name).Value;
                        } else if (attr is FormalSpec) {
                            domprop.FormalSpec = (attr as FormalSpec).Value;
                        }
                    }

                    if (string.IsNullOrEmpty (domprop.Identifier) & string.IsNullOrEmpty (domprop.Name))
                        throw new ParsingException ("Missing identifier or name for domain property");

                    if (domprop.Identifier != null && temporaryGoals.ContainsKey(domprop.Identifier))
                        throw new ParsingException (string.Format ("Identifier '{0}' is not unique", domprop.Identifier));

                    if (domprop.Identifier == null)
                        domprop.Identifier = Guid.NewGuid ().ToString ();

                    model.DomainProperties.Add (domprop);
                
                } else if (element is Obstacle) {
                    var obstacle = new KAOSFormalTools.Domain.Obstacle();
                    var parsedObstacle = element as Obstacle;

                    foreach (var attr in parsedObstacle.Attributes) {
                        if (attr is Identifier) {
                            obstacle.Identifier = (attr as Identifier).Value;
                        } else if (attr is Name) {
                            obstacle.Name = (attr as Name).Value;
                        } else if (attr is FormalSpec) {
                            obstacle.FormalSpec = (attr as FormalSpec).Value;
                        }
                    }

                    if (string.IsNullOrEmpty (obstacle.Identifier))
                        throw new ParsingException ("Missing identifier for obstacle");

                    if (temporaryGoals.ContainsKey(obstacle.Identifier))
                        throw new ParsingException (string.Format ("Identifier '{0}' is not unique", obstacle.Identifier));

                    model.Obstacles.Add (obstacle);
                }
            }

            model.Goals = new List<KAOSFormalTools.Domain.Goal> (temporaryGoals.Values);
        }

        private void SecondPass (GoalModel model, Elements elements) {
            var goalMapping = new Dictionary<string, KAOSFormalTools.Domain.Goal> ();
            foreach (var g in model.Goals) {
                goalMapping.Add (g.Identifier, g);
            }

            var obstacleMapping = new Dictionary<string, KAOSFormalTools.Domain.Obstacle> ();
            foreach (var o in model.Obstacles) {
                obstacleMapping.Add (o.Identifier, o);
            }

            foreach (var element in elements.Values) {
                if (element is Obstacle) {
                    var parsedObstacle = element as Obstacle;

                    string identifier = "";
                    var refinements = new List<ObstacleRefinement> ();
                    foreach (var attribute in parsedObstacle.Attributes) {
                        if (attribute is Identifier) {
                            identifier = (attribute as Identifier).Value;

                        } else if (attribute is RefinedByList) {
                            var children = attribute as RefinedByList;
                            var refinement = new ObstacleRefinement ();

                            foreach (var child in children.Values) {
                                if (!obstacleMapping.ContainsKey (child.Value))
                                    throw new ParsingException (string.Format ("Identifier '{0}' does not exists", child.Value));
                                refinement.Children.Add (obstacleMapping[child.Value]);
                            }

                            if (refinement.Children.Count > 0)
                                refinements.Add (refinement);

                        }
                    }

                    var obstacle = obstacleMapping[identifier];
                    obstacle.Refinements = refinements;

                } else if (element is Goal) {
                    var parsedGoal = element as Goal;

                    string identifier = "";
                    string name = "";
                    var refinements = new List<GoalRefinement> ();
                    var obstruction   = new List<KAOSFormalTools.Domain.Obstacle> ();

                    foreach (var attribute in parsedGoal.Attributes) {
                        if (attribute is Identifier) {
                            identifier = (attribute as Identifier).Value;

                        } else if (attribute is Name) {
                            name = (attribute as Name).Value;

                        } else if (attribute is RefinedByList) {
                            var children = attribute as RefinedByList;
                            var refinement = new GoalRefinement ();

                            foreach (var child in children.Values) {
                                KAOSFormalTools.Domain.Goal candidate = null;
                                if (child is Name) {
                                    candidate = (from c in goalMapping.Values where c.Name == (child as Name).Value
                                        select c).SingleOrDefault ();

                                } else if (child is Identifier) {
                                    candidate = (from c in goalMapping.Values where c.Identifier == (child as Identifier).Value
                                        select c).SingleOrDefault ();
                                }

                                if (candidate == null)
                                    throw new ParsingException (string.Format ("Identifier or name '{0}' does not refer to existing entity", child.Value));

                                refinement.Children.Add (candidate);
                            }

                            if (refinement.Children.Count > 0)
                                refinements.Add (refinement);

                        } else if (attribute is ObstructedByList) {
                            var children = attribute as ObstructedByList;

                            foreach (var child in children.Values) {
                                if (!obstacleMapping.ContainsKey (child.Value))
                                    throw new ParsingException (string.Format ("Identifier '{0}' does not exists", child.Value));
                                obstruction.Add (obstacleMapping[child.Value]);
                            }
                        }
                    }

                    KAOSFormalTools.Domain.Goal goal = null;
                    if (string.IsNullOrEmpty (identifier)) {
                        var goalsNamed = goalMapping.Values.Where (x => x.Name == name);
                        if (goalsNamed.Count() > 1) {
                            throw new ParsingException (string.Format ("Name '{0}' is ambiguous", name));
                        } else {
                            goal = goalsNamed.First ();
                        }
                    } else if (goalMapping.ContainsKey (identifier)) {
                        goal = goalMapping[identifier];
                    }

                    if (goal == null)
                        throw new ParsingException (string.Format ("Goal with identifier '{0}' and name '{1}' could not be found", identifier, name));

                    goal.Refinements = refinements;
                    goal.Obstruction = obstruction;
                }
            }
        }
    }
}

