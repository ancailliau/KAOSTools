Feature: Elements
  As a modeler,
  I want to model elements
  So that I ...
  
  Background:
    Given a model
  
  Scenario Outline: Add an element in model
    When I add <type> '<name>'
    Then the model contains '<name>'
    And '<name>' is <type>
    
    Examples:
      | type                | name              |
      | a goal              | goal-1            |
      | a domain property   | domprop-1         |
      | an obstacle         | obstacle-1        |
      | an agent            | agent-1           |
  
  Scenario Outline: Remove element from model
    Given <type> '<name>'
    When I remove '<name>'
    Then the model does not contains '<name>'
    
    Examples:
      | type                | name              |
      | a goal              | goal-1            |
      | a domain property   | domprop-1         |
      | an obstacle         | obstacle-1        |
      | an agent            | agent-1           |