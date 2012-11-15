using System;
using System.Linq;
using System.Collections.Generic;
using KAOSFormalTools.Parsing;
using KAOSFormalTools.Domain;

internal sealed partial class GoalModelParser
{   
    private KAOSFormalTools.Parsing.Element BuildElements (List<Result> results)
    {
        var attrs = new Elements();
        foreach (var result in results) {
            attrs.Values.Add (result.Value);
        }
        return attrs;
    }

    private KAOSFormalTools.Parsing.Element BuildGoal (List<Result> results)
    {
        var goal = new KAOSFormalTools.Parsing.Goal ();

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
        if (results[1].Text == "software") {
            start = 2;
            agent.Software = true;
        }

        for (int i = start; i < results.Count; i++) {
            agent.Attributes.Add (results[i].Value as KAOSFormalTools.Parsing.Attribute);
        }

        return agent;
    }


    private KAOSFormalTools.Parsing.Element BuildRefinedBy (List<Result> results)
    {
        var list = new RefinedByList ();
        for (int i = 1; i < results.Count; i = i + 2) {
            list.Values.Add (results[i].Value);
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
        for (int i = 1; i < results.Count; i = i + 2) {
            list.Values.Add (results[i].Value);
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
        return new KAOSFormalTools.Parsing.Description (results [2].Text);
    }
    
    private KAOSFormalTools.Parsing.Element BuildRDS (List<Result> results)
    {
        return new KAOSFormalTools.Parsing.RDS (float.Parse (results [1].Text));
    }
    
    private KAOSFormalTools.Parsing.Element BuildProbability (List<Result> results)
    {
        return new KAOSFormalTools.Parsing.Probability (float.Parse (results [1].Text));
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
}
