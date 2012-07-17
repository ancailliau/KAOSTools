Feature: Views for goal models
  As a modeler
  I want to have views on my goal model
  So that I can deal with complexity
  
  Background:
    Given a model
  
  Scenario: Add a view
    When I add a new view 'view-1'
    Then the model contains a view named 'view-1'
  
  Scenario: Remove a view
    Given a view 'view-1'
    When I remove the view 'view-1'
    Then the model does not contains a view named 'view-1'
  
  Scenario: Add element to a view
    Given a view 'view-1'
    And a goal 'goal-1'
    When I add 'goal-1' to 'view-1'
    Then the view 'view-1' contains 'goal-1'
  
  Scenario: Remove element from a view
    Given a view 'view-1'
    And a goal 'goal-1'
    And the view 'view-1' contains goal 'goal-1'
    When I remove 'goal-1' from 'view-1'
    Then the view 'view-1' does not contains 'goal-1'