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
        private KAOSModel model;

        public Parser (){}

        public KAOSModel Parse (string input, string filename, KAOSModel model)
        {
            Elements elements = null;

            try {
                elements = _parser.Parse (input, filename) as Elements;    
            } catch (Exception e) {
                throw new ParsingException (e.Message);
            }

            if (elements != null) {
                this.model = model;
                FirstPass  (elements);
                SecondPass (elements);
            }

            return model;
        }

        public KAOSModel Parse (string input, string filename)
        {
            return Parse (input, filename, new KAOSModel());
        }

        public KAOSModel Parse (string input)
        {
            return Parse (input, null);
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
                
                } else if (attribute is Definition) {
                    goal.Definition = (attribute as Definition).Value;

                } else if (attribute is RDS) {
                    goal.RDS = (attribute as RDS).Value;
                }
            }

            if (model.GoalModel.GoalExists (goal.Identifier)) {
                var g2 = model.GoalModel.GetGoalByIdentifier (goal.Identifier);
                if (!parsedGoal.Override) {
                    g2.Merge (goal);
                    return g2;
                } else {
                    model.GoalModel.ReplaceGoal (g2, goal);
                    return goal;
                }
            }
            
            if (identifierAttribute == null && model.GoalModel.GetGoalsByName (goal.Name).Count() == 1) {
                var g2 = model.GoalModel.GetGoalsByName (goal.Name).Single ();
                if (!parsedGoal.Override) {
                    g2.Merge (goal);
                    return g2;
                } else {
                    model.GoalModel.ReplaceGoal (g2, goal);
                    return goal;
                }
            }

            // Ensure that parsed goal has the same identifer than the new one
            // This is required for second pass, otherwise, entity could not be found
            if (identifierAttribute == null)
                parsedGoal.Attributes.Add (new Identifier (goal.Identifier));

            model.GoalModel.Goals.Add (goal);

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

                } else if (attr is Definition) {
                    domprop.Definition = (attr as Definition).Value;

                } else if (attr is Probability) {
                    domprop.EPS = (attr as Probability).Value;
                }
            }

            if (model.GoalModel.DomainPropertyExists (domprop.Identifier)) {
                var d2 = model.GoalModel.GetDomainPropertyByIdentifier (domprop.Identifier);
                d2.Merge (domprop);
                return d2;
            }
            
            if (identifierAttribute == null && model.GoalModel.GetDomainPropertiesByName (domprop.Name).Count() == 1) {
                var d2 = model.GoalModel.GetDomainPropertiesByName (domprop.Name).Single ();
                d2.Merge (domprop);
                return d2;
            }

            // Ensure that parsed domprop has the same identifer than the new one
            // This is required for second pass, otherwise, entity could not be found
            if (identifierAttribute == null)
                parsedDomProp.Attributes.Add (new Identifier (domprop.Identifier));

            model.GoalModel.DomainProperties.Add (domprop);

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

                } else if (attr is Definition) {
                    obstacle.Definition = (attr as Definition).Value;

                } else if (attr is Probability) {
                    obstacle.EPS = (attr as Probability).Value;
                }
            }

            if (model.GoalModel.ObstacleExists (obstacle.Identifier)) {
                var o2 = model.GoalModel.GetObstacleByIdentifier (obstacle.Identifier);
                o2.Merge (obstacle);
                return o2;
            }

            if (identifierAttribute == null && model.GoalModel.GetObstaclesByName (obstacle.Name).Count() == 1) {
                var o2 = model.GoalModel.GetObstaclesByName (obstacle.Name).Single ();
                o2.Merge (obstacle);
                return o2;
            }

            // Ensure that parsed obstacle has the same identifer than the new one
            // This is required for second pass, otherwise, entity could not be found
            if (identifierAttribute == null)
                parsedObstacle.Attributes.Add (new Identifier (obstacle.Identifier));

            model.GoalModel.Obstacles.Add (obstacle);

            return obstacle;
        }

        private KAOSFormalTools.Domain.Agent BuildAgent (Agent parsedAgent)
        {
            var agent = new KAOSFormalTools.Domain.Agent ();

            Identifier identifierAttribute = null;
            if (parsedAgent.Type == AgentType.Environment)
                agent.Type = KAOSFormalTools.Domain.AgentType.Environment;
            else if (parsedAgent.Type == AgentType.Software)
                agent.Type = KAOSFormalTools.Domain.AgentType.Software;

            foreach (var attr in parsedAgent.Attributes) {
                if (attr is Identifier) {
                    agent.Identifier = (attr as Identifier).Value;
                    identifierAttribute = (attr as Identifier);

                } else if (attr is Name) {
                    agent.Name = (attr as Name).Value;
                } else if (attr is Description) {
                    agent.Description = (attr as Description).Value;
                }
            }

            if (model.GoalModel.AgentExists (agent.Identifier))
                throw new ParsingException (string.Format ("Identifier '{0}' is not unique", agent.Identifier));
            
            // Ensure that parsed agent has the same identifer than the new one
            // This is required for second pass, otherwise, entity could not be found
            if (identifierAttribute == null)
                parsedAgent.Attributes.Add (new Identifier (agent.Identifier));

            model.GoalModel.Agents.Add (agent);

            return agent;
        }

        #endregion

        #region Build helpers for second pass

        private void BuildObstacleRelations (Obstacle parsedObstacle)
        {
            string identifier = "";
            string name       = "";

            var refinements   = new List<ObstacleRefinement> ();
            var resolutions   = new List<KAOSFormalTools.Domain.Goal> ();

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
                            var domprop = GetDomainProperty (child as IdentifierOrName);
                            if (domprop != null) {
                                refinement.DomainProperties.Add (domprop);
                                
                            } else {
                                var candidate = GetOrCreateObstacle (child as IdentifierOrName, true);
                                if (candidate != null)
                                    refinement.Children.Add (candidate);
                            }

                        } else if (child is Obstacle) {
                            var o = BuildObstacle (child as Obstacle);
                            refinement.Children.Add (o);
                            
                            BuildObstacleRelations (child as Obstacle);
                        }
                    }

                    if (refinement.Children.Count > 0)
                        refinements.Add (refinement);

                } else if (attribute is ResolvedByList) {
                    var children = attribute as ResolvedByList;

                    foreach (var child in children.Values) {
                        if (child is IdentifierOrName) {
                            var candidate = GetOrCreateGoal (child as IdentifierOrName, true);
                            if (candidate != null)
                                resolutions.Add (candidate);

                        } else if (child is Goal) {
                            var g = BuildGoal (child as Goal);
                            resolutions.Add (g);
                            
                            BuildGoalRelations (child as Goal);
                        }
                    }
                }
            }

            KAOSFormalTools.Domain.Obstacle obstacle = null;
            if (string.IsNullOrEmpty (identifier)) {
                var obstacles = model.GoalModel.GetObstaclesByName (name);
                if (obstacles.Count() > 1)
                    throw new ParsingException (string.Format ("Obstacle '{0}' is ambiguous", name));
                else if (obstacles.Count() == 0)
                    throw new ParsingException (string.Format ("Obstacle '{0}' not found", name));
                else 
                    obstacle = obstacles.Single ();

            } else {
                obstacle = model.GoalModel.GetObstacleByIdentifier (identifier);
                
                if (obstacle == null)
                    throw new ParsingException (string.Format ("Obstacle '{0}' not found", identifier));
            }

            foreach (var r in refinements)
                obstacle.Refinements.Add (r);

            foreach (var g in resolutions)
                obstacle.Resolutions.Add (g);
        }

        private void BuildGoalRelations (Goal parsedGoal)
        {
            string identifier    = "";
            string name          = "";
            var    refinements   = new HashSet<GoalRefinement> ();
            var    obstruction   = new HashSet<KAOSFormalTools.Domain.Obstacle> ();
            var    assignedAgents = new HashSet<KAOSFormalTools.Domain.Agent> ();

            foreach (var attribute in parsedGoal.Attributes) {
                if (attribute is Identifier) {
                    identifier = (attribute as Identifier).Value;

                } else if (attribute is Name) {
                    name = (attribute as Name).Value;

                } else if (attribute is RefinedByList) {
                    var children = attribute as RefinedByList;
                    var refinement = new GoalRefinement ();

                    refinement.AlternativeIdentifier = children.AlternativeIdentifier;

                    foreach (var child in children.Values) {
                        if (child is IdentifierOrName) {
                            var domprop = GetDomainProperty (child as IdentifierOrName);
                            if (domprop != null) {
                                refinement.DomainProperties.Add (domprop);
                                
                            } else {
                                var candidate = GetOrCreateGoal (child as IdentifierOrName, true);
                                if (candidate != null)
                                    refinement.Children.Add (candidate);
                            }

                        } else if (child is Goal) {
                            var g = BuildGoal (child as Goal);
                            refinement.Children.Add (g);
                            
                            BuildGoalRelations (child as Goal);
                        }
                    }

                    if (refinement.Children.Count > 0 | refinement.DomainProperties.Count > 0)
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
                var goals = model.GoalModel.GetGoalsByName (name);
                if (goals.Count() > 1)
                    throw new ParsingException (string.Format ("Goal '{0}' is ambiguous", name));
                else if (goals.Count() == 0)
                    throw new ParsingException (string.Format ("Goal '{0}' not found", name));
                else 
                    goal = goals.Single ();

            } else {
                goal = model.GoalModel.GetGoalByIdentifier (identifier);
                
                if (goal == null)
                    throw new ParsingException (string.Format ("Goal '{0}' not found", identifier));
            }

            if (!parsedGoal.Override) {
                foreach (var r in refinements)
                    goal.Refinements.Add (r);
                foreach (var r in obstruction)
                    goal.Obstruction.Add (r);                
                foreach (var r in assignedAgents)
                    goal.AssignedAgents.Add (r);

            } else {
                goal.Refinements = refinements;
                goal.Obstruction = obstruction;
                goal.AssignedAgents = assignedAgents;
            }
        }
    
        #endregion

        #region Get or create helpers

        private KAOSFormalTools.Domain.Obstacle GetOrCreateObstacle (IdentifierOrName attribute, bool create = true)
        {
            KAOSFormalTools.Domain.Obstacle candidate = null;

            if (attribute is Name) {
                var name = (attribute as Name).Value;
                var candidates = model.GoalModel.GetObstaclesByName (name);

                if (candidates.Count() == 0) {
                    if (create) {
                        candidate = new KAOSFormalTools.Domain.Obstacle() { 
                            Name = (attribute as Name).Value
                        };
                        model.GoalModel.Obstacles.Add (candidate);
                    } else {
                        throw new ParsingException (string.Format ("Obstacle '{0}' could not be found", (attribute as Name).Value));
                    }

                } else if (candidates.Count() > 1) {
                    candidate = candidates.First ();

                } else /* candidates.Count() == 0 */ {
                    candidate = candidates.Single ();
                }

            } else if (attribute is Identifier) {
                candidate = model.GoalModel.GetObstacleByIdentifier ((attribute as Identifier).Value);

                if (candidate == null) {
                    if (create) {
                        candidate = new KAOSFormalTools.Domain.Obstacle() { 
                            Identifier = (attribute as Identifier).Value
                        };
                        model.GoalModel.Obstacles.Add (candidate);
                    } else {
                        throw new ParsingException (string.Format ("Obstacle '{0}' could not be found", (attribute as Identifier).Value));
                    }
                }
            }

            return candidate;
        }

        private KAOSFormalTools.Domain.DomainProperty GetDomainProperty (IdentifierOrName attribute)
        {
            
            if (attribute is Name) {
                var name = (attribute as Name).Value;
                var domprop_candidate = model.GoalModel.GetDomainPropertiesByName (name);

                if (domprop_candidate.Count() > 1) {
                    return domprop_candidate.First ();
                } else if (domprop_candidate.Count() == 1) {
                    return domprop_candidate.Single ();
                }
                return null;

            } else if (attribute is Identifier) {
                var identifier = (attribute as Identifier).Value;
                return model.GoalModel.GetDomainPropertyByIdentifier (identifier);
            }

            return null;
        }

        private KAOSFormalTools.Domain.Goal GetOrCreateGoal (IdentifierOrName attribute, bool create = true)
        {
            KAOSFormalTools.Domain.Goal candidate = null;

            if (attribute is Name) {
                var name = (attribute as Name).Value;
                var candidates = model.GoalModel.GetGoalsByName (name);

                if (candidates.Count() == 0) {
                        if (create) {
                            candidate = new KAOSFormalTools.Domain.Goal() { 
                                Name = (attribute as Name).Value
                            };
                            model.GoalModel.Goals.Add (candidate);
                        } else {
                            throw new ParsingException (string.Format ("Goal '{0}' could not be found", (attribute as Name).Value));
                        }

                } else if (candidates.Count() > 1) {
                   candidate = candidates.First ();

                } else /* candidates.Count() == 0 */ {
                    candidate = candidates.Single ();
                }

            } else if (attribute is Identifier) {
                candidate = model.GoalModel.GetGoalByIdentifier ((attribute as Identifier).Value);

                if (candidate == null) {
                    if (create) {
                        candidate = new KAOSFormalTools.Domain.Goal() { 
                            Identifier = (attribute as Identifier).Value
                        };
                        model.GoalModel.Goals.Add (candidate);
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
                var candidates = model.GoalModel.GetAgentsByName (name);

                if (candidates.Count() == 0) {
                    if (create) {
                        candidate = new KAOSFormalTools.Domain.Agent() { 
                            Name = (attribute as Name).Value
                        };
                        model.GoalModel.Agents.Add (candidate);
                    } else {
                        throw new ParsingException (string.Format ("Agent '{0}' could not be found", (attribute as Name).Value));
                    }

                } else if (candidates.Count() > 1) {
                    throw new ParsingException (string.Format ("Agent '{0}' is ambiguous", (attribute as Name).Value));

                } else /* candidates.Count() == 0 */ {
                    candidate = candidates.Single ();
                }

            } else if (attribute is Identifier) {
                candidate = model.GoalModel.GetAgentByIdentifier ((attribute as Identifier).Value);

                if (candidate == null) {
                    if (create) {
                        candidate = new KAOSFormalTools.Domain.Agent () { 
                            Identifier = (attribute as Identifier).Value
                        };
                        model.GoalModel.Agents.Add (candidate);
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

