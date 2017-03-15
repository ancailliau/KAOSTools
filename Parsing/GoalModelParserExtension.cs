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
                if (value == null) 
                    throw new NullReferenceException ();

                attrs.Values.Add (value);
            }
            return attrs;
        }

        ParsedElement ModelAttribute (List<Result> results)
        {
            /*
            if (results[0].Text == "@author") {
                return new ParsedModelAttribute { Value = results[2].Text };
            }

            if (results[0].Text == "@title") {
                return new ParsedModelAttribute { Value = results[2].Text };
            }

            if (results[0].Text == "@version") {
                return new ParsedModelAttribute { Value = results[2].Text };
            }*/

//            Console.WriteLine ("<pre>");
//            for (int i = 0; i < results.Count; i++) {
//                Console.WriteLine (i + " : " + results[i].Text);
//            }
//            Console.WriteLine ("</pre>");

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
                t.Attributes.Add (results[i].Value);
            }
            
            return t;
        }

        ParsedElement BuildParsedElementWithAttributesInline<T> (List<Result> results)
            where T: ParsedElementWithAttributes, new()
        {
            var t = new T () { 
                Line = results[0].Line, 
                Col = results[0].Col, 
                Filename = m_file
            };

            if (results[0].Text == "override")
                t.Override = true;

            for (int i = 1; i < results.Count - 1; i++) {
                t.Attributes.Add (results[i].Value);
            }

            return t;
        }

        ParsedElement BuildPredicate (List<Result> results)
        {
            return BuildParsedElementWithAttributes<ParsedPredicate> (results);
        }

        ParsedElement BuildConstraint (List<Result> results)
        {
            return BuildParsedElementWithAttributes<ParsedConstraint> (results);
        }

        ParsedElement BuildSystem (List<Result> results)
        {
            return BuildParsedElementWithAttributes<ParsedSystem> (results);
        }

        ParsedElement BuildGoal (List<Result> results)
        {
            return BuildParsedElementWithAttributes<ParsedGoal> (results);
        }

        ParsedElement BuildSoftGoal (List<Result> results)
        {
            return BuildParsedElementWithAttributes<ParsedSoftGoal> (results);
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

        ParsedElement BuildAttribute (List<Result> results)
        {
            return BuildParsedElementWithAttributes<ParsedAttributeDeclaration> (results);
        }

        ParsedElement BuildGoalRefinement (List<Result> results)
        {
            return BuildParsedElementWithAttributesInline<ParsedGoalRefinement> (results);
        }

        ParsedElement BuildExpert (List<Result> results)
        {
            return BuildParsedElementWithAttributes<ParsedExpert> (results);
        }

        ParsedElement BuildCalibration (List<Result> results)
        {
            return BuildParsedElementWithAttributes<ParsedCalibration> (results);
        }

        ParsedElement BuildCostVariable (List<Result> results)
        {
            return BuildParsedElementWithAttributes<ParsedCostVariable> (results);
        }

        #endregion

        #region Attributes

        T1 BuildParsedAttributeWithValue<T1> (List<Result> results, string value)
            where T1: ParsedAttributeWithValue<string>, new()
        {
            return BuildParsedAttributeWithValue<T1, string> (results, value);
        }

        T1 BuildParsedAttributeWithValue<T1> (List<Result> results, double value)
            where T1: ParsedAttributeWithValue<double>, new()
        {
            return BuildParsedAttributeWithValue<T1, double> (results, value);
        }

        T1 BuildParsedAttributeWithValue<T1,T2> (List<Result> results, T2 value)
            where T1: ParsedAttributeWithValue<T2>, new()
        {
            return new T1 { 
                Value = value,
                Line = results[0].Line, 
                Col = results[0].Col, 
                Filename = m_file
            };
        }

        ParsedElement BuildParsedAttributeWithElements<T> (List<Result> results)
            where T: ParsedAttributeWithElements, new()
        {
            var t = new T () { 
                Line = results[0].Line, 
                Col = results[0].Col, 
                Filename = m_file
            };

            for (int i = 1; i < results.Count; i = i + 2) {
                t.Values.Add (results[i].Value);
            }

            return t;
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


        ParsedElement BuildCostAttribute (List<Result> results)
        {
            /*for (int i = 0; i < results.Count; i++) {
                var r = results [i];
                Console.WriteLine (i + " > " + r.Text);
            }*/

            var parsedElement = new ParsedCostAttribute ();

            parsedElement.Value = (results [4].Value as ParsedFloat);
            parsedElement.CostVariable = results[2].Value;

            return parsedElement;
        }

        ParsedElement BuildCustomAttribute (List<Result> results)
        {
            var parsedElement = BuildParsedAttributeWithValue<ParsedCustomAttribute> (results, results.Count == 4 ? "" : results[3].Text);
            parsedElement.Key = results [1].Text;
            return parsedElement;
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
                (results, results[1].Value);
        }

        ParsedElement BuildDefinitionAttribute (List<Result> results)
        {
            return BuildParsedAttributeWithValue<ParsedDefinitionAttribute, ParsedString> 
                (results, results[1].Value as ParsedString);
        }

        ParsedElement BuildRDS (List<Result> results)
        {
            var r = results[1].Text;
            var parsedNumber = 0d;
            if (r.EndsWith ("%", StringComparison.Ordinal)) {
                parsedNumber = double.Parse (r.Remove (r.Length - 1)) / 100d;
            } else {
                parsedNumber = double.Parse (results [1].Text);
            }

            return BuildParsedAttributeWithValue<ParsedRDSAttribute> 
                (results, parsedNumber);
        }
        
        ParsedElement BuildProbability (List<Result> results)
        {
            var r = results[1].Text;
            var parsedNumber = 0d;
            if (r.EndsWith ("%", StringComparison.Ordinal)) {
                parsedNumber = double.Parse (r.Remove (r.Length - 1)) / 100d;
            } else {
                parsedNumber = double.Parse (results [1].Text);
            }

            return BuildParsedAttributeWithValue<ParsedProbabilityAttribute> 
                (results, parsedNumber);
        }
        
        ParsedElement BuildExpertProbability (List<Result> results)
        {
            return new ParsedExpertProbabilityAttribute () {
                IdOrNAme = results [2].Value,
                Estimate = results [4].Value
            };
        }

        ParsedElement BuildObstructedBy (List<Result> results)
        {
            return BuildParsedAttributeWithValue<ParsedObstructedByAttribute, dynamic> 
                (results, results[1].Value);
        }

        ParsedElement BuildExceptionAttribute (List<Result> results)
        {
            return new ParsedExceptionAttribute { 
                ResolvingGoal = results.Count > 2 ? results[3].Value : null,
                ResolvedObstacle = results[1].Value,
                Line = results[0].Line, 
                Col = results[0].Col, 
                Filename = m_file
            };
        }
        
        ParsedElement BuildAssumptionAttribute (List<Result> results)
        {
            if (results.Count == 2)
                return BuildParsedAttributeWithValue<ParsedAssumptionAttribute, dynamic> 
                    (results, results[1].Value);

            return BuildParsedAttributeWithValue<ParsedNegativeAssumptionAttribute, dynamic> 
                (results, results[2].Value);
        }

        ParsedElement BuildResolvedBy (List<Result> results)
        {
            if (results.Count > 2) {
                var element = BuildParsedAttributeWithValue<ParsedResolvedByAttribute, dynamic> 
                    (results, results[4].Value) as ParsedResolvedByAttribute;
                element.Pattern = results[2].Value as ParsedResolutionPattern;
                return element;
            }
            return BuildParsedAttributeWithValue<ParsedResolvedByAttribute, dynamic> 
                (results, results[1].Value);
        }


        ParsedElement BuildRefinementPattern (List<Result> results)
        {
            ParsedRefinementPattern parsedRefinementPattern = null;

            if (results[0].Text == "milestone") {
                parsedRefinementPattern = new ParsedRefinementPattern { 
                    Name = ParsedRefinementPatternName.Milestone 
                } ;
            }

            else if (results[0].Text == "case") {
                parsedRefinementPattern = new ParsedRefinementPattern { 
                    Name = ParsedRefinementPatternName.Case 
                } ;
                for (int i = 2; i < results.Count - 1; i=i+2) {
                    parsedRefinementPattern.Parameters.Add (results[i].Value);
                }
            }

            else if (results[0].Text == "introduce_guard") {
                parsedRefinementPattern = new ParsedRefinementPattern { 
                    Name = ParsedRefinementPatternName.IntroduceGuard } ;
            }

            else if (results[0].Text == "divide_and_conquer") {
                parsedRefinementPattern = new ParsedRefinementPattern { 
                    Name = ParsedRefinementPatternName.DivideAndConquer } ;
            }

            else if (results[0].Text == "unmonitorability") {
                parsedRefinementPattern = new ParsedRefinementPattern { 
                    Name = ParsedRefinementPatternName.Unmonitorability } ;
            }

            else if (results[0].Text == "uncontrollability") {
                parsedRefinementPattern = new ParsedRefinementPattern { 
                    Name = ParsedRefinementPatternName.Uncontrollability } ;
            }

            else if (results[0].Text == "redundant") {
                parsedRefinementPattern = new ParsedRefinementPattern { 
                    Name = ParsedRefinementPatternName.Redundant } ;
            }

            else {
                throw new NotImplementedException (results[0].Text);
            }
            return parsedRefinementPattern;
        }

        ParsedElement BuildResolutionPattern (List<Result> results)
        {
            ParsedResolutionPattern parsedRefinementPattern = null;

            if (results[0].Text == "substitution")
                parsedRefinementPattern = new ParsedResolutionPattern { Name = "substitution" } ;

            if (results[0].Text == "prevention")
                parsedRefinementPattern = new ParsedResolutionPattern { Name = "prevention" } ;

            if (results[0].Text == "obstacle_reduction")
                parsedRefinementPattern = new ParsedResolutionPattern { Name = "obstacle_reduction" } ;

            if (results[0].Text == "restoration")
                parsedRefinementPattern = new ParsedResolutionPattern { Name = "restoration" } ;

            if (results[0].Text == "weakening")
                parsedRefinementPattern = new ParsedResolutionPattern { Name = "weakening" } ;

            if (results[0].Text == "mitigation")
                parsedRefinementPattern = new ParsedResolutionPattern { Name = "mitigation" } ;

            if (results[0].Text == "weak_mitigation")
                parsedRefinementPattern = new ParsedResolutionPattern { Name = "weak_mitigation" } ;

            if (results[0].Text == "strong_mitigation")
                parsedRefinementPattern = new ParsedResolutionPattern { Name = "strong_mitigation" } ;;
            
            if (results[0].Text == "strong_mitigation")
                parsedRefinementPattern = new ParsedResolutionPattern { Name = "strong_mitigation" } ;

            if (parsedRefinementPattern == null)
                throw new NotImplementedException ();

            if (results.Count > 1) {
                for (int i = 2; i < results.Count - 1; i = i + 2) {
                    parsedRefinementPattern.Parameters.Add (results[i].Value);
                }
            }

            return parsedRefinementPattern;
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
            else if (results[1].Text == "malicious") 
                type = ParsedAgentType.Malicious;
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
            var attribute = new ParsedRefinedByAttribute () { 
                Line = results[0].Line, 
                Col = results[0].Col, 
                Filename = m_file
            };

            int start = 1;
            if (results.Count > start && results[start].Value is ParsedRefinementPattern) {
                attribute.RefinementPattern = results[start].Value as ParsedRefinementPattern;
                start++;
            }

            if (results.Count > start && results[start].Value is ParsedSystemReference) {
                attribute.SystemIdentifier = (results[start].Value as ParsedSystemReference).Name;
                start++;
            }

            for (int i = start; i < results.Count; i = i + 2) {
                attribute.Values.Add (results[i].Value);
            }

            return attribute;
        }

        ParsedElement BuildRefinedByPattern (List<Result> results)
        {
            return results[1].Value;
        }

        ParsedElement BuildRefinedByAlternative (List<Result> results)
        {
            var systemIdentifier = new ParsedSystemReference { 
                Name = results[1].Value
            };
            for (int i = 4; i < results.Count; i = i + 2) {
                systemIdentifier.Values.Add (results[i].Value);
            }

            return systemIdentifier;
        }

        ParsedElement BuildAssignedTo (List<Result> results)
        {
            return BuildParsedAttributeWithElementsAndSystemIdentifier<ParsedAssignedToAttribute> 
                (results);
        }

        ParsedElement BuildAttributeAttribute (List<Result> results)
        {
            if (results[1].Value is ParsedAttributeDeclaration) {
                return results[1].Value;
            }

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
            return new ParsedPredicateArgumentAttribute (name.Value, type) { 
                Line = results[0].Line, 
                Col = results[0].Col, 
                Filename = m_file
            };
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

        ParsedElement BuildAttributeEntityTypeAttribute (List<Result> results)
        {
            return BuildParsedAttributeWithValue<ParsedAttributeEntityTypeAttribute,dynamic> 
                (results, results[1].Value);
        }

        ParsedElement BuildSysRefAttribute (List<Result> results)
        {
            return BuildParsedAttributeWithValue<ParsedSysRefAttribute,dynamic> 
                (results, results[1].Value);
        }

        ParsedElement BuildGoalRefinementChildren (List<Result> results)
        {
            return BuildParsedAttributeWithElements<ParsedGoalRefinementChildrenAttribute> (results);
        }
        
        ParsedElement BuildPatternAttribute (List<Result> results)
        {
            return BuildParsedAttributeWithValue<ParsedPatternAttribute,ParsedRefinementPattern> 
                (results, (ParsedRefinementPattern) results[1].Value);
        }

        

        ParsedElement BuildDerivedAttribute (List<Result> results) 
        {
            return BuildParsedAttributeWithValue<ParsedDerivedAttribute,dynamic> 
                (results, null);
        }

        ParsedElement BuildIsCompleteAttribute (List<Result> results) 
        {
            return BuildParsedAttributeWithValue<ParsedIsComplete,bool> 
                (results, results[1].Text == "yes" | results[1].Text == "true");
        }

        ParsedElement BuildSoftGoalContributionAttribute (List<Result> results) 
        {
            var attribute = new ParsedSoftGoalContributionAttribute () { 
                Line = results[0].Line, 
                Col = results[0].Col, 
                Filename = m_file
            };

            int start = 1;
            for (int i = start; i < results.Count; i = i + 3) {
                attribute.Values.Add (new ParsedSoftGoalContribution { 
                    SoftGoal = results[i+1].Value, 
                    Contribution = results[i].Text == "+" ? ParsedContribution.Positive : ParsedContribution.Negative
                });
            }

            return attribute;
        }


        #endregion

        #region Expressions
        
        ParsedElement BuildMultiplicity (List<Result> results)
        {
            if (results.Count > 3) 
                return new MultiplictyExpression { Value = results[1].Text + ".." + results[3].Text };

            return new MultiplictyExpression { Value = results[1].Text };
        }

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
        
        ParsedElement BuildName (List<Result> results)
        {
            return new NameExpression (results[1].Text) { 
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
                    f.arguments.Add (new KAOSTools.Parsing.ParsedVariableDeclaration(results[i].Text, results[i+2].Value));
                }
                f.Enclosed = results.Last().Value;
                return f;
            }

            // 'exists' S Identifier S ':' S Identifier ( ',' S Identifier S ':' S Identifier )* S '.' S StrongBinary
            if (results[0].Text == "exists") {
                var f = new ParsedExistsExpression ();
                for (int i = 1; i < results.Count - 1; i = i + 4) {
                    f.arguments.Add (new KAOSTools.Parsing.ParsedVariableDeclaration(results[i].Text, results[i+2].Value));
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

        ParsedElement BuildQuantileList (List<Result> results)
        {
            var ql = new ParsedQuantileList ();

            for (int i = 1; i < results.Count - 1; i = i + 2) {
                var r = results [i].Text;
                if (r.EndsWith ("%")) {
                    ql.Quantiles.Add (float.Parse (r.Remove (r.Length - 1)) / 100f);
                } else {
                    ql.Quantiles.Add (float.Parse (r));
                }
            }

            return ql;
        }

        ParsedElement BuildUDistribution (List<Result> results)
        {
            if (results[0].Text == "uniform")
               return new ParsedUniformDistribution { 
                    LowerBound = float.Parse (results[2].Text), 
                    UpperBound = float.Parse (results[4].Text)
                };
            else if (results[0].Text == "triangular")
               return new ParsedTriangularDistribution { 
                    Min = float.Parse (results[2].Text), 
                    Mode = float.Parse (results[4].Text), 
                    Max = float.Parse (results[6].Text)
                };
            else if (results[0].Text == "pert")
               return new ParsedPertDistribution { 
                    Min = float.Parse (results[2].Text), 
                    Mode = float.Parse (results[4].Text), 
                    Max = float.Parse (results[6].Text)
                };
            else if (results[0].Text == "beta")
               return new ParsedBetaDistribution { 
                    Alpha = float.Parse (results[2].Text), 
                    Beta = float.Parse (results[4].Text)
               };
            else
                throw new NotImplementedException ();
        }


        ParsedElement BuildConflict (List<Result> results)
        {
            var attribute = new ParsedConflictAttribute () { 
                Line = results[0].Line, 
                Col = results[0].Col, 
                Filename = m_file
            };

            int start = 1;
            for (int i = start; i < results.Count; i = i + 2) {
                attribute.Values.Add (results[i].Value);
            }

            return attribute;
        }

        ParsedElement BuildOrCst (List<Result> results)
        {
            var attribute = new ParsedOrCstAttribute () { 
                Line = results[0].Line, 
                Col = results[0].Col, 
                Filename = m_file
            };

            int start = 1;
            for (int i = start; i < results.Count; i = i + 2) {
                attribute.Values.Add (results[i].Value);
            }

            return attribute;
        }




        ParsedElement BuildDefaultValueAttribute (List<Result> results)
        {
            var item = new DefaultValueAttribute { 
                Value = results[1].Text.Equals ("true") ? true : false,
                Line = results[0].Line, 
                Col = results[0].Col, 
                Filename = m_file
            };
            return item;
        }


    }
}
