using System;
using System.Linq;
using NUnit.Framework;
using ShallTests;

namespace KAOSFormalTools.Parsing.Tests
{
    [TestFixture()]
    public class TestParsingStateMachines
    {
        private static Parser parser = new Parser ();

        [TestCase(@"begin statemachine
                        state1 -> (.90) state2
                        state1 -> (.10) state3
                    end")]
        public void TestSimpleStateMachine (string input)
        {
            var model = parser.Parse (input);
        }
    }
}

