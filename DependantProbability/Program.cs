using System;
using System.Linq;
using System.Collections.Generic;
using BoolSimplify;
using System.IO;

// TODO bugs in simplify when "single" and or "single" or 
using KAOSTools.Core;
using KAOSTools.Parsing;

namespace DependantProbability
{
    class MainClass
    {


        public static void Main (string[] args)
        {

            var input = @"
declare goal 
    id root
    name ""Root"" 
    refinedby (case[0.5,0.5]) ""Maintain [Navigation Available]"", ""Bis""
end 

declare goal 
    name ""Bis"" 
end 

declare goal 
    name ""Maintain [Navigation Available]"" 
    obstructedby ""Navigation Not Available"" 
end 

override obstacle 
    name ""Navigation Not Available"" 
    refinedby ""Garmin Navigation Not Working"", ""iPhone Navigation Not Working"" 
end

override obstacle 
    name ""Garmin Navigation Not Working"" 
    refinedby ""Garmin Device Broken"" 
    refinedby ""GPS Signal Blocked""
end

override obstacle 
    name ""iPhone Navigation Not Working"" 
    refinedby ""iPhone Device Broken"" 
    refinedby ""GPS Signal Blocked""
end

override obstacle name ""Garmin Device Broken"" probability .3 end
override obstacle name ""iPhone Device Broken"" probability .1 end
override obstacle name ""GPS Signal Blocked"" probability .2 end";

            
            var parser = new ModelBuilder ();
            
            var filename = "/Users/acailliau/Dropbox/PhD/2013/Dependent obstacles/las.kaos";
            input = File.ReadAllText (filename);
            var model = parser.Parse (input, filename);

            Console.WriteLine ("--- [Non Satisfaction Formulas]");

            foreach (var g in model.Goals () ) {
                var node = g.GetNonSatisfactionFormula ();
                Console.WriteLine ("{0}={1}", g.FriendlyName, node.Simplify ());
            }
            Console.WriteLine ();
            foreach (var g in model.Obstacles ()) {
                Console.WriteLine ("{0}={1}", g.FriendlyName, g.GetNonSatisfactionFormula ());
            }

            Console.WriteLine ("---\n");

            
            Console.WriteLine ("--- [Probabilities]");

            foreach (var g in model.Goals ()) {
                Console.WriteLine ("P({0})={1}%", g.FriendlyName, Math.Round (g.ComputeProbability (), 4)*100);
            }
            
            Console.WriteLine ();

            foreach (var o in model.Obstacles ()) {
                Console.WriteLine ("P({0})={1}%", o.FriendlyName, Math.Round (o.ComputeProbability (), 4)*100);
            }
            

            Console.WriteLine ("---\n");
        }
    }

    public static class BoolFormulaHelpers
    {
        public static Node Simplify (this Node node)
        {
            var nvars = node.ReferencedObstacles ().Count () 
                + node.ReferencedVariables ().Count () 
                    + node.ReferencedDomainProperties ().Count ();


            var conversionMap = new Dictionary<Obstacle, int> ();
            var i = 0;
            var iEnumerable = node.ReferencedObstacles ();
            foreach (var o in iEnumerable) {
                if (!conversionMap.ContainsKey (o))
                    conversionMap.Add (o, i++);
            }

            var conditionMap = new Dictionary<Condition, int> ();
            var iEnumerable2 = node.ReferencedVariables ();
            foreach (var o in iEnumerable2) {
                if (!conditionMap.ContainsKey (o))
                    conditionMap.Add (o, i++);
            }

            var dompropMap = new Dictionary<DomainProperty, int> ();
            var iEnumerable3 = node.ReferencedDomainProperties ();
            foreach (var o in iEnumerable3) {
                if (!dompropMap.ContainsKey (o))
                    dompropMap.Add (o, i++);
            }

            // Console.WriteLine (node);
            node = node.AndDistribute ();
//            Console.WriteLine (node);
            node = node.Flatten ();
//            Console.WriteLine (node);

            if (node is Or) {
                var or = (Or)node;
                var terms = new List<BoolSimplify.Term> ();
                foreach (var n in or.Nodes) {
                    var vars = new byte[nvars];
                    for (int j = 0; j < nvars; j++)
                        vars [j] = Term.DontCare;

                    if (NewMethod (n, vars, conversionMap, conditionMap, dompropMap)) {
                        terms.Add (new Term (vars));
                    }
                }

                var karnaughFormula = new BoolSimplify.Formula (terms);
                if (karnaughFormula.Terms.Count() == 0)
                    return new False ();
                 //Console.WriteLine (karnaughFormula);
                karnaughFormula.ReduceToPrimeImplicants ();
                if (karnaughFormula.Terms.Count() == 0)
                    return new False ();
//                 Console.WriteLine (karnaughFormula);
                karnaughFormula.ReducePrimeImplicantsToSubset ();
                if (karnaughFormula.Terms.Count() == 0)
                    return new False ();
//                 Console.WriteLine (karnaughFormula);

                var simplifiedFormula = new Or ();
                foreach (var t in karnaughFormula.Terms) {
                    var and = new And ();
                    for (int j = 0; j < t.NumVars; j++) {
                        if (t.Values [j] == 1) {
                            if (conversionMap.ContainsValue (j)) {
                                and.Nodes.Add (new ObstacleRef () {
                                    Obstacle = conversionMap.Where (x => x.Value == j).Select (x => x.Key).Single ()
                                });
                            }
                            if (conditionMap.ContainsValue (j)) {
                                and.Nodes.Add (conditionMap.Where (x => x.Value == j).Select (x => x.Key).Single ());
                            }
                            if (dompropMap.ContainsValue (j)) {
                                and.Nodes.Add (new DomPropRef () {
                                    DomainProperty = dompropMap.Where (x => x.Value == j).Select (x => x.Key).Single ()
                                });
                            }
                        }
                        if (t.Values [j] == 0) {
                            if (conversionMap.ContainsValue (j)) {
                                and.Nodes.Add (new ObstacleRef () {
                                    Obstacle = conversionMap.Where (x => x.Value == j).Select (x => x.Key).Single ()
                                }.Negate ());
                            }
                            if (conditionMap.ContainsValue (j)) {
                                and.Nodes.Add (conditionMap.Where (x => x.Value == j).Select (x => x.Key).Single ().Negate ());
                            }
                            if (dompropMap.ContainsValue (j)) {
                                and.Nodes.Add (new DomPropRef () {
                                    DomainProperty = dompropMap.Where (x => x.Value == j).Select (x => x.Key).Single ()
                                }.Negate ());
                            }
                        }
                    }
                    if (and.Nodes.Count > 0)
                        simplifiedFormula.Nodes.Add (and);
                }

                if (simplifiedFormula.Nodes.Count > 0)
                    return simplifiedFormula;
                else
                    return new Not { Enclosed = new False () };
            }

            if (node is ObstacleRef | node is DomPropRef | node is False)
                return node;

            return node;
            // throw new NotSupportedException (node.GetType ().ToString ());
        }

        static bool NewMethod (Node n, byte[] vars, 
                               Dictionary<Obstacle, int> conversionMap, 
                               Dictionary<Condition, int> conditionMap, 
                               Dictionary<DomainProperty, int> dompropMap)
        {
            if (n is And) {
                // Console.WriteLine ("> " + n);
                foreach (var nn in ((And)n).Nodes) {
                    if (!NewMethod (nn, vars, conversionMap, conditionMap, dompropMap)) {
                        return false;
                    }
                }
                return true;
            }

            if (n is ObstacleRef) {
                var r = ((ObstacleRef)n).Obstacle;
                if (vars [conversionMap [r]] == Term.DontCare)
                    vars [conversionMap [r]] = 1;
                else if (vars [conversionMap [r]] != 1)
                    return false;
                return true;
            }

            if (n is DomPropRef) {
                var r = ((DomPropRef)n).DomainProperty;
                if (vars [dompropMap [r]] == Term.DontCare)
                    vars [dompropMap [r]] = 1;
                else if (vars [dompropMap [r]] != 1)
                    return false;
                return true;
            }

            if (n is Condition) {
                var r = ((Condition)n);
                if (vars [conditionMap [r]] == Term.DontCare)
                    vars [conditionMap [r]] = 1;
                else if (vars [conditionMap [r]] != 1)
                    return false;
                return true;
            }

            if (n is False) 
                return false;

            if (n is Not) {
                // Assume that not have been pushed inwards

                var nn = (Not) n;

                if (nn.Enclosed is ObstacleRef) {
                    var r = ((ObstacleRef)nn.Enclosed).Obstacle;
                    if (vars [conversionMap [r]] == Term.DontCare)
                        vars [conversionMap [r]] = 0;
                    else if (vars [conversionMap [r]] != 0)
                        return false;
                    return true;
                }

                if (nn.Enclosed is DomPropRef) {
                    var r = ((DomPropRef)nn.Enclosed).DomainProperty;
                    if (vars [dompropMap [r]] == Term.DontCare)
                        vars [dompropMap [r]] = 0;
                    else if (vars [dompropMap [r]] != 0)
                        return false;
                    return true;
                }

                if (nn.Enclosed is False) {
                    return true;
                }

                if (nn.Enclosed is Condition) {
                    var r = ((Condition)nn.Enclosed);
                    if (vars [conditionMap [r]] == Term.DontCare)
                        vars [conditionMap [r]] = 0;
                    else if (vars [conditionMap [r]] != 0)
                        return false;
                    return true;
                }

                // Console.WriteLine ("> " + nn.Enclosed);

                throw new NotSupportedException (string.Format ("{0} is not supported in Not", n.GetType ()));

            }

            throw new NotSupportedException (string.Format ("{0} is not supported", n.GetType ()));
        }

        static double ComputeProbability (Node formula)
        {
            double proba = 1;
            if (formula is Or) {
                foreach (var t in ((Or) formula).Nodes) {
                    proba *= (1 - ComputeProbability (t));
                }
                return 1 - proba;
            } 

            if (formula is And) {
                var lp = 1d;
                foreach (var node in ((And) formula).Nodes)
                    lp *= ComputeProbability (node);
                return lp;
            }

            if (formula is ObstacleRef) {
                return ((ObstacleRef)formula).Obstacle.EPS;
            }

            if (formula is DomPropRef) {
                return ((DomPropRef)formula).DomainProperty.EPS;
            }

            if (formula is False)
                return 0;
            
            if (formula is Condition)
                return ((Condition) formula).Proba;

            if (formula is Not)
                return 1 - ComputeProbability(((Not) formula).Enclosed);

            throw new NotSupportedException (formula.GetType ().ToString ());
        }

        public static double ComputeProbability (this Obstacle obstacle)
        {
            var formula = obstacle.GetNonSatisfactionFormula ().Simplify ();
            return ComputeProbability (formula);
        }

        public static double ComputeProbability (this Goal goal)
        {
            var formula = goal.GetNonSatisfactionFormula ().Simplify ();
            return 1 - ComputeProbability (formula);
        }
    }
    #region Node class
    public interface Node
    {
        Node AndDistribute ();
        Node Negate ();

        Node Flatten ();

        ISet<Obstacle> ReferencedObstacles ();
        ISet<Condition> ReferencedVariables ();
        ISet<DomainProperty> ReferencedDomainProperties ();

        int NumberOfVariables ();
    }

    public class And : Node
    {
        public List<Node> Nodes;

        public And ()
        {
            Nodes = new List<Node> ();
        }

        public And (IEnumerable<Node> nodes)
        {
            Nodes = new List<Node> (nodes);
        }

        public override string ToString ()
        {
            return "(" + string.Join ("&", Nodes.Select (x => x.ToString ())) + ")";
        }

        public Node AndDistribute ()
        {
            var distributedNodes = Nodes.Select (x => x.AndDistribute ()).ToList ();
            var i = distributedNodes.FindIndex (x => x.GetType () == typeof(Or));

            if (i >= 0) {
                var pivot = (Or)distributedNodes.ElementAt (i);
                var newNode = new Or ();
                var remainingNodes = distributedNodes.Where (x => x != pivot).Select (x => x.AndDistribute ());

                var head = new List<Node> (pivot.Nodes.Take (1).Select (x => x.AndDistribute ()));
                var tail = new List<Node> (pivot.Nodes.Skip (1).Select (x => x.AndDistribute ()));

//                Console.WriteLine ("Head = " + string.Join (",", head.Select (x => x.ToString ())));
//                Console.WriteLine ("Tail = " + string.Join (",", tail.Select (x => x.ToString ())));

                newNode.Nodes.Add (new And (new List<Node> (remainingNodes).Union (head)).AndDistribute ());
                newNode.Nodes.Add (new And (new List<Node> (remainingNodes).Union (tail)).AndDistribute ());

                return newNode;
            }

            return new And { Nodes = distributedNodes };
        }

        public Node Negate ()
        {
            return new Or {
                Nodes = Nodes.Select (x => x.Negate ()).ToList ()
            };
        }

        public Node Flatten ()
        {
            if (Nodes.Count == 1)
                return Nodes.First ().Flatten ();

            Nodes = Nodes.Select (x => x.Flatten ()).ToList ();
            var andNodes = Nodes.Where (x => x.GetType () == typeof(And)).Cast <And> ().ToList ();
            Nodes.AddRange (andNodes.SelectMany (x => x.Nodes));
            Nodes.RemoveAll (x => andNodes.Contains (x));
            return this;
        }

        public ISet<Obstacle> ReferencedObstacles ()
        {
            var st = new HashSet<Obstacle> ();
            foreach (var s in Nodes)
                st.UnionWith (s.ReferencedObstacles ());
            return st;
        }

        public ISet<Condition> ReferencedVariables ()
        {
            var st = new HashSet<Condition> ();
            foreach (var s in Nodes)
                st.UnionWith (s.ReferencedVariables ());
            return st;
        }

        public ISet<DomainProperty> ReferencedDomainProperties ()
        {
            var st = new HashSet<DomainProperty> ();
            foreach (var s in Nodes)
                st.UnionWith (s.ReferencedDomainProperties ());
            return st;
        }

        public int NumberOfVariables ()
        {
            return Nodes.Sum (x => x.NumberOfVariables ());
        }
    }

    public class Or : Node
    {
        public List<Node> Nodes;

        public Or ()
        {
            Nodes = new List<Node> ();
        }

        public override string ToString ()
        {
            return "(" + string.Join ("|", Nodes.Select (x => x.ToString ())) + ")";
        }

        public Node AndDistribute ()
        {
            return new Or { Nodes = Nodes.Select (x => x.AndDistribute ()).ToList () };
        }

        public Node Negate ()
        {
            return new And {
                Nodes = Nodes.Select (x => x.Negate ()).ToList ()
            };
        }

        public Node Flatten ()
        {
            if (Nodes.Count == 1)
                return Nodes.First ().Flatten ();

            Nodes = Nodes.Select (x => x.Flatten ()).ToList ();
            var orNodes = Nodes.Where (x => x.GetType () == typeof(Or)).Cast <Or> ().ToList ();
            Nodes.AddRange (orNodes.SelectMany (x => x.Nodes));
            Nodes.RemoveAll (x => orNodes.Contains (x));
            return this;
        }

        public ISet<Obstacle> ReferencedObstacles ()
        {
            var st = new HashSet<Obstacle> ();
            foreach (var s in Nodes)
                st.UnionWith (s.ReferencedObstacles ());
            return st;
        }

        public ISet<Condition> ReferencedVariables ()
        {
            var st = new HashSet<Condition> ();
            foreach (var s in Nodes)
                st.UnionWith (s.ReferencedVariables ());
            return st;
        }

        public ISet<DomainProperty> ReferencedDomainProperties ()
        {
            var st = new HashSet<DomainProperty> ();
            foreach (var s in Nodes)
                st.UnionWith (s.ReferencedDomainProperties ());
            return st;
        }

        public int NumberOfVariables ()
        {
            return Nodes.Sum (x => x.NumberOfVariables ());
        }
    }

    public class ObstacleRef : Node
    {
        public Obstacle Obstacle;

        public override string ToString ()
        {
            return Obstacle.FriendlyName;
        }

        public Node AndDistribute ()
        {
            return this;
        }

        public Node Negate ()
        {
            return new Not {
                Enclosed = this
            };
        }

        public Node Flatten ()
        {
            return this;
        }

        public ISet<Obstacle> ReferencedObstacles ()
        {
            return new HashSet<Obstacle> (new Obstacle[] { Obstacle });
        }

        public ISet<Condition> ReferencedVariables ()
        {
            return new HashSet<Condition> ();
        }

        public ISet<DomainProperty> ReferencedDomainProperties ()
        {
            return new HashSet<DomainProperty> (new DomainProperty[] { });
        }
        public int NumberOfVariables ()
        {
            return 1;
        }
    }

    public class DomPropRef : Node
    {
        public DomainProperty DomainProperty;

        public override string ToString ()
        {
            return DomainProperty.FriendlyName;
        }

        public Node AndDistribute ()
        {
            return this;
        }

        public Node Negate ()
        {
            return new Not {
                Enclosed = this
            };
        }

        public Node Flatten ()
        {
            return this;
        }

        public ISet<DomainProperty> ReferencedDomainProperties ()
        {
            return new HashSet<DomainProperty> (new DomainProperty[] { DomainProperty });
        }

        public ISet<Obstacle> ReferencedObstacles ()
        {
            return new HashSet<Obstacle> (new Obstacle[] { });
        }

        public ISet<Condition> ReferencedVariables ()
        {
            return new HashSet<Condition> ();
        }
        public int NumberOfVariables ()
        {
            return 1;
        }
    }

    public class Condition : Node {
        public string Cond;
        public double Proba;
        public override string ToString ()
        {
            return string.Format ("{0}<{1}>", Cond, Proba);
        }
        public Node AndDistribute () {
            return this;
        }

        public Node Negate ()
        {
            return new Not {
                Enclosed = this
            };
        }
        public Node Flatten ()
        {
            return this;
        }

        public ISet<Obstacle> ReferencedObstacles ()
        {
            return new HashSet<Obstacle> ();
        }

        public ISet<Condition> ReferencedVariables ()
        {
            return new HashSet<Condition> (new Condition [] { this });
        }

        public ISet<DomainProperty> ReferencedDomainProperties ()
        {
            return new HashSet<DomainProperty> (new DomainProperty [] { });
        }
        public int NumberOfVariables ()
        {
            return 1;
        }
    }

    public class False : Node {
        public override string ToString ()
        {
            return "False";
        }
        public Node AndDistribute () {
            return this;
        }

        public Node Negate ()
        {
            return new Not {
                Enclosed = this
            };
        }
        public Node Flatten ()
        {
            return this;
        }
        public ISet<Obstacle> ReferencedObstacles ()
        {
            return new HashSet<Obstacle> ();
        }
        public ISet<Condition> ReferencedVariables ()
        {
            return new HashSet<Condition> ();
        }
        public ISet<DomainProperty> ReferencedDomainProperties ()
        {
            return new HashSet<DomainProperty> ();
        }
        public int NumberOfVariables ()
        {
            return 0;
        }
    }

    public class Not : Node {
        public Node Enclosed;
        public override string ToString ()
        {
            return "!("+Enclosed.ToString ()+")";
        }
        public Node AndDistribute () {
            return new Not { Enclosed = Enclosed.AndDistribute () };
        }

        public Node Negate ()
        {
            return Enclosed;
        }
        public Node Flatten ()
        {
            return new Not { Enclosed = Enclosed.Flatten () };
        }
        public ISet<Obstacle> ReferencedObstacles ()
        {
            return Enclosed.ReferencedObstacles ();
        }
        public ISet<Condition> ReferencedVariables ()
        {
            return Enclosed.ReferencedVariables ();
        }
        public ISet<DomainProperty> ReferencedDomainProperties ()
        {
            return Enclosed.ReferencedDomainProperties ();
        }
        public int NumberOfVariables ()
        {
            return Enclosed.NumberOfVariables ();
        }
    }

    #endregion

    public static class SatisfactionPropagator {
        public static Node GetNonSatisfactionFormula (this Obstacle o)
        {
            if (o.Refinements ().Count () == 0) {
                return new ObstacleRef { Obstacle = o };
            }

            var or = new Or ();
            foreach (var refinement in o.Refinements ()) {
                var and = new And ();
                foreach (var obstacle in refinement.SubObstacles ()) {
                    and.Nodes.Add (GetNonSatisfactionFormula (obstacle));
                }
                foreach (var domprop in refinement.DomainProperties ()) {
                    and.Nodes.Add (GetNonSatisfactionFormula (domprop));
                }

                if (and.Nodes.Count == 1)
                    or.Nodes.Add (and.Nodes.Single ());
                else
                    or.Nodes.Add (and);
            }

            if (or.Nodes.Count == 1)
                return or.Nodes.Single ();

            return or;
        }

        public static Node GetNonSatisfactionFormula (this DomainProperty o)
        {
            return new DomPropRef { DomainProperty = o };
        }

        public static Node GetNonSatisfactionFormula (this Goal g)
        {
            var refinements = g.Refinements ();
            if (refinements.Count () > 1)
                throw new NotSupportedException ();

            if (refinements.Count () == 1) {
                var refinement = refinements.Single ();

                if (refinement.RefinementPattern == RefinementPattern.Redundant) {
                    return ANDPropagate (refinement);
                }

                if (refinement.RefinementPattern == RefinementPattern.Case) {
                    var orNode = new Or();

                    if (refinement.SubGoals().Count () != 2) {
                        throw new NotImplementedException ();
                    }
                    
                    int index = 0;
                    foreach (var c in refinement.SubGoals ()) {
                        var andNode = new And ();
                        var probability = refinement.Parameters.ElementAt (index);
                        var condition = new Condition {
                            Cond = string.Format ("Case condition {0}", index),
                            Proba = probability
                        };

                        if (index == 0)
                            andNode.Nodes.Add (condition);
                        else if (index == 1)
                            andNode.Nodes.Add (condition.Negate ());

                        // TODO fix this bug, with Simplify() required.
                        var node = GetNonSatisfactionFormula (c);
                        andNode.Nodes.Add (node);

                        orNode.Nodes.Add (andNode);
                        index++;
                    }

                    return orNode;//.ToOr ();
                }

                return ORPropagate (refinement);
            }

            if (g.Obstructions().Count () == 1)
                return GetNonSatisfactionFormula (g.Obstructions().Single().Obstacle ());

            return new False ();
        }

        static Node ORPropagate (GoalRefinement r)
        {
            return new Or { 
                Nodes = r.SubGoals().Select (GetNonSatisfactionFormula).Union (r.DomainProperties().Select (GetNonSatisfactionFormula)).ToList ()
            };
        }

        static Node ANDPropagate (GoalRefinement r)
        {
            return new And { 
                Nodes = r.SubGoals().Select (GetNonSatisfactionFormula).ToList ()
            };
        }

    }
}
