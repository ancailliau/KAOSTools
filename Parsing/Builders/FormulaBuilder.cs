using System;
using KAOSTools.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using KAOSTools.Parsing.Parsers;

namespace KAOSTools.Parsing
{
    public class FormulaBuilder
    {
        private KAOSModel model;
        protected Uri relativePath;

        public FormulaBuilder (KAOSModel model, 
                               Uri relativePath)
        {
            this.model = model;
            this.relativePath = relativePath;
        }
        public Formula BuildFormula (ParsedElement value)
        {
            var declaredVariables = new Dictionary<string, Entity> ();
            return BuildFormula (value, declaredVariables);
        }

        public Formula BuildFormula (ParsedElement value, Dictionary<string, Entity> declaredVariables)
        {
            if (value == null)
                throw new ArgumentNullException ("value");

            if (value.GetType () == typeof (ParsedForallExpression)) {
                var a = new Forall ();
                var d2 = new Dictionary<string, KAOSTools.Core.Entity> (declaredVariables);
                foreach (var arg in (value as ParsedForallExpression).arguments) {
                    var name = arg.VariableName;
                    var type = GetOrCreateEntity (arg.Type);

                    if (declaredVariables.ContainsKey (name)) {
                        throw new BuilderException (string.Format ("'{0}' is already defined", name),
                                                    value.Filename, value.Line, value.Col);
                    }

                    a.Declarations.Add (new KAOSTools.Core.ArgumentDeclaration () {
                        Name = name,
                        Type = type
                    });
                    d2.Add (name, type);
                }
                a.Enclosed = BuildFormula ((value as ParsedForallExpression).Enclosed, d2);
                return a;
            } else if (value.GetType () == typeof (ParsedExistsExpression)) {
                var a = new Exists ();
                var d2 = new Dictionary<string, KAOSTools.Core.Entity> (declaredVariables);
                foreach (var arg in (value as ParsedExistsExpression).arguments) {
                    var name = arg.VariableName;
                    var type = GetOrCreateEntity (arg.Type);

                    if (declaredVariables.ContainsKey (name)) {
                        throw new BuilderException(string.Format("'{0}' is already defined", name),
													value.Filename, value.Line, value.Col);
                    }

                    a.Declarations.Add (new KAOSTools.Core.ArgumentDeclaration () {
                        Name = name,
                        Type = type
                    });
                    d2.Add (name, type);
                }
                a.Enclosed = BuildFormula ((value as ParsedExistsExpression).Enclosed, d2);
                return a;
            } else if (value.GetType () == typeof (ParsedStrongImplyExpression)) {
                return new StrongImply () {
                    Left = BuildFormula ((value as ParsedStrongImplyExpression).Left, declaredVariables),
                    Right = BuildFormula ((value as ParsedStrongImplyExpression).Right, declaredVariables)
                };
            } else if (value.GetType () == typeof (ParsedImplyExpression)) {
                return new Imply () {
                    Left = BuildFormula ((value as ParsedImplyExpression).Left, declaredVariables),
                    Right = BuildFormula ((value as ParsedImplyExpression).Right, declaredVariables)
                };
            } else if (value.GetType () == typeof (ParsedEquivalenceExpression)) {
                return new Equivalence () {
                    Left = BuildFormula ((value as ParsedEquivalenceExpression).Left, declaredVariables),
                    Right = BuildFormula ((value as ParsedEquivalenceExpression).Right, declaredVariables)
                };
            } else if (value.GetType () == typeof (ParsedUntilExpression)) {
                return new Until () {
                    Left = BuildFormula ((value as ParsedUntilExpression).Left, declaredVariables),
                    Right = BuildFormula ((value as ParsedUntilExpression).Right, declaredVariables)
                };
            } else if (value.GetType () == typeof (ParsedUnlessExpression)) {
                return new Unless () {
                    Left = BuildFormula ((value as ParsedUnlessExpression).Left, declaredVariables),
                    Right = BuildFormula ((value as ParsedUnlessExpression).Right, declaredVariables)
                };
            } else if (value.GetType () == typeof (ParsedReleaseExpression)) {
                return new Release () {
                    Left = BuildFormula ((value as ParsedReleaseExpression).Left, declaredVariables),
                    Right = BuildFormula ((value as ParsedReleaseExpression).Right, declaredVariables)
                };
            } else if (value.GetType () == typeof (ParsedAndExpression)) {
                return new And () {
                    Left = BuildFormula ((value as ParsedAndExpression).Left, declaredVariables),
                    Right = BuildFormula ((value as ParsedAndExpression).Right, declaredVariables)
                };
            } else if (value.GetType () == typeof (ParsedOrExpression)) {
                return new Or () {
                    Left = BuildFormula ((value as ParsedOrExpression).Left, declaredVariables),
                    Right = BuildFormula ((value as ParsedOrExpression).Right, declaredVariables)
                };
            } else if (value.GetType () == typeof (ParsedNotExpression)) {
                return new Not () {
                    Enclosed = BuildFormula ((value as ParsedNotExpression).Enclosed, declaredVariables)
                };
            } else if (value.GetType () == typeof (ParsedNextExpression)) {
                return new Next () {
                    Enclosed = BuildFormula ((value as ParsedNextExpression).Enclosed, declaredVariables)
                };
            } else if (value.GetType () == typeof (ParsedEventuallyExpression)) {
                return new Eventually () {
                    Enclosed = BuildFormula ((value as ParsedEventuallyExpression).Enclosed, declaredVariables),
                    TimeBound = BuildTimeBound ((value as ParsedEventuallyExpression).TimeBound)
                };

            } else if (value.GetType () == typeof (ParsedEventuallyBeforeExpression)) {
                return new EventuallyBefore () {
					Left = BuildFormula ((value as ParsedEventuallyBeforeExpression).Left, declaredVariables),
					Right = BuildFormula((value as ParsedEventuallyBeforeExpression).Right, declaredVariables),
                    TimeBound = BuildTimeBound ((value as ParsedEventuallyBeforeExpression).TimeBound)
                };

            } else if (value.GetType () == typeof (ParsedGloballyExpression)) {
                return new Globally () {
                    Enclosed = BuildFormula ((value as ParsedGloballyExpression).Enclosed, declaredVariables),
                    TimeBound = BuildTimeBound ((value as ParsedGloballyExpression).TimeBound)
                };

            } else if (value.GetType () == typeof (ParsedPredicateReferenceExpression)) {
                var prel = value as ParsedPredicateReferenceExpression;

                // Check if arguments are all defined
                foreach (var arg in prel.ActualArguments) {
                    if (!declaredVariables.ContainsKey (arg)) {
                        throw new BuilderException (string.Format ("'{0}' is not declared"),
													value.Filename, value.Line, value.Col);
                    }
                }

                return new PredicateReference () {
                    Predicate = GetOrCreatePredicate (prel, declaredVariables),
                    ActualArguments = prel.ActualArguments
                };
            } else if (value.GetType () == typeof (ParsedInRelationExpression)) {
                var prel = value as ParsedInRelationExpression;
                foreach (var arg in prel.Variables) {
                    if (!declaredVariables.ContainsKey (arg)) {
                        throw new BuilderException(string.Format("'{0}' is not declared", arg),
													value.Filename, value.Line, value.Col);
                    }
                }

                return new RelationReference () {
                    Relation = GetOrCreateRelation (value as ParsedInRelationExpression, declaredVariables),
                    ActualArguments = prel.Variables
                };
            } else if (value.GetType () == typeof (ParsedAttributeReferenceExpression)) {
                var pref = value as ParsedAttributeReferenceExpression;
                if (declaredVariables.ContainsKey (pref.Variable)) {
                    return new AttributeReference () {
                        Variable = pref.Variable,
                        Entity = declaredVariables [pref.Variable],
                        Attribute = GetOrCreateAttribute (value as ParsedAttributeReferenceExpression, declaredVariables [pref.Variable])
                    };
                } else {
					throw new BuilderException(string.Format("Variable '{0}' is not declared", pref.Variable),
													value.Filename, value.Line, value.Col);
                }

            } else if (value.GetType () == typeof (ParsedComparisonExpression)) {
                var pref = value as ParsedComparisonExpression;
                ComparisonCriteria criteria;
                if (pref.criteria == ParsedComparisonCriteria.Equals) {
                    criteria = ComparisonCriteria.Equals;
                } else if (pref.criteria == ParsedComparisonCriteria.NotEquals) {
                    criteria = ComparisonCriteria.NotEquals;
                } else if (pref.criteria == ParsedComparisonCriteria.BiggerThan) {
                    criteria = ComparisonCriteria.BiggerThan;
                } else if (pref.criteria == ParsedComparisonCriteria.BiggerThanOrEquals) {
                    criteria = ComparisonCriteria.BiggerThanOrEquals;
                } else if (pref.criteria == ParsedComparisonCriteria.LessThan) {
                    criteria = ComparisonCriteria.LessThan;
                } else if (pref.criteria == ParsedComparisonCriteria.LessThanOrEquals) {
                    criteria = ComparisonCriteria.LessThanOrEquals;
                } else {
                    throw new NotImplementedException ();
                }

                return new ComparisonPredicate () {
                    Criteria = criteria,
                    Left = BuildFormula (pref.Left, declaredVariables),
                    Right = BuildFormula (pref.Right, declaredVariables)
                };
            } else if (value.GetType () == typeof (ParsedStringConstantExpression)) {
                return new StringConstant { Value = Sanitize ((value as ParsedStringConstantExpression).Value) };

            } else if (value.GetType () == typeof (ParsedNumericConstantExpression)) {
                return new NumericConstant { Value = (value as ParsedNumericConstantExpression).Value };
            } else if (value.GetType () == typeof (ParsedBoolConstantExpression)) {
                return new BoolConstant { Value = (value as ParsedBoolConstantExpression).Value };
            } else if (value.GetType () == typeof (ParsedVariableReference)) {
                if (!declaredVariables.ContainsKey ((value as ParsedVariableReference).Value)) {
					throw new BuilderException(string.Format("Variable '{0}' is not declared", (value as ParsedVariableReference).Value),
													value.Filename, value.Line, value.Col);
                }

                return new VariableReference { Name = (value as ParsedVariableReference).Value };
            }
            
            throw new NotImplementedException (string.Format ("{0} is not yet supported", 
                                                              value.GetType ().Name));
        }
        
        public Formula BuildPredicateFormula (KAOSTools.Core.Predicate p, ParsedElement value)
        {
            var dict = new Dictionary<string, KAOSTools.Core.Entity> ();
            
            foreach (var attr in p.Arguments) {
                dict.Add (attr.Name, attr.Type);
            }
            
            return BuildFormula (value, dict);
        }

        #region 

        TimeBound BuildTimeBound (ParsedTimeBound parsed) {
            if (parsed == null)
                return null;

            TimeComparator comparator;
            if (parsed.Comparator == ParsedTimeComparator.equal) {
                comparator = TimeComparator.equal;
            } else if (parsed.Comparator == ParsedTimeComparator.greater) {
                comparator = TimeComparator.greater;
            } else if (parsed.Comparator == ParsedTimeComparator.less) {
                comparator = TimeComparator.less;
            } else if (parsed.Comparator == ParsedTimeComparator.strictly_greater) {
                comparator = TimeComparator.strictly_greater;
            } else if (parsed.Comparator == ParsedTimeComparator.strictly_less) {
                comparator = TimeComparator.strictly_less;
            } else {
                throw new NotImplementedException ();
            }

            return new TimeBound {
                Comparator = comparator,
                Bound = BuildTime (parsed.Bound)
            };
        }

        TimeSpan BuildTime (ParsedTime parsed) {
            return parsed.Constraints.Select (BuildAtomicTime).Aggregate (new TimeSpan(), (x,y) => x+y);
        }

        TimeSpan BuildAtomicTime (ParsedAtomicTime parsed) {
            if (parsed.Unit == ParsedTimeUnit.day)
                return new TimeSpan (parsed.Duration, 0, 0, 0, 0);
            
            if (parsed.Unit == ParsedTimeUnit.hour)
                return new TimeSpan (0, parsed.Duration, 0, 0, 0);

            if (parsed.Unit == ParsedTimeUnit.minute)
                return new TimeSpan (0, 0, parsed.Duration, 0, 0);
            
            if (parsed.Unit == ParsedTimeUnit.second)
                return new TimeSpan (0, 0, 0, parsed.Duration, 0);
            
            if (parsed.Unit == ParsedTimeUnit.milisecond)
                return new TimeSpan (0, 0, 0, 0, parsed.Duration);

            throw new NotImplementedException ();
        }

        #endregion

        Entity GetOrCreateEntity (dynamic idOrName) {
            Entity type;
            if (idOrName is NameExpression)
                type = model.Entities().SingleOrDefault (t => t.Name == idOrName.Value);
            else if (idOrName is IdentifierExpression)
                type = model.Entities().SingleOrDefault (t => t.Identifier == idOrName.Value);
            else 
                throw new NotImplementedException ();

            if (type == null) {
                if (idOrName is NameExpression)
                    type = new Entity (model) { Name = idOrName.Value, Implicit = true };
                else if (idOrName is IdentifierExpression)
                    type = new Entity (model) { Identifier = idOrName.Value, Implicit = true };
                else 
                    throw new NotImplementedException ();

                model.Add (type);


            } else {
                  
            }

            return type;
        }
        
        
        KAOSTools.Core.EntityAttribute GetOrCreateAttribute (ParsedAttributeReferenceExpression pref, 
            KAOSTools.Core.Entity entity) {
            Console.WriteLine (">> " + pref.AttributeSignature.Value + " <<");
            if (entity != null) {
                if (pref.AttributeSignature is NameExpression) {
                    var attribute = entity.Attributes().SingleOrDefault (x => x.Name == pref.AttributeSignature.Value);
                    if (attribute == null) {
                        attribute = new KAOSTools.Core.EntityAttribute (model) { 
                            Name = pref.AttributeSignature.Value, 
                            Implicit = true
                        } ;
                        attribute.SetEntity (entity);
                        model.Add (attribute);


                    } else {
                        
                    }
                    return attribute;

                } else if (pref.AttributeSignature is IdentifierExpression) {
                    var attribute = entity.model.Attributes().SingleOrDefault (x => x.Identifier == entity.Identifier + "." +pref.AttributeSignature.Value);
                    if (attribute == null) {
                        attribute = new KAOSTools.Core.EntityAttribute (model) { 
                            Identifier = entity.Identifier + "." + pref.AttributeSignature.Value, 
                            Implicit = true
                        } ;
                        attribute.SetEntity (entity);
                        model.Add (attribute);


                    } else {
                        
                    }
                    return attribute;
                } else 
                    throw new NotImplementedException (pref.AttributeSignature.GetType() + " is not yet supported");
                
            } else {
                throw new Exception (string.Format("Entity '{0}' not found", pref.Variable));
            }
        }
        
        Relation GetOrCreateRelation (ParsedInRelationExpression rel, Dictionary<string, KAOSTools.Core.Entity> declarations)
        {
            dynamic identifierOrName = rel.Relation;

            Relation type;
            if (identifierOrName is NameExpression) {
                type = model.Relations().SingleOrDefault (t => t.Name == identifierOrName.Value);

            } else if (identifierOrName is IdentifierExpression) {
                type = model.Relations().SingleOrDefault (t => t.Identifier == identifierOrName.Value);

            } else {
                throw new NotImplementedException ();
            }
            
            if (type == null) {
                if (identifierOrName is NameExpression) {
                    type = new Relation(model) { Name = identifierOrName.Value, Implicit = true };
                } else if (identifierOrName is IdentifierExpression) {
                    type = new Relation(model) { Identifier = identifierOrName.Value, Implicit = true };
                }

                foreach (var arg in rel.Variables) {
                    type.Links.Add (new Link(model) { Target = declarations[arg] });
                }

                model.Add (type);


            } else {
                // Check that types matches
                // TODO make this shit more robust. In the case of two links to a same entity, this
                // check will fail...
                foreach (var arg in rel.Variables) {
                    if (type.Links.Count(x => x.Target == declarations[arg]) == 0) {
						throw new BuilderException("Relation and formal spec are incompatible.",
													rel.Filename, rel.Line, rel.Col);
                    }
                }

             }


            return type;
        }
        
        Predicate GetOrCreatePredicate (ParsedPredicateReferenceExpression parsedPred,
                                        Dictionary<string, KAOSTools.Core.Entity> declarations)
        {
            var idOrName = parsedPred.PredicateSignature;
            Predicate predicate;
            if (idOrName is NameExpression)
                predicate = model.Predicates().SingleOrDefault (t => t.Name == idOrName.Value);
            else if (idOrName is IdentifierExpression)
                predicate = model.Predicates().SingleOrDefault (t => t.Identifier == idOrName.Value);
            else
                throw new NotImplementedException (idOrName.GetType().Name + " is not yet supported");

            if (predicate == null) {
                if (idOrName is NameExpression)
                    predicate = new Predicate(model) { Name = idOrName.Value, Implicit = true };
                else if (idOrName is IdentifierExpression)
                    predicate = new Predicate(model) { Identifier = idOrName.Value, Implicit = true };
                else
                    throw new NotImplementedException ();

                foreach (var arg in parsedPred.ActualArguments) {
                    predicate.Arguments.Add (new PredicateArgument() { Name = arg, Type = declarations[arg] });
                }

                model.Add (predicate);

            } else {
                // Check that same number of arguments are used (if none is already declared)
                if (predicate.Arguments.Count > 0 && parsedPred.ActualArguments.Count != predicate.Arguments.Count) {
                    throw new BuilderException ("Predicate '" + idOrName.Value + "' arguments mismatch. " +
													"Expect " + predicate.Arguments.Count + " arguments but " + parsedPred.ActualArguments.Count + " received.",
												parsedPred.Filename, parsedPred.Line, parsedPred.Col);
                } else {
                    // Check that arguments match the declared type (if none is already declared)
                    if (predicate.Arguments.Count > 0) {
                        for (int i = 0; i < parsedPred.ActualArguments.Count; i++) {
                            var parsedArg = parsedPred.ActualArguments[i];
                            if (!declarations[parsedArg].Ancestors().Contains(predicate.Arguments[i].Type)) {
                                throw new BuilderException ("Predicate '" + idOrName + "' arguments mismatch. " +
                                                            "Argument '" + parsedArg + "' is type '" + declarations[parsedArg].FriendlyName + "' " +
                                                            "but type '" + predicate.Arguments[i].Type.FriendlyName + "' is expected. Make sure that you do not mix names and identifiers.",
                                                            parsedPred.Filename, parsedPred.Line, parsedPred.Col);
                            }
                        }
                    }
                    
                    // Otherwise, create all needed arguments
                    else {
                        for (int i = 0; i < parsedPred.ActualArguments.Count; i++) {
                            var parsedArg = parsedPred.ActualArguments[i];
                            predicate.Arguments.Add (new PredicateArgument () { 
                                Name = parsedArg, 
                                Type = declarations[parsedArg]
                            });
                        }
                    }
                }



            }
            
            return predicate;
        }

        protected string Sanitize (string text) 
        {
            var t = Regex.Replace(text, @"\s+", " ", RegexOptions.Multiline).Trim ();
            t = Regex.Replace (t, "\"\"", "\"", RegexOptions.Multiline);
            return t;
        }
    }
}

