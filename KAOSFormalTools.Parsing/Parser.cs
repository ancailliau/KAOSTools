using System;
using System.Linq;
using System.Collections.Generic;
using KAOSFormalTools.Parsing;
using KAOSFormalTools.Domain;

namespace KAOSFormalTools.Parsing
{
    public class Parser
    {
        private GoalModelParser _parser = new GoalModelParser ();
        private GoalModel model;

        public Parser (){}

        public GoalModel Parse (string input)
        {
            var elements = _parser.Parse (input) as Elements;

            model = new GoalModel ();
            FirstPass  (elements);
            SecondPass (elements);

            return model;
        }

        private void FirstPass (Elements elements)
        {
            foreach (var element in elements.Values) {
                if (element is Goal) {
                    BuildGoal (element as Goal);

                } else if (element is DomainProperty) {
                    BuildDomainProperty (element as DomainProperty);
                
                } else if (element is Obstacle) {
                    BuildObstacle (element as Obstacle);

                } else if (element is Agent) {
                    BuildAgent (element as Agent);

                }
            }
        }

        private void SecondPass (Elements elements) {
            foreach (var element in elements.Values) {
                if (element is Obstacle) {
                    BuildObstacleRelations (element as Obstacle);

                } else if (element is Goal) {
                    BuildGoalRelations (element as Goal);
                }
            }
        }
        
        #region Build helpers for first pass
        
        private void BuildGoal (Goal parsedGoal)
        {
            var goal = new KAOSFormalTools.Domain.Goal ();

            foreach (var attribute in parsedGoal.Attributes) {
                if (attribute is Identifier) {
                    goal.Identifier = (attribute as Identifier).Value;

                } else if (attribute is Name) {
                    goal.Name = (attribute as Name).Value;

                } else if (attribute is FormalSpec) {
                    goal.FormalSpec = (attribute as FormalSpec).Value;
                }
            }

            if (model.GoalExists (goal.Identifier))
                throw new ParsingException (string.Format ("Identifier '{0}' is not unique", goal.Identifier));

            model.Goals.Add (goal);
        }

        private void BuildDomainProperty (DomainProperty parsedDomProp)
        {
            var domprop = new KAOSFormalTools.Domain.DomainProperty();

            foreach (var attr in parsedDomProp.Attributes) {
                if (attr is Identifier) {
                    domprop.Identifier = (attr as Identifier).Value;

                } else if (attr is Name) {
                    domprop.Name = (attr as Name).Value;

                } else if (attr is FormalSpec) {
                    domprop.FormalSpec = (attr as FormalSpec).Value;
                }
            }

            if (model.DomainPropertyExists (domprop.Identifier))
                throw new ParsingException (string.Format ("Identifier '{0}' is not unique", domprop.Identifier));

            model.DomainProperties.Add (domprop);
        }

        private void BuildObstacle (Obstacle parsedObstacle)
        {
            var obstacle = new KAOSFormalTools.Domain.Obstacle();

            foreach (var attr in parsedObstacle.Attributes) {
                if (attr is Identifier) {
                    obstacle.Identifier = (attr as Identifier).Value;

                } else if (attr is Name) {
                    obstacle.Name = (attr as Name).Value;

                } else if (attr is FormalSpec) {
                    obstacle.FormalSpec = (attr as FormalSpec).Value;
                }
            }

            if (model.ObstacleExists (obstacle.Identifier))
                throw new ParsingException (string.Format ("Identifier '{0}' is not unique", obstacle.Identifier));

            model.Obstacles.Add (obstacle);
        }

        private void BuildAgent (Agent parsedAgent)
        {
            var agent = new KAOSFormalTools.Domain.Agent ();
            agent.Software = parsedAgent.Software;

            foreach (var attr in parsedAgent.Attributes) {
                if (attr is Identifier) {
                    agent.Identifier = (attr as Identifier).Value;

                } else if (attr is Name) {
                    agent.Name = (attr as Name).Value;
                }
            }

            if (model.AgentExists (agent.Identifier))
                throw new ParsingException (string.Format ("Identifier '{0}' is not unique", agent.Identifier));

            model.Agents.Add (agent);
        }

        #endregion

        #region Build helpers for second pass

        private void BuildObstacleRelations (Obstacle parsedObstacle)
        {
            string identifier = "";
            string name       = "";

            var refinements   = new List<ObstacleRefinement> ();

            foreach (var attribute in parsedObstacle.Attributes) {
                if (attribute is Identifier) {
                    identifier = (attribute as Identifier).Value;

                } else if (attribute is Name) {
                    name = (attribute as Name).Value;

                } else if (attribute is RefinedByList) {
                    var children = attribute as RefinedByList;
                    var refinement = new ObstacleRefinement ();

                    foreach (var child in children.Values) {
                        var candidate = GetOrCreateObstacle (child, true);
                        if (candidate != null)
                            refinement.Children.Add (candidate);
                    }

                    if (refinement.Children.Count > 0)
                        refinements.Add (refinement);
                }
            }
            
            KAOSFormalTools.Domain.Obstacle obstacle = null;
            if (string.IsNullOrEmpty (identifier)) {
                var obstacles = model.GetObstaclesByName (name);
                if (obstacles.Count() > 1)
                    throw new ParsingException (string.Format ("Obstacle '{0}' is ambiguous", name));
                else if (obstacles.Count() == 0)
                    throw new ParsingException (string.Format ("Obstacle '{0}' not found", name));
                else 
                    obstacle = obstacles.Single ();

            } else {
                obstacle = model.GetObstacleByIdentifier (identifier);
                
                if (obstacle == null)
                    throw new ParsingException (string.Format ("Obstacle '{0}' not found", identifier));
            }

            obstacle.Refinements = refinements;
        }

        private void BuildGoalRelations (Goal parsedGoal)
        {
            string identifier    = "";
            string name          = "";
            var    refinements   = new List<GoalRefinement> ();
            var    obstruction   = new List<KAOSFormalTools.Domain.Obstacle> ();
            var    assignedAgents = new List<KAOSFormalTools.Domain.Agent> ();

            foreach (var attribute in parsedGoal.Attributes) {
                if (attribute is Identifier) {
                    identifier = (attribute as Identifier).Value;

                } else if (attribute is Name) {
                    name = (attribute as Name).Value;

                } else if (attribute is RefinedByList) {
                    var children = attribute as RefinedByList;
                    var refinement = new GoalRefinement ();

                    foreach (var child in children.Values) {
                        var candidate = GetOrCreateGoal (child, true);
                        if (candidate != null)
                            refinement.Children.Add (candidate);
                    }

                    if (refinement.Children.Count > 0)
                        refinements.Add (refinement);

                } else if (attribute is ObstructedByList) {
                    var children = attribute as ObstructedByList;

                    foreach (var child in children.Values) {
                        var candidate = GetOrCreateObstacle (child, true);
                        if (candidate != null)
                            obstruction.Add (candidate);
                    }
                
                } else if (attribute is AssignedToList) {
                    foreach (var assignedto in (attribute as AssignedToList).Values) {
                        var candidate = GetOrCreateAgent (assignedto, true);
                        if (candidate != null)
                            assignedAgents.Add (candidate);
                    }
                }
            }

            KAOSFormalTools.Domain.Goal goal = null;
            if (string.IsNullOrEmpty (identifier)) {
                var goals = model.GetGoalsByName (name);
                if (goals.Count() > 1)
                    throw new ParsingException (string.Format ("Goal '{0}' is ambiguous", name));
                else if (goals.Count() == 0)
                    throw new ParsingException (string.Format ("Goal '{0}' not found", name));
                else 
                    goal = goals.Single ();

            } else {
                goal = model.GetGoalByIdentifier (identifier);
                
                if (goal == null)
                    throw new ParsingException (string.Format ("Goal '{0}' not found", identifier));
            }

            goal.Refinements    = refinements;
            goal.Obstruction    = obstruction;
            goal.AssignedAgents = assignedAgents;
        }
    
        #endregion

        #region Get or create helpers

        private KAOSFormalTools.Domain.Obstacle GetOrCreateObstacle (IdentifierOrName attribute, bool create = true)
        {
            KAOSFormalTools.Domain.Obstacle candidate = null;

            if (attribute is Name) {
                var name = (attribute as Name).Value;
                var candidates = model.GetObstaclesByName (name);

                if (candidates.Count() == 0) {
                    if (create) {
                        candidate = new KAOSFormalTools.Domain.Obstacle() { 
                            Name = (attribute as Name).Value
                        };
                        model.Obstacles.Add (candidate);
                    } else {
                        throw new ParsingException (string.Format ("Obstacle '{0}' could not be found", (attribute as Name).Value));
                    }

                } else if (candidates.Count() > 1) {
                    throw new ParsingException (string.Format ("Obstacle '{0}' is ambiguous", (attribute as Name).Value));

                } else /* candidates.Count() == 0 */ {
                    candidate = candidates.Single ();
                }

            } else if (attribute is Identifier) {
                candidate = model.GetObstacleByIdentifier ((attribute as Identifier).Value);

                if (candidate == null) {
                    if (create) {
                        candidate = new KAOSFormalTools.Domain.Obstacle() { 
                            Identifier = (attribute as Identifier).Value
                        };
                        model.Obstacles.Add (candidate);
                    } else {
                        throw new ParsingException (string.Format ("Obstacle '{0}' could not be found", (attribute as Identifier).Value));
                    }
                }
            }

            return candidate;
        }
        
        private KAOSFormalTools.Domain.Goal GetOrCreateGoal (IdentifierOrName attribute, bool create = true)
        {
            KAOSFormalTools.Domain.Goal candidate = null;

            if (attribute is Name) {
                var name = (attribute as Name).Value;
                var candidates = model.GetGoalsByName (name);

                if (candidates.Count() == 0) {
                    if (create) {
                        candidate = new KAOSFormalTools.Domain.Goal() { 
                            Name = (attribute as Name).Value
                        };
                        model.Goals.Add (candidate);
                    } else {
                        throw new ParsingException (string.Format ("Goal '{0}' could not be found", (attribute as Name).Value));
                    }

                } else if (candidates.Count() > 1) {
                    throw new ParsingException (string.Format ("Goal '{0}' is ambiguous", (attribute as Name).Value));

                } else /* candidates.Count() == 0 */ {
                    candidate = candidates.Single ();
                }

            } else if (attribute is Identifier) {
                candidate = model.GetGoalByIdentifier ((attribute as Identifier).Value);

                if (candidate == null) {
                    if (create) {
                        candidate = new KAOSFormalTools.Domain.Goal() { 
                            Identifier = (attribute as Identifier).Value
                        };
                        model.Goals.Add (candidate);
                    } else {
                        throw new ParsingException (string.Format ("Goal '{0}' could not be found", (attribute as Identifier).Value));
                    }
                }
            }

            return candidate;
        }

        private KAOSFormalTools.Domain.Agent GetOrCreateAgent (IdentifierOrName attribute, bool create = true)
        {
            KAOSFormalTools.Domain.Agent candidate = null;

            if (attribute is Name) {
                var name = (attribute as Name).Value;
                var candidates = model.GetAgentsByName (name);

                if (candidates.Count() == 0) {
                    if (create) {
                        candidate = new KAOSFormalTools.Domain.Agent() { 
                            Name = (attribute as Name).Value
                        };
                        model.Agents.Add (candidate);
                    } else {
                        throw new ParsingException (string.Format ("Agent '{0}' could not be found", (attribute as Name).Value));
                    }

                } else if (candidates.Count() > 1) {
                    throw new ParsingException (string.Format ("Agent '{0}' is ambiguous", (attribute as Name).Value));

                } else /* candidates.Count() == 0 */ {
                    candidate = candidates.Single ();
                }

            } else if (attribute is Identifier) {
                candidate = model.GetAgentByIdentifier ((attribute as Identifier).Value);

                if (candidate == null) {
                    if (create) {
                        candidate = new KAOSFormalTools.Domain.Agent () { 
                            Identifier = (attribute as Identifier).Value
                        };
                        model.Agents.Add (candidate);
                    } else {
                        throw new ParsingException (string.Format ("Agent '{0}' could not be found", (attribute as Identifier).Value));
                    }
                }
            }

            return candidate;
        }

        #endregion
    }
}

