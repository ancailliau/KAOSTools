Feature: Formal specifications for goal models
  As a modeler
  I want to specify my goal formally
  So that I can perform automated checks
  
  Background:
    Given a model
  
  Scenario: Formal specification of goals
    Given a goal 'goal-1'
    When I specify 'goal-1' as 'G (current -> F target)'
    Then the goal 'goal-1' is formally specified

  Scenario: Formal specification of domain properties
    Given a domain property 'domprop-1'
    When I specify 'domprop-1' as 'G (current -> G goodCondition)'
    Then the domain property 'domprop-1' is formally specified

  Scenario: Formal specification of obstacles
    Given an obstacle 'obstacle-1'
    When I specify 'obstacle-1' as 'F (current & G !target)'
    Then the obstacle 'obstacle-1' is formally specified