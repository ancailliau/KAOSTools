using System;
using System.Linq;
using System.Collections.Generic;
using KAOSTools.Parsing;
using System.IO;
using KAOSTools.Parsing.Plugins;
using System.Text.RegularExpressions;
using KAOSTools.Parsing.Parsers;

namespace KAOSTools.Parsing.Parsers {
    sealed partial class GoalModelParser
    {   
        List<string> files_imported = new List<string> ();

        partial void OnCtorEpilog()
        {
			Add(new AgentDeclareParser());
			Add(new AssociationDeclareParser());
            Add(new CalibrationDeclareParser());
			Add(new DomainPropertyDeclareParser());
			Add(new DomainHypothesisDeclareParser());
            Add(new EntityDeclareParser());
            Add(new ExpertDeclareParser());
			Add(new GoalDeclareParser());
			Add(new ObstacleDeclareParser());
			Add(new PredicateDeclareParser());
			Add(new SoftGoalDeclareParser());
            Add(new TypeDeclareParser());
        }

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
                if (value == null) 
                    throw new NullReferenceException ();

                attrs.Values.Add (value);
            }
            return attrs;
        }

        ParsedElement ModelAttribute (List<Result> results)
        {
            return new ParsedModelAttribute { Value = results[3].Text, Name = results[1].Text };
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

        public List<DeclareParser> level0Parser = new List<DeclareParser>();

		public void Add(DeclareParser parser)
        {
            level0Parser.Add(parser);
        }

        ParsedElement BuildDeclareItem (List<Result> results)
		{
            var declaredItem = results[1].Text;
            Console.WriteLine("BuildDeclareItem [ "  + declaredItem + " ]");

            foreach (var parser in level0Parser)
            {
                var regex = new Regex(@"^"+parser.GetIdentifier()+"$");
                var match = regex.Match(declaredItem);
                if (match.Success)
				{
					var identifier = results[3].Text;

					var attributes = new List<dynamic>();
					for (int i = 5; i < results.Count - 1; i++)
					{
						var attributeValue = (NParsedAttribute)results[i].Value;
						attributes.Add(parser.ParsedAttribute(attributeValue.AttributeName, attributeValue.Parameters, attributeValue.AttributeValue));
					}

					return parser.ParsedDeclare(identifier, attributes);
                }
            }

            throw new NotImplementedException(string.Format ("declare '{0}' is not supported.", declaredItem));
		}

		ParsedElement BuildAttribute (List<Result> results)
		{
            if (results.Count == 2)
            {
                return new NParsedAttribute()
                {
                    AttributeName = results[0].Text,
                    AttributeValue = (NParsedAttributeValue)results[1].Value
                };
            }

			if (results.Count == 3)
			{
                return new NParsedAttribute()
                {
                    AttributeName = results[0].Text,
                    Parameters = (NParsedAttributeValue)results[1].Value,
                    AttributeValue = (NParsedAttributeValue)results[2].Value
                };
			}

			throw new NotImplementedException("BuildAttribute");
		}
		
        ParsedElement BuildAttributeParameters(List<Result> results)
		{
            return results[1].Value;
		}

		ParsedElement BuildAttributeValue(List<Result> results)
		{
            Console.WriteLine(results.Count);
            Console.WriteLine("--");
            Console.WriteLine(string.Join("\n", results.Select(x => x.Text.ToString() + "("+x.Value?.GetType()+")")));
            Console.WriteLine("--");

            if (results.Count == 1)
            {
				return results[0].Value;
            }

		    var elements = new List<ParsedElement>();
			for (int i = 0; i < results.Count; i = i + 2)
			{
                var attributeValue = (ParsedElement) results[i].Value;
				elements.Add(attributeValue);
			}

            return new NParsedAttributeList(elements);
		}

		ParsedElement BuildAttributeDecoratedValue(List<Result> results)
		{
			if (results.Count == 1)
			{
				return new NParsedAttributeAtomic(results[0].Value);
			}

			if (results[1].Text == ":")
			{
				return new NParsedAttributeColon()
				{
					Left = results[0].Value,
					Right = results[2].Value
				};
			}

			if (results[1].Text == "[")
			{
				return new NParsedAttributeBracket();
			}

			throw new NotImplementedException("BuildAttributeDecoratedValue");
		}
		
        #endregion

        #region Expressions

        ParsedElement BuildVariable (List<Result> results)
        {
            return new ParsedVariableReference (results[0].Text) { 
                Line = results[0].Line, 
                Col = results[0].Col, 
                Filename = m_file
            };
        }

        ParsedElement BuildIdentifier (List<Result> results)
        {
            return new IdentifierExpression (string.Join ("", results.Select (x => x.Text))) { 
                Line = results[0].Line, 
                Col = results[0].Col, 
                Filename = m_file
            };
        }

        ParsedElement BuildString (List<Result> results)
        {
            return new ParsedString { 
                Value = string.Join("", results.Select(x => x.Text)),
                Line = results[0].Line, 
                Col = results[0].Col, 
                Filename = m_file
            };
        }

        ParsedElement BuildQuotedString (List<Result> results)
        {
            if (results.Count == 2 | (results.Count == 3 && results[0].Text == "@")) { 
                return new ParsedString () {
                    Value = "",
                    Line = results[0].Line,
                    Col = results[0].Col,
                    Filename = m_file
                };
            }

            if (results.Count == 4) {
                var val = results[2].Value as ParsedString;
                val.Verbatim = true;
                return val;
            }

            return results[1].Value;
        }

        ParsedElement BuildFloat (List<Result> results)
        {
            return new ParsedFloat {
                Value = double.Parse (string.Join("", results.Select(x => x.Text))),
                Line = results[0].Line, 
                Col = results[0].Col, 
                Filename = m_file
            };
		}

		ParsedElement BuildInteger(List<Result> results)
		{
            return new ParsedInteger
			{
				Value = int.Parse(string.Join("", results.Select(x => x.Text))),
				Line = results[0].Line,
				Col = results[0].Col,
				Filename = m_file
			};
		}

		ParsedElement BuildPercentage(List<Result> results)
		{
            return new ParsedPercentage
			{
                Value = double.Parse(string.Join("", results[0].Text)),
				Line = results[0].Line,
				Col = results[0].Col,
				Filename = m_file
			};
		}

		ParsedElement BuildBool(List<Result> results)
		{
            return new ParsedBool
			{
                Value = bool.Parse(results[0].Text),
				Line = results[0].Line,
				Col = results[0].Col,
				Filename = m_file
			};
		}

        #endregion


        #region Logic Formula

        ParsedElement BuildLogicFormula (List<Result> results)
        {
            // LogicFormulaBinary
            if (results.Count == 1) 
                return results[0].Value;

            return null;
        }

        ParsedElement BuildLogicFormulaBinary (List<Result> results)
        {
            // 'if' S LogicFormulaAnd S 'then' S LogicFormula
            if (results.Count == 4)
                return new ParsedImplyExpression () { Left = results[1].Value, Right = results[3].Value };

            // LogicFormulaAnd
            if (results.Count == 1) 
                return results[0].Value;

            // LogicFormulaAnd S 'iff' S LogicFormula
            return new ParsedEquivalenceExpression () { Left = results[0].Value, Right = results[2].Value };
        }

        ParsedElement BuildLogicFormulaAnd (List<Result> results)
        {
            // LogicFormulaOr
            if (results.Count == 1) 
                return results[0].Value;

            // LogicFormulaOr S 'and' S LogicFormula
            return new ParsedAndExpression () { Left = results[0].Value, Right = results[2].Value };
        }

        ParsedElement BuildLogicFormulaOr (List<Result> results)
        {
            // LogicFormulaUnary
            if (results.Count == 1) 
                return results[0].Value;

            // LogicFormulaUnary S 'or' S LogicFormula
            return new ParsedOrExpression () { Left = results[0].Value, Right = results[2].Value };

        }

        ParsedElement BuildLogicFormulaUnary (List<Result> results)
        {
            // LogicFormulaAtom
            if (results.Count == 1) 
                return results[0].Value;

            // 'not' S LogicFormula
            if (results[0].Text == "not")
                return new ParsedNotExpression () { Enclosed = results[1].Value };
            
            throw new NotImplementedException ();
        }

        ParsedElement BuildLogicFormulaAtom (List<Result> results)
        {
            if (results [0].Text == "(")
                return results [1].Value;
            else if (results [0].Text == "true")
                return new ParsedBoolConstantExpression (bool.Parse (results [0].Text));
            else if (results [0].Text == "false")
                return new ParsedBoolConstantExpression (bool.Parse (results [0].Text));
            else
                return results[0].Value;
        }

        #endregion

        #region Formal spec

        ParsedElement BuildFormula (List<Result> results)
        {
            // 'forall' S Identifier S ':' S Identifier ( ',' S Identifier S ':' S Identifier )* S '.' S StrongBinary
            if (results[0].Text == "forall") {
                var f = new ParsedForallExpression ();
                for (int i = 1; i < results.Count - 1; i = i + 4) {
                    f.arguments.Add (new ParsedVariableDeclaration(results[i].Text, results[i+2].Value));
                }
                f.Enclosed = results.Last().Value;
                return f;
            }

            // 'exists' S Identifier S ':' S Identifier ( ',' S Identifier S ':' S Identifier )* S '.' S StrongBinary
            if (results[0].Text == "exists") {
                var f = new ParsedExistsExpression ();
                for (int i = 1; i < results.Count - 1; i = i + 4) {
                    f.arguments.Add (new ParsedVariableDeclaration(results[i].Text, results[i+2].Value));
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
            if (results.Count == 1) 
                return results[0].Value;

			if (results[1].Text == "until") {
				if (results.Count == 3) {
					return new ParsedUntilExpression {
						Left = results[0].Value,
						Right = results[2].Value
					};
				} 

				if (results.Count == 4) {
					return new ParsedUntilExpression {
						Left = results[0].Value,
						Right = results[3].Value,
						TimeBound = results[2].Value as ParsedTimeBound
					};
				}
			}

            if (results[1].Text == "release")
                return new ParsedReleaseExpression { 
                    Left = results[0].Value, 
                    Right = results[2].Value
                };

			if (results[1].Text == "unless") {
				if (results.Count == 3) {
					return new ParsedUnlessExpression {
						Left = results[0].Value,
						Right = results[2].Value
					};
				}

				if (results.Count == 4) {
					return new ParsedUnlessExpression {
						Left = results[0].Value,
						Right = results[3].Value,
						TimeBound = results[1].Value as ParsedTimeBound
					};
				}
			}
            throw new NotImplementedException ();
        }
        
        ParsedElement BuildAnd (List<Result> results)
        {
            // Or
            if (results.Count == 1) 
                return results[0].Value;
            
            // Or S 'and' S And
            return new ParsedAndExpression () { Left = results[0].Value, Right = results[2].Value };
        }
        
        ParsedElement BuildOr (List<Result> results)
        {
            // Unary
            if (results.Count == 1) 
                return results[0].Value;
            
            // Unary S 'or' S Or
            return new ParsedOrExpression () { Left = results[0].Value, Right = results[2].Value };

        }

		ParsedElement BuildUnary(List<Result> results)
		{
			// Atom
			if (results.Count == 1)
				return results[0].Value;

			// 'not' S Unary
			if (results[0].Text == "not")
				return new ParsedNotExpression() { Enclosed = results[1].Value };

			// 'next' S Unary
			if (results[0].Text == "next")
				return new ParsedNextExpression() { Enclosed = results[1].Value };

			// ('sooner-or-later' / 'eventually') (S EventuallyTimeBoundEmphasis)? S Unary
			if (results[0].Text == "sooner-or-later" | results[0].Text == "eventually") {
				if (results.Count < 4) {
					return new ParsedEventuallyExpression() {
						Enclosed = results.Count == 2 ? results[1].Value : results[2].Value,
						TimeBound = results.Count == 2 ? null : (results[1].Value as ParsedTimeBound)
					};
				}
			}

			// ('always' / 'globally') (S GloballyTimeBoundEmphasis)? S Unary
			if (results[0].Text == "always" | results[0].Text == "globally") {
				return new ParsedGloballyExpression() {
					Enclosed = results.Count == 2 ? results[1].Value : results[2].Value,
					TimeBound = results.Count == 2 ? null : (results[1].Value as ParsedTimeBound)
				};
			}

			throw new NotImplementedException ();
        }
        
        ParsedElement BuildPredicateReference (List<Result> results)
        {
            var p = new ParsedPredicateReferenceExpression (results[0].Value) { 
                Line = results[0].Line, 
                Col = results[0].Col, 
                Filename = m_file
            };

            for (int i = 2; i < results.Count - 1; i = i + 2) {
                p.ActualArguments.Add (results[i].Text);
            }

            return p;
        }

        ParsedElement BuildRelationReference (List<Result> results)
        {
            var p = new ParsedInRelationExpression ();
            foreach (var r in results) {
                if (r.Text == "(" | r.Text == ")" | r.Text == ",")
                    continue;

                if (r.Text == "in")
                    break;

                p.Variables.Add (r.Text);
            }
            p.Relation = results.Last().Value;
            return p;
        }

        ParsedElement BuildAttributeReference (List<Result> results)
        {
            return new ParsedAttributeReferenceExpression(results[0].Text, 
                                                          results[2].Value){ 
                Line = results[0].Line, 
                Col = results[0].Col, 
                Filename = m_file
            };
        }

        ParsedElement BuildComparison (List<Result> results)
        {
            ParsedComparisonCriteria criteria;
            if (results[1].Text == "==") {
                criteria = ParsedComparisonCriteria.Equals;
            } else if (results[1].Text == "!=") {
                criteria = ParsedComparisonCriteria.NotEquals;
            } else if (results[1].Text == ">=") {
                criteria = ParsedComparisonCriteria.BiggerThanOrEquals;
            } else if (results[1].Text == "<=") {
                criteria = ParsedComparisonCriteria.LessThanOrEquals;
            } else if (results[1].Text == ">") {
                criteria = ParsedComparisonCriteria.BiggerThan;
            } else if (results[1].Text == "<") {
                criteria = ParsedComparisonCriteria.LessThan;
            } else {
                throw new NotImplementedException ("Comparison '" + results[1].Text + "' is not supported");
            }

            return new ParsedComparisonExpression (criteria, 
                                                   results[0].Value, 
                                                   results[2].Value);
        }

        ParsedElement BuildComparisonMember (List<Result> results)
        {
            if (results[0].Text == "\"") {
                return new ParsedStringConstantExpression (results.Count == 2 ? "" : results[1].Text);
            }

            if (results[0].Text == "true" | results[0].Text == "false")
                return new ParsedBoolConstantExpression (bool.Parse (results[0].Text));

            double val;
            if (double.TryParse(results[0].Text, out val))
                return new ParsedNumericConstantExpression (val);

            return results[0].Value;
        }

        ParsedElement BuildAtom (List<Result> results)
        {
            if (results[0].Text == "(")
               return results[1].Value;
            else
                return results[0].Value;
        }

        #endregion

        #region Time bounds

        ParsedElement BuildEventuallyTimeBound (List<Result> results) {
            if (results[0].Text == "strictly") {
                if (results[1].Text == "after")
                    return new ParsedTimeBound {
                    Comparator = ParsedTimeComparator.strictly_greater,
                    Bound = results[2].Value as ParsedTime
                };

                if (results[1].Text == "before")
                    return new ParsedTimeBound {
                    Comparator = ParsedTimeComparator.strictly_less,
                    Bound = results[2].Value as ParsedTime
                };

                throw new NotImplementedException ();
            }

            if (results[0].Text == "after")
                return new ParsedTimeBound {
                Comparator = ParsedTimeComparator.greater,
                Bound = results[1].Value as ParsedTime
            };

            if (results[0].Text == "before")
                return new ParsedTimeBound {
                Comparator = ParsedTimeComparator.less,
                Bound = results[1].Value as ParsedTime
            };

            return new ParsedTimeBound {
                Comparator = ParsedTimeComparator.equal,
                Bound = results[1].Value as ParsedTime
            };
        }

        ParsedElement BuildGloballyTimeBound (List<Result> results) {
            if (results[1].Text == "strictly") {
                if (results[2].Text == "more")
                    return new ParsedTimeBound {
                        Comparator = ParsedTimeComparator.strictly_greater,
                        Bound = results[4].Value as ParsedTime
                    };

                if (results[2].Text == "less")
                    return new ParsedTimeBound {
                        Comparator = ParsedTimeComparator.strictly_less,
                        Bound = results[4].Value as ParsedTime
                    };

                throw new NotImplementedException ();
            }

            if (results[1].Text == "more")
                return new ParsedTimeBound {
                    Comparator = ParsedTimeComparator.greater,
                    Bound = results[3].Value as ParsedTime
                };

            if (results[1].Text == "less")
                return new ParsedTimeBound {
                    Comparator = ParsedTimeComparator.less,
                    Bound = results[3].Value as ParsedTime
                };

            return new ParsedTimeBound {
                Comparator = ParsedTimeComparator.equal,
                Bound = results[1].Value as ParsedTime
            };
        }

        ParsedElement BuildTimeConstraint (List<Result> results) {
            var constraints = new ParsedTime ();

            for (int i = 0; i < results.Count; i = i + 2) {
                var atomicConstraints = new ParsedAtomicTime ();
                atomicConstraints.Duration = int.Parse (results[i].Text);

                var timeunit = results[i+1].Text;
                if (timeunit.StartsWith ("day") | timeunit.Equals("d")) {
                    atomicConstraints.Unit = ParsedTimeUnit.day;

                } else if (timeunit.StartsWith ("hour") | timeunit.Equals("h")) {
                    atomicConstraints.Unit = ParsedTimeUnit.hour;

                } else if (timeunit.StartsWith ("minute") | timeunit.Equals("m")) {
                    atomicConstraints.Unit = ParsedTimeUnit.minute;

                } else if (timeunit.StartsWith ("second") | timeunit.Equals("s")) {
                    atomicConstraints.Unit = ParsedTimeUnit.second;

                } else if (timeunit.StartsWith ("milisecond") | timeunit.Equals("ms")) {
                    atomicConstraints.Unit = ParsedTimeUnit.milisecond;

                } else {
                    throw new NotImplementedException ();
                }

                constraints.Constraints.Add (atomicConstraints);
            }

            return constraints;
        }

        #endregion
    }
}
