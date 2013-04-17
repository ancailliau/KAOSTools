
/*

    public static class MarkovChainExtensions {
        public IDictionary<string, double> LongTermFraction {
            get {
                return ComputeLongTermFraction ();
            }
        }

        private IDictionary<string, double> ComputeLongTermFraction ()
        {
            var conversionTable = BuildConversionTable ();
            var transitionMatrix = GetTransitionMatrix (conversionTable).Transpose ();

            Console.WriteLine ("***");
            Console.WriteLine (transitionMatrix.Transpose ());
            Console.WriteLine ("***");

            var matrix = new DenseMatrix(transitionMatrix - new DiagonalMatrix (States.Count, States.Count, 1));
            matrix.SetRow (matrix.RowCount - 1, /* matrix.Row (matrix.RowCount - 1) + */ new DenseVector (matrix.ColumnCount, 1));

var vector = new DenseVector (States.Count, 0);
vector[vector.Count - 1] = 1;

Console.WriteLine (matrix);
Console.WriteLine (vector);

var result = matrix.LU().Solve (vector);

Console.WriteLine (result);

var longTermFraction = new Dictionary<string, double> ();
for (int i = 0; i < result.Count; i++) {
    string identifier = conversionTable.Where (x => x.Value == i).Single ().Key;
    longTermFraction.Add (identifier, result[i]);
}

return longTermFraction;
}

private DenseMatrix GetTransitionMatrix (Dictionary<string, int> conversionTable)
{
    var m = new DenseMatrix (States.Count, States.Count, 0);
    foreach (var state in States) {
        foreach (var transition in this.Transitions.Where (t => t.From.Equals (state))) {
            m [conversionTable [state.Identifier], conversionTable [transition.To.Identifier]] = transition.Probability;
        }
    }
    return m;
}

private Dictionary<string, int> BuildConversionTable ()
{
    var conversionTable = new Dictionary<string, int> ();
    int i = 0;
    foreach (var state in this._states) {
        conversionTable.Add (state.Identifier, i);
        i++;
    }
    
    return conversionTable;
}

public Dictionary<string, Dictionary<string, double>> TransitionMatrix { 
    get {
        var conversionTable = BuildConversionTable ();
        var m = GetTransitionMatrix (conversionTable);
        
        var m2 = new Dictionary<string, Dictionary<string, double>> ();
        for (int i = 0; i < m.RowCount; i++) {
            string id1 = conversionTable.Where (x => x.Value == i).Single ().Key;
            m2.Add (id1, new Dictionary<string, double>());
            for (int j = 0; j < m.ColumnCount; j++) {
                string id2 = conversionTable.Where (x => x.Value == j).Single ().Key;
                m2[id1].Add (id2, m[i,j]);
            }
        }
        
        return m2;
    }
}
}

public partial class MarkovChain {
    
    public double Probability (string formula) 
    {
        return Probability (LtlSharp.Parser.Parse (formula));
    }
    
    public double Probability (LTLFormula formula)
    {
        var ltf = this.LongTermFraction;
        var d = new Dictionary<string, double> ();
        
        foreach (var state in States) {
            d[state.Identifier] = Satisfy (state, formula);
            Console.WriteLine ("P({0}) = {1:F2} x {2:F2} = {3:F2}", 
                               state.Identifier,
                               d[state.Identifier],
                               ltf[state.Identifier],
                               d[state.Identifier] * ltf[state.Identifier] );
        }
        
        foreach (var i in ltf.Keys) {
            d[i] *= ltf[i];
        }
        
        Console.WriteLine ("sum : " + ltf.Values.Sum ());
        
        return d.Values.Sum ();
    }
    
    public double Satisfy (State s, LTLFormula formula)
    {
        if (formula is True) return 1;
        if (formula is False) return 0;
        
        if (formula is Next) return Satisfy (s, formula as Next);
        if (formula is Proposition) return Satisfy (s, formula as Proposition);
        if (formula is Finally) return Satisfy (s, formula as Finally);
        if (formula is Until) return Satisfy (s, formula as Until);
        if (formula is Globally) return 1 - Satisfy (s, new Finally(new Negation((formula as Globally).Enclosed)));
        
        if (formula is Implication) return Satisfy (s, formula as Implication);
        if (formula is Disjunction) return Satisfy (s, formula as Disjunction);
        if (formula is Conjunction) return Satisfy (s, formula as Conjunction);
        if (formula is Negation) return Satisfy (s, formula as Negation);
        
        if (formula is ParenthesedExpression) return Satisfy (s, (formula as ParenthesedExpression).Enclosed);
        
        throw new NotImplementedException(formula.GetType ().Name + " not implemented");
    }
    
    public double Satisfy (State currentState, Negation negation)
    {
        return 1 - Satisfy (currentState, negation.Enclosed);
    }
    
    Dictionary<Until, Dictionary<State, double>> caching = new Dictionary<Until, Dictionary<State, double>> ();
    
    public double Satisfy (State currentState, Until until)
    {
        if (caching.ContainsKey (until)) {
            return caching[until][currentState];
        }
        
        caching[until] = new Dictionary<State, double> ();
        
        var B = until.Left;
        var C = until.Right;
        
        // Compute SO
        var s0 = States.Except (ComputeNotS0 (States.Where (s => Satisfy (s, B) == 1), C));
        Console.WriteLine ("S0 : " + string.Join (",", s0.Select (s => s.Identifier)));
        
        foreach (var s in s0) 
            caching[until].Add (s, 0);
        
        // Compute S1
        var s1 = ComputeS1 (B, C);
        Console.WriteLine ("S1 : " + string.Join (",", s1.Select (s => s.Identifier)));
        
        foreach (var s in s1) 
            caching[until].Add (s, 1);
        
        // Compute S?
        var spending = States.Except (s0.Union(s1));
        Console.WriteLine ("S? : " + string.Join (",", spending.Select (s => s.Identifier)));
        
        if (spending.Count() > 0) {
            // Build a SM with only states from S?
            var sm = new MarkovChain ();
            foreach (var s in spending) {
                sm.Add (s);
                foreach (var t in spending) {
                    var transition = Transitions.Where (trans => trans.From.Equals (s)
                                                        & trans.To.Equals (t)).SingleOrDefault ();
                    if (transition != null) 
                        sm.Add (transition);
                }
            }
            
            var table = sm.BuildConversionTable ();
            var A = sm.GetTransitionMatrix (table);
            var b = new DenseVector (
                sm.States.Select (x => 
                              Transitions.Where (y => y.From.Equals (x))
                              .Where (y => s1.Contains (y.To))
                              .Select (y => y.Probability).Sum ()
                              ).ToArray ()
                );
            
            Console.WriteLine ("---------- [Conversion Table]");
            Console.WriteLine (string.Join ("\n", table.Select (x => x.Key + ":" + x.Value)));
            Console.WriteLine ("----------");
            
            Console.WriteLine ("---------- [A]");
            Console.WriteLine (A);
            Console.WriteLine ("----------");
            
            Console.WriteLine ("---------- [b]");
            Console.WriteLine (b);
            Console.WriteLine ("----------");
            
            var epsilon = 0.001;
            
            MathNet.Numerics.LinearAlgebra.Generic.Vector<double> prevsol = new DenseVector (b.Count, 0);
            MathNet.Numerics.LinearAlgebra.Generic.Vector<double> currentsol = new DenseVector (b.Count, 0);
            do {
                prevsol = currentsol;
                currentsol = A.Multiply (prevsol) + b;
                
            } while ((prevsol - currentsol).AbsoluteMaximum() > epsilon);
            
            Console.WriteLine ("---------- [solution]");
            Console.WriteLine (currentsol);
            Console.WriteLine ("----------");
            
            foreach (var s in spending) 
                caching[until].Add(s, currentsol[table[s.Identifier]]);
            
            
            Console.WriteLine ("---------- [Final result]");
            Console.WriteLine (string.Join ("\n", caching[until].Select (x => x.Key.Identifier + " : " + x.Value)));
            Console.WriteLine ("----------");
        }
        
        return caching[until][currentState];
    }
    
    IEnumerable<State> Pre (IEnumerable<State> b)
    {
        return Transitions.Where (t => b.Contains (t.To)).Select (t => t.From).Distinct ();
    }
    
    private IEnumerable<State> ComputeNotS0 (IEnumerable<State> b, LTLFormula C)
    {
        // take the pre of b
        var states = Pre (b).Where (s => Satisfy (s, C) == 1);
        
        // if we did add somes states, fixed point is not reached
        if (states.Any (s => !b.Contains (s))) {
            return ComputeNotS0 (states.Union (b).Distinct (), C);
        }
        
        return states.Union (b).Distinct ();
    }
    
    private IEnumerable<State> ComputeS1 (LTLFormula b, LTLFormula c)
    {
        var B = States.Where (s => Satisfy (s, b) == 1);
        var C = States.Where (s => Satisfy (s, c) == 1); 
        var buscub = B.Union (States.Except (C.Union (B)));
        
        var newSM = new MarkovChain ();
        foreach (var s in States) {
            newSM.Add (s);
            foreach (var t in States) {
                if (buscub.Contains (s)) {
                    newSM.Add (new Transition (1, s, t));
                } else {
                    var transition = Transitions.Where (trans => trans.From.Equals (s)
                                                        & trans.To.Equals (t)).SingleOrDefault ();
                    if (transition != null) 
                        newSM.Add (transition);
                }
            }
        }
        
        return newSM.States.Where (s => Satisfy (s, new Finally(b)) == 1);
    }
    
    public double Satisfy (State s, Finally @finally)
    {
        // Search states such that enclosed in finally is true
        var validStates = States.Where (x => Satisfy(x, @finally.Enclosed) > 0);
        if (validStates.Contains (s)) 
            return 1;
        else if (validStates.Count () == 0)
            return 0;
        
        Console.WriteLine ("---------- [States satisfying enclosed]");
        Console.WriteLine (string.Join (", ", validStates.Select (x => x.Identifier)));
        Console.WriteLine ("----------");
        
        var smToValidStates = new MarkovChain ();
        foreach (var state in validStates) {
            DFS (validStates, smToValidStates);
        }
        
        Console.WriteLine ("---------- [States in S tilde]");
        Console.WriteLine (string.Join (", ", smToValidStates.States.Select (x => x.Identifier)));
        Console.WriteLine ("----------");
        
        var table = smToValidStates.BuildConversionTable ();
        var transitions = smToValidStates.GetTransitionMatrix(table);
        
        /*
            Console.WriteLine ("---------- [Conversion Table]");
            Console.WriteLine (string.Join ("\n", table.Select (x => x.Key + ":" + x.Value)));
            Console.WriteLine ("----------");

            Console.WriteLine ("---------- [Transitions]");
            Console.WriteLine (transitions);
            Console.WriteLine ("----------");
            */
        
        // Build the transition matrix for this sub state-machine
        
        var system = new DiagonalMatrix (transitions.RowCount, transitions.ColumnCount, 1)
            - transitions;
        
        /*
            Console.WriteLine ("---------- [System]");
            Console.WriteLine (system);
            Console.WriteLine ("----------");
            */
        
        var vector = new DenseVector (
            smToValidStates.States.Select (x => 
                                       Transitions.Where (y => y.From.Equals (x))
                                       .Where (y => validStates.Contains (y.To))
                                       .Select (y => y.Probability).Sum ()
                                       ).ToArray ()
            );
        /*
            Console.WriteLine ("---------- [Vector]");
            Console.WriteLine (vector);
            Console.WriteLine ("----------");
            */
        
        var result = system.LU ().Solve (vector);
        
        /*
            Console.WriteLine ("---------- [Result]");
            Console.WriteLine (result);
            Console.WriteLine ("----------");
            */
        
        return table.ContainsKey (s.Identifier) ? result[table[s.Identifier]] : 0;
    }
    
    private IEnumerable<State> PreStar (IEnumerable<State> B)
    {
        // take the pre of b
        var pre = Pre (B);
        
        // if we did add somes states, fixed point is not reached
        if (pre.Any (s => !B.Contains (s))) {
            return PreStar (pre.Union (B).Distinct ());
        }
        
        return pre.Union (B).Distinct ();
    }
    
    private void DFS (IEnumerable<State> B, MarkovChain sm)
    {
        var prestarb = PreStar (B).Except (B);
        
        foreach (var s in prestarb) {
            sm.Add (s);
            foreach (var t in prestarb) {
                var transition = Transitions.Where (trans => trans.From.Equals (s)
                                                    & trans.To.Equals (t)).SingleOrDefault ();
                if (transition != null) 
                    sm.Add (transition);
            }
        }
    }
    
    public double Satisfy (State s, Implication imply)
    {
        var satAntecedant = Satisfy (s, imply.Left);
        var satConsequent = Satisfy (s, imply.Right);
        
        return satAntecedant * satConsequent + (1 - satAntecedant);
    }
    
    public double Satisfy (State s, Conjunction conjunct)
    {
        return conjunct.Expressions.Select (x => Satisfy (s, x)).Aggregate (1d, (acc, val) => acc * val);
    }
    
    public double Satisfy (State s, Disjunction disjunct)
    {
        return 1 - disjunct.Expressions.Select (x => 1 - Satisfy (s, x)).Aggregate (1d, (acc, val) => acc * val);
    }
    
    public double Satisfy (State s, Next next)
    {
        return Transitions.Where (t => t.From.Equals (s))
            .Select (x => x.Probability * Satisfy (x.To, next.Enclosed))
                .Sum ();
    }
    
    public double Satisfy (State s, Proposition proposition)
    {
        if (s.ValidPropositions.Contains (proposition)) {
            return 1;
        } else {
            return 0; 
        }
    }
    
}

*/
