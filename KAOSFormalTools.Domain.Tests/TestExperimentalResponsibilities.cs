using System;
using System.Linq;
using NUnit.Framework;
using KAOSFormalTools.Parsing;

namespace KAOSFormalTools.Domain.Tests
{
    [TestFixture()]
    public class Test
    {
        [Test()]
        public void TestCase ()
        {
            var input = @"
begin goal 
    id root
    refinedby child1, child2
end

begin goal 
    id child1
    assignedto myagent
end

begin goal 
    id child2
    assignedto myagent
end

begin agent
    id myagent
end
";
            var parser = new Parser ();
            var model = parser.Parse (input);

            var responsibilities = model.GoalModel.GetResponsibilities ();
            printresponsibilities2(responsibilities, 0);
        }
        
        [Test()]
        public void TestCase2 ()
        {
            var input = @"
begin goal 
    id root
    refinedby child1, child2
    refinedby child3
end

begin goal 
    id child1
    assignedto myagent
end

begin goal 
    id child2
    assignedto myagent
end

begin goal 
    id child3
    assignedto myagent
end

begin agent
    id myagent
end
";
            var parser = new Parser ();
            var model = parser.Parse (input);

            var responsibilities = model.GoalModel.GetResponsibilities ();
            printresponsibilities2(responsibilities, 0);
        }
        
        [Test()]
        public void TestCase3 ()
        {
            var input = @"
begin goal 
    id root
    refinedby child1, child2
    refinedby child3
end

begin goal 
    id child1
    assignedto myagent
end

begin goal 
    id child2
    assignedto myagent
end

begin goal 
    id child3
    refinedby child4, child5
end

begin goal 
    id child4
    assignedto myagent
end

begin goal 
    id child5
    assignedto myagent
end

begin agent
    id myagent
end
";
            var parser = new Parser ();
            var model = parser.Parse (input);

            var responsibilities = model.GoalModel.GetResponsibilities ();
            printresponsibilities2(responsibilities, 0);
        }
        
        [Test()]
        public void TestCase4 ()
        {
            var input = @"
begin goal 
    id root
    refinedby child1, child2
    refinedby child3, child2
end

begin goal 
    id child1
    assignedto myagent
end

begin goal 
    id child2
    assignedto myagent
end

begin goal 
    id child3
    refinedby child4, child5
end

begin goal 
    id child4
    assignedto myagent
end

begin goal 
    id child5
    assignedto myagent
end

begin agent
    id myagent
end
";
            var parser = new Parser ();
            var model = parser.Parse (input);

            var responsibilities = model.GoalModel.GetResponsibilities ();
            printresponsibilities2(responsibilities, 0);
        }
        
        [Test()]
        public void TestCase5 ()
        {
            var input = @"
begin goal 
    id root
    refinedby child1, child2
    refinedby child1, child2
    refinedby child3, child4
end

begin goal 
    id child1
    assignedto myagent
end

begin goal 
    id child2
    assignedto myagent
end

begin goal 
    id child3
    assignedto myagent
end

begin goal 
    id child4
    assignedto myagent
end

begin agent
    id myagent
end
";
            var parser = new Parser ();
            var model = parser.Parse (input);
            
            var responsibilities = model.GoalModel.GetResponsibilities ();
            printresponsibilities2(responsibilities, 0);
        }

        private void printresponsibilities2 (ResponsibilityNode r, int level) 
        {
            if (r.Responsibility.Count == 0) {
                Console.WriteLine (new String('-', level) + "empty");

            } else {
                Console.Write (new String('-', level) + " ");
                int i = 0;
                foreach (var kv in r.Responsibility) {
                    Console.Write (kv.Key.Identifier + " (" + kv.Value.Count + ")" + " : " + string.Join (",", kv.Value.Select(g => g.Identifier)));
                    if (i < r.Responsibility.Count - 1)
                        Console.Write ("\n" + new String(' ', level + 1));
                    i++;
                }
                Console.WriteLine ();
            }

            foreach (var child in r.children) {
                printresponsibilities2 (child, level+1);
            }
        }
    }
}

