using System;
using System.Linq;
using System.Collections.Generic;
using KAOSTools.Parsing;
using KAOSTools.MetaModel;
using System.IO;

internal sealed partial class GoalModelParser
{   
    private List<string> files_imported = new List<string> ();

    private KAOSTools.Parsing.ParsedElement BuildElements (List<Result> results)
    {
        var attrs = new ParsedElements();
        foreach (var result in results) {
            BuildElement (attrs, result.Value);
        }
        return attrs;
    }

    private KAOSTools.Parsing.ParsedElement BuildElement (ParsedElements attrs, ParsedElement value)
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

    private ParsedElement BuildPredicate (List<Result> results)
    {
        var predicate = new ParsedPredicate () 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };
        
        if (results[0].Text == "override")
            predicate.Override = true;

        for (int i = 2; i < results.Count - 1; i++) {
            predicate.Attributes.Add (results[i].Value as KAOSTools.Parsing.ParsedAttribute);
        }
        
        return predicate;
    }

    private KAOSTools.Parsing.ParsedElement BuildGoal (List<Result> results)
    {
        var goal = new KAOSTools.Parsing.ParsedGoal () 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };

        if (results[0].Text == "override")
            goal.Override = true;
        
        for (int i = 2; i < results.Count - 1; i++) {
            goal.Attributes.Add (results[i].Value as KAOSTools.Parsing.ParsedAttribute);
        }

        return goal;
    }

    private KAOSTools.Parsing.ParsedElement BuildDomainProperty (List<Result> results)
    {
        var domprop = new KAOSTools.Parsing.ParsedDomainProperty () 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };

        if (results[0].Text == "override")
            domprop.Override = true;

        for (int i = 2; i < results.Count - 1; i++) {
            domprop.Attributes.Add (results[i].Value as KAOSTools.Parsing.ParsedAttribute);
        }

        return domprop;
    }
    
    private KAOSTools.Parsing.ParsedElement BuildDomainHypothesis (List<Result> results)
    {
        var domhyp = new KAOSTools.Parsing.ParsedDomainHypothesis () 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };

        if (results[0].Text == "override")
            domhyp.Override = true;

        for (int i = 2; i < results.Count - 1; i++) {
            domhyp.Attributes.Add (results[i].Value as KAOSTools.Parsing.ParsedAttribute);
        }
        
        return domhyp;
    }
    
    private KAOSTools.Parsing.ParsedElement BuildType (List<Result> results)
    {
        var entity = new KAOSTools.Parsing.ParsedGivenType () 
        { Line = results[0].Line, Col = results[0].Col, Filename = m_file };
        
        if (results[0].Text == "override")
            entity.Override = true;        
        
        for (int i = 2; i < results.Count - 1; i++) {
            entity.Attributes.Add (results[i].Value as KAOSTools.Parsing.ParsedAttribute);
        }
        
        return entity;
    }

    private KAOSTools.Parsing.ParsedElement BuildEntity (List<Result> results)
    {
        var entity = new KAOSTools.Parsing.ParsedEntity () 
        { Line = results[0].Line, Col = results[0].Col, Filename = m_file };
        
        if (results[0].Text == "override")
            entity.Override = true;        

        int start = 2;
        if (results[1].Text == "software") {
            entity.EntityType = KAOSTools.Parsing.EntityType.Software;
            start = 3;
        } else if (results[1].Text == "environment") {
            entity.EntityType = KAOSTools.Parsing.EntityType.Environment;
            start = 3;
        } else if (results[1].Text == "shared") {
            entity.EntityType = KAOSTools.Parsing.EntityType.Shared;
            start = 3;
        }

        for (int i = start; i < results.Count - 1; i++) {
            entity.Attributes.Add (results[i].Value as KAOSTools.Parsing.ParsedAttribute);
        }
        
        return entity;
    }

    private KAOSTools.Parsing.ParsedElement BuildAssociation (List<Result> results)
    {
        var association = new KAOSTools.Parsing.ParsedAssociation () 
        { Line = results[0].Line, Col = results[0].Col, Filename = m_file };
        
        if (results[0].Text == "override")
            association.Override = true;        
        
        for (int i = 2; i < results.Count - 1; i++) {
            association.Attributes.Add (results[i].Value as KAOSTools.Parsing.ParsedAttribute);
        }
        
        return association;
    }

    private KAOSTools.Parsing.ParsedElement BuildAttribute (List<Result> results)
    {
        var name = results [1].Value as KAOSTools.Parsing.ParsedAttribute;

        KAOSTools.Parsing.IdentifierOrNameExpression type = null;
        if (results.Count == 4) {
            type = results [3].Value as KAOSTools.Parsing.IdentifierOrNameExpression;
        }
        return new ParsedAttributeAttribute (name as NameExpression, type);
    }

    private KAOSTools.Parsing.ParsedElement BuildArgument (List<Result> results)
    {
        var name = results [1].Value as KAOSTools.Parsing.ParsedAttribute;
        
        KAOSTools.Parsing.IdentifierOrNameExpression type = null;
        if (results.Count == 4) {
            type = results [3].Value as KAOSTools.Parsing.IdentifierOrNameExpression;
        }
        return new ParsedPredicateArgumentAttribute (name as NameExpression, type);
    }

    private KAOSTools.Parsing.ParsedElement BuildLink (List<Result> results)
    {
        var link = new KAOSTools.Parsing.ParsedLinkAttribute () { 
            Line = results[0].Line, 
            Col = results[0].Col, 
            Filename = m_file
        };

        if (results.Count == 2) {
            link.Target = results[1].Value as IdentifierOrNameExpression;
        } else if (results.Count == 3) {
            link.Multiplicity = (results[1].Value as MultiplictyExpression).Value;
            link.Target = results[2].Value as IdentifierOrNameExpression;
        }

        return link;
    }
    
    private KAOSTools.Parsing.ParsedElement BuildIsA (List<Result> results)
    {
        var link = new KAOSTools.Parsing.ParsedIsAAttribute () { 
            Line = results[0].Line, 
            Col = results[0].Col, 
            Filename = m_file
        };
        
        link.Target = results[1].Value as IdentifierOrNameExpression;

        return link;
    }

    private KAOSTools.Parsing.ParsedElement BuildMultiplicity (List<Result> results)
    {
        if (results.Count > 3) 
            return new MultiplictyExpression { Value = results[1].Text + ".." + results[3].Text };
        else
            return new MultiplictyExpression { Value = results[1].Text };
    }

    private KAOSTools.Parsing.ParsedElement BuildSystem (List<Result> results)
    {
        var system = new KAOSTools.Parsing.ParsedSystem () 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };

        if (results[0].Text == "override")
            system.Override = true;

        for (int i = 2; i < results.Count - 1; i++) {
            system.Attributes.Add (results[i].Value as KAOSTools.Parsing.ParsedAttribute);
        }
        
        return system;
    }

    private KAOSTools.Parsing.ParsedElement BuildObstacle (List<Result> results)
    {
        var obstacle = new KAOSTools.Parsing.ParsedObstacle () 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };

        if (results[0].Text == "override")
            obstacle.Override = true;

        for (int i = 2; i < results.Count - 1; i++) {
            obstacle.Attributes.Add (results[i].Value as KAOSTools.Parsing.ParsedAttribute);
        }

        return obstacle;
    }
    
    private KAOSTools.Parsing.ParsedElement BuildAgent (List<Result> results)
    {
        var agent = new KAOSTools.Parsing.ParsedAgent () 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };

        if (results[0].Text == "override")
            agent.Override = true;

        for (int i = 2; i < results.Count - 1; i++) {
            agent.Attributes.Add (results[i].Value as ParsedAttribute);
        }

        return agent;
    }


    private KAOSTools.Parsing.ParsedElement BuildRefinedBy (List<Result> results)
    {
        var list = new ParsedRefinedByAttribute () 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };

        if (results[1].Text == "[") {
            list.SystemIdentifier = results[2].Value;
            for (int i = 4; i < results.Count; i = i + 2) {
                list.Values.Add (results[i].Value);
            }

        } else {
            for (int i = 1; i < results.Count; i = i + 2) {
                list.Values.Add (results[i].Value);
            }
        }

        return list;
    }

    private KAOSTools.Parsing.ParsedElement BuildAlternative (List<Result> results)
    {
        return new ParsedAlternativeAttribute () 
            { Value = results[1].Value, Line = results[0].Line, Col = results[0].Col, Filename = m_file };
    }
    
    private KAOSTools.Parsing.ParsedElement BuildResolvedBy (List<Result> results)
    {
        return new ParsedResolvedByAttribute () 
        { Value = results[1].Value, Line = results[0].Line, Col = results[0].Col, Filename = m_file };
    }

    private KAOSTools.Parsing.ParsedElement BuildObstructedBy (List<Result> results)
    {
        return new ParsedObstructedByAttribute { Value = results[1].Value, Line = results[0].Line, Col = results[0].Col, Filename = m_file };
    }

    private KAOSTools.Parsing.ParsedElement BuildAssignedTo (List<Result> results)
    {
        var list = new ParsedAssignedToAttribute () 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };

        if (results[1].Text == "[") {
            list.SystemIdentifier = (results[2].Value as IdentifierOrNameExpression);
            for (int i = 4; i < results.Count; i = i + 2) {
                list.Values.Add (results[i].Value);
            }
            
        } else {
            for (int i = 1; i < results.Count; i = i + 2) {
                list.Values.Add (results[i].Value);
            }
        }

        return list;
    }


    private KAOSTools.Parsing.ParsedElement BuildAgentTypeAttribute (List<Result> results)
    {
        return new KAOSTools.Parsing.ParsedAgentTypeAttribute { 
            Value = results[1].Text == "software" ? ParsedAgentType.Software : ParsedAgentType.Environment, 
            Line = results[0].Line, Col = results[0].Col, Filename = m_file };
    }

    private KAOSTools.Parsing.ParsedElement BuildIdentifierAttribute (List<Result> results)
    {
        return new KAOSTools.Parsing.ParsedIdentifierAttribute { 
            Value = results[1].Text, 
            Line = results[0].Line, Col = results[0].Col, Filename = m_file };
    }

    private KAOSTools.Parsing.ParsedElement BuildNameAttribute (List<Result> results)
    {
        return new KAOSTools.Parsing.ParsedNameAttribute { Value = results[2].Text,
            Line = results[0].Line, Col = results[0].Col, Filename = m_file };
    }

    private KAOSTools.Parsing.ParsedElement BuildSignature (List<Result> results)
    {
        return new KAOSTools.Parsing.ParsedSignatureAttribute { Value = results[2].Text,
            Line = results[0].Line, Col = results[0].Col, Filename = m_file };
    }

    private KAOSTools.Parsing.ParsedElement BuildFormalSpec (List<Result> results)
    {
        return new ParsedFormalSpecAttribute () { Value = results[2].Value,
            Line = results[0].Line, Col = results[0].Col, Filename = m_file };
    }

    // TODO deprecate this method
    private KAOSTools.Parsing.ParsedElement BuildStringFormalSpec (List<Result> results)
    {
        return new ParsedFormalSpecAttribute () { Value = results[2].Value,
            Line = results[0].Line, Col = results[0].Col, Filename = m_file };
    }
    
    private KAOSTools.Parsing.ParsedElement BuildDefinition (List<Result> results)
    {
        return new KAOSTools.Parsing.ParsedDefinitionAttribute { Value = results [2].Text, 
             Line = results[0].Line, Col = results[0].Col, Filename = m_file };
    }
        
    private KAOSTools.Parsing.ParsedElement BuildDescription (List<Result> results)
    {
        if (results.Count == 4)
            return new KAOSTools.Parsing.ParsedDescriptionAttribute { Value = results [2].Text, 
                 Line = results[0].Line, Col = results[0].Col, Filename = m_file };
        else 
            return new KAOSTools.Parsing.ParsedDescriptionAttribute { Value = "",
                 Line = results[0].Line, Col = results[0].Col, Filename = m_file };
    }
    
    private KAOSTools.Parsing.ParsedElement BuildRDS (List<Result> results)
    {
        return new KAOSTools.Parsing.ParsedRDSAttribute { Value = double.Parse (results [1].Text),
             Line = results[0].Line, Col = results[0].Col, Filename = m_file };
    }
    
    private KAOSTools.Parsing.ParsedElement BuildProbability (List<Result> results)
    {
        return new KAOSTools.Parsing.ParsedProbabilityAttribute { Value = double.Parse (results [1].Text),
             Line = results[0].Line, Col = results[0].Col, Filename = m_file };
    }

    private KAOSTools.Parsing.ParsedElement BuildIdOrName (List<Result> results)
    {
        if (results.Count == 3) {
            return new NameExpression (results[1].Text) 
                { Line = results[0].Line, Col = results[0].Col, Filename = m_file };

        } else if (results.Count == 1) {
            return new IdentifierExpression (results[0].Text) 
                { Line = results[0].Line, Col = results[0].Col, Filename = m_file };
        }
        return null;
    }

    private KAOSTools.Parsing.ParsedElement BuildIdentifier (List<Result> results)
    {
        return new IdentifierExpression (string.Join ("", results.Select (x => x.Text))) 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };
    }

    private KAOSTools.Parsing.ParsedElement BuildName (List<Result> results)
    {
        if (results.Count == 3) {
            return new NameExpression (results[1].Text) 
                { Line = results[0].Line, Col = results[0].Col, Filename = m_file };
        }
        return null;
    }
    
    private KAOSTools.Parsing.ParsedElement Import (string file)
    {
        var filename = Path.Combine (Path.GetDirectoryName (m_file), file);
        if (files_imported.Contains (Path.GetFullPath (filename))) {
            return new ParsedElements ();
        }

        if (File.Exists (filename)) {
            files_imported.Add (Path.GetFullPath (filename));

            string input = File.ReadAllText (filename);
            var parser = new GoalModelParser();
            parser.files_imported = this.files_imported;
            var m2 = parser.Parse (input, filename);
            return m2;

        } else {
            throw new FileNotFoundException ("Included file `" + filename + "` not found", filename);
        }
    }

    #region Formal spec

    private KAOSTools.Parsing.ParsedElement BuildFormula (List<Result> results)
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

    private KAOSTools.Parsing.ParsedElement BuildStrongBinary (List<Result> results)
    {
        // Binary
        if (results.Count == 1) 
            return results[0].Value;

        // 'when' S Binary S 'then' S Formula
        else {
            return new ParsedStrongImplyExpression () { Left = results[1].Value, Right = results[3].Value };
        }
    }

    private KAOSTools.Parsing.ParsedElement BuildBinary (List<Result> results)
    {
        // TemporalBinary
        if (results.Count == 1) 
            return results[0].Value;

        // 'if' S TemporalBinary S 'then' S Binary
        else if (results.Count == 4)
            return new ParsedImplyExpression () { Left = results[1].Value, Right = results[3].Value };

        // TemporalBinary S 'iff' S Binary
        else
            return new ParsedEquivalenceExpression () { Left = results[0].Value, Right = results[2].Value };
    }
    
    private KAOSTools.Parsing.ParsedElement BuildTemporalBinary (List<Result> results)
    {
        // And
        if (results.Count == 1) 
            return results[0].Value;

        // And S 'until' S TemporalBinary
        else if (results[2].Text == "until")
            return new ParsedUntilExpression () { Left = results[0].Value, Right = results[2].Value };

        // And S 'release' S TemporalBinary
        else if (results[2].Text == "release")
            return new ParsedReleaseExpression () { Left = results[0].Value, Right = results[2].Value };

        // And S 'unless' S TemporalBinary
        else if (results[2].Text == "unless")
            return new ParsedUnlessExpression () { Left = results[0].Value, Right = results[2].Value };

        return null;
    }
    
    private KAOSTools.Parsing.ParsedElement BuildAnd (List<Result> results)
    {
        // Or
        if (results.Count == 1) 
            return results[0].Value;
        
        // Or S 'and' S And
        else
            return new ParsedAndExpression () { Left = results[0].Value, Right = results[2].Value };
    }
    
    private KAOSTools.Parsing.ParsedElement BuildOr (List<Result> results)
    {
        // Unary
        if (results.Count == 1) 
            return results[0].Value;
        
        // Unary S 'or' S Or
        else
            return new ParsedOrExpression () { Left = results[0].Value, Right = results[2].Value };

    }
    
    private KAOSTools.Parsing.ParsedElement BuildUnary (List<Result> results)
    {
        // Atom
        if (results.Count == 1) 
            return results[0].Value;

        // 'not' S Unary
        else if (results[0].Text == "not")
            return new ParsedNotExpression () { Enclosed = results[1].Value };

        // 'next' S Unary
        else if (results[0].Text == "next")
            return new ParsedNextExpression () { Enclosed = results[1].Value };
        
        // ('sooner-or-later' / 'eventually') S Unary
        else if (results[0].Text == "sooner-or-later" | results[0].Text == "eventually") {
            return new ParsedEventuallyExpression () { Enclosed = results[1].Value };
        }

        // ('always' / 'globally') S Unary
        else if (results[0].Text == "always" | results[0].Text == "globally")
            return new ParsedGloballyExpression () { Enclosed = results[1].Value };

        return null;
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
        if (results[0].Text == "(") {
            return results[1].Value;
        }

        throw new NotImplementedException ();
    }

    #endregion


}
