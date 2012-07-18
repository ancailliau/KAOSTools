Feature: Usability for modeling
  As a modeler,
  I want to easily connect elements each other
  So that I do not spend time in wasteful clicks
  
  Background:
    Given a model
  
  # goal with goal

  Scenario: Connect a goal and a goal
    Given a goal 'goal-1'
    And a goal 'goal-2'
    When I connect 'goal-1' to 'goal-2'
    Then there exists a refinement for 'goal-1'
    And this refinement contains 'goal-2'

  # goal with domain property
  
  Scenario: Connect a goal and a domain property
    Given a domain property 'domprop-1'
    And a goal 'goal-1'
    When I connect 'domprop-1' to 'goal-1'
    Then there exists a refinement for 'goal-1'
    And this refinement contains 'domprop-1'

  # goal with obstacle

  Scenario: Connect a goal and an obstacle
    Given a goal 'goal-1'
    And an obstacle 'obstacle-1'
    When I connect 'goal-1' to 'obstacle-1'
    Then 'obstacle-1' obstructs 'goal-1'
  
  # goal with agent

  Scenario: Connect a goal and an agent
    Given a goal 'goal-1'
    And an agent 'agent-1'
    When I connect 'goal-1' to 'agent-1'
    Then 'agent-1' is assigned to 'goal-1'
  
  # goal with refinement
  
  Scenario: Connect a goal and a refinement
    Given a goal 'goal-1'
    And a refinement 'refinement-1'
    When I connect 'goal-1' to 'refinement-1'
    Then refinement 'refinement-1' contains 'goal-2'

  # goal with obstacle refinement
   
  Scenario: Connect a goal and an obstacle refinement
    Given a goal 'goal-1'
    And an obstacle refinement 'refinement-1'
    When I connect 'goal-1' to 'refinement-1'
    Then nothing change
    
  # domain property with goal
 
  Scenario: Connect a domain property and a goal
    Given a domain property 'domprop-1'
    And a goal 'goal-1'
    When I connect 'domprop-1' to 'goal-1'
    Then there exists a refinement for 'goal-1'
    And this refinement contains 'domprop-1'
  
  # domain property with domain property
 
  Scenario: Connect a domain property and a domain property
    Given a domain property 'domprop-1'
    And a domain property 'domprop-2'
    When I connect 'domprop-1' to 'domprop-2'
    Then nothing change
  
  # domain property with obstacle
 
  Scenario: Connect a domain property and an obstacle
    Given a domain property 'domprop-1'
    And an obstacle 'obstacle-1'
    When I connect 'domprop-1' to 'obstacle-1'
    Then nothing change
    
  # domain property with agent
 
  Scenario: Connect a domain property and an agent
    Given a domain property 'domprop-1'
    And an agent 'agent-1'
    When I connect 'domprop-1' to 'agent-1'
    Then nothing change
    
  # domain property with refinement
 
  Scenario: Connect a domain property and a refinement
    Given a domain property 'domprop-1'
    And a refinement 'refinement-1'
    When I connect 'domprop-1' to 'refinement-1'
    Then refinement 'refinement-1' contains 'domprop-1'
    
  # domain property with obstacle refinement
  
  Scenario: Connect a domain property and an obstacle refinement
    Given a domain property 'domprop-1'
    And an obstacle refinement 'refinement-1'
    When I connect 'domprop-1' to 'refinement-1'
    Then refinement 'refinement-1' contains 'domprop-1'
  
  # obstacle with goal
  
  Scenario: Connect an obstacle and a goal
    Given an obstacle 'obstacle-1'
    And a goal 'goal-1'
    When I connect 'obstacle-1' to 'goal-1'
    Then 'obstacle-1' obstructs 'goal-1'
  
  # obstacle with domain property
  
  Scenario: Connect an obstacle and a domain property
    Given an obstacle 'obstacle-1'
    And a domain property 'domprop-1'
    When I connect 'obstacle-1' to 'domprop-1'
    Then nothing change
    
  # obstacle with obstacle
  
  Scenario: Connect an obstacle and an obstacle
    Given an obstacle 'obstacle-1'
    And an obstacle 'obstacle-2'
    When I connect 'obstacle-1' to 'obstacle-2'
    Then there exists an obstacle refinement for 'obstacle-1'
    And this refinement contains 'obstacle-2'
    
  # obstacle with agent
  
  Scenario: Connect an obstacle and a agent
    Given an obstacle 'obstacle-1'
    And a agent 'agent-1'
    When I connect 'obstacle-1' to 'agent-1'
    Then nothing change
    
  # obstacle with refinement
  
  Scenario: Connect an obstacle and a refinement
    Given an obstacle 'obstacle-1'
    And a refinement 'refinement-1'
    When I connect 'obstacle-1' to 'refinement-1'
    Then nothing change
    
  # obstacle with obstacle refinement
  
  Scenario: Connect an obstacle and an obstacle refinement
    Given an obstacle 'obstacle-1'
    And an obstacle refinement 'refinement-1'
    When I connect 'obstacle-1' to 'refinement-1'
    Then refinement 'refinement-1' contains 'obstacle-1'
  
  # agent with goal
  
  Scenario: Connect an agent and a goal
    Given an agent 'agent-1'
    And a goal 'goal-1'
    When I connect 'agent-1' to 'goal-1'
    Then 'agent-1' is assigned to 'goal-1'
  
  # agent with domain property
  # agent with obstacle
  # agent with agent
  # agent with refinement
  # agent with obstacle refinement
  
  Scenario Outline: Connect an agent and not a goal
    Given an agent 'agent-1'
    And <type> '<name>'
    When I connect 'agent-1' to '<name>'
    Then nothing change
    
    Examples:
      | type                    | name          |
      | a domain property       | domprop-1     |
      | an agent                | agent-2       |
      | a refinement            | refinement-1  |
      | an obstacle refinement  | refinement-1  |
    
  # refinement with goal
  
  Scenario: Connect a refinement and a goal
    Given a refinement 'refinement-1'
    And a goal 'goal-1'
    When I connect 'refinement-1' to 'goal-1'
    Then 'refinement-1' refines 'goal-1'
  
  # refinement with refinement of same goal
  
  Scenario: Connect a refinement and a refinement of same goal
    Given a goal 'goal-1'
    And a goal 'goal-2'
    And a goal 'goal-3'
    And a refinement 'refinement-1' for 'goal-1'
    And a refinement 'refinement-2' for 'goal-1'
    And refinement 'refinement-1' contains 'goal-2'
    And refinement 'refinement-2' contains 'goal-3'
    When I connect 'refinement-1' to 'refinement-2'
    Then there exists a refinement for 'goal-1'
    And this refinement contains 'goal-2'
    And this refinement contains 'goal-3'
    And 'refinement-1' no longer exists
    And 'refinement-2' no longer exists

  # refinement with refinement of a different goal
  
  Scenario: Connect a refinement and a refinement of a different goal
    Given a goal 'goal-1'
    And a goal 'goal-2'
    And a refinement 'refinement-1' for 'goal-1'
    And a refinement 'refinement-2' for 'goal-2'
    When I connect 'refinement-1' to 'refinement-2'
    Then nothing change
    
  # refinement with domain property
  # refinement with obstacle
  # refinement with agent
  # refinement with obstacle refinement
  
  Scenario Outline: Connect a refinement and not a goal nor a refinement
    Given a refinement 'refinement-1'
    And <type> '<name>'
    When I connect 'refinement-1' to '<name>'
    Then nothing change
    
    Examples:
      | type                    | name          |
      | a domain property       | domprop-1     |
      | an obstacle             | obstacle-2    |
      | an agent                | agent-2       |
      | an obstacle refinement  | refinement-2  |
  
  # obstacle refinement with obstacle
  
  Scenario: Connect an obstacle refinement and an obstacle
    Given an obstacle refinement 'refinement-1'
    And an obstacle 'obstacle-1'
    When I connect 'refinement-1' to 'obstacle-1'
    Then 'refinement-1' refines 'obstacle-1'
  
  # obstacle refinement with goal
  # obstacle refinement with domain property
  # obstacle refinement with agent
  # obstacle refinement with refinement
  
  Scenario Outline: Connect an obstacle refinement and not an obstacle nor an obstacle refinement
    Given an obstacle refinement 'refinement-1'
    And <type> '<name>'
    When I connect 'refinement-1' to '<name>'
    Then nothing change
    
    Examples:
      | type                | name          |
      | a goal              | goal-1        |
      | a domain property   | domprop-1     |
      | an agent            | agent-2       |
      | a refinement        | refinement-2  |
  
  # obstacle refinement with obstacle refinement
  
  Scenario: Connect an obstacle refinement and an obstacle refinement of same obstacle
    Given an obstacle 'obstacle-1'
    And an obstacle 'obstacle-2'
    And an obstacle 'obstacle-3'
    And an obstacle refinement 'refinement-1' for 'obstacle-1'
    And an obstacle refinement 'refinement-2' for 'obstacle-1'
    And refinement 'refinement-1' contains 'obstacle-2'
    And refinement 'refinement-2' contains 'obstacle-3'
    When I connect 'refinement-1' to 'refinement-2'
    Then there exists an obstacle refinement for 'obstacle-1'
    And this refinement contains 'obstacle-2'
    And this refinement contains 'obstacle-3'
    
  Scenario: Connect an obstacle refinement and an obstacle refinement of an other obstacle
    Given an obstacle 'obstacle-1'
    And an obstacle 'obstacle-2'
    And an obstacle refinement 'refinement-1' for 'obstacle-1'
    And an obstacle refinement 'refinement-2' for 'obstacle-2'
    When I connect 'refinement-1' to 'refinement-2'
    Then nothing change
  