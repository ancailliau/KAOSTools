using KAOSTools.Core;
using System.Collections.Generic;
using System;
using System.Linq;
using UCLouvain.KAOSTools.Core.Agents;

namespace KAOSTools.Parsing
{
    public class SecondStageBuilder : Builder
    {
        public SecondStageBuilder (KAOSModel model, Uri relativePath)
            : base (model, relativePath)
        {
        }

        public void BuildElement (ParsedElements elements)
        {
            foreach (dynamic element in elements.Values) {
                BuildElement (element);
            }
        }

        public void BuildElement (ParsedModelAttribute element)
        {
        }

        public void BuildElement (ParsedElement element)
        {
            throw new NotImplementedException (string.Format("'{0}' is not yet supported", 
                                                             element.GetType().Name));
        }

#region

        public void BuildElement(ParsedGoal goal)
        {
			var e = model.goalRepository.GetGoal(goal.Identifier);
			if (e == null)
                throw new InvalidOperationException(string.Format("Goal '{0}' was not pre-built.", goal.Identifier));
            
			BuildElement(goal, e);
		}

        public void BuildElement(ParsedObstacle obstacle)
		{
            var e = model.obstacleRepository.GetObstacle(obstacle.Identifier);
			if (e == null)
				throw new InvalidOperationException(string.Format("Obstacle '{0}' was not pre-built.", obstacle.Identifier));

			BuildElement(obstacle, e);
		}

        public void BuildElement(ParsedAgent agent)
		{
            var e = model.agentRepository.GetAgent(agent.Identifier);
			if (e == null)
				throw new InvalidOperationException(string.Format("Agent '{0}' was not pre-built.", agent.Identifier));

			BuildElement(agent, e);
		}

        public void BuildElement(ParsedDomainProperty domprop)
		{
            var e = model.domainRepository.GetDomainProperty(domprop.Identifier);
			if (e == null)
				throw new InvalidOperationException(string.Format("Domain property '{0}' was not pre-built.", domprop.Identifier));

			BuildElement(domprop, e);
		}

        public void BuildElement(ParsedDomainHypothesis domhyp)
		{
            var e = model.domainRepository.GetDomainHypothesis(domhyp.Identifier);
			if (e == null)
				throw new InvalidOperationException(string.Format("Domain hypothesis '{0}' was not pre-built.", domhyp.Identifier));

			BuildElement(domhyp, e);
		}

        public void BuildElement(ParsedSoftGoal softgoal)
		{
            var e = model.goalRepository.GetSoftGoal(softgoal.Identifier);
			if (e == null)
				throw new InvalidOperationException(string.Format("Soft goal '{0}' was not pre-built.", softgoal.Identifier));

			BuildElement(softgoal, e);
		}

        public void BuildElement(ParsedPredicate predicate)
		{
            var e = model.formalSpecRepository.GetPredicate(predicate.Identifier);
			if (e == null)
				throw new InvalidOperationException(string.Format("Predicate '{0}' was not pre-built.", predicate.Identifier));

			BuildElement(predicate, e);
		}

#endregion


        public void BuildElement (ParsedDeclare element, dynamic e)
        {
            foreach (dynamic attribute in element.Attributes) {
                Handle (e, attribute);
            }
        }


        public void Handle (KAOSCoreElement element, object attribute)
        {
            throw new NotImplementedException (string.Format("'{0}' is not yet supported on '{1}'", 
                                                             attribute.GetType().Name,
                                                             element.GetType ().Name));
        }

        public void Handle (Predicate predicate, ParsedPredicateArgumentAttribute ppa)
        {
            // ignore
        }

        public void Handle (Goal element, ParsedCostAttribute attribute)
        {
            CostVariable v;
            string id = ((IdentifierExpression)attribute.CostVariable).Value;

            if ((v = model.modelMetadataRepository.GetCostVariable(id)) != null) {
                element.Costs.Add (v, attribute.Value.Value);
            } else {
                throw new NotImplementedException ();
            }
        }

        public void Handle (KAOSCoreElement element, ParsedCustomAttribute attribute)
        {
            element.CustomData.Add (attribute.Key, attribute.Value);
        }

        public void Handle (GoalRefinement element, 
                            ParsedSoftGoalContributionAttribute parsedAttribute)
        {
            foreach (var compositeChild in parsedAttribute.Values) {

                var child = compositeChild.SoftGoal;
                var contribution = (ParsedContribution) compositeChild.Contribution;

                SoftGoal goal;

                if (child is IdentifierExpression) {
                    var id = ((IdentifierExpression)child).Value;
                    if ((goal = model.goalRepository.GetSoftGoal (id)) == null) {
                        throw new ParserException(parsedAttribute.Line, parsedAttribute.Col, parsedAttribute.Filename,
                                                  string.Format("Soft goal '{0}' was not found", id));
                    }

                } else {
                    // TODO use string.Format
                    throw new NotImplementedException (
                        "'" + child.GetType().Name + "' is not supported in '" 
                        + parsedAttribute.GetType().Name + "' on '" + element.GetType().Name + "'");
                }

                if (contribution == ParsedContribution.Negative) {
                    element.AddNegativeSoftGoal (goal);
                } else if (contribution == ParsedContribution.Positive) {
                    element.AddPositiveSoftGoal (goal);
                } else {
                    throw new NotImplementedException ();
                }
            }
        }

        public void Handle (GoalRefinement element, 
                            ParsedPatternAttribute pattern)
        {
            if (pattern.Value.Name == ParsedRefinementPatternName.Milestone) {
                element.RefinementPattern = RefinementPattern.Milestone;
            }

            else if (pattern.Value.Name == ParsedRefinementPatternName.Case) {
                element.RefinementPattern = RefinementPattern.Case;
                foreach (var p in pattern.Value.Parameters) {
                    element.Parameters.Add ((p as ParsedFloat).Value);    
                }
            }

            else if (pattern.Value.Name == ParsedRefinementPatternName.IntroduceGuard) {
                element.RefinementPattern = RefinementPattern.IntroduceGuard;
            }

            else if (pattern.Value.Name == ParsedRefinementPatternName.DivideAndConquer) { 
                element.RefinementPattern = RefinementPattern.DivideAndConquer;
            }

            else if (pattern.Value.Name == ParsedRefinementPatternName.Uncontrollability) { 
                element.RefinementPattern = RefinementPattern.Uncontrollability;
            }

            else if (pattern.Value.Name ==  ParsedRefinementPatternName.Unmonitorability) { 
                element.RefinementPattern = RefinementPattern.Unmonitorability;
            }
            
            else if (pattern.Value.Name ==  ParsedRefinementPatternName.Redundant) { 
                element.RefinementPattern = RefinementPattern.Redundant;
            }

            else {
                throw new NotImplementedException ();
            }
        }

        public void Handle (GoalRefinement element, 
                            ParsedIsComplete parsedAttribute)
        {
            element.IsComplete = parsedAttribute.Value;
        }

        public void Handle (KAOSTools.Core.EntityAttribute attribute, ParsedDerivedAttribute parsedAttribute)
        {
            attribute.Derived = true;
        }

        public void Handle (Goal element, ParsedExceptionAttribute exception)
        {
            Goal goal;
            Obstacle obstacle;

            if (exception.ResolvingGoal == null) {
                goal = null;

            } else {
                if (exception.ResolvingGoal is IdentifierExpression) {
                    var id = ((IdentifierExpression)exception.ResolvingGoal).Value;
                    goal = model.goalRepository.GetGoal(id);

                } else {
                    throw new NotImplementedException (string.Format ("'{0}' is not supported in '{1}' on '{2}'",
                                                                      exception.ResolvingGoal.GetType().Name,
                                                                      exception.GetType().Name,
                                                                      element.GetType().Name));
                }
            }

            if (exception.ResolvedObstacle is IdentifierExpression) {
				var id = ((IdentifierExpression)exception.ResolvedObstacle).Value;
                    obstacle = model.obstacleRepository.GetObstacle (id);

            } else {
                throw new NotImplementedException (string.Format ("'{0}' is not supported in '{1}' on '{2}'",
                                                                  exception.ResolvedObstacle.GetType().Name,
                                                                  exception.GetType().Name,
                                                                  element.GetType().Name));
            }

            var goalException = new GoalException (element.model);

            goalException.SetObstacle (obstacle);
            goalException.SetResolvingGoal (goal);

            element.model.Add (goalException);
        }


        public void Handle (Obstacle element, ParsedResolvedByAttribute resolvedBy)
        {
            Goal goal;
            if (resolvedBy.Value is IdentifierExpression ) {

				var id = ((IdentifierExpression)resolvedBy.Value).Value;

				goal = model.goalRepository.GetGoal(id);

            } else {
                throw new NotImplementedException (string.Format ("'{0}' is not supported in '{1}' on '{2}'", 
                                                                  resolvedBy.Value.GetType().Name,
                                                                  resolvedBy.GetType().Name,
                                                                  element.GetType().Name));
            }

            var resolution = new Resolution (model);
            resolution.SetObstacle (element);
            resolution.SetResolvingGoal (goal);

            if (resolvedBy.Pattern != null) {
                if (resolvedBy.Pattern.Name == "substitution")
                    resolution.ResolutionPattern = ResolutionPattern.GoalSubstitution;

                else if (resolvedBy.Pattern.Name == "prevention")
                    resolution.ResolutionPattern = ResolutionPattern.ObstaclePrevention;

                else if (resolvedBy.Pattern.Name == "obstacle_reduction")
                    resolution.ResolutionPattern = ResolutionPattern.ObstacleReduction;

                else if (resolvedBy.Pattern.Name == "restoration")
                    resolution.ResolutionPattern = ResolutionPattern.GoalRestoration;

                else if (resolvedBy.Pattern.Name == "weakening")
                    resolution.ResolutionPattern = ResolutionPattern.GoalWeakening;

                else if (resolvedBy.Pattern.Name == "mitigation")
                    resolution.ResolutionPattern = ResolutionPattern.ObstacleMitigation;

                else if (resolvedBy.Pattern.Name == "weak_mitigation")
                    resolution.ResolutionPattern = ResolutionPattern.ObstacleWeakMitigation;

                else if (resolvedBy.Pattern.Name == "strong_mitigation")
                    resolution.ResolutionPattern = ResolutionPattern.ObstacleStrongMitigation;

                else
                    throw new NotImplementedException ();

                // TODO WTF is that doing???
                //foreach (var parameter in resolvedBy.Pattern.Parameters) {
                //    DomainHypothesis hypothesis;
                //    if (!Get (parameter, out hypothesis)) {
                //        Goal goalAsParameter;
                //        if (!Get (parameter, out goalAsParameter)) {
                //            goalAsParameter = Create<Goal> (parameter);
                //        }
                //        resolution.Parameters.Add (goalAsParameter);
                //    } else {
                //        resolution.Parameters.Add (hypothesis);
                //    }
                //}
            }

            model.Add (resolution);

        }

        public void Handle (Entity element, ParsedAttributeAttribute attribute)
        {
            var a = new KAOSTools.Core.EntityAttribute (model);

            Handle (a, new ParsedNameAttribute() { 
                Value = attribute.Name
            });

            Handle (a, new ParsedAttributeEntityTypeAttribute() { 
                Value = attribute.Type
            });

            a.SetEntity (element);
            model.Add (a);


        }

        public void Handle (Entity element, ParsedAttributeDeclaration attribute)
        {
            var a = new KAOSTools.Core.EntityAttribute (model);
            a.SetEntity (element);

            foreach (dynamic attr in attribute.Attributes) {
                Handle (a, attr);
            }

            model.Add (a);


        }

        public void Handle (KAOSTools.Core.EntityAttribute element, 
                            ParsedAttributeEntityTypeAttribute attribute) {

            GivenType givenType = null;
            if (attribute.Value != null) {
                if (attribute.Value is IdentifierExpression) {
                    string id = ((IdentifierExpression)attribute.Value).Value;
                    givenType = model.entityRepository.GetGivenType(id);

                } else {
                    throw new NotImplementedException (string.Format ("'{0}' is not supported in '{1}' on '{2}'", 
                                                                      attribute.Value.GetType().Name,
                                                                      attribute.GetType().Name,
                                                                      element.GetType().Name));
                }
            }

            if (givenType != null)
                element.SetType(givenType);
        }

        public void Handle (Goal element, ParsedObstructedByAttribute obstructedBy)
        {
            var obstruction = new Obstruction (model);
            obstruction.SetObstructedGoal (element);

            if (obstructedBy.Value is IdentifierExpression) {
                var id = ((IdentifierExpression)obstructedBy.Value).Value;

                Obstacle obstacle;
                if ((obstacle = model.obstacleRepository.GetObstacle(id)) != null)
                {
                    obstruction.SetObstacle(obstacle);
                }
                else
                {
                    throw new ParserException(obstructedBy.Line, obstructedBy.Col, obstructedBy.Filename, 
                                              "The obstacle '{0}' was not found", id);
                }

            } else {
                
                // TODO use string.Format
                throw new NotImplementedException (
                    "'" + obstructedBy.Value.GetType().Name 
                    + "' is not supported in '" 
                    + obstructedBy.GetType().Name + "' on '" 
                    + element.GetType().Name + "'");
            }

            model.Add (obstruction);
        }

        public void Handle (Goal element, ParsedAssignedToAttribute assignedTo)
        {
            var assignment = new GoalAgentAssignment (model);
            assignment.GoalIdentifier = element.Identifier;


            foreach (var child in assignedTo.Values) {
                if (child is IdentifierExpression)
                {
                    var id = ((IdentifierExpression)child).Value;
                    
                    Agent agent;
                    if ((agent = model.agentRepository.GetAgent(id)) != null)
                    {
                        assignment.Add(agent);
                        continue;
                    }
                }

                throw new NotImplementedException (string.Format("'{0}' is not supported in '{1}' on '{2}'", child.GetType().Name, assignedTo.GetType().Name, element.GetType().Name));
            }

            if (!assignment.IsEmpty)
                model.Add (assignment);
        }

        public void Handle (Goal element, ParsedRefinedByAttribute refinedBy)
        {
            var refinement = new GoalRefinement (model);
            refinement.SetParentGoal(element);

            // Parse the reference to children
            foreach (var child in refinedBy.Values) {
                if (child is IdentifierExpression)
                {
                    var id = ((IdentifierExpression)child).Value;

                    Goal goal;
                    if ((goal = model.goalRepository.GetGoal(id)) != null)
                    {
                        refinement.Add(goal);
                        continue;
                    }

                    DomainProperty domprop;
                    if ((domprop = model.domainRepository.GetDomainProperty(id)) != null)
                    {
                        refinement.Add(domprop);
                        continue;
                    }

                    DomainHypothesis domhyp;
                    if ((domhyp = model.domainRepository.GetDomainHypothesis(id)) != null)
                    {
                        refinement.Add(domhyp);
                        continue;
                    }

                    throw new BuilderException("Could not find goal, domain property or domain hypothesis corresponding " +
                                               "to " + id + ".", ((IdentifierExpression)child).Filename, ((IdentifierExpression)child).Line, ((IdentifierExpression)child).Col);
                }

                throw new NotImplementedException(string.Format("'{0}' is not supported in '{1}' on '{2}'", child.GetType().Name, refinedBy.GetType().Name, element.GetType().Name));
            }

            // Parse the refinement pattern provided
            if (refinedBy.RefinementPattern != null) {
                if (refinedBy.RefinementPattern.Name == ParsedRefinementPatternName.Milestone) {
                    refinement.RefinementPattern = RefinementPattern.Milestone;
                }

                else if (refinedBy.RefinementPattern.Name == ParsedRefinementPatternName.Case) {
                    refinement.RefinementPattern = RefinementPattern.Case;
                    // TODO Refactor that allowing to specify how much the subgoal contribute (alone) to the parent goal.
                    foreach (var p in refinedBy.RefinementPattern.Parameters) {
                        refinement.Parameters.Add ((p as ParsedFloat).Value);    
                    }
                }

                else if (refinedBy.RefinementPattern.Name == ParsedRefinementPatternName.IntroduceGuard) {
                    refinement.RefinementPattern = RefinementPattern.IntroduceGuard;
                }

                else if (refinedBy.RefinementPattern.Name == ParsedRefinementPatternName.DivideAndConquer) { 
                    refinement.RefinementPattern = RefinementPattern.DivideAndConquer;
                }

                else if (refinedBy.RefinementPattern.Name == ParsedRefinementPatternName.Uncontrollability) { 
                    refinement.RefinementPattern = RefinementPattern.Uncontrollability;
                }

                else if (refinedBy.RefinementPattern.Name ==  ParsedRefinementPatternName.Unmonitorability) { 
                    refinement.RefinementPattern = RefinementPattern.Unmonitorability;
                }

                else if (refinedBy.RefinementPattern.Name ==  ParsedRefinementPatternName.Redundant) { 
                    refinement.RefinementPattern = RefinementPattern.Redundant;
                }


                else {
                    throw new NotImplementedException ();
                }
            }

            if (!refinement.IsEmpty)
                model.Add (refinement);
        }

        public void Handle (Obstacle element, ParsedRefinedByAttribute refinedBy)
        {
            var refinement = new ObstacleRefinement (model);
            refinement.SetParentObstacle(element);
            
            foreach (var child in refinedBy.Values) {
                if (child is IdentifierExpression)
                {
                    var id = ((IdentifierExpression)child).Value;

                    DomainProperty domprop;
                    if ((domprop = model.domainRepository.GetDomainProperty(id)) != null)
                    {
                        refinement.Add(domprop);
                        continue;
                    }

                    DomainHypothesis domhyp;
                    if ((domhyp = model.domainRepository.GetDomainHypothesis(id)) != null)
                    {
                        refinement.Add(domhyp);
                        continue;
                    }

                    Obstacle obstacle;
                    if ((obstacle = model.obstacleRepository.GetObstacle(id)) != null)
                    {
                        refinement.Add(obstacle);
                        continue;
                    }

                }

                throw new NotImplementedException (
string.Format("'{0}' is not supported in '{1}' on '{2}'", child.GetType().Name, refinedBy.GetType().Name, element.GetType().Name));
            }
            
            if (!refinement.IsEmpty)
                model.Add (refinement);
        }
        
        public void Handle (Entity element, ParsedIsAAttribute attribute)
        {
            if (attribute.Value is IdentifierExpression) {
                Entity entity;
                string id = ((IdentifierExpression)attribute.Value).Value;
				entity = model.entityRepository.GetEntity(id);

                element.AddParent (entity);

                
            } else {
                throw new NotImplementedException (string.Format ("'{0}' is not supported in '{1}' on '{2}'", 
                                                                  attribute.Value.GetType().Name,
                                                                  attribute.GetType().Name,
                                                                  element.GetType().Name));
            }
        }
        
        public void Handle (Relation element, ParsedLinkAttribute attribute)
        {
            Entity entity;
            if (attribute.Target is IdentifierExpression | attribute.Target is NameExpression) {

				string id = ((IdentifierExpression)attribute.Target).Value;
				entity = model.entityRepository.GetEntity(id);

            } else {
                throw new NotImplementedException (string.Format ("'{0}' is not supported in '{1}' on '{2}'", 
                                                                  attribute.Target.GetType().Name,
                                                                  attribute.GetType().Name,
                                                                  element.GetType().Name));
            }

            element.Links.Add (new Link (model) { Target = entity, Multiplicity = attribute.Multiplicity });
        }

        public void Handle (KAOSCoreElement element, ParsedAgentTypeAttribute attribute)
        {
            if (attribute.Value == ParsedAgentType.Environment)
                Handle (element, AgentType.Environment, "Type");

            else if (attribute.Value == ParsedAgentType.Software)
                Handle (element, AgentType.Software, "Type");

            else if (attribute.Value == ParsedAgentType.Malicious)
                Handle (element, AgentType.Malicious, "Type");

            else 
                throw new NotImplementedException ();
        }

        public void Handle (KAOSCoreElement element, ParsedEntityTypeAttribute attribute)
        {
            EntityType type;
            if (attribute.Value == ParsedEntityType.Software) {
                type = EntityType.Software;
            } else if (attribute.Value == ParsedEntityType.Environment) {
                type = EntityType.Environment;
            } else if (attribute.Value == ParsedEntityType.Shared) {
                type = EntityType.Shared;
            } else {
                type = EntityType.None;
            }

            Handle (element, type, "Type");
        }

        public void Handle (KAOSCoreElement element, ParsedRDSAttribute attribute)
        {
            Handle (element, attribute.Value, "RDS");
        }
        
        public void Handle (KAOSCoreElement element, ParsedProbabilityAttribute attribute)
        {
            Handle (element, attribute.Value, "EPS");
        }
        
        public void Handle (KAOSCoreElement element, ParsedExpertProbabilityAttribute attribute)
        {
            if (element is Obstacle) {

                var o = (Obstacle)element;

                /*
            UncertaintyDistribution distribution;
            if (attribute.Estimate is ParsedUniformDistribution) {
                distribution = new UniformDistribution {
                    LowerBound = (attribute.Value as ParsedUniformDistribution).LowerBound,
                    UpperBound = (attribute.Value as ParsedUniformDistribution).UpperBound
                };
            } else if (attribute.Estimate is ParsedTriangularDistribution) {
                distribution = new TriangularDistribution {
                    Min = (attribute.Value as ParsedTriangularDistribution).Min,
                    Mode = (attribute.Value as ParsedTriangularDistribution).Mode,
                    Max = (attribute.Value as ParsedTriangularDistribution).Max
                };
            } else if (attribute.Estimate is ParsedPertDistribution) {
                distribution = new PERTDistribution {
                    Min = (attribute.Value as ParsedPertDistribution).Min,
                    Mode = (attribute.Value as ParsedPertDistribution).Mode,
                    Max = (attribute.Value as ParsedPertDistribution).Max
                };
            } else if (attribute.Estimate is ParsedBetaDistribution) {
                distribution = new BetaDistribution {
                    Alpha = (attribute.Value as ParsedBetaDistribution).Alpha,
                    Beta = (attribute.Value as ParsedBetaDistribution).Beta
                };
            } else if (attribute.Estimate is ParsedQuantileList) {
                distribution = new QuantileList {
                    Quantiles = (attribute.Estimate as ParsedQuantileList).Quantiles
                };
            } else {
                throw new NotImplementedException ();
            }
            */
                //var expertEstimates = o.ExpertEstimates ?? new Dictionary<Expert, QuantileList> ();

                //var child = attribute.IdOrNAme;
                //if (child is IdentifierExpression | child is NameExpression) {
                //    Expert expert;
                //    if (Get (child, out expert)) {
                //        expertEstimates.Add (expert, new QuantileList {
                //            Quantiles = (attribute.Estimate as ParsedQuantileList).Quantiles
                //        });
                //    }

                //} else {
                //    throw new NotImplementedException ();
                //}

                //Handle (element, expertEstimates, "ExpertEstimates");
            } else if (element is Calibration) {

                //var o = (Calibration)element;
                //var expertEstimates = o.ExpertEstimates ?? new Dictionary<Expert, QuantileList> ();

                //var child = attribute.IdOrNAme;
                //if (child is IdentifierExpression | child is NameExpression) {
                //    Expert expert;
                //    if (Get (child, out expert)) {
                //        expertEstimates.Add (expert, new QuantileList {
                //            Quantiles = (attribute.Estimate as ParsedQuantileList).Quantiles
                //        });
                //    }

                //} else {
                //    throw new NotImplementedException ();
                //}

                //Handle (element, expertEstimates, "ExpertEstimates");

            } else {
                throw new NotSupportedException ();
            }
        }

        public void Handle (KAOSCoreElement element, ParsedNameAttribute name)
        {
            Handle (element, Sanitize (name.Value), "Name");
        }

        public void Handle (KAOSCoreElement element, ParsedDefinitionAttribute definition)
        {
            Handle (element, definition.Value.Verbatim ? definition.Value.Value : Sanitize (definition.Value.Value), "Definition");
        }

        public void Handle (KAOSTools.Core.EntityAttribute element, ParsedIdentifierAttribute identifier)
        {
            Handle (element, element.EntityIdentifier + "." + identifier.Value, "Identifier");
        }

        public void Handle (KAOSCoreElement element, ParsedIdentifierAttribute identifier)
        {
            Handle (element, identifier.Value, "Identifier");
        }
        
        public void Handle (Predicate element, ParsedFormalSpecAttribute formalSpec)
        {
            // ignore, see third stage
        }
        
        public void Handle (KAOSCoreElement element, ParsedSignatureAttribute attribute)
        {
            Handle (element, attribute.Value, "Signature");
        }

        public void Handle (KAOSCoreElement element, ParsedFormalSpecAttribute formalSpec)
        {
            // ignore, see third stage
        }

        public void Handle (KAOSCoreElement element, ParsedConflictAttribute conflict)
        {
            if (element is Constraint) {
                var cst = (Constraint)element;
                foreach (var e in conflict.Values) {
                    if (e is IdentifierExpression) {
                        cst.Conflict.Add (((IdentifierExpression)e).Value);
                    }
                }
            }
        }

        public void Handle (KAOSCoreElement element, ParsedOrCstAttribute conflict)
        {
            if (element is Constraint) {
                var cst = (Constraint)element;
                foreach (var e in conflict.Values) {
                    if (e is IdentifierExpression) {
                        cst.Or.Add (((IdentifierExpression)e).Value);
                    }
                }
            }
        }

        public void Handle (Predicate element, DefaultValueAttribute def)
        {
            element.DefaultValue = def.Value;
        }

        public void Handle<T> (KAOSCoreElement element, 
                               T value, 
                               string propertyName) {
            var definitionProperty = element.GetType ().GetProperty (propertyName);
            if (definitionProperty == null)
                throw new InvalidOperationException (string.Format ("'{0}' has not property '{1}'", 
                                                                    element.GetType (), propertyName));

            if (definitionProperty.PropertyType != typeof(T))
                throw new InvalidOperationException (string.Format ("Type of property '{1}' in '{0}' is not '{2}'", 
                                                                    element.GetType (), propertyName, typeof(T).Name));

            definitionProperty.SetValue (element, value, null);
        }
    }
}

