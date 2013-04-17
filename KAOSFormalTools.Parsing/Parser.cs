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
                    
                } else if (element is DomainHypothesis) {
                    BuildDomainHypothesis (element as DomainHypothesis);

                } else if (element is Obstacle) {
                    BuildObstacle (element as Obstacle);

                } else if (element is Agent) {
                    BuildAgent (element as Agent);

                } else if (element is Predicate) {
                    BuildPredicate (element as Predicate);

                } else if (element is System) {
                    BuildSystem (element as System);
                    
                } else {
                    throw new NotImplementedException (string.Format("'{0}' not yet implemented", element.GetType ().Name));
                }
            }
        }

        private void SecondPass (Elements elements) {
            foreach (var element in elements.Values) {
                if (element is Obstacle) {
                    BuildObstacleRelations (element as Obstacle);

                } else if (element is Goal) {
                    BuildGoalRelations (element as Goal);

                } else if (element is System) {
                    BuildSystemRelations (element as System);

                }
            }
        }
        
        #region Build helpers for first pass

        private KAOSFormalTools.Domain.System BuildSystem (System parsedAlternative) 
        {
            var alternative = new KAOSFormalTools.Domain.System ();
            Identifier identifierAttribute = null;
            
            foreach (var attribute in parsedAlternative.Attributes) {
                if (attribute is Identifier) {
                    alternative.Identifier = (attribute as Identifier).Value;
                    identifierAttribute = (attribute as Identifier);
                    
                } else if (attribute is Name) {
                    alternative.Name = (attribute as Name).Value;
                    
                } else if (attribute is Description) {
                    alternative.Description = (attribute as Description).Value;
                    
                }
            }

            if (model.GoalModel.SystemExists (alternative.Identifier)) {
                if (parsedAlternative.Override) {
                    var s2 = model.GoalModel.GetSystemByIdentifier (alternative.Identifier);
                    s2.Merge (alternative);
                    return s2;
                } else {
                    throw new ParsingException (string.Format ("System '{0}' is declared multiple times", alternative.Identifier));
                }
            }
            
            if (identifierAttribute == null && model.GoalModel.GetSystemsByName (alternative.Name).Count() == 1) {
                if (parsedAlternative.Override) {
                    var s2 = model.GoalModel.GetSystemsByName (alternative.Name).Single ();
                    s2.Merge (alternative);
                    return s2;
                } else {
                    throw new ParsingException (string.Format ("System '{0}' is declared multiple times", alternative.Identifier));
                }
            }

            model.GoalModel.Systems.Add (alternative);

            return alternative;
        }

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
                if (parsedGoal.Override) {
                    g2.Merge (goal);
                    return g2;
                } else {
                    throw new ParsingException (string.Format ("Goal '{0}' is declared multiple times", goal.Identifier));
                }
            }
            
            if (identifierAttribute == null && model.GoalModel.GetGoalsByName (goal.Name).Count() == 1) {
                var g2 = model.GoalModel.GetGoalsByName (goal.Name).Single ();
                if (parsedGoal.Override) {
                    g2.Merge (goal);
                    return g2;
                } else {
                    throw new ParsingException (string.Format ("Goal '{0}' is declared multiple times", goal.Name));
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
                if (parsedDomProp.Override) {
                    var d2 = model.GoalModel.GetDomainPropertyByIdentifier (domprop.Identifier);
                    d2.Merge (domprop);
                    return d2;
                } else {
                    throw new ParsingException (string.Format ("Domain property '{0}' is declared multiple times", domprop.Identifier));
                }
            }
            
            if (identifierAttribute == null && model.GoalModel.GetDomainPropertiesByName (domprop.Name).Count() == 1) {
                if (parsedDomProp.Override) {
                    var d2 = model.GoalModel.GetDomainPropertiesByName (domprop.Name).Single ();
                    d2.Merge (domprop);
                    return d2;
                } else {
                    throw new ParsingException (string.Format ("Domain property '{0}' is declared multiple times", domprop.Name));
                }
            }

            // Ensure that parsed domprop has the same identifer than the new one
            // This is required for second pass, otherwise, entity could not be found
            if (identifierAttribute == null)
                parsedDomProp.Attributes.Add (new Identifier (domprop.Identifier));

            model.GoalModel.DomainProperties.Add (domprop);

            return domprop;
        }

        private KAOSFormalTools.Domain.DomainHypothesis BuildDomainHypothesis (DomainHypothesis parsedDomHyp)
        {
            var domHyp = new KAOSFormalTools.Domain.DomainHypothesis();
            Identifier identifierAttribute = null;
            
            foreach (var attr in parsedDomHyp.Attributes) {
                if (attr is Identifier) {
                    domHyp.Identifier = (attr as Identifier).Value;
                    identifierAttribute = (attr as Identifier);
                    
                } else if (attr is Name) {
                    domHyp.Name = (attr as Name).Value;

                } else if (attr is Definition) {
                    domHyp.Definition = (attr as Definition).Value;

                }
            }
            
            if (model.GoalModel.DomainHypothesisExists (domHyp.Identifier)) {
                if (parsedDomHyp.Override) {
                    var d2 = model.GoalModel.GetDomainHypothesisByIdentifier (domHyp.Identifier);
                    d2.Merge (domHyp);
                    return d2;
                } else {
                    throw new ParsingException (string.Format ("Domain hypothesis '{0}' is declared multiple times", domHyp.Identifier));
                }
            }
            
            if (identifierAttribute == null && model.GoalModel.GetDomainHypothesesByName (domHyp.Name).Count() == 1) {
                if (parsedDomHyp.Override) {
                    var d2 = model.GoalModel.GetDomainHypothesesByName (domHyp.Name).Single ();
                    d2.Merge (domHyp);
                    return d2;
                } else {
                    throw new ParsingException (string.Format ("Domain hypothesis '{0}' is declared multiple times", domHyp.Name));
                }
            }
            
            // Ensure that parsed domhyp has the same identifer than the new one
            // This is required for second pass, otherwise, entity could not be found
            if (identifierAttribute == null)
                parsedDomHyp.Attributes.Add (new Identifier (domHyp.Identifier));
            
            model.GoalModel.DomainHypotheses.Add (domHyp);
            
            return domHyp;
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
                if (parsedObstacle.Override) {
                    var o2 = model.GoalModel.GetObstacleByIdentifier (obstacle.Identifier);
                    o2.Merge (obstacle);
                    return o2;
                } else {
                    throw new ParsingException (string.Format ("Obstacle '{0}' is declared multiple times", obstacle.Identifier));
                }
            }

            if (identifierAttribute == null && model.GoalModel.GetObstaclesByName (obstacle.Name).Count() == 1) {
                if (parsedObstacle.Override) {
                    var o2 = model.GoalModel.GetObstaclesByName (obstacle.Name).Single ();
                    o2.Merge (obstacle);
                    return o2;
                } else {
                    throw new ParsingException (string.Format ("Obstacle '{0}' is declared multiple times", obstacle.Name));
                }
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

            if (model.GoalModel.AgentExists (agent.Identifier)) {
                if (parsedAgent.Override) {
                    var o2 = model.GoalModel.GetAgentByIdentifier (agent.Identifier);
                    o2.Merge (agent);
                    return o2;
                } else {
                    throw new ParsingException (string.Format ("Agent '{0}' is declared multiple times", agent.Identifier));
                }
            }
            
            if (identifierAttribute == null && model.GoalModel.GetAgentsByName (agent.Name).Count() == 1) {
                if (parsedAgent.Override) {
                    var o2 = model.GoalModel.GetAgentsByName (agent.Name).Single ();
                    o2.Merge (agent);
                    return o2;
                } else {
                    throw new ParsingException (string.Format ("Obstacle '{0}' is declared multiple times", agent.Name));
                }
            }   
            
            // Ensure that parsed agent has the same identifer than the new one
            // This is required for second pass, otherwise, entity could not be found
            if (identifierAttribute == null)
                parsedAgent.Attributes.Add (new Identifier (agent.Identifier));

            model.GoalModel.Agents.Add (agent);

            return agent;
        }

        private KAOSFormalTools.Domain.Predicate BuildPredicate (Predicate parsedPredicate)
        {
            var predicate = new KAOSFormalTools.Domain.Predicate ();
            
            Identifier identifierAttribute = null;

            foreach (var attr in parsedPredicate.Attributes) {
                if (attr is Name) {
                    predicate.Name = (attr as Name).Value;
                } else if (attr is Definition) {
                    predicate.Definition = (attr as Definition).Value;
                } else if (attr is StringFormalSpec) {
                    predicate.FormalSpec = (attr as StringFormalSpec).Value;
                } else if (attr is Signature) {
                    predicate.Signature = (attr as Signature).Value;
                }
            }
            
            if (string.IsNullOrWhiteSpace (predicate.Name))
                throw new ParsingException ("Predicate shall have a name");

            if (model.Predicates.ContainsKey (predicate.Name))
                throw new ParsingException (string.Format ("Predicate '{0}' is not unique", predicate.Name));

            if (parsedPredicate.Override)
                throw new NotImplementedException ();

            model.Predicates.Add (predicate.Name, predicate);
            
            return predicate;
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
            var    assignedAgents = new HashSet<KAOSFormalTools.Domain.AgentAssignment> ();

            foreach (var attribute in parsedGoal.Attributes) {
                if (attribute is Identifier) {
                    identifier = (attribute as Identifier).Value;

                } else if (attribute is Name) {
                    name = (attribute as Name).Value;

                } else if (attribute is RefinedByList) {
                    var children = attribute as RefinedByList;
                    var refinement = new GoalRefinement ();

                    if (children.SystemIdentifier != null)
                        refinement.SystemIdentifier = GetOrCreateAlternative (children.SystemIdentifier, true);

                    foreach (var child in children.Values) {
                        if (child is IdentifierOrName) {
                            var domprop = GetDomainProperty (child as IdentifierOrName);
                            var domhyp = GetDomainHypothesis (child as IdentifierOrName);
                            if (domprop != null) {
                                refinement.DomainProperties.Add (domprop);
                            
                            } else if (domhyp != null) {
                                refinement.DomainHypotheses.Add (domhyp);

                            } else {
                                var candidate = GetOrCreateGoal (child as IdentifierOrName, true);
                                if (candidate != null)
                                    refinement.Children.Add (candidate);
                            }

                        } else if (child is Goal) {
                            var g = BuildGoal (child as Goal);
                            refinement.Children.Add (g);
                            
                            BuildGoalRelations (child as Goal);

                        } else if (child is DomainProperty) {
                            var g = BuildDomainProperty (child as DomainProperty);
                            refinement.DomainProperties.Add (g);

                        } else if (child is DomainHypothesis) {
                            var g = BuildDomainHypothesis (child as DomainHypothesis);
                            refinement.DomainHypotheses.Add (g);

                        }
                    }

                    if ((refinement.Children.Count + refinement.DomainHypotheses.Count + refinement.DomainProperties.Count) > 0)
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
                    var assignment = new AgentAssignment();

                    if ((attribute as AssignedToList).SystemIdentifier != null)
                        assignment.AlternativeIdentifier = GetOrCreateAlternative ((attribute as AssignedToList).SystemIdentifier);

                    foreach (var assignedto in (attribute as AssignedToList).Values) {
                        if (assignedto is IdentifierOrName) {
                            var candidate = GetOrCreateAgent (assignedto as IdentifierOrName, true);
                            if (candidate != null)
                                assignment.Agents.Add (candidate);
                        } else if (assignedto is Agent) {
                            var a = BuildAgent (assignedto as Agent);
                            assignment.Agents.Add (a);
                        }
                    }
                    assignedAgents.Add (assignment);
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

            if (parsedGoal.Override) {
                foreach (var r in refinements)
                    goal.Refinements.Add (r);
                foreach (var r in obstruction)
                    goal.Obstruction.Add (r);                
                foreach (var r in assignedAgents) {
                    goal.AssignedAgents.Add (r);
                }

            } else {
                goal.Refinements = refinements;
                goal.Obstruction = obstruction;
                goal.AssignedAgents = assignedAgents;
            }
        }

        private void BuildSystemRelations (System parsedSystem)
        {
            string identifier    = "";
            string name          = "";
            var    alternatives  = new HashSet<Domain.System> ();

            foreach (var attribute in parsedSystem.Attributes) {
                if (attribute is Identifier) {
                    identifier = (attribute as Identifier).Value;
                    
                } else if (attribute is Name) {
                    name = (attribute as Name).Value;
                    
                } else if (attribute is AlternativeList) {
                    foreach (var child in (attribute as AlternativeList).Values) {
                        if (child is IdentifierOrName) {
                            var candidate = GetOrCreateAlternative (child as IdentifierOrName, true);
                            if (candidate != null)
                                alternatives.Add (candidate);
                        } else if (child is System) {
                            var s = BuildSystem (child as System);
                            alternatives.Add (s);
                            BuildSystemRelations (child as System);
                        }
                    }
                }
            }
            
            KAOSFormalTools.Domain.System system = null;
            if (string.IsNullOrEmpty (identifier)) {
                var goals = model.GoalModel.GetSystemsByName (name);
                if (goals.Count() > 1)
                    throw new ParsingException (string.Format ("System '{0}' is ambiguous", name));
                else if (goals.Count() == 0)
                    throw new ParsingException (string.Format ("System '{0}' not found", name));
                else 
                    system = goals.Single ();
                
            } else {
                system = model.GoalModel.GetSystemByIdentifier (identifier);
                
                if (system == null)
                    throw new ParsingException (string.Format ("Sytem '{0}' not found", identifier));
            }
            
            if (!parsedSystem.Override) {
                foreach (var r in alternatives)
                    system.Alternatives.Add (r);

            } else {
                system.Alternatives = alternatives;
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

        private KAOSFormalTools.Domain.DomainHypothesis GetDomainHypothesis (IdentifierOrName attribute)
        {
            
            if (attribute is Name) {
                var name = (attribute as Name).Value;
                var domhyp_candidate = model.GoalModel.GetDomainHypothesesByName (name);
                
                if (domhyp_candidate.Count() > 1) {
                    return domhyp_candidate.First ();
                } else if (domhyp_candidate.Count() == 1) {
                    return domhyp_candidate.Single ();
                }
                return null;
                
            } else if (attribute is Identifier) {
                var identifier = (attribute as Identifier).Value;
                return model.GoalModel.GetDomainHypothesisByIdentifier (identifier);
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

        private KAOSFormalTools.Domain.System GetOrCreateAlternative (IdentifierOrName attribute, bool create = true)
        {
            KAOSFormalTools.Domain.System candidate = null;
            
            if (attribute is Name) {
                var name = (attribute as Name).Value;
                var candidates = model.GoalModel.Systems.Where (a => a.Name == name);
                
                if (candidates.Count() == 0) {
                    if (create) {
                        candidate = new KAOSFormalTools.Domain.System() { 
                            Name = (attribute as Name).Value
                        };
                        model.GoalModel.Systems.Add (candidate);
                    } else {
                        throw new ParsingException (string.Format ("Alternative '{0}' could not be found", (attribute as Name).Value));
                    }
                    
                } else if (candidates.Count() > 1) {
                    candidate = candidates.First ();
                    
                } else /* candidates.Count() == 1 */ {
                    candidate = candidates.Single ();
                }
                
            } else if (attribute is Identifier) {
                candidate = model.GoalModel.Systems.Where (a => a.Identifier == ((attribute as Identifier).Value)).SingleOrDefault ();
                
                if (candidate == null) {
                    if (create) {
                        candidate = new KAOSFormalTools.Domain.System() { 
                            Identifier = (attribute as Identifier).Value
                        };
                        model.GoalModel.Systems.Add (candidate);

                    } else {
                        throw new ParsingException (string.Format ("Alternative '{0}' could not be found", (attribute as Identifier).Value));
                    }
                }
            }
            
            return candidate;
        }

        #endregion
    }
}

