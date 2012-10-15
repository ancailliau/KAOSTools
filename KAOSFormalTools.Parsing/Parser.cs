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
        
        private KAOSFormalTools.Domain.Goal BuildGoal (Goal parsedGoal)
        {
            var goal = new KAOSFormalTools.Domain.Goal ();
            Identifier identifierAttribute = null;

            foreach (var attribute in parsedGoal.Attributes) {
                if (attribute is Identifier) {
                    goal.Identifier = (attribute as Identifier).Value;
                    identifierAttribute = (attribute as Identifier);

                } else if (attribute is Name) {
                    goal.Name = (attribute as Name).Value;

                } else if (attribute is FormalSpec) {
                    goal.FormalSpec = (attribute as FormalSpec).Value;
                }
            }

            if (model.GoalExists (goal.Identifier))
                throw new ParsingException (string.Format ("Identifier '{0}' is not unique", goal.Identifier));
            
            // Ensure that parsed goal has the same identifer than the new one
            // This is required for second pass, otherwise, entity could not be found
            if (identifierAttribute == null)
                parsedGoal.Attributes.Add (new Identifier (goal.Identifier));

            model.Goals.Add (goal);

            return goal;
        }

        private KAOSFormalTools.Domain.DomainProperty BuildDomainProperty (DomainProperty parsedDomProp)
        {
            var domprop = new KAOSFormalTools.Domain.DomainProperty();
            Identifier identifierAttribute = null;

            foreach (var attr in parsedDomProp.Attributes) {
                if (attr is Identifier) {
                    domprop.Identifier = (attr as Identifier).Value;
                    identifierAttribute = (attr as Identifier);

                } else if (attr is Name) {
                    domprop.Name = (attr as Name).Value;

                } else if (attr is FormalSpec) {
                    domprop.FormalSpec = (attr as FormalSpec).Value;
                }
            }

            if (model.DomainPropertyExists (domprop.Identifier))
                throw new ParsingException (string.Format ("Identifier '{0}' is not unique", domprop.Identifier));
            
            // Ensure that parsed domprop has the same identifer than the new one
            // This is required for second pass, otherwise, entity could not be found
            if (identifierAttribute == null)
                parsedDomProp.Attributes.Add (new Identifier (domprop.Identifier));

            model.DomainProperties.Add (domprop);

            return domprop;
        }

        private KAOSFormalTools.Domain.Obstacle BuildObstacle (Obstacle parsedObstacle)
        {
            var obstacle = new KAOSFormalTools.Domain.Obstacle();
            Identifier identifierAttribute = null;

            foreach (var attr in parsedObstacle.Attributes) {
                if (attr is Identifier) {
                    obstacle.Identifier = (attr as Identifier).Value;
                    identifierAttribute = (attr as Identifier);

                } else if (attr is Name) {
                    obstacle.Name = (attr as Name).Value;

                } else if (attr is FormalSpec) {
                    obstacle.FormalSpec = (attr as FormalSpec).Value;
                }
            }

            if (model.ObstacleExists (obstacle.Identifier)) {
                var o2 = model.GetObstacleByIdentifier (obstacle.Identifier);
                o2.Merge (obstacle);
                return o2;
            }

            if (identifierAttribute == null && model.GetObstaclesByName (obstacle.Name).Count() == 1) {
                var o2 = model.GetObstaclesByName (obstacle.Name).Single ();
                o2.Merge (obstacle);
                return o2;
            }

            // Ensure that parsed obstacle has the same identifer than the new one
            // This is required for second pass, otherwise, entity could not be found
            if (identifierAttribute == null)
                parsedObstacle.Attributes.Add (new Identifier (obstacle.Identifier));

            model.Obstacles.Add (obstacle);

            return obstacle;
        }

        private KAOSFormalTools.Domain.Agent BuildAgent (Agent parsedAgent)
        {
            var agent = new KAOSFormalTools.Domain.Agent ();
            agent.Software = parsedAgent.Software;
            Identifier identifierAttribute = null;

            foreach (var attr in parsedAgent.Attributes) {
                if (attr is Identifier) {
                    agent.Identifier = (attr as Identifier).Value;
                    identifierAttribute = (attr as Identifier);

                } else if (attr is Name) {
                    agent.Name = (attr as Name).Value;
                }
            }

            if (model.AgentExists (agent.Identifier))
                throw new ParsingException (string.Format ("Identifier '{0}' is not unique", agent.Identifier));
            
            // Ensure that parsed agent has the same identifer than the new one
            // This is required for second pass, otherwise, entity could not be found
            if (identifierAttribute == null)
                parsedAgent.Attributes.Add (new Identifier (agent.Identifier));

            model.Agents.Add (agent);

            return agent;
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
                        if (child is IdentifierOrName) {
                            var candidate = GetOrCreateObstacle (child as IdentifierOrName, true);
                            if (candidate != null)
                                refinement.Children.Add (candidate);
                        } else if (child is Obstacle) {
                            var o = BuildObstacle (child as Obstacle);
                            refinement.Children.Add (o);
                            
                            BuildObstacleRelations (child as Obstacle);
                        }
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

            foreach (var r in refinements)
                obstacle.Refinements.Add (r);
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
                        if (child is IdentifierOrName) {
                            var candidate = GetOrCreateGoal (child as IdentifierOrName, true);
                            if (candidate != null)
                                refinement.Children.Add (candidate);
                        } else if (child is Goal) {
                            var g = BuildGoal (child as Goal);
                            refinement.Children.Add (g);
                            
                            BuildGoalRelations (child as Goal);
                        }
                    }

                    if (refinement.Children.Count > 0)
                        refinements.Add (refinement);

                } else if (attribute is ObstructedByList) {
                    var children = attribute as ObstructedByList;

                    foreach (var child in children.Values) {
                        if (child is IdentifierOrName) {
                            var candidate = GetOrCreateObstacle (child as IdentifierOrName, true);
                            if (candidate != null)
                                obstruction.Add (candidate);
                        } else if (child is Obstacle) {
                            var o = BuildObstacle (child as Obstacle);
                            obstruction.Add (o);
                            
                            BuildObstacleRelations (child as Obstacle);
                        }
                    }
                
                } else if (attribute is AssignedToList) {
                    foreach (var assignedto in (attribute as AssignedToList).Values) {
                        if (assignedto is IdentifierOrName) {
                            var candidate = GetOrCreateAgent (assignedto as IdentifierOrName, true);
                            if (candidate != null)
                                assignedAgents.Add (candidate);
                        } else if (assignedto is Agent) {
                            var a = BuildAgent (assignedto as Agent);
                            assignedAgents.Add (a);
                        }
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

            foreach (var r in refinements)
                goal.Refinements.Add (r);

            foreach (var r in obstruction)
                goal.Obstruction.Add (r);
            
            foreach (var r in assignedAgents)
                goal.AssignedAgents.Add (r);
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
                    candidate = candidates.First ();

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
                   candidate = candidates.First ();

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

