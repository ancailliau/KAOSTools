Feature: Probability
  As a modeler,
  I want to have accurate simulation
  So that I can base my conclusion on right assumptions

  Background:
    Given a model
    And a goal 'goal-1'
    And a goal 'goal-2'
    And a goal 'goal-3'
    And an obstacle 'obstacle-1' with a probability of 3%
    And 'obstacle-1' obstructs 'goal-3'
  
  Scenario Outline: Milestone refinement
    And a <pattern> refinement 'refinement-1' for 'goal-1'
    And 'refinement-1' contains 'goal-2'
    And 'refinement-1' contains 'goal-3'
    When I run a simulation
    Then 'goal-1' has a probability of <result>
    
    Examples:
      | pattern                     | result    |
      | milestone-driven            | 97%       |
      | equitable case-driven       | 98.5%     |