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
    [NUnit.Framework.DescriptionAttribute("Usability for modeling")]
    public partial class UsabilityForModelingFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        #line 1 "easyusage.feature"
        #line hidden
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Usability for modeling", "As a modeler,\nI want to easily connect elements each other\nSo that I do not spend time in wasteful clicks", ProgrammingLanguage.CSharp, ((string[])(null)));
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
        [NUnit.Framework.DescriptionAttribute("Connect a goal and a goal")]
        public virtual void ConnectAGoalAndAGoal()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect a goal and a goal", ((string[])(null)));
#line 11
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 12
    testRunner.Given("a goal 'goal-1'");
#line 13
    testRunner.And("a goal 'goal-2'");
#line 14
    testRunner.When("I connect 'goal-1' to 'goal-2'");
#line 15
    testRunner.Then("there exists a refinement for 'goal-1'");
#line 16
    testRunner.And("this refinement contains 'goal-2'");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect a goal and a domain property")]
        public virtual void ConnectAGoalAndADomainProperty()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect a goal and a domain property", ((string[])(null)));
#line 20
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 21
    testRunner.Given("a goal 'goal-1'");
#line 22
    testRunner.And("a domain property 'domprop-1'");
#line 23
    testRunner.When("I connect 'goal-1' to 'domprop-1'");
#line 24
    testRunner.Then("nothing change");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect a goal and an obstacle")]
        public virtual void ConnectAGoalAndAnObstacle()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect a goal and an obstacle", ((string[])(null)));
#line 28
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 29
    testRunner.Given("a goal 'goal-1'");
#line 30
    testRunner.And("an obstacle 'obstacle-1'");
#line 31
    testRunner.When("I connect 'goal-1' to 'obstacle-1'");
#line 32
    testRunner.Then("'obstacle-1' obstructs 'goal-1'");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect a goal and an agent")]
        public virtual void ConnectAGoalAndAnAgent()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect a goal and an agent", ((string[])(null)));
#line 36
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 37
    testRunner.Given("a goal 'goal-1'");
#line 38
    testRunner.And("an agent 'agent-1'");
#line 39
    testRunner.When("I connect 'goal-1' to 'agent-1'");
#line 40
    testRunner.Then("'agent-1' is assigned to 'goal-1'");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect a goal and a refinement")]
        public virtual void ConnectAGoalAndARefinement()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect a goal and a refinement", ((string[])(null)));
#line 44
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 45
    testRunner.Given("a goal 'goal-1'");
#line 46
    testRunner.And("a refinement 'refinement-1'");
#line 47
    testRunner.When("I connect 'goal-1' to 'refinement-1'");
#line 48
    testRunner.Then("refinement 'refinement-1' contains 'goal-2'");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect a goal and an obstacle refinement")]
        public virtual void ConnectAGoalAndAnObstacleRefinement()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect a goal and an obstacle refinement", ((string[])(null)));
#line 52
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 53
    testRunner.Given("a goal 'goal-1'");
#line 54
    testRunner.And("an obstacle refinement 'refinement-1'");
#line 55
    testRunner.When("I connect 'goal-1' to 'refinement-1'");
#line 56
    testRunner.Then("nothing change");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect a domain property and a goal")]
        public virtual void ConnectADomainPropertyAndAGoal()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect a domain property and a goal", ((string[])(null)));
#line 60
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 61
    testRunner.Given("a domain property 'domprop-1'");
#line 62
    testRunner.And("a goal 'goal-1'");
#line 63
    testRunner.When("I connect 'domprop-1' to 'goal-1'");
#line 64
    testRunner.Then("there exists a refinement for 'goal-1'");
#line 65
    testRunner.And("this refinement contains 'domprop-1'");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect a domain property and a domain property")]
        public virtual void ConnectADomainPropertyAndADomainProperty()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect a domain property and a domain property", ((string[])(null)));
#line 69
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 70
    testRunner.Given("a domain property 'domprop-1'");
#line 71
    testRunner.And("a domain property 'domprop-2'");
#line 72
    testRunner.When("I connect 'domprop-1' to 'domprop-2'");
#line 73
    testRunner.Then("nothing change");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect a domain property and an obstacle")]
        public virtual void ConnectADomainPropertyAndAnObstacle()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect a domain property and an obstacle", ((string[])(null)));
#line 77
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 78
    testRunner.Given("a domain property 'domprop-1'");
#line 79
    testRunner.And("an obstacle 'obstacle-1'");
#line 80
    testRunner.When("I connect 'domprop-1' to 'obstacle-1'");
#line 81
    testRunner.Then("nothing change");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect a domain property and an agent")]
        public virtual void ConnectADomainPropertyAndAnAgent()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect a domain property and an agent", ((string[])(null)));
#line 85
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 86
    testRunner.Given("a domain property 'domprop-1'");
#line 87
    testRunner.And("an agent 'agent-1'");
#line 88
    testRunner.When("I connect 'domprop-1' to 'agent-1'");
#line 89
    testRunner.Then("nothing change");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect a domain property and a refinement")]
        public virtual void ConnectADomainPropertyAndARefinement()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect a domain property and a refinement", ((string[])(null)));
#line 93
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 94
    testRunner.Given("a domain property 'domprop-1'");
#line 95
    testRunner.And("a refinement 'refinement-1'");
#line 96
    testRunner.When("I connect 'domprop-1' to 'refinement-1'");
#line 97
    testRunner.Then("refinement 'refinement-1' contains 'domprop-1'");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect a domain property and an obstacle refinement")]
        public virtual void ConnectADomainPropertyAndAnObstacleRefinement()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect a domain property and an obstacle refinement", ((string[])(null)));
#line 101
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 102
    testRunner.Given("a domain property 'domprop-1'");
#line 103
    testRunner.And("an obstacle refinement 'refinement-1'");
#line 104
    testRunner.When("I connect 'domprop-1' to 'refinement-1'");
#line 105
    testRunner.Then("refinement 'refinement-1' contains 'domprop-1'");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect an obstacle and a goal")]
        public virtual void ConnectAnObstacleAndAGoal()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect an obstacle and a goal", ((string[])(null)));
#line 109
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 110
    testRunner.Given("an obstacle 'obstacle-1'");
#line 111
    testRunner.And("a goal 'goal-1'");
#line 112
    testRunner.When("I connect 'obstacle-1' to 'goal-1'");
#line 113
    testRunner.Then("'obstacle-1' obstructs 'goal-1'");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect an obstacle and a domain property")]
        public virtual void ConnectAnObstacleAndADomainProperty()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect an obstacle and a domain property", ((string[])(null)));
#line 117
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 118
    testRunner.Given("an obstacle 'obstacle-1'");
#line 119
    testRunner.And("a domain property 'domprop-1'");
#line 120
    testRunner.When("I connect 'obstacle-1' to 'goal-1'");
#line 121
    testRunner.Then("nothing change");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect an obstacle and an obstacle")]
        public virtual void ConnectAnObstacleAndAnObstacle()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect an obstacle and an obstacle", ((string[])(null)));
#line 125
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 126
    testRunner.Given("an obstacle 'obstacle-1'");
#line 127
    testRunner.And("an obstacle 'obstacle-1'");
#line 128
    testRunner.When("I connect 'obstacle-1' to 'obstacle-1'");
#line 129
    testRunner.Then("a refinement for 'obstacle-1' exists");
#line 130
    testRunner.And("this refinement contains 'obstacle-2'");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect an obstacle and a agent")]
        public virtual void ConnectAnObstacleAndAAgent()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect an obstacle and a agent", ((string[])(null)));
#line 134
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 135
    testRunner.Given("an obstacle 'obstacle-1'");
#line 136
    testRunner.And("a agent 'agent-1'");
#line 137
    testRunner.When("I connect 'obstacle-1' to 'agent-1'");
#line 138
    testRunner.Then("nothing change");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect an obstacle and a refinement")]
        public virtual void ConnectAnObstacleAndARefinement()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect an obstacle and a refinement", ((string[])(null)));
#line 142
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 143
    testRunner.Given("an obstacle 'obstacle-1'");
#line 144
    testRunner.And("a refinement 'refinement-1'");
#line 145
    testRunner.When("I connect 'obstacle-1' to 'refinement-1'");
#line 146
    testRunner.Then("nothing change");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect an obstacle and an obstacle refinement")]
        public virtual void ConnectAnObstacleAndAnObstacleRefinement()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect an obstacle and an obstacle refinement", ((string[])(null)));
#line 150
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 151
    testRunner.Given("an obstacle 'obstacle-1'");
#line 152
    testRunner.And("an obstacle refinement 'refinement-1'");
#line 153
    testRunner.When("I connect 'obstacle-1' to 'refinement-1'");
#line 154
    testRunner.Then("refinement 'refinement-1' contains 'obstacle-1'");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect an agent and a goal")]
        public virtual void ConnectAnAgentAndAGoal()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect an agent and a goal", ((string[])(null)));
#line 158
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 159
    testRunner.Given("an agent 'agent-1'");
#line 160
    testRunner.And("a goal 'goal-1'");
#line 161
    testRunner.When("I connect 'agent-1' to 'goal-1'");
#line 162
    testRunner.Then("'agent-1' is assigned to 'goal-1'");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect an agent and not a goal")]
        [NUnit.Framework.TestCaseAttribute("a domain property", "domprop-1", new string[0])]
        [NUnit.Framework.TestCaseAttribute("an agent", "agent-2", new string[0])]
        [NUnit.Framework.TestCaseAttribute("a refinement", "refinement-1", new string[0])]
        [NUnit.Framework.TestCaseAttribute("an obstacle refinement", "refinement-1", new string[0])]
        public virtual void ConnectAnAgentAndNotAGoal(string type, string name, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect an agent and not a goal", exampleTags);
#line 170
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 171
    testRunner.Given("an agent 'agent-1'");
#line 172
    testRunner.And(string.Format("{0} '{1}'", type, name));
#line 173
    testRunner.When(string.Format("I connect 'agent-1' to '{0}'", name));
#line 174
    testRunner.Then("nothing change");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect a refinement and a goal")]
        public virtual void ConnectARefinementAndAGoal()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect a refinement and a goal", ((string[])(null)));
#line 185
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 186
    testRunner.Given("a refinement 'refinement-1'");
#line 187
    testRunner.And("a goal 'goal-1'");
#line 188
    testRunner.When("I connect 'refinement-1' to 'goal-1'");
#line 189
    testRunner.Then("'refinement-1' refines 'goal-1'");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect a refinement and a refinement of same goal")]
        public virtual void ConnectARefinementAndARefinementOfSameGoal()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect a refinement and a refinement of same goal", ((string[])(null)));
#line 193
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 194
    testRunner.Given("a goal 'goal-1'");
#line 195
    testRunner.And("a goal 'goal-2'");
#line 196
    testRunner.And("a goal 'goal-3'");
#line 197
    testRunner.And("a refinement 'refinement-1' for 'goal-1'");
#line 198
    testRunner.And("a refinement 'refinement-2' for 'goal-1'");
#line 199
    testRunner.And("refinement 'refinement-1' contains 'goal-2'");
#line 200
    testRunner.And("refinement 'refinement-2' contains 'goal-3'");
#line 201
    testRunner.When("I connect 'refinement-1' to 'refinement-2'");
#line 202
    testRunner.Then("there exists a refinement for 'goal-1'");
#line 203
    testRunner.And("this refinement contains 'goal-1'");
#line 204
    testRunner.And("this refinement contains 'goal-2'");
#line 205
    testRunner.And("'refinement-1' no longer exists");
#line 206
    testRunner.And("'refinement-2' no longer exists");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect a refinement and a refinement of a different goal")]
        public virtual void ConnectARefinementAndARefinementOfADifferentGoal()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect a refinement and a refinement of a different goal", ((string[])(null)));
#line 210
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 211
    testRunner.Given("a goal 'goal-1'");
#line 212
    testRunner.And("a goal 'goal-2'");
#line 213
    testRunner.And("a refinement 'refinement-1' for 'goal-1'");
#line 214
    testRunner.And("a refinement 'refinement-2' for 'goal-2'");
#line 215
    testRunner.When("I connect 'refinement-1' to 'refinement-2'");
#line 216
    testRunner.Then("nothing change");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect a refinement and not a goal nor a refinement")]
        [NUnit.Framework.TestCaseAttribute("a domain property", "domprop-1", new string[0])]
        [NUnit.Framework.TestCaseAttribute("an obstacle", "obstacle-2", new string[0])]
        [NUnit.Framework.TestCaseAttribute("an agent", "agent-2", new string[0])]
        [NUnit.Framework.TestCaseAttribute("an obstacle refinement", "refinement-1", new string[0])]
        public virtual void ConnectARefinementAndNotAGoalNorARefinement(string type, string name, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect a refinement and not a goal nor a refinement", exampleTags);
#line 223
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 224
    testRunner.Given("a refinement 'refinement-1'");
#line 225
    testRunner.And(string.Format("{0} '{1}'", type, name));
#line 226
    testRunner.When(string.Format("I connect 'refinement-1' to '{0}'", name));
#line 227
    testRunner.Then("nothing change");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect an obstacle refinement and an obstacle")]
        public virtual void ConnectAnObstacleRefinementAndAnObstacle()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect an obstacle refinement and an obstacle", ((string[])(null)));
#line 238
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 239
    testRunner.Given("an obstacle refinement 'refinement-1'");
#line 240
    testRunner.And("an obstacle 'obstacle-1'");
#line 241
    testRunner.When("I connect 'refinement-1' to 'obstacle-1'");
#line 242
    testRunner.Then("'refinement-1' refines 'obstacle-1'");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect an obstacle refinement and not an obstacle nor an obstacle refinement")]
        [NUnit.Framework.TestCaseAttribute("a goal", "goal-1", new string[0])]
        [NUnit.Framework.TestCaseAttribute("a domain property", "domprop-1", new string[0])]
        [NUnit.Framework.TestCaseAttribute("an agent", "agent-2", new string[0])]
        [NUnit.Framework.TestCaseAttribute("an refinement", "refinement-1", new string[0])]
        public virtual void ConnectAnObstacleRefinementAndNotAnObstacleNorAnObstacleRefinement(string type, string name, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect an obstacle refinement and not an obstacle nor an obstacle refinement", exampleTags);
#line 249
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 250
    testRunner.Given("an obstacle refinement 'refinement-1'");
#line 251
    testRunner.And(string.Format("{0} '{1}'", type, name));
#line 252
    testRunner.When(string.Format("I connect 'refinement-1' to '{0}'", name));
#line 253
    testRunner.Then("nothing change");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect an obstacle refinement and an obstacle refinement of same obstacle")]
        public virtual void ConnectAnObstacleRefinementAndAnObstacleRefinementOfSameObstacle()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect an obstacle refinement and an obstacle refinement of same obstacle", ((string[])(null)));
#line 264
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 265
    testRunner.Given("an obstacle 'obstacle-1'");
#line 266
    testRunner.And("an obstacle 'obstacle-2'");
#line 267
    testRunner.And("an obstacle 'obstacle-3'");
#line 268
    testRunner.And("an obstacle refinement 'refinement-1' for 'obstacle-1'");
#line 269
    testRunner.And("an obstacle refinement 'refinement-2' for 'obstacle-1'");
#line 270
    testRunner.And("refinement 'refinement-1' contains 'obstacle-2'");
#line 271
    testRunner.And("refinement 'refinement-2' contains 'obstacle-3'");
#line 272
    testRunner.When("I connect 'refinement-1' to 'refinement-2'");
#line 273
    testRunner.Then("there exists a refinement for 'obstacle-1'");
#line 274
    testRunner.And("this refinement contains 'obstacle-2'");
#line 275
    testRunner.And("this refinement contains 'obstacle-3'");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Connect an obstacle refinement and an obstacle refinement of an other obstacle")]
        public virtual void ConnectAnObstacleRefinementAndAnObstacleRefinementOfAnOtherObstacle()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Connect an obstacle refinement and an obstacle refinement of an other obstacle", ((string[])(null)));
#line 277
  this.ScenarioSetup(scenarioInfo);
#line 6
  this.FeatureBackground();
#line 278
    testRunner.Given("an obstacle 'obstacle-1'");
#line 279
    testRunner.And("an obstacle 'obstacle-2'");
#line 280
    testRunner.And("an obstacle refinement 'refinement-1' for 'obstacle-1'");
#line 281
    testRunner.And("an obstacle refinement 'refinement-2' for 'obstacle-2'");
#line 282
    testRunner.When("I connect 'refinement-1' to 'refinement-2'");
#line 283
    testRunner.Then("nothing change");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion