using System;
using System.Linq;
using NUnit.Framework;
using KAOSFormalTools.Parsing;
using LtlSharp;

namespace KAOSFormalTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingObstacle
    {
        private static Parser parser = new Parser ();
        
        [Test()]
        public void TestMissingIdentifier ()
        {
            var input = @"begin obstacle end";

            Assert.Throws (typeof(KAOSFormalTools.Parsing.ParsingException), () => {
                parser.Parse (input);
            });
        }

        [Test()]
        public void TestIdentifier ()
        {
            var input = @"
begin obstacle
    id test
end
";
            var gm = parser.Parse (input);
            Assert.AreEqual (1, gm.Obstacles.Count);
            Assert.AreEqual ("test", gm.Obstacles.First().Identifier);
        }

        [Test()]
        public void TestMultipleIdentifier ()
        {
            var input = @"
begin obstacle
    id test
    id test2
end
";
            
            var gm = parser.Parse (input);
            Assert.AreEqual (1, gm.Obstacles.Count);
            Assert.AreEqual ("test2", gm.Obstacles.First().Identifier);
        }

        [Test()]
        public void TestName ()
        {
            var input = @"
begin obstacle
    id test
    name ""My obstacle""
end
";
            
            var gm = parser.Parse (input);
            var root = gm.Obstacles.First ();
            Assert.AreEqual ("My obstacle", root.Name);
        }

        [Test()]
        public void TestFormalSpec ()
        {
            var input = @"
begin obstacle
    id          test
    name        ""My obstacle""
    formalspec  ""G (incidentReported -> F ambulanceOnScene)""
end
";
            
            var gm = parser.Parse (input);
            var root = gm.Obstacles.First ();
            Assert.IsInstanceOf (typeof(Globally), root.FormalSpec);
        }

        [Test()]
        public void TestMultipleObstacle ()
        {
            var input = @"
begin obstacle
    id test
end

begin obstacle
    id test2
end
";
            var gm = parser.Parse (input);
            Assert.AreEqual (2, gm.Obstacles.Count);
        }
                   
        [Test()]
        public void TestRefinement ()
        {
            var input = @"
begin obstacle
    id         test
    refinedby  test2, test3
end

begin obstacle 
    id test2 
end

begin obstacle 
    id test3
end
";
            var gm = parser.Parse (input);
            Assert.AreEqual (3, gm.Obstacles.Count);

            var root = gm.Obstacles.First ();
            Assert.AreEqual ("test", root.Identifier);
            Assert.AreEqual (1, root.Refinements.Count);

            var refinement = root.Refinements.First ();
            Assert.AreEqual ("test2", refinement.Children[0].Identifier);
            Assert.AreEqual ("test3", refinement.Children[1].Identifier);
        }

        [Test()]
        public void TestMultipleRefinement ()
        {
            var input = @"
begin obstacle
    id         test
    refinedby  test2
    refinedby  test3
end

begin obstacle 
    id test2 
end

begin obstacle 
    id test3
end
";
            var gm = parser.Parse (input);
            Assert.AreEqual (3, gm.Obstacles.Count);

            var root = gm.Obstacles.First ();
            Assert.AreEqual ("test", root.Identifier);
            Assert.AreEqual (2, root.Refinements.Count);

            Assert.AreEqual ("test2", root.Refinements[0].Children[0].Identifier);
            Assert.AreEqual ("test3", root.Refinements[1].Children[0].Identifier);
        }

        [Test()]
        public void TestObstruction ()
        {
            var input = @"
begin goal
    obstructedby test2, test3
    name         ""My obstacle""
    
    id         test
end

begin obstacle id test2 end
begin obstacle id test3 end
";
            var gm = parser.Parse (input);
            Assert.AreEqual (1, gm.RootGoals.Count);
            Assert.AreEqual (2, gm.Obstacles.Count);

            var root = gm.RootGoals.First ();
            Assert.AreEqual (2, root.Obstruction.Count);
            Assert.AreEqual ("test2", root.Obstruction[0].Identifier);
            Assert.AreEqual ("test3", root.Obstruction[1].Identifier);
        }

        [Test()]
        public void TestUnknownIdentifierReference ()
        {
            var input = @"
begin goal
    obstructedby  test2
    name          ""My goal""
    
    id         test
end
";
            var gm = parser.Parse (input);
            Assert.AreEqual (1, gm.Goals.Count);
            Assert.AreEqual (1, gm.Obstacles.Count);

            var root = gm.RootGoals.First ();
            Assert.AreEqual (1, root.Obstruction.Count);
            Assert.AreEqual ("test2", root.Obstruction[0].Identifier);
        }
        
        [Test()]
        public void TestUnknownNameReference ()
        {
            var input = @"
begin goal
    obstructedby  ""test2""
    name          ""My goal""
    
    id         test
end
";
            var gm = parser.Parse (input);
            Assert.AreEqual (1, gm.Goals.Count);
            Assert.AreEqual (1, gm.Obstacles.Count);

            var root = gm.RootGoals.First ();
            Assert.AreEqual (1, root.Obstruction.Count);
            Assert.AreEqual ("test2", root.Obstruction[0].Name);
        }
    }

}

