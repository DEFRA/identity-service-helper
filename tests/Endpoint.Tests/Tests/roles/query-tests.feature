@sequential @role @query
Feature: Role query tests

  @list-result @success
  Scenario: Get all roles returns the seeded roles
    Given I am an Owner user
    And the request url is '/roles'
    And I am using the default header settings
    When I 'GET' this request
    Then I receive the HTTP status code 'OK'
    And I have received JSON data in the response
    And the response array contains at least 4 objects
