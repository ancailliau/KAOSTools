using System;
using System.Linq;
using System.Collections.Generic;
using KAOSTools.Parsing;
using KAOSTools.MetaModel;
using System.Text.RegularExpressions;
using System.IO;

namespace KAOSTools.Parsing
{

    /// <summary>
    /// This is more a compiler than a parser. This class will parse and compile (in two-phases) a text-based 
    /// description of a KAOS model in an object-based representation of that model.
    /// </summary>
    public class ModelBuilder
    {
        /// <summary>
        /// The internal parser. This parser is generated from a grammar (see <c>GoalModelParsed.peg</c> for more 
        /// details)
        /// </summary>
        private GoalModelParser _parser = new GoalModelParser ();

        /// <summary>
        /// The model being built.
        /// </summary>
        private KAOSModel model;

        /// <summary>
        /// The declarations for each elemnt of the model.
        /// </summary>
        public IDictionary<KAOSMetaModelElement, IList<Declaration>> Declarations { get; set; }

        FirstStageBuilder FSB;
        SecondStageBuilder SSB;
        FormulaBuilder FB;

        Uri RelativePath = null;

        /// <summary>
        /// Initializes a new instance of the compiler.
        /// </summary>
        public ModelBuilder (){}

        [Obsolete("Do not provide a KAOS Model")]
        public KAOSModel Parse (string input, string filename, KAOSModel model)
        {
            Declarations = new Dictionary<KAOSMetaModelElement, IList<Declaration>> ();

            if (!string.IsNullOrEmpty (filename))
                RelativePath = new Uri(Path.GetFullPath (Path.GetDirectoryName(filename) + "/"));

            FSB = new FirstStageBuilder (model, Declarations, RelativePath);
            FB = new FormulaBuilder (model, Declarations, FSB);

            SSB = new SecondStageBuilder (model, Declarations, FSB, FB);

            try {
                var elements = _parser.Parse (input, filename) as ParsedElements;    

                this.model = model;

                FSB.BuildElementWithKeys (elements);
                SSB.BuildElement (elements);

                // SecondPass (elements);
                return model;

            } catch (Exception e) {
                throw new CompilationException (e.Message, e);
            }
        }

        public KAOSModel Parse (string input, string filename)
        {
            return Parse (input, filename, new KAOSModel());
        }

        public KAOSModel Parse (string input)
        {
            return Parse (input, null);
        }

#if false

        private void SecondPass (ParsedElements elements) {
            foreach (var element in elements.Values) {
                if (element is ParsedObstacle) {
                    BuildObstacleRelations (element as ParsedObstacle);

                } else if (element is ParsedGoal) {
                    BuildGoalRelations (element as ParsedGoal);

                } else if (element is ParsedSystem) {
                    BuildSystemRelations (element as ParsedSystem);

                } else if (element is ParsedEntity) {
                    BuildEntityRelations (element as ParsedEntity);

                } else if (element is ParsedAssociation) {
                    BuildAssociationRelations (element as ParsedAssociation);

                } else if (element is ParsedPredicate) {
                    BuildPredicateRelations (element as ParsedPredicate);
                    
                }
            }
        }
        
        #region Build helpers for first pass



        #endregion

        #region Build helpers for second pass

        private void BuildObstacleRelations (ParsedObstacle parsedObstacle)
        {
            string identifier = "";
            string name       = "";

            var refinements   = new List<ObstacleRefinement> ();
            var resolutions   = new List<KAOSTools.MetaModel.Goal> ();

            foreach (var attribute in parsedObstacle.Attributes) {
                if (attribute is IdentifierExpression) {
                    identifier = (attribute as IdentifierExpression).Value;

                } else if (attribute is NameExpression) {
                    name = (attribute as NameExpression).Value;

                } else if (attribute is ParsedRefinedByAttribute) {
                    var children = attribute as ParsedRefinedByAttribute;
                    var refinement = new ObstacleRefinement ();

                    foreach (var child in children.Values) {
                        if (child is IdentifierOrNameExpression) {
                            var domprop = GetDomainProperty (child as IdentifierOrNameExpression);
                            if (domprop != null) {
                                refinement.DomainProperties.Add (domprop);
                                
                            } else {
                                var candidate = GetOrCreateObstacle (child as IdentifierOrNameExpression, true);
                                if (candidate != null)
                                    refinement.Subobstacles.Add (candidate);
                            }

                        } else if (child is ParsedObstacle) {
                            var o = FSB.BuildElementWithKeys (child as ParsedObstacle) as Obstacle;
                            refinement.Subobstacles.Add (o);
                            
                            BuildObstacleRelations (child as ParsedObstacle);
                        }
                    }

                    if (refinement.Subobstacles.Count > 0)
                        refinements.Add (refinement);

                } else if (attribute is ParsedResolvedByAttribute) {
                    var children = attribute as ParsedResolvedByAttribute;

                    foreach (var child in children.Values) {
                        if (child is IdentifierOrNameExpression) {
                            var candidate = GetOrCreateGoal (child as IdentifierOrNameExpression, true);
                            if (candidate != null)
                                resolutions.Add (candidate);

                        } else if (child is ParsedGoal) {
                            var g = FSB.BuildElementWithKeys (child as ParsedGoal) as Goal;
                            resolutions.Add (g);
                            
                            BuildGoalRelations (child as ParsedGoal);
                        }
                    }
                }
            }

            KAOSTools.MetaModel.Obstacle obstacle = null;
            if (string.IsNullOrEmpty (identifier)) {
                var obstacles = model.GoalModel.GetObstaclesByName (name);
                if (obstacles.Count() > 1)
                    throw new CompilationException (string.Format ("Obstacle '{0}' is ambiguous", name));
                else if (obstacles.Count() == 0)
                    throw new CompilationException (string.Format ("Obstacle '{0}' not found", name));
                else 
                    obstacle = obstacles.Single ();

            } else {
                obstacle = model.GoalModel.GetObstacleByIdentifier (identifier);
                
                if (obstacle == null)
                    throw new CompilationException (string.Format ("Obstacle '{0}' not found", identifier));
            }

            foreach (var r in refinements)
                obstacle.Refinements.Add (r);

            foreach (var g in resolutions)
                obstacle.Resolutions.Add (g);
        }

        private void BuildGoalRelations (ParsedGoal parsedGoal)
        {
            string identifier    = "";
            string name          = "";
            Formula formalSpec = null;
            var    refinements   = new HashSet<GoalRefinement> ();
            var    obstruction   = new HashSet<KAOSTools.MetaModel.Obstacle> ();
            var    assignedAgents = new HashSet<KAOSTools.MetaModel.AgentAssignment> ();

            foreach (var attribute in parsedGoal.Attributes) {
                if (attribute is IdentifierExpression) {
                    identifier = (attribute as IdentifierExpression).Value;

                } else if (attribute is NameExpression) {
                    name = (attribute as NameExpression).Value;

                } else if (attribute is ParsedFormalSpecAttribute) {
                    formalSpec = FB.BuildFormula ((attribute as ParsedFormalSpecAttribute).Value, new Dictionary<string, KAOSTools.MetaModel.Entity>());

                } else if (attribute is ParsedRefinedByAttribute) {
                    var children = attribute as ParsedRefinedByAttribute;
                    var refinement = new GoalRefinement ();

                    if (children.SystemIdentifier != null)
                        refinement.SystemReference = GetOrCreateAlternative (children.SystemIdentifier, true);

                    foreach (var child in children.Values) {
                        if (child is IdentifierOrNameExpression) {
                            var domprop = GetDomainProperty (child as IdentifierOrNameExpression);
                            var domhyp = GetDomainHypothesis (child as IdentifierOrNameExpression);
                            if (domprop != null) {
                                refinement.DomainProperties.Add (domprop);
                            
                            } else if (domhyp != null) {
                                refinement.DomainHypotheses.Add (domhyp);

                            } else {
                                var candidate = GetOrCreateGoal (child as IdentifierOrNameExpression, true);
                                if (candidate != null)
                                    refinement.Subgoals.Add (candidate);
                            }

                        } else if (child is ParsedGoal) {
                            var g = FSB.BuildElementWithKeys (child as ParsedGoal) as Goal;
                            refinement.Subgoals.Add (g);
                            
                            BuildGoalRelations (child as ParsedGoal);

                        } else if (child is ParsedDomainProperty) {
                            var g = FSB.BuildElementWithKeys (child as ParsedDomainProperty) as DomainProperty;
                            refinement.DomainProperties.Add (g);

                        } else if (child is ParsedDomainHypothesis) {
                            var g = FSB.BuildElementWithKeys (child as ParsedDomainHypothesis) as DomainHypothesis;
                            refinement.DomainHypotheses.Add (g);

                        }
                    }

                    if ((refinement.Subgoals.Count + refinement.DomainHypotheses.Count + refinement.DomainProperties.Count) > 0)
                        refinements.Add (refinement);

                } else if (attribute is ParsedObstructedByAttribute) {
                    var children = attribute as ParsedObstructedByAttribute;

                    foreach (var child in children.Values) {
                        if (child is IdentifierOrNameExpression) {
                            var candidate = GetOrCreateObstacle (child as IdentifierOrNameExpression, true);
                            if (candidate != null)
                                obstruction.Add (candidate);
                        } else if (child is ParsedObstacle) {
                            var o = FSB.BuildElementWithKeys (child as ParsedObstacle) as Obstacle;
                            obstruction.Add (o);
                            
                            BuildObstacleRelations (child as ParsedObstacle);
                        }
                    }
                
                } else if (attribute is AssignedToList) {
                    var assignment = new AgentAssignment();

                    if ((attribute as AssignedToList).SystemIdentifier != null)
                        assignment.SystemReference = GetOrCreateAlternative ((attribute as AssignedToList).SystemIdentifier);

                    foreach (var assignedto in (attribute as AssignedToList).Values) {
                        if (assignedto is IdentifierOrNameExpression) {
                            var candidate = GetOrCreateAgent (assignedto as IdentifierOrNameExpression, true);
                            if (candidate != null)
                                assignment.Agents.Add (candidate);
                        } else if (assignedto is ParsedAgent) {
                            var a = FSB.BuildElementWithKeys (assignedto as ParsedAgent) as Agent;
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
                    throw new CompilationException (string.Format ("Goal '{0}' is ambiguous", name));
                else if (goals.Count() == 0)
                    throw new CompilationException (string.Format ("Goal '{0}' not found", name));
                else 
                    goal = goals.Single ();

            } else {
                goal = model.GoalModel.GetGoalByIdentifier (identifier);
                
                if (goal == null)
                    throw new CompilationException (string.Format ("Goal '{0}' not found", identifier));
            }

            foreach (var r in refinements)
                goal.Refinements.Add (r);

            foreach (var r in obstruction)
                goal.Obstructions.Add (r);

            foreach (var r in assignedAgents)
                goal.AgentAssignments.Add (r);

            goal.FormalSpec = formalSpec;
        }

        private void BuildPredicateRelations (ParsedPredicate parsedPredicate)
        {
            string identifier    = "";
            string name          = "";
            string signature     = "";

            foreach (var attribute in parsedPredicate.Attributes) {
                if (attribute is IdentifierExpression) {
                    identifier = (attribute as IdentifierExpression).Value;
                    
                } else if (attribute is NameExpression) {
                    name = (attribute as NameExpression).Value;

                } else if (attribute is ParsedSignatureAttribute) {
                    signature = (attribute as ParsedSignatureAttribute).Value;
                }
            }

            KAOSTools.MetaModel.Predicate predicate = null;
            if (!string.IsNullOrEmpty (identifier)) {
                predicate = model.Predicates.SingleOrDefault(x => x.Identifier == identifier);
                
                if (predicate == null)
                    throw new CompilationException (string.Format ("Predicate '{0}' not found", identifier));
            
            } else if (!string.IsNullOrEmpty (name)) {
                var predicates = model.Predicates.Where(x => x.Name == name);
                if (predicates.Count() > 1)
                    throw new CompilationException (string.Format ("Predicate '{0}' is ambiguous", name));
                else if (predicates.Count() == 0)
                    throw new CompilationException (string.Format ("Predicate '{0}' not found", name));
                else 
                    predicate = predicates.Single ();
                
            } else if (!string.IsNullOrEmpty (signature)) {
                var predicates = model.Predicates.Where(x => x.Signature == signature);
                if (predicates.Count() > 1)
                    throw new CompilationException (string.Format ("Predicate '{0}' is ambiguous", signature));
                else if (predicates.Count() == 0)
                    throw new CompilationException (string.Format ("Predicate '{0}' not found", signature));
                else 
                    predicate = predicates.Single ();
                
            }

            int argumentIndex = 0;
            foreach (var attribute in parsedPredicate.Attributes) {
                if (attribute is ParsedFormalSpecAttribute) {
                    predicate.FormalSpec = FB.BuildPredicateFormula (predicate, (attribute as ParsedFormalSpecAttribute).Value);
                
                } else if (attribute is ParsedPredicateArgumentAttribute) {
                    var ppa = attribute as ParsedPredicateArgumentAttribute;
                    var arg_name = ppa.Name.Value;
                    var arg_type = ppa.Type == null ? null : GetOrCreateEntity (ppa.Type);

                    // No argument with the same name is already declared
                    if (predicate.Arguments.Count (w => w.Name == arg_name) == 0) {
                        predicate.Arguments.Add (new PredicateArgument() { Name = arg_name, Type = arg_type });
                    } 

                    // Otherwise, it shall be the same to the one already declared
                    else {
                        // if no type is already declared, use the new one (may be not declared)
                        if (predicate.Arguments [argumentIndex].Type == null) {
                            predicate.Arguments[argumentIndex].Type = arg_type;
                        } 
                        
                        // if a type was already declared, it shall be the same
                        else if (predicate.Arguments [argumentIndex].Type != arg_type) {
                            throw new CompilationException ("Argument " + argumentIndex + " has a mismatching type. " +
                                "Expected " + predicate.Arguments[argumentIndex].Type.Name + " " +
                                "but received " + arg_type.Name 
                                                        + " ("+parsedPredicate.Filename+":"+parsedPredicate.Line+","+parsedPredicate.Col+")");
                        }

                        // if no name is already declared, use the new one (may be not declared)
                        if (predicate.Arguments[argumentIndex].Name == null) {
                            predicate.Arguments[argumentIndex].Name = arg_name;
                        }

                        // if a name was already defined, it shall be the same
                        else if (predicate.Arguments[argumentIndex].Name != arg_name) {
                            throw new CompilationException ("Argument " + argumentIndex + " has a mismatching name. Expected " + predicate.Arguments[argumentIndex].Name + " but received " + arg_name);
                        }
                    }

                    argumentIndex++;
                }
            }

        }

        private void BuildSystemRelations (ParsedSystem parsedSystem)
        {
            string identifier    = "";
            string name          = "";
            var    alternatives  = new HashSet<KAOSTools.MetaModel.AlternativeSystem> ();

            foreach (var attribute in parsedSystem.Attributes) {
                if (attribute is IdentifierExpression) {
                    identifier = (attribute as IdentifierExpression).Value;
                    
                } else if (attribute is NameExpression) {
                    name = (attribute as NameExpression).Value;
                    
                } else if (attribute is ParsedAlternativeAttribute) {
                    foreach (var child in (attribute as ParsedAlternativeAttribute).Values) {
                        if (child is IdentifierOrNameExpression) {
                            var candidate = GetOrCreateAlternative (child as IdentifierOrNameExpression, true);
                            if (candidate != null)
                                alternatives.Add (candidate);
                        } else if (child is ParsedSystem) {
                            var s = FSB.BuildElementWithKeys (child as ParsedSystem) as AlternativeSystem;
                            alternatives.Add (s);
                            BuildSystemRelations (child as ParsedSystem);
                        }
                    }
                }
            }
            
            KAOSTools.MetaModel.AlternativeSystem system = null;
            if (string.IsNullOrEmpty (identifier)) {
                var goals = model.GoalModel.GetSystemsByName (name);
                if (goals.Count() > 1)
                    throw new CompilationException (string.Format ("System '{0}' is ambiguous", name));
                else if (goals.Count() == 0)
                    throw new CompilationException (string.Format ("System '{0}' not found", name));
                else 
                    system = goals.Single ();
                
            } else {
                system = model.GoalModel.GetSystemByIdentifier (identifier);
                
                if (system == null)
                    throw new CompilationException (string.Format ("Sytem '{0}' not found", identifier));
            }
            
            if (!parsedSystem.Override) {
                foreach (var r in alternatives)
                    system.Alternatives.Add (r);

            } else {
                system.Alternatives = alternatives;
            }
        }

        private void BuildEntityRelations (ParsedEntity parsedEntity)
        {
            string identifier    = "";
            string name          = "";
            var    attributes    = new HashSet<KAOSTools.MetaModel.Attribute> ();
            var    parents          = new HashSet<KAOSTools.MetaModel.Entity> ();
            
            foreach (var attribute in parsedEntity.Attributes) {
                if (attribute is IdentifierExpression) {
                    identifier = (attribute as IdentifierExpression).Value;
                    
                } else if (attribute is NameExpression) {
                    name = (attribute as NameExpression).Value;
                    
                } else if (attribute is ParsedAttributeAttribute) {
                    attributes.Add (new KAOSTools.MetaModel.Attribute () {
                        Name = ((attribute as ParsedAttributeAttribute).Name as NameExpression).Value,
                        Type = GetOrCreateAttributeType ((attribute as ParsedAttributeAttribute).Type)
                    });
                } else if (attribute is ParsedIsAAttribute) {
                    parents.Add (GetOrCreateEntity((attribute as ParsedIsAAttribute).Target));
                }
            }
            
            KAOSTools.MetaModel.Entity entity = null;
            if (string.IsNullOrEmpty (identifier)) {
                var entities = model.Entities.Where (e => e.Name == name);
                if (entities.Count() > 1)
                    throw new CompilationException (string.Format ("Entity '{0}' is ambiguous", name));
                else if (entities.Count() == 0)
                    throw new CompilationException (string.Format ("Entity '{0}' not found", name));
                else 
                    entity = entities.Single ();
                
            } else {
                entity = model.Entities.SingleOrDefault (e => e.Identifier == identifier);
                
                if (entity == null)
                    throw new CompilationException (string.Format ("Entity '{0}' not found", identifier));
            }
            
            if (!parsedEntity.Override) {
                foreach (var r in attributes)
                    entity.Attributes.Add (r);
                foreach (var r in parents)
                    entity.Parents.Add (r);

            } else {
                entity.Attributes = attributes;
                entity.Parents = parents;
            }
        }

        private void BuildAssociationRelations (ParsedAssociation parsedAssociation)
        {
            string identifier    = "";
            string name          = "";
            var    links         = new HashSet<KAOSTools.MetaModel.Link> ();
            var    attributes    = new HashSet<KAOSTools.MetaModel.Attribute> ();

            foreach (var attribute in parsedAssociation.Attributes) {
                if (attribute is IdentifierExpression) {
                    identifier = (attribute as IdentifierExpression).Value;
                    
                } else if (attribute is NameExpression) {
                    name = (attribute as NameExpression).Value;
                    
                } else if (attribute is ParsedAttributeAttribute) {
                    attributes.Add (new KAOSTools.MetaModel.Attribute () {
                        Name = ((attribute as ParsedAttributeAttribute).Name as NameExpression).Value,
                        Type = GetOrCreateAttributeType ((attribute as ParsedAttributeAttribute).Type)
                    });
                } else if (attribute is ParsedLinkAttribute) {
                    var target = GetOrCreateEntity ((attribute as ParsedLinkAttribute).Target);
                    var multiplicity = (attribute as ParsedLinkAttribute).Multiplicity;
                    links.Add (new KAOSTools.MetaModel.Link () {
                        Target = target, Multiplicity = multiplicity
                    });
                }
            }
            
            KAOSTools.MetaModel.Relation relation = null;
            if (string.IsNullOrEmpty (identifier)) {
                var relations = model.Entities.Where (e => e.GetType() == typeof(Relation) 
                                                           && e.Name == name);
                if (relations.Count() > 1)
                    throw new CompilationException (string.Format ("Relation '{0}' is ambiguous", name));
                else if (relations.Count() == 0)
                    throw new CompilationException (string.Format ("Relation '{0}' not found", name));
                else 
                    relation = relations.Single () as Relation;
                
            } else {
                relation = model.Entities.SingleOrDefault (e => e.Identifier == identifier) as Relation;
                
                if (relation == null)
                    throw new CompilationException (string.Format ("Relation '{0}' not found", identifier));
            }
            
            if (!parsedAssociation.Override) {
                foreach (var r in links)
                    relation.Links.Add (r);
                foreach (var r in attributes)
                    relation.Attributes.Add (r);
                
            } else {
                relation.Links = links;
                relation.Attributes = attributes;
            }
        }

        #endregion

        #region Get or create helpers

        private KAOSTools.MetaModel.Obstacle GetOrCreateObstacle (IdentifierOrNameExpression attribute, bool create = true)
        {
            KAOSTools.MetaModel.Obstacle candidate = null;

            if (attribute is NameExpression) {
                var name = (attribute as NameExpression).Value;
                var candidates = model.GoalModel.GetObstaclesByName (name);

                if (candidates.Count() == 0) {
                    if (create) {
                        candidate = new KAOSTools.MetaModel.Obstacle() { 
                            Name = (attribute as NameExpression).Value,
                            Implicit = true
                        };
                        model.GoalModel.Obstacles.Add (candidate);
                        Declarations.Add (candidate, new List<Declaration> { new Declaration (attribute.Line, attribute.Col, attribute.Filename) });

                    } else {
                        throw new CompilationException (string.Format ("Obstacle '{0}' could not be found", (attribute as NameExpression).Value));
                    }

                } else if (candidates.Count() > 1) {
                    candidate = candidates.First ();

                } else /* candidates.Count() == 0 */ {
                    candidate = candidates.Single ();
                }

            } else if (attribute is IdentifierExpression) {
                candidate = model.GoalModel.GetObstacleByIdentifier ((attribute as IdentifierExpression).Value);

                if (candidate == null) {
                    if (create) {
                        candidate = new KAOSTools.MetaModel.Obstacle() { 
                            Identifier = (attribute as IdentifierExpression).Value,
                            Implicit = true
                        };
                        model.GoalModel.Obstacles.Add (candidate);
                        Declarations.Add (candidate, new List<Declaration> { new Declaration (attribute.Line, attribute.Col, attribute.Filename) });
                    } else {
                        throw new CompilationException (string.Format ("Obstacle '{0}' could not be found", (attribute as IdentifierExpression).Value));
                    }
                }
            }

            return candidate;
        }

        private KAOSTools.MetaModel.DomainProperty GetDomainProperty (IdentifierOrNameExpression attribute)
        {
            
            if (attribute is NameExpression) {
                var name = (attribute as NameExpression).Value;
                var domprop_candidate = model.GoalModel.GetDomainPropertiesByName (name);

                if (domprop_candidate.Count() > 1) {
                    return domprop_candidate.First ();
                } else if (domprop_candidate.Count() == 1) {
                    return domprop_candidate.Single ();
                }
                return null;

            } else if (attribute is IdentifierExpression) {
                var identifier = (attribute as IdentifierExpression).Value;
                var candidate = model.GoalModel.GetDomainPropertyByIdentifier (identifier);
                return candidate;
            }

            return null;
        }

        private KAOSTools.MetaModel.DomainHypothesis GetDomainHypothesis (IdentifierOrNameExpression attribute)
        {
            
            if (attribute is NameExpression) {
                var name = (attribute as NameExpression).Value;
                var domhyp_candidate = model.GoalModel.GetDomainHypothesesByName (name);
                
                if (domhyp_candidate.Count() > 1) {
                    return domhyp_candidate.First ();
                } else if (domhyp_candidate.Count() == 1) {
                    return domhyp_candidate.Single ();
                }
                return null;
                
            } else if (attribute is IdentifierExpression) {
                var identifier = (attribute as IdentifierExpression).Value;
                return model.GoalModel.GetDomainHypothesisByIdentifier (identifier);
            }
            
            return null;
        }

        private KAOSTools.MetaModel.Goal GetOrCreateGoal (IdentifierOrNameExpression attribute, bool create = true)
        {
            KAOSTools.MetaModel.Goal candidate = null;

            if (attribute is NameExpression) {
                var name = (attribute as NameExpression).Value;
                var candidates = model.GoalModel.GetGoalsByName (name);

                if (candidates.Count() == 0) {
                        if (create) {
                            candidate = new KAOSTools.MetaModel.Goal() { 
                                Name = (attribute as NameExpression).Value,
                                Implicit = true
                            };
                            model.GoalModel.Goals.Add (candidate);
                            Declarations.Add (candidate, new List<Declaration> { new Declaration (attribute.Line, attribute.Col, attribute.Filename) });

                        } else {
                            throw new CompilationException (string.Format ("Goal '{0}' could not be found", (attribute as NameExpression).Value));
                        }

                } else if (candidates.Count() > 1) {
                   candidate = candidates.First ();

                } else /* candidates.Count() == 0 */ {
                    candidate = candidates.Single ();
                }

            } else if (attribute is IdentifierExpression) {
                candidate = model.GoalModel.GetGoalByIdentifier ((attribute as IdentifierExpression).Value);

                if (candidate == null) {
                    if (create) {
                        candidate = new KAOSTools.MetaModel.Goal() { 
                            Identifier = (attribute as IdentifierExpression).Value,
                            Implicit = true
                        };
                        model.GoalModel.Goals.Add (candidate);
                        Declarations.Add (candidate, new List<Declaration> { new Declaration (attribute.Line, attribute.Col, attribute.Filename) });

                    } else {
                        throw new CompilationException (string.Format ("Goal '{0}' could not be found", (attribute as IdentifierExpression).Value));
                    }
                }
            }

            return candidate;
        }

        private KAOSTools.MetaModel.Agent GetOrCreateAgent (IdentifierOrNameExpression attribute, bool create = true)
        {
            KAOSTools.MetaModel.Agent candidate = null;

            if (attribute is NameExpression) {
                var name = (attribute as NameExpression).Value;
                var candidates = model.GoalModel.GetAgentsByName (name);

                if (candidates.Count() == 0) {
                    if (create) {
                        candidate = new KAOSTools.MetaModel.Agent() { 
                            Name = (attribute as NameExpression).Value,
                            Implicit = true
                        };
                        model.GoalModel.Agents.Add (candidate);
                        Declarations.Add (candidate, new List<Declaration> { new Declaration (attribute.Line, attribute.Col, attribute.Filename) });

                    } else {
                        throw new CompilationException (string.Format ("Agent '{0}' could not be found", (attribute as NameExpression).Value));
                    }

                } else if (candidates.Count() > 1) {
                    throw new CompilationException (string.Format ("Agent '{0}' is ambiguous", (attribute as NameExpression).Value));

                } else /* candidates.Count() == 0 */ {
                    candidate = candidates.Single ();
                }

            } else if (attribute is IdentifierExpression) {
                candidate = model.GoalModel.GetAgentByIdentifier ((attribute as IdentifierExpression).Value);

                if (candidate == null) {
                    if (create) {
                        candidate = new KAOSTools.MetaModel.Agent () { 
                            Identifier = (attribute as IdentifierExpression).Value,
                            Implicit = true
                        };
                        model.GoalModel.Agents.Add (candidate);
                        Declarations.Add (candidate, new List<Declaration> { new Declaration (attribute.Line, attribute.Col, attribute.Filename) });

                    } else {
                        throw new CompilationException (string.Format ("Agent '{0}' could not be found", (attribute as IdentifierExpression).Value));
                    }
                }
            }

            return candidate;
        }

        private KAOSTools.MetaModel.AlternativeSystem GetOrCreateAlternative (IdentifierOrNameExpression attribute, bool create = true)
        {
            KAOSTools.MetaModel.AlternativeSystem candidate = null;
            
            if (attribute is NameExpression) {
                var name = (attribute as NameExpression).Value;
                var candidates = model.GoalModel.Systems.Where (a => a.Name == name);
                
                if (candidates.Count() == 0) {
                    if (create) {
                        candidate = new KAOSTools.MetaModel.AlternativeSystem() { 
                            Name = (attribute as NameExpression).Value,
                            Implicit = true
                        };
                        model.GoalModel.Systems.Add (candidate);
                        Declarations.Add (candidate, new List<Declaration> { new Declaration (attribute.Line, attribute.Col, attribute.Filename) });

                    } else {
                        throw new CompilationException (string.Format ("Alternative '{0}' could not be found", (attribute as NameExpression).Value));
                    }
                    
                } else if (candidates.Count() > 1) {
                    candidate = candidates.First ();
                    
                } else /* candidates.Count() == 1 */ {
                    candidate = candidates.Single ();
                }
                
            } else if (attribute is IdentifierExpression) {
                candidate = model.GoalModel.Systems.Where (a => a.Identifier == ((attribute as IdentifierExpression).Value)).SingleOrDefault ();
                
                if (candidate == null) {
                    if (create) {
                        candidate = new KAOSTools.MetaModel.AlternativeSystem() { 
                            Identifier = (attribute as IdentifierExpression).Value,
                            Implicit = true
                        };
                        model.GoalModel.Systems.Add (candidate);
                        Declarations.Add (candidate, new List<Declaration> { new Declaration (attribute.Line, attribute.Col, attribute.Filename) });

                    } else {
                        throw new CompilationException (string.Format ("Alternative '{0}' could not be found", (attribute as IdentifierExpression).Value));
                    }
                }
            }
            
            return candidate;
        }

        private KAOSTools.MetaModel.GivenType GetOrCreateAttributeType (IdentifierOrNameExpression attribute)
        {
            
            if (attribute is NameExpression) {
                var name = (attribute as NameExpression).Value;
                var type = model.GivenTypes.FirstOrDefault (t => t.Name == name);

                if (type == null) {
                    type = new KAOSTools.MetaModel.GivenType() { Name = name, Implicit = true };
                    model.GivenTypes.Add (type);
                }

                return type;

            } else if (attribute is IdentifierExpression) {
                var identifier = (attribute as IdentifierExpression).Value;
                var type = model.GivenTypes.SingleOrDefault (t => t.Identifier == identifier);
                
                if (type == null) {
                    type = new KAOSTools.MetaModel.GivenType() { Identifier = identifier, Implicit = true };
                    model.GivenTypes.Add (type);
                }

                return type;
            }
            
            return null;
        }

        private KAOSTools.MetaModel.Entity GetOrCreateEntity (IdentifierOrNameExpression attribute)
        {
            if (attribute is NameExpression) {
                var name = (attribute as NameExpression).Value;
                var type = model.Entities.FirstOrDefault (t => t.Name == name);
                
                if (type == null) {
                    type = new KAOSTools.MetaModel.Entity() { Name = name, Implicit = true };
                    model.Entities.Add (type);
                }
                
                return type;
                
            } else if (attribute is IdentifierExpression) {
                var identifier = (attribute as IdentifierExpression).Value;
                var type = model.Entities.SingleOrDefault (t => t.Identifier == identifier);
                
                if (type == null) {
                    type = new KAOSTools.MetaModel.Entity() { Identifier = identifier, Implicit = true };
                    model.Entities.Add (type);
                }
                
                return type;
            }
            
            return null;
        }


        #endregion

        #region Formula



        #endregion

#endif

    }


}

