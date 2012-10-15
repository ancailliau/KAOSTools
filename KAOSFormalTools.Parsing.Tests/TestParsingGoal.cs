using System;
using System.Linq;
using NUnit.Framework;
using KAOSFormalTools.Parsing;
using LtlSharp;

namespace KAOSFormalTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingGoal
    {
        private static Parser parser = new Parser ();
       
        [Test()]
        public void TestComment ()
        {
            var input = @"
begin goal
# test
    id test
end
";
            var gm = parser.Parse (input);
            Assert.AreEqual (1, gm.RootGoals.Count);
            Assert.AreEqual ("test", gm.RootGoals.First().Identifier);
        }

        [Test()]
        public void TestGoalIdentifier ()
        {
            var input = @"
begin goal
    id test
end
";
            var gm = parser.Parse (input);
            Assert.AreEqual (1, gm.RootGoals.Count);
            Assert.AreEqual ("test", gm.RootGoals.First().Identifier);
        }

        [Test()]
        public void TestGoalMultipleIdentifier ()
        {
            var input = @"
begin goal
    id test
    id test2
end
";
            
            var gm = parser.Parse (input);
            Assert.AreEqual (1, gm.RootGoals.Count);
            Assert.AreEqual ("test2", gm.RootGoals.First().Identifier);
        }

        [Test()]
        public void TestGoalName ()
        {
            var input = @"
begin goal
    id test
    name ""My goal name""
end
";
            
            var gm = parser.Parse (input);
            var root = gm.RootGoals.First ();
            Assert.AreEqual ("My goal name", root.Name);
        }

        [Test()]
        public void TestFormalSpec ()
        {
            var input = @"
begin goal
    id          test
    name        ""My goal name""
    formalspec  ""G (incidentReported -> F ambulanceOnScene)""
end
";
            
            var gm = parser.Parse (input);
            var root = gm.RootGoals.First ();
            Assert.IsInstanceOf (typeof(Globally), root.FormalSpec);
        }

        [Test()]
        public void TestMultipleGoals ()
        {
            var input = @"
begin goal
    id test
    name ""My goal name""
end

begin goal
    id test2
    name ""My goal name2""
end
";
            var gm = parser.Parse (input);
            Assert.AreEqual (2, gm.RootGoals.Count);
            Assert.AreEqual ("test", gm.RootGoals.First().Identifier);
            Assert.AreEqual ("My goal name", gm.RootGoals.First().Name);
            
            Assert.AreEqual ("test2", gm.RootGoals[1].Identifier);
            Assert.AreEqual ("My goal name2", gm.RootGoals[1].Name);
        }
                
        [Test()]
        public void TestRefinement ()
        {
            var input = @"
begin goal
    refinedby  test2 , test3
    name       ""My goal name""
    
    id         test
end

begin goal id test2 end
begin goal id test3 end
";
            var gm = parser.Parse (input);
            Assert.AreEqual (1, gm.RootGoals.Count);

            var root = gm.RootGoals.First ();
            Assert.AreEqual ("test", root.Identifier);
            Assert.AreEqual (1, root.Refinements.Count);

            var refinement = root.Refinements.First ();
            var child = refinement.Children.First ();
            Assert.AreEqual ("test2", child.Identifier);
        }

        [Test()]
        public void TestRefinementInline ()
        {
            var input = @"
begin goal
    refinedby  begin goal id test2 end , begin goal id test3 end
    name       ""My goal name""
    
    id         test
end
";
            var gm = parser.Parse (input);
            Assert.AreEqual (1, gm.RootGoals.Count);

            var root = gm.RootGoals.First ();
            Assert.AreEqual ("test", root.Identifier);
            Assert.AreEqual (1, root.Refinements.Count);

            var refinement = root.Refinements.First ();
            var child = refinement.Children.First ();
            Assert.AreEqual ("test2", child.Identifier);
        }

        [Test()]
        public void TestRefinementInlineRecursive ()
        {
            var input = @"
begin goal
    refinedby begin goal 
                id test2
                refinedby begin goal 
                  id test3
                end
              end
end
";
            var gm = parser.Parse (input);
            Assert.AreEqual (1, gm.RootGoals.Count);

            var root = gm.RootGoals.First ();
            Assert.AreEqual (1, root.Refinements.Count);

            var refinement = root.Refinements.First ();
            var child = refinement.Children.First ();
            Assert.AreEqual ("test2", child.Identifier);

            Assert.AreEqual (1, child.Refinements.Count);
            Assert.AreEqual ("test3", child.Refinements[0].Children[0].Identifier);
        }

        [Test()]
        public void TestUnknownIdentifierReference ()
        {
            var input = @"
begin goal
    refinedby  test2
    name       ""My goal name""
    
    id         test
end
";
            var gm = parser.Parse (input);
            Assert.AreEqual (2, gm.Goals.Count);
            
            var root = gm.RootGoals.First ();
            Assert.AreEqual (1, root.Refinements.Count);
            Assert.AreEqual ("test2", root.Refinements[0].Children[0].Identifier);
        }
        
        [Test()]
        public void TestUnknownNameReference ()
        {
            var input = @"
begin goal
    refinedby  ""test2""
    name       ""My goal name""
    
    id         test
end
";
            var gm = parser.Parse (input);
            Assert.AreEqual (2, gm.Goals.Count);
            
            var root = gm.RootGoals.First ();
            Assert.AreEqual (1, root.Refinements.Count);
            Assert.AreEqual ("test2", root.Refinements[0].Children[0].Name);
        }

        
        [Test()]
        public void TestNameWithoutId ()
        {
            var input = @"
begin goal
    name       ""My goal""
end
";
            var gm = parser.Parse (input);
            Assert.AreEqual (1, gm.RootGoals.Count);

            var root = gm.RootGoals.First ();
            Assert.AreEqual ("My goal", root.Name);
        }

           
        [Test()]
        public void TestRefinementByName ()
        {
            var input = @"
begin goal
    refinedby  ""test2"" , ""test3""
    name       ""My goal""
end

begin goal name ""test2"" end
begin goal name ""test3"" end
";
            var gm = parser.Parse (input);
            Assert.AreEqual (1, gm.RootGoals.Count);

            var root = gm.RootGoals.First ();
            Assert.AreEqual (1, root.Refinements.Count);

            var refinement = root.Refinements.First ();
            Assert.AreEqual ("test2", refinement.Children[0].Name);
            Assert.AreEqual ("test3", refinement.Children[1].Name);
        }
    }

}

