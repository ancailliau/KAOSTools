using System;
using System.Linq;
using System.Collections.Generic;
using KAOSTools.Parsing;
using KAOSTools.MetaModel;
using System.Text.RegularExpressions;
using System.IO;

namespace KAOSTools.Parsing
{
    public class Declaration
    {
        public static Uri RelativePath { get; set; }
        public int    Line     { get; set; }
        public int    Col      { get; set; }
        public string Filename { get; set; }
        public Declaration (int line, int col, string filename)
        {
            this.Line = line; this.Col = col; 
            this.Filename = RelativePath.MakeRelativeUri (new Uri(Path.GetFullPath (filename))).ToString ();
        }
    }

    public class Parser
    {
        private GoalModelParser _parser = new GoalModelParser ();
        private KAOSModel model;
        public IDictionary<KAOSMetaModelElement, IList<Declaration>> Declarations;

        public Parser (){}

        public KAOSModel Parse (string input, string filename, KAOSModel model)
        {
            Elements elements = null;
            Declarations = new Dictionary<KAOSMetaModelElement, IList<Declaration>> ();
            Declaration.RelativePath = new Uri(Path.GetFullPath (filename));

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

        private KAOSTools.MetaModel.System BuildSystem (System parsedAlternative) 
        {
            var alternative = new KAOSTools.MetaModel.System ();
            Identifier identifierAttribute = null;
            
            foreach (var attribute in parsedAlternative.Attributes) {
                if (attribute is Identifier) {
                    alternative.Identifier = (attribute as Identifier).Value;
                    identifierAttribute = (attribute as Identifier);
                    
                } else if (attribute is Name) {
                    alternative.Name = Sanitize((attribute as Name).Value);
                    
                } else if (attribute is Description) {
                    alternative.Description = Sanitize((attribute as Description).Value);
                    
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

        private KAOSTools.MetaModel.Goal BuildGoal (Goal parsedGoal)
        {
            var goal = new KAOSTools.MetaModel.Goal ();
            Identifier identifierAttribute = null;

            foreach (var attribute in parsedGoal.Attributes) {
                if (attribute is Identifier) {
                    goal.Identifier = (attribute as Identifier).Value;
                    identifierAttribute = (attribute as Identifier);

                } else if (attribute is Name) {
                    goal.Name = Sanitize((attribute as Name).Value);

                } else if (attribute is FormalSpec) {
                    goal.FormalSpec = (attribute as FormalSpec).Value;
                
                } else if (attribute is Definition) {
                    goal.Definition = Sanitize((attribute as Definition).Value);

                } else if (attribute is RDS) {
                    goal.RDS = (attribute as RDS).Value;
                }
            }

            if (model.GoalModel.GoalExists (goal.Identifier)) {
                var g2 = model.GoalModel.GetGoalByIdentifier (goal.Identifier);
                if (parsedGoal.Override) {
                    Declarations[g2].Add (new Declaration (parsedGoal.Line, parsedGoal.Col, parsedGoal.Filename));
                    g2.Merge (goal);
                    return g2;
                } else {
                    var declaration = Declarations [g2].First ();
                    throw new DuplicateDeclarationException (string.Format ("Goal '{0}'", goal.Identifier),
                                                             declaration.Filename, declaration.Line, declaration.Col,
                                                             parsedGoal.Filename, parsedGoal.Line, parsedGoal.Col);
                }
            }
            
            if (identifierAttribute == null && model.GoalModel.GetGoalsByName (goal.Name).Count() == 1) {
                var g2 = model.GoalModel.GetGoalsByName (goal.Name).Single ();
                if (parsedGoal.Override) {
                    Declarations[g2].Add (new Declaration (parsedGoal.Line, parsedGoal.Col, parsedGoal.Filename));
                    g2.Merge (goal);
                    return g2;
                } else {
                    var declaration = Declarations [g2].First ();
                    throw new DuplicateDeclarationException (string.Format ("Goal '{0}'", goal.Name),
                                                               declaration.Filename, declaration.Line, declaration.Col,
                                                               parsedGoal.Filename, parsedGoal.Line, parsedGoal.Col);
                }
            }

            // Ensure that parsed goal has the same identifer than the new one
            // This is required for second pass, otherwise, entity could not be found
            if (identifierAttribute == null)
                parsedGoal.Attributes.Add (new Identifier (goal.Identifier));

            model.GoalModel.Goals.Add (goal);
            Declarations.Add (goal, new List<Declaration> { new Declaration (parsedGoal.Line, parsedGoal.Col, parsedGoal.Filename) });

            return goal;
        }

        private KAOSTools.MetaModel.DomainProperty BuildDomainProperty (DomainProperty parsedDomProp)
        {
            var domprop = new KAOSTools.MetaModel.DomainProperty();
            Identifier identifierAttribute = null;

            foreach (var attr in parsedDomProp.Attributes) {
                if (attr is Identifier) {
                    domprop.Identifier = (attr as Identifier).Value;
                    identifierAttribute = (attr as Identifier);

                } else if (attr is Name) {
                    domprop.Name = Sanitize((attr as Name).Value);

                } else if (attr is FormalSpec) {
                    domprop.FormalSpec = (attr as FormalSpec).Value;

                } else if (attr is Definition) {
                    domprop.Definition = Sanitize((attr as Definition).Value);

                } else if (attr is Probability) {
                    domprop.EPS = (attr as Probability).Value;
                }
            }

            if (model.GoalModel.DomainPropertyExists (domprop.Identifier)) {
                var d2 = model.GoalModel.GetDomainPropertyByIdentifier (domprop.Identifier);
                if (parsedDomProp.Override) {
                    Declarations[d2].Add (new Declaration (parsedDomProp.Line, parsedDomProp.Col, parsedDomProp.Filename));
                    d2.Merge (domprop);
                    return d2;
                } else {
                    var declaration = Declarations [d2].First ();
                    throw new DuplicateDeclarationException (string.Format ("Domain property '{0}'", domprop.Identifier),
                                                             declaration.Filename, declaration.Line, declaration.Col,
                                                             parsedDomProp.Filename, parsedDomProp.Line, parsedDomProp.Col);
                }
            }
            
            if (identifierAttribute == null && model.GoalModel.GetDomainPropertiesByName (domprop.Name).Count() == 1) {
                var d2 = model.GoalModel.GetDomainPropertiesByName (domprop.Name).Single ();
                if (parsedDomProp.Override) {
                    Declarations[d2].Add (new Declaration (parsedDomProp.Line, parsedDomProp.Col, parsedDomProp.Filename));
                    d2.Merge (domprop);
                    return d2;
                } else {
                    var declaration = Declarations [d2].First ();
                    throw new DuplicateDeclarationException (string.Format ("Domain property '{0}'", domprop.Name),
                                                             declaration.Filename, declaration.Line, declaration.Col,
                                                             parsedDomProp.Filename, parsedDomProp.Line, parsedDomProp.Col);
                }
            }

            // Ensure that parsed domprop has the same identifer than the new one
            // This is required for second pass, otherwise, entity could not be found
            if (identifierAttribute == null)
                parsedDomProp.Attributes.Add (new Identifier (domprop.Identifier));

            model.GoalModel.DomainProperties.Add (domprop);
            Declarations.Add (domprop, new List<Declaration> { new Declaration (parsedDomProp.Line, parsedDomProp.Col, parsedDomProp.Filename) });

            return domprop;
        }

        private KAOSTools.MetaModel.DomainHypothesis BuildDomainHypothesis (DomainHypothesis parsedDomHyp)
        {
            var domHyp = new KAOSTools.MetaModel.DomainHypothesis();
            Identifier identifierAttribute = null;
            
            foreach (var attr in parsedDomHyp.Attributes) {
                if (attr is Identifier) {
                    domHyp.Identifier = (attr as Identifier).Value;
                    identifierAttribute = (attr as Identifier);
                    
                } else if (attr is Name) {
                    domHyp.Name = Sanitize((attr as Name).Value);

                } else if (attr is Definition) {
                    domHyp.Definition = Sanitize((attr as Definition).Value);

                }
            }
            
            if (model.GoalModel.DomainHypothesisExists (domHyp.Identifier)) {
                var d2 = model.GoalModel.GetDomainHypothesisByIdentifier (domHyp.Identifier);
                if (parsedDomHyp.Override) {
                    Declarations[d2].Add (new Declaration (parsedDomHyp.Line, parsedDomHyp.Col, parsedDomHyp.Filename));
                    d2.Merge (domHyp);
                    return d2;
                } else {
                    var declaration = Declarations [d2].First ();
                    throw new DuplicateDeclarationException (string.Format ("Domain hypothesis '{0}'", domHyp.Identifier),
                                                             declaration.Filename, declaration.Line, declaration.Col,
                                                             parsedDomHyp.Filename, parsedDomHyp.Line, parsedDomHyp.Col);
                }
            }
            
            if (identifierAttribute == null && model.GoalModel.GetDomainHypothesesByName (domHyp.Name).Count() == 1) {
                var d2 = model.GoalModel.GetDomainHypothesesByName (domHyp.Name).Single ();
                if (parsedDomHyp.Override) {
                    Declarations[d2].Add (new Declaration (parsedDomHyp.Line, parsedDomHyp.Col, parsedDomHyp.Filename));
                    d2.Merge (domHyp);
                    return d2;
                } else {
                    var declaration = Declarations [d2].First ();
                    throw new DuplicateDeclarationException (string.Format ("Domain hypothesis '{0}'", domHyp.Name),
                                                             declaration.Filename, declaration.Line, declaration.Col,
                                                             parsedDomHyp.Filename, parsedDomHyp.Line, parsedDomHyp.Col);
                }
            }
            
            // Ensure that parsed domhyp has the same identifer than the new one
            // This is required for second pass, otherwise, entity could not be found
            if (identifierAttribute == null)
                parsedDomHyp.Attributes.Add (new Identifier (domHyp.Identifier));
            
            model.GoalModel.DomainHypotheses.Add (domHyp);
            Declarations.Add (domHyp, new List<Declaration> { new Declaration (parsedDomHyp.Line, parsedDomHyp.Col, parsedDomHyp.Filename) });

            return domHyp;
        }

        private KAOSTools.MetaModel.Obstacle BuildObstacle (Obstacle parsedObstacle)
        {
            var obstacle = new KAOSTools.MetaModel.Obstacle();
            Identifier identifierAttribute = null;

            foreach (var attr in parsedObstacle.Attributes) {
                if (attr is Identifier) {
                    obstacle.Identifier = (attr as Identifier).Value;
                    identifierAttribute = (attr as Identifier);

                } else if (attr is Name) {
                    obstacle.Name = Sanitize((attr as Name).Value);

                } else if (attr is FormalSpec) {
                    obstacle.FormalSpec = (attr as FormalSpec).Value;

                } else if (attr is Definition) {
                    obstacle.Definition = Sanitize((attr as Definition).Value);

                } else if (attr is Probability) {
                    obstacle.EPS = (attr as Probability).Value;
                }
            }

            if (model.GoalModel.ObstacleExists (obstacle.Identifier)) {
                var o2 = model.GoalModel.GetObstacleByIdentifier (obstacle.Identifier);
                if (parsedObstacle.Override) {
                    Declarations[o2].Add (new Declaration (parsedObstacle.Line, parsedObstacle.Col, parsedObstacle.Filename));
                    o2.Merge (obstacle);
                    return o2;
                } else {
                    var declaration = Declarations [o2].First ();
                    throw new DuplicateDeclarationException (string.Format ("Obstacle '{0}'", obstacle.Identifier),
                                                             declaration.Filename, declaration.Line, declaration.Col,
                                                             parsedObstacle.Filename, parsedObstacle.Line, parsedObstacle.Col);
                }
            }

            if (identifierAttribute == null && model.GoalModel.GetObstaclesByName (obstacle.Name).Count() == 1) {
                var o2 = model.GoalModel.GetObstaclesByName (obstacle.Name).Single ();
                if (parsedObstacle.Override) {
                    Declarations[o2].Add (new Declaration (parsedObstacle.Line, parsedObstacle.Col, parsedObstacle.Filename));
                    o2.Merge (obstacle);
                    return o2;
                } else {
                    var declaration = Declarations [o2].First ();
                    throw new DuplicateDeclarationException (string.Format ("Obstacle '{0}'", obstacle.Name),
                                                             declaration.Filename, declaration.Line, declaration.Col,
                                                             parsedObstacle.Filename, parsedObstacle.Line, parsedObstacle.Col);
                }
            }

            // Ensure that parsed obstacle has the same identifer than the new one
            // This is required for second pass, otherwise, entity could not be found
            if (identifierAttribute == null)
                parsedObstacle.Attributes.Add (new Identifier (obstacle.Identifier));

            model.GoalModel.Obstacles.Add (obstacle);
            Declarations.Add (obstacle, new List<Declaration> { new Declaration (parsedObstacle.Line, parsedObstacle.Col, parsedObstacle.Filename) });

            return obstacle;
        }

        private KAOSTools.MetaModel.Agent BuildAgent (Agent parsedAgent)
        {
            var agent = new KAOSTools.MetaModel.Agent ();

            Identifier identifierAttribute = null;
            if (parsedAgent.Type == AgentType.Environment)
                agent.Type = KAOSTools.MetaModel.AgentType.Environment;
            else if (parsedAgent.Type == AgentType.Software)
                agent.Type = KAOSTools.MetaModel.AgentType.Software;

            foreach (var attr in parsedAgent.Attributes) {
                if (attr is Identifier) {
                    agent.Identifier = (attr as Identifier).Value;
                    identifierAttribute = (attr as Identifier);

                } else if (attr is Name) {
                    agent.Name = Sanitize((attr as Name).Value);
                } else if (attr is Description) {
                    agent.Description = Sanitize((attr as Description).Value);
                }
            }

            if (model.GoalModel.AgentExists (agent.Identifier)) {
                var o2 = model.GoalModel.GetAgentByIdentifier (agent.Identifier);
                if (parsedAgent.Override) {
                    Declarations[o2].Add (new Declaration (parsedAgent.Line, parsedAgent.Col, parsedAgent.Filename));
                    o2.Merge (agent);
                    return o2;
                } else {
                    var declaration = Declarations [o2].First ();
                    throw new DuplicateDeclarationException (string.Format ("Agent '{0}'", agent.Identifier),
                                                             declaration.Filename, declaration.Line, declaration.Col,
                                                             parsedAgent.Filename, parsedAgent.Line, parsedAgent.Col);
                }
            }
            
            if (identifierAttribute == null && model.GoalModel.GetAgentsByName (agent.Name).Count() == 1) {
                var o2 = model.GoalModel.GetAgentsByName (agent.Name).Single ();
                if (parsedAgent.Override) {
                    Declarations[o2].Add (new Declaration (parsedAgent.Line, parsedAgent.Col, parsedAgent.Filename));
                    o2.Merge (agent);
                    return o2;
                } else {
                    var declaration = Declarations [o2].First ();
                    throw new DuplicateDeclarationException (string.Format ("Agent '{0}'", agent.Name),
                                                             declaration.Filename, declaration.Line, declaration.Col,
                                                             parsedAgent.Filename, parsedAgent.Line, parsedAgent.Col);
                }
            }   
            
            // Ensure that parsed agent has the same identifer than the new one
            // This is required for second pass, otherwise, entity could not be found
            if (identifierAttribute == null)
                parsedAgent.Attributes.Add (new Identifier (agent.Identifier));

            model.GoalModel.Agents.Add (agent);
            Declarations.Add (agent, new List<Declaration> { new Declaration (parsedAgent.Line, parsedAgent.Col, parsedAgent.Filename) });

            return agent;
        }

        private KAOSTools.MetaModel.Predicate BuildPredicate (Predicate parsedPredicate)
        {
            var predicate = new KAOSTools.MetaModel.Predicate ();
            
            Identifier identifierAttribute = null;

            foreach (var attr in parsedPredicate.Attributes) {
                if (attr is Name) {
                    predicate.Name = Sanitize((attr as Name).Value);
                } else if (attr is Definition) {
                    predicate.Definition = Sanitize((attr as Definition).Value);
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
            Declarations.Add (predicate, new List<Declaration> { new Declaration (parsedPredicate.Line, parsedPredicate.Col, parsedPredicate.Filename) });

            return predicate;
        }

        #endregion

        #region Build helpers for second pass

        private void BuildObstacleRelations (Obstacle parsedObstacle)
        {
            string identifier = "";
            string name       = "";

            var refinements   = new List<ObstacleRefinement> ();
            var resolutions   = new List<KAOSTools.MetaModel.Goal> ();

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
                                    refinement.Subobstacles.Add (candidate);
                            }

                        } else if (child is Obstacle) {
                            var o = BuildObstacle (child as Obstacle);
                            refinement.Subobstacles.Add (o);
                            
                            BuildObstacleRelations (child as Obstacle);
                        }
                    }

                    if (refinement.Subobstacles.Count > 0)
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

            KAOSTools.MetaModel.Obstacle obstacle = null;
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
            var    obstruction   = new HashSet<KAOSTools.MetaModel.Obstacle> ();
            var    assignedAgents = new HashSet<KAOSTools.MetaModel.AgentAssignment> ();

            foreach (var attribute in parsedGoal.Attributes) {
                if (attribute is Identifier) {
                    identifier = (attribute as Identifier).Value;

                } else if (attribute is Name) {
                    name = (attribute as Name).Value;

                } else if (attribute is RefinedByList) {
                    var children = attribute as RefinedByList;
                    var refinement = new GoalRefinement ();

                    if (children.SystemIdentifier != null)
                        refinement.SystemReference = GetOrCreateAlternative (children.SystemIdentifier, true);

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
                                    refinement.Subgoals.Add (candidate);
                            }

                        } else if (child is Goal) {
                            var g = BuildGoal (child as Goal);
                            refinement.Subgoals.Add (g);
                            
                            BuildGoalRelations (child as Goal);

                        } else if (child is DomainProperty) {
                            var g = BuildDomainProperty (child as DomainProperty);
                            refinement.DomainProperties.Add (g);

                        } else if (child is DomainHypothesis) {
                            var g = BuildDomainHypothesis (child as DomainHypothesis);
                            refinement.DomainHypotheses.Add (g);

                        }
                    }

                    if ((refinement.Subgoals.Count + refinement.DomainHypotheses.Count + refinement.DomainProperties.Count) > 0)
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
                        assignment.SystemReference = GetOrCreateAlternative ((attribute as AssignedToList).SystemIdentifier);

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

            KAOSTools.MetaModel.Goal goal = null;
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
                    goal.Obstructions.Add (r);                
                foreach (var r in assignedAgents) {
                    goal.AgentAssignments.Add (r);
                }

            } else {
                goal.Refinements = refinements;
                goal.Obstructions = obstruction;
                goal.AgentAssignments = assignedAgents;
            }
        }

        private void BuildSystemRelations (System parsedSystem)
        {
            string identifier    = "";
            string name          = "";
            var    alternatives  = new HashSet<KAOSTools.MetaModel.System> ();

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
            
            KAOSTools.MetaModel.System system = null;
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

        private KAOSTools.MetaModel.Obstacle GetOrCreateObstacle (IdentifierOrName attribute, bool create = true)
        {
            KAOSTools.MetaModel.Obstacle candidate = null;

            if (attribute is Name) {
                var name = (attribute as Name).Value;
                var candidates = model.GoalModel.GetObstaclesByName (name);

                if (candidates.Count() == 0) {
                    if (create) {
                        candidate = new KAOSTools.MetaModel.Obstacle() { 
                            Name = (attribute as Name).Value,
                            Implicit = true
                        };
                        model.GoalModel.Obstacles.Add (candidate);
                        Declarations.Add (candidate, new List<Declaration> { new Declaration (attribute.Line, attribute.Col, attribute.Filename) });

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
                        candidate = new KAOSTools.MetaModel.Obstacle() { 
                            Identifier = (attribute as Identifier).Value,
                            Implicit = true
                        };
                        model.GoalModel.Obstacles.Add (candidate);
                        Declarations.Add (candidate, new List<Declaration> { new Declaration (attribute.Line, attribute.Col, attribute.Filename) });
                    } else {
                        throw new ParsingException (string.Format ("Obstacle '{0}' could not be found", (attribute as Identifier).Value));
                    }
                }
            }

            return candidate;
        }

        private KAOSTools.MetaModel.DomainProperty GetDomainProperty (IdentifierOrName attribute)
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
                var candidate = model.GoalModel.GetDomainPropertyByIdentifier (identifier);
                return candidate;
            }

            return null;
        }

        private KAOSTools.MetaModel.DomainHypothesis GetDomainHypothesis (IdentifierOrName attribute)
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

        private KAOSTools.MetaModel.Goal GetOrCreateGoal (IdentifierOrName attribute, bool create = true)
        {
            KAOSTools.MetaModel.Goal candidate = null;

            if (attribute is Name) {
                var name = (attribute as Name).Value;
                var candidates = model.GoalModel.GetGoalsByName (name);

                if (candidates.Count() == 0) {
                        if (create) {
                            candidate = new KAOSTools.MetaModel.Goal() { 
                                Name = (attribute as Name).Value,
                                Implicit = true
                            };
                            model.GoalModel.Goals.Add (candidate);
                            Declarations.Add (candidate, new List<Declaration> { new Declaration (attribute.Line, attribute.Col, attribute.Filename) });

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
                        candidate = new KAOSTools.MetaModel.Goal() { 
                            Identifier = (attribute as Identifier).Value,
                            Implicit = true
                        };
                        model.GoalModel.Goals.Add (candidate);
                        Declarations.Add (candidate, new List<Declaration> { new Declaration (attribute.Line, attribute.Col, attribute.Filename) });

                    } else {
                        throw new ParsingException (string.Format ("Goal '{0}' could not be found", (attribute as Identifier).Value));
                    }
                }
            }

            return candidate;
        }

        private KAOSTools.MetaModel.Agent GetOrCreateAgent (IdentifierOrName attribute, bool create = true)
        {
            KAOSTools.MetaModel.Agent candidate = null;

            if (attribute is Name) {
                var name = (attribute as Name).Value;
                var candidates = model.GoalModel.GetAgentsByName (name);

                if (candidates.Count() == 0) {
                    if (create) {
                        candidate = new KAOSTools.MetaModel.Agent() { 
                            Name = (attribute as Name).Value,
                            Implicit = true
                        };
                        model.GoalModel.Agents.Add (candidate);
                        Declarations.Add (candidate, new List<Declaration> { new Declaration (attribute.Line, attribute.Col, attribute.Filename) });

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
                        candidate = new KAOSTools.MetaModel.Agent () { 
                            Identifier = (attribute as Identifier).Value,
                            Implicit = true
                        };
                        model.GoalModel.Agents.Add (candidate);
                        Declarations.Add (candidate, new List<Declaration> { new Declaration (attribute.Line, attribute.Col, attribute.Filename) });

                    } else {
                        throw new ParsingException (string.Format ("Agent '{0}' could not be found", (attribute as Identifier).Value));
                    }
                }
            }

            return candidate;
        }

        private KAOSTools.MetaModel.System GetOrCreateAlternative (IdentifierOrName attribute, bool create = true)
        {
            KAOSTools.MetaModel.System candidate = null;
            
            if (attribute is Name) {
                var name = (attribute as Name).Value;
                var candidates = model.GoalModel.Systems.Where (a => a.Name == name);
                
                if (candidates.Count() == 0) {
                    if (create) {
                        candidate = new KAOSTools.MetaModel.System() { 
                            Name = (attribute as Name).Value,
                            Implicit = true
                        };
                        model.GoalModel.Systems.Add (candidate);
                        Declarations.Add (candidate, new List<Declaration> { new Declaration (attribute.Line, attribute.Col, attribute.Filename) });

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
                        candidate = new KAOSTools.MetaModel.System() { 
                            Identifier = (attribute as Identifier).Value,
                            Implicit = true
                        };
                        model.GoalModel.Systems.Add (candidate);
                        Declarations.Add (candidate, new List<Declaration> { new Declaration (attribute.Line, attribute.Col, attribute.Filename) });

                    } else {
                        throw new ParsingException (string.Format ("Alternative '{0}' could not be found", (attribute as Identifier).Value));
                    }
                }
            }
            
            return candidate;
        }

        #endregion

        private string Sanitize (string text) 
        {
            var t = Regex.Replace(text, @"\s+", " ", RegexOptions.Multiline).Trim ();
            t = Regex.Replace (t, "\"\"", "\"", RegexOptions.Multiline);
            return t;
        }
    }


}

