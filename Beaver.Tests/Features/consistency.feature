Feature: Consistency for goal models
  As a modeler
  I want to have consistent views
  So that I avoid dummy errors
  
  Background:
    Given a model
  
  Scenario: Remove element from all views
    Given a goal 'goal-1'
    And a view 'view-1'
    And the view 'view-1' contains goal 'goal-1'
    When I delete 'goal-1'
    Then the view 'view-1' does not contains 'goal-1'

  Scenario: Added elements exists
    Given a view 'view-1'
    When I add 'goal-1' to 'view-1'
    Then the view 'view-1' does not contains 'goal-1'