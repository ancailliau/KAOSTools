using System;
using KAOSTools.MetaModel;
using System.Collections.Generic;
using System.Linq;

namespace KAOSTools.Parsing
{
    public class FormulaBuilder
    {
        private KAOSModel model;
        private IDictionary<KAOSMetaModelElement, IList<Declaration>> Declarations;
        private FirstStageBuilder FSB;

        public FormulaBuilder (KAOSModel model, 
                               IDictionary<KAOSMetaModelElement, 
                               IList<Declaration>> declarations, 
                               FirstStageBuilder fsb)
        {
            this.model = model;
            this.Declarations = declarations;
            this.FSB = fsb;
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
                    Enclosed = BuildFormula ((value as ParsedEventuallyExpression).Enclosed, declaredVariables)
                };
            } else if (value.GetType() == typeof (ParsedGloballyExpression)) {
                return new Globally () { 
                    Enclosed = BuildFormula ((value as ParsedGloballyExpression).Enclosed, declaredVariables)
                };
                
            } else if (value.GetType() == typeof (ParsedPredicateReferenceExpression)) {
                var prel = value as ParsedPredicateReferenceExpression;
                
                // Check if arguments are all defined
                foreach (var arg in prel.ActualArguments) {
                    if (!declaredVariables.ContainsKey (arg.Value)) {
                        throw new CompilationException (string.Format("'{0}' is not declared ({1}:{2},{3})", 
                                                                      arg.Value, prel.Filename, prel.Line, prel.Col));
                    }
                }
                
                return new PredicateReference () {
                    Predicate = GetOrCreatePredicate (prel, declaredVariables)
                };
            } else if (value.GetType() == typeof (ParsedInRelationExpression)) {
                var prel = value as ParsedInRelationExpression;
                foreach (var arg in prel.Variables) {
                    if (!declaredVariables.ContainsKey (arg)) {
                        throw new CompilationException (string.Format("'{0}' is not declared", arg));
                    }
                }
                
                return new RelationReference () {
                    Relation = GetOrCreateRelation (value as ParsedInRelationExpression, declaredVariables)
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
                return new StringConstant { Value = (value as ParsedStringConstantExpression).Value };
                
            } else if (value.GetType() == typeof (ParsedBoolConstantExpression)) {
                return new NumericConstant { Value = (value as ParsedNumericConstantExpression).Value };
            }
            
            throw new NotImplementedException ();
        }
        
        public Formula BuildPredicateFormula (KAOSTools.MetaModel.Predicate p, ParsedElement value)
        {
            var dict = new Dictionary<string, KAOSTools.MetaModel.Entity> ();
            
            foreach (var attr in p.Arguments) {
                dict.Add (attr.Name, attr.Type);
            }
            
            return BuildFormula (value, dict);
        }

        
        private KAOSTools.MetaModel.Entity GetOrCreateEntity (string signature) {
            var type = model.Entities.SingleOrDefault (t => t.Name == signature);
            
            if (type == null) {
                type = new KAOSTools.MetaModel.Entity() { Name = signature, Implicit = true };
                model.Entities.Add (type);
            }
            
            return type;
        }
        
        
        private KAOSTools.MetaModel.Attribute GetOrCreateAttribute (ParsedAttributeReferenceExpression pref, KAOSTools.MetaModel.Entity entity) {
            if (entity != null) {
                var attribute = entity.Attributes.SingleOrDefault (x => x.Name == pref.AttributeSignature);
                if (attribute == null) {
                    attribute = new KAOSTools.MetaModel.Attribute () { Name = pref.AttributeSignature, Implicit = true } ;
                    entity.Attributes.Add (attribute);
                }
                return attribute;
                
            } else {
                throw new Exception (string.Format("Entity '{0}' not found", pref.Variable));
            }
        }
        
        private KAOSTools.MetaModel.Relation GetOrCreateRelation (ParsedInRelationExpression rel, Dictionary<string, KAOSTools.MetaModel.Entity> declarations)
        {
            string identifierOrName = rel.Relation;
            var type = model.Entities.SingleOrDefault (t => t.GetType() == typeof(Relation) & t.Identifier == identifierOrName) as Relation;
            
            if (type == null) {
                type = model.Entities.FirstOrDefault (t => t.GetType() == typeof(Relation) & t.Name == identifierOrName) as Relation;
                
                if (type == null) {
                    type = new KAOSTools.MetaModel.Relation() { Identifier = identifierOrName, Implicit = true };
                    foreach (var arg in rel.Variables) {
                        type.Links.Add (new KAOSTools.MetaModel.Link(declarations[arg]));
                    }
                    model.Entities.Add (type);
                }
            } else {
                // Check that types matches
                // TODO make this shit more robust. In the case of two links to a same entity, this
                // check will fail...
                foreach (var arg in rel.Variables) {
                    if (type.Links.Count(x => x.Target == declarations[arg]) == 0) {
                        throw new CompilationException ("Relation and formal spec are incompatible.");
                    }
                }
            }
            
            return type;
        }
        
        private KAOSTools.MetaModel.Predicate GetOrCreatePredicate (ParsedPredicateReferenceExpression parsedPred, 
                                                                    Dictionary<string, KAOSTools.MetaModel.Entity> declarations)
        {
            var signature = parsedPred.PredicateSignature;
            var predicate = model.Predicates.SingleOrDefault (t => t.Signature == signature);
            
            if (predicate == null) {
                predicate = new KAOSTools.MetaModel.Predicate() { Signature = signature, Implicit = true };
                foreach (var arg in parsedPred.ActualArguments) {
                    predicate.Arguments.Add (new PredicateArgument() { Name = arg.Value, Type = declarations[arg.Value] });
                }
                model.Predicates.Add (predicate);
            } else {
                // Check that same number of arguments are used (if none is already declared)
                if (predicate.Arguments.Count > 0 && parsedPred.ActualArguments.Count != predicate.Arguments.Count) {
                    throw new CompilationException ("Predicate '" + signature + "' arguments mismatch. " +
                                                    "Expect " + predicate.Arguments.Count + " arguments but " + parsedPred.ActualArguments.Count + " received.");
                } else {
                    // Check that arguments match the declared type (if none is already declared)
                    if (predicate.Arguments.Count > 0) {
                        for (int i = 0; i < parsedPred.ActualArguments.Count; i++) {
                            var parsedArg = parsedPred.ActualArguments[i];
                            if (declarations[parsedArg.Value] != predicate.Arguments[i].Type) {
                                throw new CompilationException ("Predicate '" + signature + "' arguments mismatch. " +
                                                                "Argument '" + parsedArg.Value + "' is type '" + declarations[parsedArg.Value].Name + "' " +
                                                                "but type '" + predicate.Arguments[i].Name + "' is expected.");
                            }
                        }
                    }
                    
                    // Otherwise, create all needed arguments
                    else {
                        for (int i = 0; i < parsedPred.ActualArguments.Count; i++) {
                            var parsedArg = parsedPred.ActualArguments[i];
                            predicate.Arguments.Add (new PredicateArgument () { 
                                Name = parsedArg.Value, 
                                Type = declarations[parsedArg.Value]
                            });
                        }
                    }
                }
            }
            
            return predicate;
        }
    }
}

