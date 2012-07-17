﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.8.1.0
//      SpecFlow Generator Version:1.8.0.0
//      Runtime Version:2.0.50727.1433
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Beaver.Tests.Features
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.8.1.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Elements")]
    public partial class ElementsFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        #line 1 "elements.feature"
        #line hidden
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Elements", "As a modeler,\nI want to model elements\nSo that I ...", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.TestFixtureTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 6
  #line 7
    testRunner.Given("a model");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Add an element in model")]
        [NUnit.Framework.TestCaseAttribute("a goal", "goal-1", new string[0])]
        [NUnit.Framework.TestCaseAttribute("a domain property", "domprop-1", new string[0])]
        [NUnit.Framework.TestCaseAttribute("an obstacle", "obstacle-1", new string[0])]
        [NUnit.Framework.TestCaseAttribute("an agent", "agent-1", new string[0])]
        public virtual void AddAnElementInModel(string type, string name, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Add an element in model", exampleTags);
#line 9
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 10
    testRunner.When(string.Format("I add {0} '{1}'", type, name));
#line 11
    testRunner.Then(string.Format("the model contains '{0}'", name));
#line 12
    testRunner.And(string.Format("'{0}' is {1}", name, type));
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Remove element from model")]
        [NUnit.Framework.TestCaseAttribute("a goal", "goal-1", new string[0])]
        [NUnit.Framework.TestCaseAttribute("a domain property", "domprop-1", new string[0])]
        [NUnit.Framework.TestCaseAttribute("an obstacle", "obstacle-1", new string[0])]
        [NUnit.Framework.TestCaseAttribute("an agent", "agent-1", new string[0])]
        public virtual void RemoveElementFromModel(string type, string name, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Remove element from model", exampleTags);
#line 21
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 22
    testRunner.Given(string.Format("{0} '{1}'", type, name));
#line 23
    testRunner.When(string.Format("I remove '{0}'", name));
#line 24
    testRunner.Then(string.Format("the model does not contains '{0}'", name));
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
