using System;
using System.Linq;
using System.Collections.Generic;
using KAOSTools.Parsing;
using KAOSTools.MetaModel;
using System.IO;

internal sealed partial class GoalModelParser
{   
    private List<string> files_imported = new List<string> ();

    private KAOSTools.Parsing.Element BuildElements (List<Result> results)
    {
        var attrs = new Elements();
        foreach (var result in results) {
            BuildElement (attrs, result.Value);
        }
        return attrs;
    }

    private KAOSTools.Parsing.Element BuildElement (Elements attrs, Element value)
    {
        if (value is Elements) {
            foreach (var result2 in ((Elements) value).Values) {
                BuildElement (attrs, result2);
            }
        } else {
            attrs.Values.Add (value);
        }
        return attrs;
    }

    private KAOSTools.Parsing.Predicate BuildPredicate (List<Result> results)
    {
        var predicate = new KAOSTools.Parsing.Predicate () 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };

        for (int i = 1; i < results.Count; i++) {
            predicate.Attributes.Add (results[i].Value as KAOSTools.Parsing.Attribute);
        }
        
        return predicate;
    }

    private KAOSTools.Parsing.Element BuildGoal (List<Result> results)
    {
        var goal = new KAOSTools.Parsing.Goal () 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };

        if (results[0].Text == "override")
            goal.Override = true;

        for (int i = 1; i < results.Count; i++) {
            goal.Attributes.Add (results[i].Value as KAOSTools.Parsing.Attribute);
        }

        return goal;
    }

    private KAOSTools.Parsing.Element BuildDomainProperty (List<Result> results)
    {
        var domprop = new KAOSTools.Parsing.DomainProperty () 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };

        if (results[0].Text == "override")
            domprop.Override = true;


        for (int i = 1; i < results.Count; i++) {
            domprop.Attributes.Add (results[i].Value as KAOSTools.Parsing.Attribute);
        }

        return domprop;
    }
    
    private KAOSTools.Parsing.Element BuildDomainHypothesis (List<Result> results)
    {
        var domhyp = new KAOSTools.Parsing.DomainHypothesis () 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };

        if (results[0].Text == "override")
            domhyp.Override = true;

        
        for (int i = 1; i < results.Count; i++) {
            domhyp.Attributes.Add (results[i].Value as KAOSTools.Parsing.Attribute);
        }
        
        return domhyp;
    }
    
    private KAOSTools.Parsing.Element BuildSystem (List<Result> results)
    {
        var system = new KAOSTools.Parsing.System () 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };

        if (results[0].Text == "override")
            system.Override = true;

        for (int i = 1; i < results.Count; i++) {
            system.Attributes.Add (results[i].Value as KAOSTools.Parsing.Attribute);
        }
        
        return system;
    }

    private KAOSTools.Parsing.Element BuildObstacle (List<Result> results)
    {
        var obstacle = new KAOSTools.Parsing.Obstacle () 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };

        if (results[0].Text == "override")
            obstacle.Override = true;


        for (int i = 1; i < results.Count; i++) {
            obstacle.Attributes.Add (results[i].Value as KAOSTools.Parsing.Attribute);
        }

        return obstacle;
    }
    
    private KAOSTools.Parsing.Element BuildAgent (List<Result> results)
    {
        var agent = new KAOSTools.Parsing.Agent () 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };

        if (results[0].Text == "override")
            agent.Override = true;

        int start = 1;
        if (results[1].Text != "agent") {
            start = 2;
            if (results[1].Text == "software")
                agent.Type = KAOSTools.Parsing.AgentType.Software;
            else if (results[1].Text == "environment") 
                agent.Type = KAOSTools.Parsing.AgentType.Environment;
        }

        for (int i = start; i < results.Count; i++) {
            agent.Attributes.Add (results[i].Value as KAOSTools.Parsing.Attribute);
        }

        return agent;
    }


    private KAOSTools.Parsing.Element BuildRefinedBy (List<Result> results)
    {
        var list = new RefinedByList () 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };

        if (results[1].Text == "[") {
            list.SystemIdentifier = results[2].Value as IdentifierOrName;
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

    private KAOSTools.Parsing.Element BuildAlternative (List<Result> results)
    {
        var list = new AlternativeList () 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };

        for (int i = 1; i < results.Count; i = i + 2) {
            list.Values.Add (results[i].Value);
        }
        
        return list;
    }
    
    private KAOSTools.Parsing.Element BuildResolvedBy (List<Result> results)
    {
        var list = new ResolvedByList () 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };

        for (int i = 1; i < results.Count; i = i + 2) {
            list.Values.Add (results[i].Value);
        }
        
        return list;
    }

    private KAOSTools.Parsing.Element BuildObstructedBy (List<Result> results)
    {
        var list = new ObstructedByList () 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };

        for (int i = 1; i < results.Count; i = i + 2) {
            var val = results[i].Value;
            list.Values.Add (val);
        }

        return list;
    }

    private KAOSTools.Parsing.Element BuildAssignedTo (List<Result> results)
    {
        var list = new AssignedToList () 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };

        if (results[1].Text == "[") {
            list.SystemIdentifier = (results[2].Value as IdentifierOrName);
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

    private KAOSTools.Parsing.Element BuildId (List<Result> results)
    {
        return new KAOSTools.Parsing.Identifier(results[1].Text) 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };
    }

    private KAOSTools.Parsing.Element BuildName (List<Result> results)
    {
        return new KAOSTools.Parsing.Name(results[2].Text) 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };
    }

    private KAOSTools.Parsing.Element BuildStringFormalSpec (List<Result> results)
    {
        return new KAOSTools.Parsing.StringFormalSpec(results[2].Text) 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };
    }

    private KAOSTools.Parsing.Element BuildSignature (List<Result> results)
    {
        return new KAOSTools.Parsing.Signature(results[2].Text) 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };
    }

    private KAOSTools.Parsing.Element BuildFormalSpec (List<Result> results)
    {
        var formalSpec = new FormalSpec (results [2].Text) 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };
        if (formalSpec.Value == null)
            throw new ParsingException (string.Format ("Unable to parse '{0}'", results[2].Text));

        return formalSpec;
    }
    
    private KAOSTools.Parsing.Element BuildDefinition (List<Result> results)
    {
        return new KAOSTools.Parsing.Definition (results [2].Text) 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };
    }
        
    private KAOSTools.Parsing.Element BuildDescription (List<Result> results)
    {
        if (results.Count == 4)
            return new KAOSTools.Parsing.Description (results [2].Text) 
                { Line = results[0].Line, Col = results[0].Col, Filename = m_file };
        else 
            return new KAOSTools.Parsing.Description ("") 
                { Line = results[0].Line, Col = results[0].Col, Filename = m_file };
    }
    
    private KAOSTools.Parsing.Element BuildRDS (List<Result> results)
    {
        return new KAOSTools.Parsing.RDS (double.Parse (results [1].Text)) 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };
    }
    
    private KAOSTools.Parsing.Element BuildProbability (List<Result> results)
    {
        return new KAOSTools.Parsing.Probability (double.Parse (results [1].Text)) 
            { Line = results[0].Line, Col = results[0].Col, Filename = m_file };
    }

    private KAOSTools.Parsing.Element BuildIdOrName (List<Result> results)
    {
        if (results.Count == 3) {
            return new Name (results[1].Text) 
                { Line = results[0].Line, Col = results[0].Col, Filename = m_file };

        } else if (results.Count == 1) {
            return new Identifier (results[0].Text) 
                { Line = results[0].Line, Col = results[0].Col, Filename = m_file };
        }
        return null;
    }
    
    private KAOSTools.Parsing.Element Import (string file)
    {
        var filename = Path.Combine (Path.GetDirectoryName (m_file), file);
        if (files_imported.Contains (Path.GetFullPath (filename))) {
            return new Elements ();
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
}
