using System;
using System.Linq;
using System.Collections.Generic;
using KAOSTools.Parsing;
using System.IO;

namespace KAOSTools.Parsing {
    sealed partial class GoalModelParser
    {   
        List<string> files_imported = new List<string> ();

        ParsedElement BuildElements (List<Result> results)
        {
            var attrs = new ParsedElements();
            foreach (var result in results) {
                BuildElement (attrs, result.Value);
            }
            return attrs;
        }

        ParsedElement BuildElement (ParsedElements attrs, ParsedElement value)
        {
            if (value is ParsedElements) {
                foreach (var result2 in ((ParsedElements) value).Values) {
                    BuildElement (attrs, result2);
                }
            } else {
                attrs.Values.Add (value);
            }
            return attrs;
        }

        ParsedElement Import (string file)
        {
            var filename = Path.Combine (Path.GetDirectoryName (m_file), file);
            if (files_imported.Contains (Path.GetFullPath (filename))) {
                return new ParsedElements ();
            }
            
            if (File.Exists (filename)) {
                files_imported.Add (Path.GetFullPath (filename));
                
                string input = File.ReadAllText (filename);
                var parser = new GoalModelParser ();
                parser.files_imported = this.files_imported;
                var m2 = parser.Parse (input, filename);
                return m2;
            }

            throw new FileNotFoundException ("Included file `" + filename + "` not found", filename);
        }

        #region First-class declarations

        ParsedElement BuildParsedElementWithAttributes<T> (List<Result> results)
            where T: ParsedElementWithAttributes, new()
        {
            var t = new T () { 
                Line = results[0].Line, 
                Col = results[0].Col, 
                Filename = m_file
            };
            
            if (results[0].Text == "override")
                t.Override = true;
            
            for (int i = 2; i < results.Count - 1; i++) {
                t.Attributes.Add (results[i].Value as ParsedAttribute);
            }
            
            return t;
        }

        ParsedElement BuildPredicate (List<Result> results)
        {
            return BuildParsedElementWithAttributes<ParsedPredicate> (results);
        }

        ParsedElement BuildSystem (List<Result> results)
        {
            return BuildParsedElementWithAttributes<ParsedSystem> (results);
        }

        ParsedElement BuildGoal (List<Result> results)
        {
            return BuildParsedElementWithAttributes<ParsedGoal> (results);
        }
        
        ParsedElement BuildDomainProperty (List<Result> results)
        {
            return BuildParsedElementWithAttributes<ParsedDomainProperty> (results);
        }
        
        ParsedElement BuildObstacle (List<Result> results)
        {
            return BuildParsedElementWithAttributes<ParsedObstacle> (results);
        }
        
        ParsedElement BuildAgent (List<Result> results)
        {
            return BuildParsedElementWithAttributes<ParsedAgent> (results);
        }
        
        ParsedElement BuildDomainHypothesis (List<Result> results)
        {
            return BuildParsedElementWithAttributes<ParsedDomainHypothesis> (results);
        }
        
        ParsedElement BuildEntity (List<Result> results)
        {
            return BuildParsedElementWithAttributes<ParsedEntity> (results);
        }
        
        ParsedElement BuildType (List<Result> results)
        {
            return BuildParsedElementWithAttributes<ParsedGivenType> (results);
        }
        
        ParsedElement BuildAssociation (List<Result> results)
        {
            return BuildParsedElementWithAttributes<ParsedAssociation> (results);
        }

        #endregion

        #region Attributes

        ParsedElement BuildParsedAttributeWithValue<T1> (List<Result> results, string value)
            where T1: ParsedAttributeWithValue<string>, new()
        {
            return BuildParsedAttributeWithValue<T1, string> (results, value);
        }

        ParsedElement BuildParsedAttributeWithValue<T1> (List<Result> results, double value)
            where T1: ParsedAttributeWithValue<double>, new()
        {
            return BuildParsedAttributeWithValue<T1, double> (results, value);
        }

        ParsedElement BuildParsedAttributeWithValue<T1,T2> (List<Result> results, T2 value)
            where T1: ParsedAttributeWithValue<T2>, new()
        {
            return new T1 { 
                Value = value,
                Line = results[0].Line, 
                Col = results[0].Col, 
                Filename = m_file
            };
        }

        ParsedElement BuildParsedAttributeWithElementsAndSystemIdentifier<T> (List<Result> results)
            where T: ParsedAttributeWithElementsAndSystemIdentifier, new()
        {
            var t = new T () { 
                Line = results[0].Line, 
                Col = results[0].Col, 
                Filename = m_file
            };
            
            if (results[1].Text == "[") {
                t.SystemIdentifier = results[2].Value;
                for (int i = 4; i < results.Count; i = i + 2) {
                    t.Values.Add (results[i].Value);
                }
                
            } else {
                for (int i = 1; i < results.Count; i = i + 2) {
                    t.Values.Add (results[i].Value);
                }
            }
            
            return t;
        }

        ParsedElement BuildIdentifierAttribute (List<Result> results)
        {
            return BuildParsedAttributeWithValue<ParsedIdentifierAttribute> 
                (results, results[1].Text);
        }

        ParsedElement BuildNameAttribute (List<Result> results)
        {
            return BuildParsedAttributeWithValue<ParsedNameAttribute> 
                (results, results.Count == 3 ? "" : results[2].Text);
        }

        ParsedElement BuildSignatureAttribute (List<Result> results)
        {
            return BuildParsedAttributeWithValue<ParsedSignatureAttribute> 
                (results, results.Count == 3 ? "" : results[2].Text);
        }

        ParsedElement BuildFormalSpecAttribute (List<Result> results)
        {
            return BuildParsedAttributeWithValue<ParsedFormalSpecAttribute, ParsedElement> 
                (results, results.Count == 3 ? null : results[2].Value);
        }

        ParsedElement BuildDefinitionAttribute (List<Result> results)
        {
            return BuildParsedAttributeWithValue<ParsedDefinitionAttribute> 
                (results, results.Count == 3 ? "" : results[2].Text);
        }

        ParsedElement BuildRDS (List<Result> results)
        {
            return BuildParsedAttributeWithValue<ParsedRDSAttribute> 
                (results, double.Parse (results[1].Text));
        }
        
        ParsedElement BuildProbability (List<Result> results)
        {
            return BuildParsedAttributeWithValue<ParsedProbabilityAttribute> 
                (results, double.Parse (results[1].Text));
        }

        ParsedElement BuildObstructedBy (List<Result> results)
        {
            return BuildParsedAttributeWithValue<ParsedObstructedByAttribute, dynamic> 
                (results, results[1].Value);
        }

        ParsedElement BuildResolvedBy (List<Result> results)
        {
            return BuildParsedAttributeWithValue<ParsedResolvedByAttribute, dynamic> 
                (results, results[1].Value);
        }

        ParsedElement BuildAlternativeAttribute (List<Result> results)
        {
            return BuildParsedAttributeWithValue<ParsedAlternativeAttribute, dynamic> 
                (results, results[1].Value);
        }

        ParsedElement BuildIsA (List<Result> results)
        {
            return BuildParsedAttributeWithValue<ParsedIsAAttribute, dynamic> 
                (results, results[1].Value);
        }

        ParsedElement BuildAgentTypeAttribute (List<Result> results)
        {
            ParsedAgentType type;
            if (results[1].Text == "software") 
                type = ParsedAgentType.Software;
            else if (results[1].Text == "environment") 
                type = ParsedAgentType.Environment;
            else
                type = ParsedAgentType.None;
            
            return BuildParsedAttributeWithValue<ParsedAgentTypeAttribute,ParsedAgentType> 
                (results, type);
        }
        
        ParsedElement BuildEntityTypeAttribute (List<Result> results)
        {
            ParsedEntityType type;
            if (results[1].Text == "software") 
                type = ParsedEntityType.Software;
            else if (results[1].Text == "environment") 
                type = ParsedEntityType.Environment;
            else if (results[1].Text == "shared") 
                type = ParsedEntityType.Shared;
            else
                type = ParsedEntityType.None;
            
            
            return BuildParsedAttributeWithValue<ParsedEntityTypeAttribute,ParsedEntityType> 
                (results, type);
        }

        ParsedElement BuildRefinedBy (List<Result> results)
        {
            return BuildParsedAttributeWithElementsAndSystemIdentifier<ParsedRefinedByAttribute> 
                (results);
        }

        ParsedElement BuildAssignedTo (List<Result> results)
        {
            return BuildParsedAttributeWithElementsAndSystemIdentifier<ParsedAssignedToAttribute> 
                (results);
        }

        ParsedElement BuildAttribute (List<Result> results)
        {
            var name = results [1].Value as NameExpression;
            
            dynamic type = null;
            if (results.Count == 4) {
                type = results [3].Value;
            }
            return new ParsedAttributeAttribute (name.Value, type);
        }
        
        ParsedElement BuildArgument (List<Result> results)
        {
            var name = results [1].Value as IdentifierExpression;
            
            dynamic type = null;
            if (results.Count == 4) {
                type = results [3].Value;
            }
            return new ParsedPredicateArgumentAttribute (name.Value, type);
        }
        
        ParsedElement BuildLink (List<Result> results)
        {
            var link = new KAOSTools.Parsing.ParsedLinkAttribute () { 
                Line = results[0].Line, 
                Col = results[0].Col, 
                Filename = m_file
            };
            
            if (results.Count == 2) {
                link.Target = results[1].Value;
            } else if (results.Count == 3) {
                link.Multiplicity = (results[1].Value as MultiplictyExpression).Value;
                link.Target = results[2].Value;
            }
            
            return link;
        }

        #endregion

        #region Expressions
        
        ParsedElement BuildMultiplicity (List<Result> results)
        {
            if (results.Count > 3) 
                return new MultiplictyExpression { Value = results[1].Text + ".." + results[3].Text };

            return new MultiplictyExpression { Value = results[1].Text };
        }

        ParsedElement BuildIdentifier (List<Result> results)
        {
            return new IdentifierExpression (string.Join ("", results.Select (x => x.Text))) { 
                Line = results[0].Line, 
                Col = results[0].Col, 
                Filename = m_file
            };
        }
        
        ParsedElement BuildName (List<Result> results)
        {
            return new NameExpression (results[1].Text) { 
                Line = results[0].Line, 
                Col = results[0].Col, 
                Filename = m_file
            };
        }

        #endregion

        #region Formal spec

        ParsedElement BuildFormula (List<Result> results)
        {
            // 'forall' S Identifier S ':' S Identifier ( ',' S Identifier S ':' S Identifier )* S '.' S StrongBinary
            if (results[0].Text == "forall") {
                var f = new ParsedForallExpression ();
                for (int i = 1; i < results.Count - 1; i = i + 4) {
                    f.arguments.Add (new KAOSTools.Parsing.ParsedVariableDeclaration(results[i].Text, results[i+2].Text));
                }
                f.Enclosed = results.Last().Value;
                return f;
            }

            // 'exists' S Identifier S ':' S Identifier ( ',' S Identifier S ':' S Identifier )* S '.' S StrongBinary
            if (results[0].Text == "exists") {
                var f = new ParsedExistsExpression ();
                for (int i = 1; i < results.Count - 1; i = i + 4) {
                    f.arguments.Add (new KAOSTools.Parsing.ParsedVariableDeclaration(results[i].Text, results[i+2].Text));
                }
                f.Enclosed = results.Last().Value;
                return f;
            }

            // StrongBinary
            if (results.Count == 1) 
                return results[0].Value;

            return null;
        }

        ParsedElement BuildStrongBinary (List<Result> results)
        {
            // Binary
            if (results.Count == 1) 
                return results[0].Value;

            // 'when' S Binary S 'then' S Formula
            return new ParsedStrongImplyExpression () { Left = results[1].Value, Right = results[3].Value };
        }

        ParsedElement BuildBinary (List<Result> results)
        {
            // TemporalBinary
            if (results.Count == 1) 
                return results[0].Value;

            // 'if' S TemporalBinary S 'then' S Binary
            if (results.Count == 4)
                return new ParsedImplyExpression () { Left = results[1].Value, Right = results[3].Value };

            // TemporalBinary S 'iff' S Binary
            return new ParsedEquivalenceExpression () { Left = results[0].Value, Right = results[2].Value };
        }
        
        ParsedElement BuildTemporalBinary (List<Result> results)
        {
            // And
            if (results.Count == 1) 
                return results[0].Value;

            // And S 'until' S TemporalBinary
            if (results[2].Text == "until")
                return new ParsedUntilExpression () { Left = results[0].Value, Right = results[2].Value };

            // And S 'release' S TemporalBinary
            if (results[2].Text == "release")
                return new ParsedReleaseExpression () { Left = results[0].Value, Right = results[2].Value };

            // And S 'unless' S TemporalBinary
            // if (results[2].Text == "unless")
            return new ParsedUnlessExpression () { Left = results[0].Value, Right = results[2].Value };
        }
        
        private KAOSTools.Parsing.ParsedElement BuildAnd (List<Result> results)
        {
            // Or
            if (results.Count == 1) 
                return results[0].Value;
            
            // Or S 'and' S And
            return new ParsedAndExpression () { Left = results[0].Value, Right = results[2].Value };
        }
        
        private KAOSTools.Parsing.ParsedElement BuildOr (List<Result> results)
        {
            // Unary
            if (results.Count == 1) 
                return results[0].Value;
            
            // Unary S 'or' S Or
            return new ParsedOrExpression () { Left = results[0].Value, Right = results[2].Value };

        }
        
        private KAOSTools.Parsing.ParsedElement BuildUnary (List<Result> results)
        {
            // Atom
            if (results.Count == 1) 
                return results[0].Value;

            // 'not' S Unary
            if (results[0].Text == "not")
                return new ParsedNotExpression () { Enclosed = results[1].Value };

            // 'next' S Unary
            if (results[0].Text == "next")
                return new ParsedNextExpression () { Enclosed = results[1].Value };
            
            // ('sooner-or-later' / 'eventually') S Unary
            if (results[0].Text == "sooner-or-later" | results[0].Text == "eventually")
                return new ParsedEventuallyExpression () { Enclosed = results[1].Value };

            // ('always' / 'globally') S Unary
            // if (results[0].Text == "always" | results[0].Text == "globally")
            return new ParsedGloballyExpression () { Enclosed = results[1].Value };
        }
        
        private KAOSTools.Parsing.ParsedElement BuildAtom (List<Result> results)
        {
            // Identifier
            if (results.Count == 1) {
                return new ParsedPredicateReferenceExpression (results[0].Text)
                    { Line = results[0].Line, Col = results[0].Col, Filename = m_file };
            }

            // '(' S Identifier ( S ',' S Identifier )* S ')' S 'in' S Identifier
            if (results.Count > 3 && results[0].Text == "(") {
                var p = new ParsedInRelationExpression ();
                foreach (var r in results) {
                    if (r.Text == "(" | r.Text == ")" | r.Text == ",")
                        continue;

                    if (r.Text == "in")
                        break;

                    p.Variables.Add (r.Text);
                }
                p.Relation = results.Last().Text;
                return p;
            }

            // Identifier ( S '(' S Identifier ( S ',' S Identifier )* S ')' )
            if (results[1].Text == "(") {
                var p = new ParsedPredicateReferenceExpression (results[0].Text)
                    { Line = results[0].Line, Col = results[0].Col, Filename = m_file };

                for (int i = 2; i < results.Count; i = i + 2) {
                    p.ActualArguments.Add (results[i].Value as IdentifierExpression);
                }
                return p;
            }

            // Identifier '.' Identifier ( ( '==' / '!=' / '>' / '<' / '>=' / '<=' ) ( Identifier '.' Identifier / '"' String '"' / [0-9]+ / Identifier ) )?
            if (results[1].Text == ".") {
                if (results.Count > 3) {
                    ParsedComparisonCriteria criteria;
                    if (results[3].Text == "==") {
                        criteria = ParsedComparisonCriteria.Equals;
                    } else if (results[3].Text == "!=") {
                        criteria = ParsedComparisonCriteria.NotEquals;
                    } else if (results[3].Text == ">=") {
                        criteria = ParsedComparisonCriteria.BiggerThanOrEquals;
                    } else if (results[3].Text == "<=") {
                        criteria = ParsedComparisonCriteria.LessThanOrEquals;
                    } else if (results[3].Text == ">") {
                        criteria = ParsedComparisonCriteria.BiggerThan;
                    } else if (results[3].Text == "<") {
                        criteria = ParsedComparisonCriteria.LessThan;
                    } else {
                        throw new NotImplementedException ("Comparison '" + results[3].Text + "' is not supported");
                    }

                    ParsedElement left = new ParsedAttributeReferenceExpression(results[0].Text, results[2].Text);
                    ParsedElement right = null;

                    if (results[4].Text == "'") {
                        right = new ParsedStringConstantExpression (results[5].Text);
                    } else if (results.Count > 5 && results[5].Text == ".") {
                        right = new ParsedAttributeReferenceExpression(results[4].Text, results[6].Text);
                    } else if (results.Count == 5) {
                        right = new ParsedNumericConstantExpression(double.Parse (results[4].Text));
                    } else {
                        throw new NotImplementedException (string.Join ("", results.Select(x => x.Text)));
                    }

                    return new ParsedComparisonExpression (criteria, left, right);
                } else {
                    return new ParsedAttributeReferenceExpression(results[0].Text, results[2].Text);
                }
            }

            // '(' S Formula S ')'
            // if (results[0].Text == "(")
            return results[1].Value;
        }

        #endregion
    }
}
