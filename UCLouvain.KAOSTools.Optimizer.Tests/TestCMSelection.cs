using NUnit.Framework;
using System;
using UCLouvain.KAOSTools.Parsing;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Propagators.BDD;
using System.Linq;

namespace UCLouvain.KAOSTools.Optimizer.Tests
{
    [TestFixture ()]
    public class TestCMSelection
    {
        [Test ()]
        public void TestBestCountermeasureSelection ()
        {
            var input = @"declare goal [ anchor ] rsr .75 refinedby child1, child2 end
                          declare goal [ child1 ] obstructedby o1 end
                          declare goal [ child2 ] obstructedby o2 end
                          declare obstacle [ o1 ] probability .2 resolvedby [substitution:anchor] cm1 end
                          declare obstacle [ so1 ] probability .1 resolvedby [substitution:anchor] cm2 end
                          declare obstacle [ so2 ] probability .3 resolvedby [substitution:anchor] cm3 end
                          declare obstacle [ o2 ] refinedby so1 refinedby so2 end";
                          
            ModelBuilder parser = new ModelBuilder ();
            var model = parser.Parse (input);

        }
    }
}
