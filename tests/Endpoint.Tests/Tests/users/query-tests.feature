@user @query
Feature: User query tests

  @list-result @success
  Scenario: Get all users returns the seeded user accounts
    Given I am an Owner user
    And the request url is '/users'
    And I am using the default header settings
    When I 'GET' this request
    Then I receive the HTTP status code 'OK'
    And I have received JSON data in the response
    And the response array contains at least 14 objects

  @single-result @success
  Scenario Outline: Getting a user by id returns the user account
    Given I am an Owner user
    And the request url is '/users/<id>'
    And I am using the default header settings
    When I 'GET' this request
    Then I receive the HTTP status code 'OK'
    And I have received JSON data in the response
    And the response contains the following values:
      | path          | value             |
      | email         | <email>           |
      | first_name    | <first_name>      |
      | last_name     | <last_name>       |
      | display_name  | <display_name>    |
      | active        | True              |
  Examples:
    | id                                   | email                            | first_name | last_name    | display_name     |
    | 0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1 | test@test.com                    | test       | user         | Test User         |
    | a17a772c-604e-495d-950e-3dbee2ba6e98 | max.bladen-clark@esynergy.co.uk  | Max        | Bladen-Clark | Max Bladen-Clark  |

  @error @error-not-found
  Scenario: Getting a user by an unknown id returns a 404
    Given I am an Owner user
    And the request url is '/users/99999999-9999-9999-9999-999999999999'
    And I am using the default header settings
    When I 'GET' this request
    Then I receive the HTTP status code 'NotFound'
