using System;
using KAOSTools.MetaModel;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace KAOSTools.Parsing
{
    public class FormulaBuilder
    {
        private KAOSModel model;
        private IDictionary<KAOSMetaModelElement, IList<Declaration>> Declarations;
        private FirstStageBuilder FSB;
        protected Uri relativePath;

        public FormulaBuilder (KAOSModel model, 
                               IDictionary<KAOSMetaModelElement, 
                               IList<Declaration>> declarations, 
                               FirstStageBuilder fsb,
                               Uri relativePath)
        {
            this.model = model;
            this.Declarations = declarations;
            this.FSB = fsb;
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
            
            if (value.GetType() == typeof (ParsedForallExpression)) {
                var a = new Forall ();
                var d2 = new Dictionary<string, KAOSTools.MetaModel.Entity> (declaredVariables);
                foreach (var arg in (value as ParsedForallExpression).arguments) {
                    var name = arg.VariableName;
                    var type = GetOrCreateEntity (arg.Type);
                    
                    if (declaredVariables.ContainsKey(name)) {
                        throw new CompilationException (string.Format ("'{0}' is already defined", name));
                    }
                    
                    a.Declarations.Add (new KAOSTools.MetaModel.ArgumentDeclaration() {
                        Name = name,
                        Type = type
                    });
                    d2.Add (name, type);
                }
                a.Enclosed = BuildFormula ((value as ParsedForallExpression).Enclosed, d2);
                return a;
            } else if (value.GetType() == typeof (ParsedExistsExpression)) {
                var a = new Exists ();
                var d2 = new Dictionary<string, KAOSTools.MetaModel.Entity> (declaredVariables);
                foreach (var arg in (value as ParsedExistsExpression).arguments) {
                    var name = arg.VariableName;
                    var type = GetOrCreateEntity (arg.Type);
                    
                    if (declaredVariables.ContainsKey(name)) {
                        throw new CompilationException (string.Format ("'{0}' is already defined", name));
                    }
                    
                    a.Declarations.Add (new KAOSTools.MetaModel.ArgumentDeclaration() {
                        Name = name,
                        Type = type
                    });
                    d2.Add (name, type);
                }
                a.Enclosed = BuildFormula ((value as ParsedExistsExpression).Enclosed, d2);
                return a;
            } else if (value.GetType() == typeof (ParsedStrongImplyExpression)) {
                return new StrongImply () { 
                    Left = BuildFormula ((value as ParsedStrongImplyExpression).Left, declaredVariables),
                    Right = BuildFormula ((value as ParsedStrongImplyExpression).Right, declaredVariables)
                };
            } else if (value.GetType() == typeof (ParsedImplyExpression)) {
                return new Imply () { 
                    Left = BuildFormula ((value as ParsedImplyExpression).Left, declaredVariables),
                    Right = BuildFormula ((value as ParsedImplyExpression).Right, declaredVariables)
                };
            } else if (value.GetType() == typeof (ParsedEquivalenceExpression)) {
                return new Equivalence () { 
                    Left = BuildFormula ((value as ParsedEquivalenceExpression).Left, declaredVariables),
                    Right = BuildFormula ((value as ParsedEquivalenceExpression).Right, declaredVariables)
                };
            } else if (value.GetType() == typeof (ParsedUntilExpression)) {
                return new Until () { 
                    Left = BuildFormula ((value as ParsedUntilExpression).Left, declaredVariables),
                    Right = BuildFormula ((value as ParsedUntilExpression).Right, declaredVariables)
                };
            } else if (value.GetType() == typeof (ParsedUnlessExpression)) {
                return new Unless () { 
                    Left = BuildFormula ((value as ParsedUnlessExpression).Left, declaredVariables),
                    Right = BuildFormula ((value as ParsedUnlessExpression).Right, declaredVariables)
                };
            } else if (value.GetType() == typeof (ParsedReleaseExpression)) {
                return new Release () { 
                    Left = BuildFormula ((value as ParsedReleaseExpression).Left, declaredVariables),
                    Right = BuildFormula ((value as ParsedReleaseExpression).Right, declaredVariables)
                };
            } else if (value.GetType() == typeof (ParsedAndExpression)) {
                return new And () { 
                    Left = BuildFormula ((value as ParsedAndExpression).Left, declaredVariables),
                    Right = BuildFormula ((value as ParsedAndExpression).Right, declaredVariables)
                };
            } else if (value.GetType() == typeof (ParsedOrExpression)) {
                return new Or () { 
                    Left = BuildFormula ((value as ParsedOrExpression).Left, declaredVariables),
                    Right = BuildFormula ((value as ParsedOrExpression).Right, declaredVariables)
                };
            } else if (value.GetType() == typeof (ParsedNotExpression)) {
                return new Not () { 
                    Enclosed = BuildFormula ((value as ParsedNotExpression).Enclosed, declaredVariables)
                };
            } else if (value.GetType() == typeof (ParsedNextExpression)) {
                return new Next () { 
                    Enclosed = BuildFormula ((value as ParsedNextExpression).Enclosed, declaredVariables)
                };
            } else if (value.GetType() == typeof (ParsedEventuallyExpression)) {
                return new Eventually () { 
                    Enclosed = BuildFormula ((value as ParsedEventuallyExpression).Enclosed, declaredVariables),
                    TimeBound = BuildTimeBound ((value as ParsedEventuallyExpression).TimeBound)
                };

            } else if (value.GetType() == typeof (ParsedEventuallyBeforeExpression)) {
                return new EventuallyBefore () { 
                    Left = BuildFormula ((value as ParsedEventuallyBeforeExpression).Left, declaredVariables),
                    Right = BuildFormula ((value as ParsedEventuallyBeforeExpression).Right, declaredVariables),
                    TimeBound = BuildTimeBound ((value as ParsedEventuallyBeforeExpression).TimeBound)
                };

            } else if (value.GetType() == typeof (ParsedGloballyExpression)) {
                return new Globally () { 
                    Enclosed = BuildFormula ((value as ParsedGloballyExpression).Enclosed, declaredVariables),
                    TimeBound = BuildTimeBound ((value as ParsedGloballyExpression).TimeBound)
                };
                
            } else if (value.GetType() == typeof (ParsedPredicateReferenceExpression)) {
                var prel = value as ParsedPredicateReferenceExpression;
                
                // Check if arguments are all defined
                foreach (var arg in prel.ActualArguments) {
                    if (!declaredVariables.ContainsKey (arg)) {
                        throw new CompilationException (string.Format("'{0}' is not declared ({1}:{2},{3})", 
                                                                      arg, prel.Filename, prel.Line, prel.Col));
                    }
                }

                return new PredicateReference () {
                    Predicate = GetOrCreatePredicate (prel, declaredVariables),
                    ActualArguments = prel.ActualArguments
                };
            } else if (value.GetType() == typeof (ParsedInRelationExpression)) {
                var prel = value as ParsedInRelationExpression;
                foreach (var arg in prel.Variables) {
                    if (!declaredVariables.ContainsKey (arg)) {
                        throw new CompilationException (string.Format("'{0}' is not declared", arg));
                    }
                }
                
                return new RelationReference () {
                    Relation = GetOrCreateRelation (value as ParsedInRelationExpression, declaredVariables),
                    ActualArguments = prel.Variables
                };
            } else if (value.GetType() == typeof (ParsedAttributeReferenceExpression)) {
                var pref = value as ParsedAttributeReferenceExpression;
                if (declaredVariables.ContainsKey(pref.Variable)) {
                    return new AttributeReference () {
                        Variable = pref.Variable,
                        Entity = declaredVariables[pref.Variable],
                        Attribute = GetOrCreateAttribute (value as ParsedAttributeReferenceExpression, declaredVariables[pref.Variable])
                    };
                } else {
                    throw new CompilationException (string.Format ("Variable '{0}' is not declared", pref.Variable));
                }
                
            } else if (value.GetType() == typeof (ParsedComparisonExpression)) {
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
            } else if (value.GetType() == typeof (ParsedStringConstantExpression)) {
                return new StringConstant { Value = Sanitize((value as ParsedStringConstantExpression).Value) };
                
            } else if (value.GetType() == typeof (ParsedNumericConstantExpression)) {
                return new NumericConstant { Value = (value as ParsedNumericConstantExpression).Value };
            } else if (value.GetType() == typeof (ParsedBoolConstantExpression)) {
                return new BoolConstant { Value = (value as ParsedBoolConstantExpression).Value };
            } else if (value.GetType() == typeof (ParsedVariableReference)) {
                if (!declaredVariables.ContainsKey((value as ParsedVariableReference).Value)) {
                    throw new CompilationException (string.Format ("Variable '{0}' is not declared", (value as ParsedVariableReference).Value));
                }

                return new VariableReference { Name = (value as ParsedVariableReference).Value };
            }
            
            throw new NotImplementedException (string.Format ("{0} is not yet supported", 
                                                              value.GetType ().Name));
        }
        
        public Formula BuildPredicateFormula (KAOSTools.MetaModel.Predicate p, ParsedElement value)
        {
            var dict = new Dictionary<string, KAOSTools.MetaModel.Entity> ();
            
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
                type = model.Entities.SingleOrDefault (t => t.Name == idOrName.Value);
            else if (idOrName is IdentifierExpression)
                type = model.Entities.SingleOrDefault (t => t.Identifier == idOrName.Value);
            else 
                throw new NotImplementedException ();

            if (type == null) {
                if (idOrName is NameExpression)
                    type = new Entity { Name = idOrName.Value, Implicit = true };
                else if (idOrName is IdentifierExpression)
                    type = new Entity { Identifier = idOrName.Value, Implicit = true };
                else 
                    throw new NotImplementedException ();

                model.Entities.Add (type);

                Declarations.Add (type, new List<Declaration> () {
                    new Declaration (idOrName.Line, idOrName.Col, idOrName.Filename, relativePath, DeclarationType.Reference)
                });
            } else {
                Declarations[type].Add (
                    new Declaration (idOrName.Line, idOrName.Col, idOrName.Filename, relativePath, DeclarationType.Reference)
                );
            }

            return type;
        }
        
        
        KAOSTools.MetaModel.Attribute GetOrCreateAttribute (ParsedAttributeReferenceExpression pref, KAOSTools.MetaModel.Entity entity) {
            if (entity != null) {
                if (pref.AttributeSignature is NameExpression) {
                    var attribute = entity.Attributes.SingleOrDefault (x => x.Name == pref.AttributeSignature.Value);
                    if (attribute == null) {
                        attribute = new KAOSTools.MetaModel.Attribute () { Name = pref.AttributeSignature.Value, Implicit = true } ;
                        entity.Attributes.Add (attribute);

                        Declarations.Add (attribute, new List<Declaration> () {
                            new Declaration (pref.Line, pref.Col, pref.Filename, relativePath, DeclarationType.Reference)
                        });
                    } else {
                        Declarations[attribute].Add (
                            new Declaration (pref.Line, pref.Col, pref.Filename, relativePath, DeclarationType.Reference)
                            );
                    }
                    return attribute;

                } else if (pref.AttributeSignature is IdentifierExpression) {
                    var attribute = entity.Attributes.SingleOrDefault (x => x.Identifier == pref.AttributeSignature.Value);
                    if (attribute == null) {
                        attribute = new KAOSTools.MetaModel.Attribute () { Identifier = pref.AttributeSignature.Value, Implicit = true } ;
                        entity.Attributes.Add (attribute);

                        Declarations.Add (attribute, new List<Declaration> () {
                            new Declaration (pref.Line, pref.Col, pref.Filename, relativePath, DeclarationType.Reference)
                        });
                    } else {
                        Declarations[attribute].Add (
                            new Declaration (pref.Line, pref.Col, pref.Filename, relativePath, DeclarationType.Reference)
                            );
                    }
                    return attribute;
                } else 
                    throw new NotImplementedException (pref.AttributeSignature.GetType() + " is not yet supported");
                
            } else {
                throw new Exception (string.Format("Entity '{0}' not found", pref.Variable));
            }
        }
        
        Relation GetOrCreateRelation (ParsedInRelationExpression rel, Dictionary<string, KAOSTools.MetaModel.Entity> declarations)
        {
            dynamic identifierOrName = rel.Relation;

            Relation type;
            if (identifierOrName is NameExpression) {
                type = model.Entities.SingleOrDefault (t => t.GetType() == typeof(Relation) & t.Name == identifierOrName.Value) as Relation;

            } else if (identifierOrName is IdentifierExpression) {
                type = model.Entities.SingleOrDefault (t => t.GetType() == typeof(Relation) & t.Identifier == identifierOrName.Value) as Relation;

            } else {
                throw new NotImplementedException ();
            }
            
            if (type == null) {
                if (identifierOrName is NameExpression) {
                    type = new Relation() { Name = identifierOrName.Value, Implicit = true };
                } else if (identifierOrName is IdentifierExpression) {
                    type = new Relation() { Identifier = identifierOrName.Value, Implicit = true };
                }

                foreach (var arg in rel.Variables) {
                    type.Links.Add (new Link(declarations[arg]));
                }

                model.Entities.Add (type);

                Declarations.Add (type, new List<Declaration> () {
                    new Declaration (identifierOrName.Line, identifierOrName.Col, identifierOrName.Filename, relativePath, DeclarationType.Reference)
                });
            } else {
                // Check that types matches
                // TODO make this shit more robust. In the case of two links to a same entity, this
                // check will fail...
                foreach (var arg in rel.Variables) {
                    if (type.Links.Count(x => x.Target == declarations[arg]) == 0) {
                        throw new CompilationException ("Relation and formal spec are incompatible.");
                    }
                }
                Declarations[type].Add (
                    new Declaration (identifierOrName.Line, identifierOrName.Col, identifierOrName.Filename, relativePath, DeclarationType.Reference)
                    );

             }


            return type;
        }
        
        Predicate GetOrCreatePredicate (ParsedPredicateReferenceExpression parsedPred,
                                        Dictionary<string, KAOSTools.MetaModel.Entity> declarations)
        {
            var idOrName = parsedPred.PredicateSignature;
            Predicate predicate;
            if (idOrName is NameExpression)
                predicate = model.Predicates.SingleOrDefault (t => t.Name == idOrName.Value);
            else if (idOrName is IdentifierExpression)
                predicate = model.Predicates.SingleOrDefault (t => t.Identifier == idOrName.Value);
            else
                throw new NotImplementedException (idOrName.GetType().Name + " is not yet supported");

            if (predicate == null) {
                if (idOrName is NameExpression)
                    predicate = new Predicate() { Name = idOrName.Value, Implicit = true };
                else if (idOrName is IdentifierExpression)
                    predicate = new Predicate() { Identifier = idOrName.Value, Implicit = true };
                else
                    throw new NotImplementedException ();

                foreach (var arg in parsedPred.ActualArguments) {
                    predicate.Arguments.Add (new PredicateArgument() { Name = arg, Type = declarations[arg] });
                }

                model.Predicates.Add (predicate);
                Declarations.Add (predicate, new List<Declaration> () {
                    new Declaration (idOrName.Line, idOrName.Col, idOrName.Filename, relativePath, DeclarationType.Reference)
                });

            } else {
                // Check that same number of arguments are used (if none is already declared)
                if (predicate.Arguments.Count > 0 && parsedPred.ActualArguments.Count != predicate.Arguments.Count) {
                    throw new CompilationException ("Predicate '" + idOrName.Value + "' arguments mismatch. " +
                                                    "Expect " + predicate.Arguments.Count + " arguments but " + parsedPred.ActualArguments.Count + " received.");
                } else {
                    // Check that arguments match the declared type (if none is already declared)
                    if (predicate.Arguments.Count > 0) {
                        for (int i = 0; i < parsedPred.ActualArguments.Count; i++) {
                            var parsedArg = parsedPred.ActualArguments[i];
                            if (!declarations[parsedArg].Ancestors.Contains(predicate.Arguments[i].Type)) {
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

                Declarations[predicate].Add (
                    new Declaration (idOrName.Line, idOrName.Col, idOrName.Filename, relativePath, DeclarationType.Reference)
                    );

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

