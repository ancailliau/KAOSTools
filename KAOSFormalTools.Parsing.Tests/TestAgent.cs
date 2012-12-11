using System;
using System.Linq;
using NUnit.Framework;

namespace KAOSFormalTools.Parsing.Tests
{
    [TestFixture()]
    public class TestAgent
    {
        private static Parser parser = new Parser ();

        [Test()]
        public void TestSoftwareAgent ()
        {
            var input = @"
begin software agent
    id test
end
";
            var gm = parser.Parse (input);
            Assert.AreEqual (1, gm.Agents.Count);
            Assert.AreEqual ("test", gm.Agents.First().Identifier);
            Assert.IsTrue (gm.Agents.First().Software);
        }

        [Test()]
        public void TestIdentifier ()
        {
            var input = @"
begin agent
    id test
end
";
            var gm = parser.Parse (input);
            Assert.AreEqual (1, gm.Agents.Count);
            Assert.AreEqual ("test", gm.Agents.First().Identifier);
        }

        [Test()]
        public void TestName ()
        {
            var input = @"
begin agent
    name ""test""
end
";
            var gm = parser.Parse (input);
            Assert.AreEqual (1, gm.Agents.Count);
            Assert.AreEqual ("test", gm.Agents.First().Name);
        }
        
        [Test()]
        public void TestDescription ()
        {
            var input = @"
begin agent
    name ""test""
    description ""My description""
end
";
            var gm = parser.Parse (input);
            Assert.AreEqual (1, gm.Agents.Count);
            Assert.AreEqual ("My description", gm.Agents.First().Description);
        }


        [Test()]
        public void TestAssignedTo ()
        {
            var input = @"
begin agent
    name ""test""
end

begin goal
    name ""My goal""
    assignedto ""test""
end
";
            var gm = parser.Parse (input);
            Assert.AreEqual (1, gm.Agents.Count);
            Assert.AreEqual (1, gm.RootGoals.Count);
            
            Assert.AreEqual (1, gm.RootGoals.First ().AssignedAgents.Count);
            Assert.AreEqual ("test", gm.RootGoals.First ().AssignedAgents.First ().Name);
        }

        [Test()]
        public void TestAssignedToInline ()
        {
            var input = @"
begin goal
    name ""My goal""
    assignedto begin agent
                 name ""test""
               end
end
";
            var gm = parser.Parse (input);
            Assert.AreEqual (1, gm.Agents.Count);
            Assert.AreEqual (1, gm.RootGoals.Count);
            
            Assert.AreEqual (1, gm.RootGoals.First ().AssignedAgents.Count);
            Assert.AreEqual ("test", gm.RootGoals.First ().AssignedAgents.First ().Name);
        }

        [Test()]
        public void TestAssignedToMultipleAgents ()
        {
            var input = @"
begin agent
    name ""test""
end

begin agent
    name ""test2""
end

begin goal
    name ""My goal""
    assignedto ""test"", ""test2""
end
";
            var gm = parser.Parse (input);
            Assert.AreEqual (2, gm.RootGoals.First ().AssignedAgents.Count);

            gm.RootGoals
                .SelectMany (x => x.AssignedAgents)
                .Select (a => a.Name)
                .ShallContain ("test");
            
            gm.RootGoals
                .SelectMany (x => x.AssignedAgents)
                .Select (a => a.Name)
                .ShallContain ("test2");

        }
    }
}

