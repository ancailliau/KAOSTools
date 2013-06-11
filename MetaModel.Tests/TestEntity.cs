using System;
using System.Linq;
using NUnit.Framework;
using KAOSTools.Parsing;

namespace KAOSTools.MetaModel.Tests
{
    [TestFixture()]
    public class TestEntity
    {
        [Test()]
        public void TestCase ()
        {
            var input = @"
declare entity
    id parent
end

declare entity
    id child_1
    is parent
end

declare entity
    id child_2
    is parent
end
";
            var parser = new ModelBuilder ();
            var model = parser.Parse (input);
            
            var e = model.Entities.Single(x => x.Identifier == "child_1");
            Console.WriteLine (string.Join(",", e.Ancestors.Select(x => x.FriendlyName)));
        }
    }
}

