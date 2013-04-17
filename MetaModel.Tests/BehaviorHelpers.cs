using System;
using LtlSharp;

namespace KAOSTools.MetaModel.Tests
{
    public static class TestHelpers {
        
        public static MarkovChain BuildSingleStateSM () 
        {
            var sm = new MarkovChain ();
            
            var s1 = sm.Add (new State ("s1", true));
            s1.ValidPropositions.Add (new Proposition ("state_one"));
            
            sm.Add (new Transition ("t1", 1, s1, s1));
            
            return sm;
        }
        
        public static MarkovChain BuildChain () 
        {
            var sm = new MarkovChain ();
            
            var s1 = sm.Add (new State ("s1", true));
            var s2 = sm.Add (new State ("s2"));
            
            sm.Add (new Transition ("t1", 1, s1, s2));
            sm.Add (new Transition ("t2", 1, s2, s2));
            
            return sm;
        }
        
        public static MarkovChain BuildY () 
        {
            var sm = new MarkovChain ();
            
            var s1 = sm.Add (new State ("s1", true));
            var s2 = sm.Add (new State ("s2"));
            var s3 = sm.Add (new State ("s3"));
            
            sm.Add (new Transition ("t1", .5, s1, s2));
            sm.Add (new Transition ("t2", 1, s2, s2));
            
            sm.Add (new Transition ("t3", .5, s1, s3));
            sm.Add (new Transition ("t4", 1, s3, s3));
            
            return sm;
        }
        
        public static MarkovChain BuildTwoStatesSM () 
        {
            var sm = new MarkovChain ();
            
            var s1 = sm.Add (new State ("s1", true));
            var s2 = sm.Add (new State ("s2"));
            
            s1.ValidPropositions.Add (new Proposition ("state_one"));
            s2.ValidPropositions.Add (new Proposition ("state_two"));
            
            sm.Add (new Transition ("t1", 1, s1, s2));
            sm.Add (new Transition ("t2", 1, s2, s1));
            
            return sm;
        }
        
        public static MarkovChain BuildTwoStatesSMWithSelfLoop () 
        {
            var sm = new MarkovChain ();
            
            var s1 = sm.Add (new State ("s1", true));
            var s2 = sm.Add (new State ("s2"));
            
            s1.ValidPropositions.Add (new Proposition ("state_one"));
            s2.ValidPropositions.Add (new Proposition ("state_two"));
            
            sm.Add (new Transition ("t1", .5, s1, s2));
            sm.Add (new Transition ("t2", .5, s1, s1));
            
            sm.Add (new Transition ("t3", 1, s2, s1));
            
            return sm;
        }
        
        public static MarkovChain BuildThreeStatesSM () 
        {
            var sm = new MarkovChain ();
            
            var s1 = sm.Add (new State ("s1", true));
            var s2 = sm.Add (new State ("s2"));
            var s3 = sm.Add (new State ("s3"));
            
            s1.ValidPropositions.Add (new Proposition ("state_one"));
            s2.ValidPropositions.Add (new Proposition ("state_two"));
            s3.ValidPropositions.Add (new Proposition ("state_three"));
            
            sm.Add (new Transition ("t12", .5, s1, s2));
            sm.Add (new Transition ("t13", .5, s1, s3));
            
            sm.Add (new Transition ("t21", .7, s2, s1));
            sm.Add (new Transition ("t23", .3, s2, s3));
            
            sm.Add (new Transition ("t31", .9, s3, s1));
            sm.Add (new Transition ("t32", .1, s3, s2));
            
            return sm;
        }
        
        public static MarkovChain BuildSimpleCommunicationProtocol () 
        {
            var sm = new MarkovChain ();
            
            var start     = sm.Add (new State ("start", true));
            var @try      = sm.Add (new State ("try"));
            var delivered = sm.Add (new State ("delivered"));
            var lost      = sm.Add (new State ("lost"));
            
            start.ValidPropositions.Add (new Proposition ("start"));
            @try.ValidPropositions.Add (new Proposition ("try"));
            delivered.ValidPropositions.Add (new Proposition ("delivered"));
            lost.ValidPropositions.Add (new Proposition ("lost"));
            
            sm.Add (new Transition (1, start, @try));
            sm.Add (new Transition (.9, @try, delivered));
            sm.Add (new Transition (.1, @try, lost));
            sm.Add (new Transition (1, lost, @try));
            sm.Add (new Transition (1, delivered, start));
            
            return sm;
        }
        
        public static MarkovChain BuildCrapsGame () 
        {
            var sm = new MarkovChain ();
            
            var start     = sm.Add (new State ("start", true));
            var won       = sm.Add (new State ("won"));
            var lost      = sm.Add (new State ("lost"));
            
            var s4        = sm.Add (new State ("4"));
            var s10       = sm.Add (new State ("10"));
            var s5        = sm.Add (new State ("5"));
            var s9        = sm.Add (new State ("9"));
            var s6        = sm.Add (new State ("6"));
            var s8        = sm.Add (new State ("8"));
            
            won.ValidPropositions.Add (new Proposition ("b"));
            
            start.ValidPropositions.Add (new Proposition ("c"));
            s4.ValidPropositions.Add (new Proposition ("c"));
            s5.ValidPropositions.Add (new Proposition ("c"));
            s6.ValidPropositions.Add (new Proposition ("c"));
            
            sm.Add (new Transition (2d/9, start, won));
            sm.Add (new Transition (1d/12, start, s4));
            sm.Add (new Transition (1d/12, start, s10));
            sm.Add (new Transition (1d/9, start, s5));
            sm.Add (new Transition (1d/9, start, s9));
            sm.Add (new Transition (5d/36, start, s6));
            sm.Add (new Transition (5d/36, start, s8));
            sm.Add (new Transition (1d/9, start, lost));
            
            sm.Add (new Transition (3d/4, s4, s4));
            sm.Add (new Transition (1d/12, s4, won));
            sm.Add (new Transition (1d/6, s4, lost));
            
            sm.Add (new Transition (3d/4, s10, s10));
            sm.Add (new Transition (1d/12, s10, won));
            sm.Add (new Transition (1d/6, s10, lost));
            
            sm.Add (new Transition (13d/18, s5, s5));
            sm.Add (new Transition (1d/9, s5, won));
            sm.Add (new Transition (1d/6, s5, lost));
            
            sm.Add (new Transition (13d/18, s9, s9));
            sm.Add (new Transition (1d/9, s9, won));
            sm.Add (new Transition (1d/6, s9, lost));
            
            sm.Add (new Transition (25d/36, s6, s6));
            sm.Add (new Transition (5d/36, s6, won));
            sm.Add (new Transition (1d/6, s6, lost));
            
            sm.Add (new Transition (25d/36, s8, s8));
            sm.Add (new Transition (5d/36, s8, won));
            sm.Add (new Transition (1d/6, s8, lost));
            
            sm.Add (new Transition (1d, won, won));
            sm.Add (new Transition (1d, lost, lost));
            
            return sm;
        }
    }
}

