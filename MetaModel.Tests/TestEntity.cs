using System;
using System.Linq;
using NUnit.Framework;
using KAOSTools.Parsing;

namespace KAOSTools.Core.Tests
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
        }
    }
}

