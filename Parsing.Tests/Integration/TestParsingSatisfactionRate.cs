using System;
using System.Linq;
using NUnit.Framework;
using UCLouvain.KAOSTools.Parsing;
using System.Collections.Generic;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Parsing.Parsers;
using UCLouvain.KAOSTools.Core.SatisfactionRates;

namespace UCLouvain.KAOSTools.Parsing.Tests
{
    [TestFixture()]
	public class TestParsingSatisfactionRate
    {
        static ModelBuilder parser = new ModelBuilder ();

        [TestCase(@"declare obstacle [ test ] probability 0.95 end", 0.95)]
        [TestCase(@"declare obstacle [ test ] probability 1    end", 1)]
        [TestCase(@"declare obstacle [ test ] probability 0    end", 0)]
        [TestCase(@"declare obstacle [ test ] probability .01  end", .01)]
        [TestCase(@"declare obstacle [ test ] esr 0.95 end", 0.95)]
        [TestCase(@"declare obstacle [ test ] esr 1    end", 1)]
        [TestCase(@"declare obstacle [ test ] esr 0    end", 0)]
        [TestCase(@"declare obstacle [ test ] esr .01  end", .01)]
        public void TestProbability (string input, double expected)
        {
            var model = parser.Parse (input);

            var sr = model.satisfactionRateRepository.GetObstacleSatisfactionRate ("test");
            Assert.IsInstanceOf(typeof(DoubleSatisfactionRate), sr);
            Assert.AreEqual(expected, ((DoubleSatisfactionRate)sr).SatisfactionRate);
        }

		[TestCase(@"declare obstacle [ test ]
                        probability .01
                        probability .02
                    end", new double[] { .01, .02 })]
		public void TestMultipleProbabilities(string input, double[] expected)
		{
			var model = parser.Parse(input);

			var srs = model.satisfactionRateRepository.GetObstacleSatisfactionRates("test");
            foreach (var sr in srs) {
                Assert.IsInstanceOf(typeof(DoubleSatisfactionRate), sr);
                Assert.Contains(((DoubleSatisfactionRate)sr).SatisfactionRate, expected);
            }
		}

        [Test()]
		public void TestExpertProbabilities()
		{
            var input = 
                @"declare obstacle [ test ]
                    probability [ expert1 ] .01
                    probability [ expert2 ] .02
                  end
                  declare expert [ expert1 ] end
                  declare expert [ expert2 ] end";

            var expected = new Tuple<string, double>[] {
                new Tuple<string, double> ("expert1", .01),
                new Tuple<string, double> ("expert2", .02) };
            
			var model = parser.Parse(input);

			var srs = model.satisfactionRateRepository.GetObstacleSatisfactionRates("test");
			foreach (var sr in srs) {
				Assert.IsInstanceOf(typeof(DoubleSatisfactionRate), sr);

                var dsr = (DoubleSatisfactionRate)sr;
                var expected_item = expected.Where(t => t.Item1 == dsr.ExpertIdentifier && t.Item2 == dsr.SatisfactionRate);
			}
		}

		[TestCase(@"declare obstacle [ test ] probability uniform[.5,.8] end", new double[] { .5, .8 })]
		[TestCase(@"declare obstacle [ test ] probability uniform[0.5,0.8] end", new double[] { .5, .8 })]
		[TestCase(@"declare obstacle [ test ] probability uniform[50%,80%] end", new double[] { .5, .8 })]
        [TestCase(@"declare obstacle [ test ] probability uniform[0,1] end", new double[] { 0, 1 })]
        public void TestUniformSatisfactionRate(string input, double[] bounds)
		{
			var model = parser.Parse(input);

			var sr = model.satisfactionRateRepository.GetObstacleSatisfactionRate("test");
            Assert.IsInstanceOf(typeof(UniformSatisfactionRate), sr);
			Assert.AreEqual(bounds[0], ((UniformSatisfactionRate)sr).LowerBound);
			Assert.AreEqual(bounds[1], ((UniformSatisfactionRate)sr).UpperBound);
		}

		[TestCase(@"declare obstacle [ test ] probability triangular[.3,.5,.8] end", new double[] { .3, .5, .8 })]
		[TestCase(@"declare obstacle [ test ] probability triangular[0.3,0.5,0.8] end", new double[] { .3, .5, .8 })]
		[TestCase(@"declare obstacle [ test ] probability triangular[30%,50%,80%] end", new double[] { .3, .5, .8 })]
		[TestCase(@"declare obstacle [ test ] probability triangular[0,0,1] end", new double[] { 0, 0, 1 })]
		public void TestTriangularSatisfactionRate(string input, double[] bounds)
		{
			var model = parser.Parse(input);

			var sr = model.satisfactionRateRepository.GetObstacleSatisfactionRate("test");
            Assert.IsInstanceOf(typeof(TriangularSatisfactionRate), sr);
            Assert.AreEqual(bounds[0], ((TriangularSatisfactionRate)sr).Min);
            Assert.AreEqual(bounds[1], ((TriangularSatisfactionRate)sr).Mode);
            Assert.AreEqual(bounds[2], ((TriangularSatisfactionRate)sr).Max);
		}

		[TestCase(@"declare obstacle [ test ] probability pert[.3,.5,.8] end", new double[] { .3, .5, .8 })]
		[TestCase(@"declare obstacle [ test ] probability pert[0.3,0.5,0.8] end", new double[] { .3, .5, .8 })]
		[TestCase(@"declare obstacle [ test ] probability pert[30%,50%,80%] end", new double[] { .3, .5, .8 })]
		[TestCase(@"declare obstacle [ test ] probability pert[0,0,1] end", new double[] { 0, 0, 1 })]
		public void TestPERTSatisfactionRate(string input, double[] bounds)
		{
			var model = parser.Parse(input);

			var sr = model.satisfactionRateRepository.GetObstacleSatisfactionRate("test");
            Assert.IsInstanceOf(typeof(PERTSatisfactionRate), sr);
			Assert.AreEqual(bounds[0], ((PERTSatisfactionRate)sr).Min);
			Assert.AreEqual(bounds[1], ((PERTSatisfactionRate)sr).Mode);
			Assert.AreEqual(bounds[2], ((PERTSatisfactionRate)sr).Max);
		}

		[TestCase(@"declare obstacle [ test ] probability beta[3,5] end", new double[] { 3, 5 })]
		public void TestBetaSatisfactionRate(string input, double[] bounds)
		{
			var model = parser.Parse(input);

			var sr = model.satisfactionRateRepository.GetObstacleSatisfactionRate("test");
            Assert.IsInstanceOf(typeof(BetaSatisfactionRate), sr);
            Assert.AreEqual(bounds[0], ((BetaSatisfactionRate)sr).Alpha);
            Assert.AreEqual(bounds[1], ((BetaSatisfactionRate)sr).Beta);
		}
    }
}

