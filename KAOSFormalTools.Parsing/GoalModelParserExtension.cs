using System;
using System.Linq;
using System.Collections.Generic;
using KAOSFormalTools.Parsing;
using KAOSFormalTools.Domain;
using System.IO;

internal sealed partial class GoalModelParser
{   
    private List<string> files_imported = new List<string> ();

    private KAOSFormalTools.Parsing.Element BuildElements (List<Result> results)
    {
        var attrs = new Elements();
        foreach (var result in results) {
            BuildElement (attrs, result.Value);
        }
        return attrs;
    }

    private KAOSFormalTools.Parsing.Element BuildElement (Elements attrs, Element value)
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

    private KAOSFormalTools.Parsing.Predicate BuildPredicate (List<Result> results)
    {
        var predicate = new KAOSFormalTools.Parsing.Predicate ();

        for (int i = 1; i < results.Count; i++) {
            predicate.Attributes.Add (results[i].Value as KAOSFormalTools.Parsing.Attribute);
        }
        
        return predicate;
    }

    private KAOSFormalTools.Parsing.Element BuildGoal (List<Result> results)
    {
        var goal = new KAOSFormalTools.Parsing.Goal ();
        if (results[0].Text == "override")
            goal.Override = true;

        for (int i = 1; i < results.Count; i++) {
            goal.Attributes.Add (results[i].Value as KAOSFormalTools.Parsing.Attribute);
        }

        return goal;
    }

    private KAOSFormalTools.Parsing.Element BuildDomainProperty (List<Result> results)
    {
        var domprop = new KAOSFormalTools.Parsing.DomainProperty ();

        for (int i = 1; i < results.Count; i++) {
            domprop.Attributes.Add (results[i].Value as KAOSFormalTools.Parsing.Attribute);
        }

        return domprop;
    }
    
    private KAOSFormalTools.Parsing.Element BuildDomainHypothesis (List<Result> results)
    {
        var domhyp = new KAOSFormalTools.Parsing.DomainHypothesis ();
        
        for (int i = 1; i < results.Count; i++) {
            domhyp.Attributes.Add (results[i].Value as KAOSFormalTools.Parsing.Attribute);
        }
        
        return domhyp;
    }
    
    private KAOSFormalTools.Parsing.Element BuildAlternative (List<Result> results)
    {
        var alternative = new KAOSFormalTools.Parsing.Alternative ();
        
        for (int i = 1; i < results.Count; i++) {
            alternative.Attributes.Add (results[i].Value as KAOSFormalTools.Parsing.Attribute);
        }
        
        return alternative;
    }

    private KAOSFormalTools.Parsing.Element BuildObstacle (List<Result> results)
    {
        var obstacle = new KAOSFormalTools.Parsing.Obstacle ();

        for (int i = 1; i < results.Count; i++) {
            obstacle.Attributes.Add (results[i].Value as KAOSFormalTools.Parsing.Attribute);
        }

        return obstacle;
    }
    
    private KAOSFormalTools.Parsing.Element BuildAgent (List<Result> results)
    {
        var agent = new KAOSFormalTools.Parsing.Agent ();

        int start = 1;
        if (results[1].Text != "agent") {
            start = 2;
            if (results[1].Text == "software")
                agent.Type = KAOSFormalTools.Parsing.AgentType.Software;
            else if (results[1].Text == "environment") 
                agent.Type = KAOSFormalTools.Parsing.AgentType.Environment;
        }

        for (int i = start; i < results.Count; i++) {
            agent.Attributes.Add (results[i].Value as KAOSFormalTools.Parsing.Attribute);
        }

        return agent;
    }


    private KAOSFormalTools.Parsing.Element BuildRefinedBy (List<Result> results)
    {
        var list = new RefinedByList ();

        if (results[1].Text == "[") {
            list.AlternativeIdentifier = results[2].Value as IdentifierOrName;
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

    
    private KAOSFormalTools.Parsing.Element BuildResolvedBy (List<Result> results)
    {
        var list = new ResolvedByList ();
        for (int i = 1; i < results.Count; i = i + 2) {
            list.Values.Add (results[i].Value);
        }
        
        return list;
    }

    private KAOSFormalTools.Parsing.Element BuildObstructedBy (List<Result> results)
    {
        var list = new ObstructedByList ();
        for (int i = 1; i < results.Count; i = i + 2) {
            var val = results[i].Value;
            list.Values.Add (val);
        }

        return list;
    }

    private KAOSFormalTools.Parsing.Element BuildAssignedTo (List<Result> results)
    {
        var list = new AssignedToList ();
        if (results[1].Text == "[") {
            list.AlternativeIdentifier = (results[2].Value as IdentifierOrName);
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

    private KAOSFormalTools.Parsing.Element BuildId (List<Result> results)
    {
        return new KAOSFormalTools.Parsing.Identifier(results[1].Text);
    }

    private KAOSFormalTools.Parsing.Element BuildName (List<Result> results)
    {
        return new KAOSFormalTools.Parsing.Name(results[2].Text);
    }

    private KAOSFormalTools.Parsing.Element BuildStringFormalSpec (List<Result> results)
    {
        return new KAOSFormalTools.Parsing.StringFormalSpec(results[2].Text);
    }

    private KAOSFormalTools.Parsing.Element BuildSignature (List<Result> results)
    {
        return new KAOSFormalTools.Parsing.Signature(results[2].Text);
    }

    private KAOSFormalTools.Parsing.Element BuildFormalSpec (List<Result> results)
    {
        var formalSpec = new FormalSpec (results [2].Text);
        if (formalSpec.Value == null)
            throw new ParsingException (string.Format ("Unable to parse '{0}'", results[2].Text));

        return formalSpec;
    }
    
    private KAOSFormalTools.Parsing.Element BuildDefinition (List<Result> results)
    {
        return new KAOSFormalTools.Parsing.Definition (results [2].Text);
    }
        
    private KAOSFormalTools.Parsing.Element BuildDescription (List<Result> results)
    {
        if (results.Count == 4)
            return new KAOSFormalTools.Parsing.Description (results [2].Text);
        else 
            return new KAOSFormalTools.Parsing.Description ("");
    }
    
    private KAOSFormalTools.Parsing.Element BuildRDS (List<Result> results)
    {
        return new KAOSFormalTools.Parsing.RDS (double.Parse (results [1].Text));
    }
    
    private KAOSFormalTools.Parsing.Element BuildProbability (List<Result> results)
    {
        return new KAOSFormalTools.Parsing.Probability (double.Parse (results [1].Text));
    }

    private KAOSFormalTools.Parsing.Element BuildIdOrName (List<Result> results)
    {
        if (results.Count == 3) {
            return new Name (results[1].Text);
        } else if (results.Count == 1) {
            return new Identifier (results[0].Text);
        }
        return null;
    }
    
    private KAOSFormalTools.Parsing.Element Import (string file)
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
