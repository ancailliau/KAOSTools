using System;
using System.Linq;
using ShallTests;
using NUnit.Framework;
using LtlSharp;

namespace KAOSTools.MetaModel.Tests
{

    [TestFixture()]
    public class TestBehaviorModel
    {
        [Test()]
        public void TestInitialStates ()
        {
            var sm = TestHelpers.BuildTwoStatesSM ();

            sm.States.Select (x => x.Identifier).ShallOnlyContain (new string [] { "s1", "s2" });
            sm.InitialStates.Select (x => x.Identifier).ShallOnlyContain (new string [] { "s1" });
        }

        [Test()]
        public void TestTransitionMatrix ()
        {
            var sm = TestHelpers.BuildTwoStatesSM ();
            var matrix = sm.TransitionMatrix;

            matrix["s1"]["s1"].ShallEqual (0);
            matrix["s1"]["s2"].ShallEqual (1);
            matrix["s2"]["s1"].ShallEqual (1);
            matrix["s2"]["s2"].ShallEqual (0);
        }

        #region Test LTF (long-term fraction)
        
        [Test()]
        public void TestStableDistributionForSingleState ()
        {
            var sm = TestHelpers.BuildChain ();
            
            sm.LongTermFraction.Keys.ShallOnlyContain (new string [] { "s1", "s2" });
            sm.LongTermFraction["s1"].ShallEqual (0);
            sm.LongTermFraction["s2"].ShallEqual (1);
        }

        [Test()]
        public void TestStableDistributionForChain ()
        {
            var sm = TestHelpers.BuildSingleStateSM ();

            sm.LongTermFraction.Keys.ShallOnlyContain (new string [] { "s1" });
            sm.LongTermFraction["s1"].ShallEqual (1);
        }
        
        [Test()]
        public void TestStableDistributionForY ()
        {
            var sm = TestHelpers.BuildY ();
            
            sm.LongTermFraction.Keys.ShallOnlyContain (new string [] { "s1", "s2", "s3" });
            sm.LongTermFraction["s1"].ShallEqual (0);
            sm.LongTermFraction["s2"].ShallEqual (0.5);
            sm.LongTermFraction["s3"].ShallEqual (0.5);
        }

        [Test()]
        public void TestStableDistributionForTwoStates ()
        {
            var sm = TestHelpers.BuildTwoStatesSM ();
            
            sm.LongTermFraction.Keys.ShallOnlyContain (new string [] { "s1", "s2" });
            sm.LongTermFraction["s1"].ShallEqual (0.5);
            sm.LongTermFraction["s2"].ShallEqual (0.5);
        }
        
        [Test()]
        public void TestStableDistributionForTwoStatesWithSelfLoop ()
        {
            var sm = TestHelpers.BuildTwoStatesSMWithSelfLoop ();

            sm.LongTermFraction.Keys.ShallOnlyContain (new string [] { "s1", "s2" });
            sm.LongTermFraction["s1"].ShallEqual (0.666, 0.001);
            sm.LongTermFraction["s2"].ShallEqual (0.333, 0.001);
        }

        [Test()]
        public void TestStableDistributionForTwoThreeStates ()
        {
            var sm = TestHelpers.BuildThreeStatesSM ();
            
            sm.LongTermFraction.Keys.ShallOnlyContain (new string [] { "s1", "s2", "s3" });

            sm.LongTermFraction["s1"].ShallEqual (0.447, 0.001);
            sm.LongTermFraction["s2"].ShallEqual (0.253, 0.001);
            sm.LongTermFraction["s3"].ShallEqual (0.299, 0.001);
        }

        #endregion

    }

    [TestFixture()]
    public class TestProbabilityWithLTL {

        [Test()]
        public void TestNext ()
        {
            var sm = TestHelpers.BuildSingleStateSM ();
            
            sm.Probability (new Next(new Proposition("state_one"))).ShallEqual (1);
            sm.Probability (new Next(new Proposition("unknown_proposition"))).ShallEqual (0);
        }

        [Test()]
        public void TestNextTwoState ()
        {
            var sm = TestHelpers.BuildTwoStatesSM ();

            sm.Probability (new Next(new Proposition("state_one"))).ShallEqual (0.5);
            sm.Probability (new Next(new Proposition("state_two"))).ShallEqual (0.5);
        }
        
        [Test()]
        public void TestNextTwoStateWithLoop ()
        {
            var sm = TestHelpers.BuildTwoStatesSMWithSelfLoop ();
            
            sm.Probability ("X state_one").ShallEqual (0.666, 0.001);
            sm.Probability ("X state_two").ShallEqual (0.333, 0.001);
        }

        [Test()]
        public void TestImply ()
        {
            var sm = TestHelpers.BuildSingleStateSM ();
            
            sm.Probability ("state_one -> state_one").ShallEqual (1);
            sm.Probability ("state_one -> unknown").ShallEqual (0);

            sm.Probability ("unknown -> state_one").ShallEqual (1);
            sm.Probability ("unknown -> unknown").ShallEqual (1);
        }

        [Test()]
        public void TestImplyWithTwoStates ()
        {
            var sm = TestHelpers.BuildTwoStatesSM ();

            /*

P (s1, state_one) = 1
P (s1, X state_one) = 0
P (s1, state_one -> X state_one) = P (s1, state_one /\ X state_one | !state_one)
                                 = P (s1, state_one /\ X state_one) + P (s1, !state_one)
                                 = P (s1, state_one) * P (s1, X state_one) + P (s1, !state_one)
                                 = 1 * 0 + 0 
                                 = 0

P (s2, state_one) = 0
P (s2, X state_one) = 1
P (s2, state_one -> X state_one) = P (s2, state_one /\ X state_one | !state_one)
                                 = P (s2, state_one /\ X state_one) + P (s2, !state_one)
                                 = P (s2, state_one) * P (s2, X state_one) + P (s2, !state_one)
                                 = 0 * 1 + 1 
                                 = 1

P (s1) = P (s2) = 0.5

P (state_one -> X state_one) = P (s1, state_one -> X state_one) * P(s1)
                               + P (s2, state_one -> X state_one) * P(s2)
                             = 0 * .5 + 1 * .5 = .5

            */

            sm.Probability ("state_one -> X state_one").ShallEqual (.5);
            sm.Probability ("state_one -> X state_two").ShallEqual (1);
            
            sm.Probability ("state_two -> X state_one").ShallEqual (1);
            sm.Probability ("state_two -> X state_two").ShallEqual (.5);
        }

        [Test()]
        public void TestConjunctWithTwoStates ()
        {
            var sm = TestHelpers.BuildTwoStatesSM ();

            sm.Probability ("state_one & state_one").ShallEqual (.5);
            sm.Probability ("state_two & state_two").ShallEqual (.5);
            sm.Probability ("state_one & state_two").ShallEqual (0);
        }
        
        [Test()]
        public void TestDisjunctionWithTwoStates ()
        {
            var sm = TestHelpers.BuildTwoStatesSM ();
            
            sm.Probability ("state_one | state_two").ShallEqual (1);
        }

        [Test()]
        public void TestNegationWithTwoStates ()
        {
            var sm = TestHelpers.BuildTwoStatesSM ();
            
            sm.Probability ("!(state_one | state_two)").ShallEqual (0);
            sm.Probability ("!state_one & !state_two").ShallEqual (0);
            sm.Probability ("!state_one | !state_two").ShallEqual (1);
        }

        [Test()]
        public void TestEventually ()
        {
            var sm = TestHelpers.BuildSimpleCommunicationProtocol ();
            sm.Probability ("F delivered").ShallEqual (1, 0.0001);
        }
        
        [Test()]
        public void TestUntil ()
        {
            var sm = TestHelpers.BuildCrapsGame ();
            Console.WriteLine (sm.Satisfy (sm.States.Where (x => x.Identifier == "start").Single (), 
                        LtlSharp.Parser.Parse ("b U c")));
        }

        [Test()]
        public void TestMaintain ()
        {
            var sm = TestHelpers.BuildSimpleCommunicationProtocol ();
            sm.Probability ("G delivered").ShallEqual (0);
            sm.Probability ("G !unknown").ShallEqual (1, 0.0001);
        }
        
        [Test()]
        public void TestMaintainImply ()
        {
            var sm = TestHelpers.BuildSimpleCommunicationProtocol ();
            sm.Probability ("G (start -> F delivered)").ShallEqual (1, 0.0001);
        }

    }
}

