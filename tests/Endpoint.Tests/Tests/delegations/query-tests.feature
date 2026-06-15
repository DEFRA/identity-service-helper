@delegation @query
Feature: CPH delegation query tests

  @list-result @success
  Scenario: Get all delegations returns the seeded delegation invitations
    Given I am an Owner user
    And the request url is '/delegations'
    And I am using the default header settings
    When I 'GET' this request
    Then I receive the HTTP status code 'OK'
    And I have received JSON data in the response
    And the response array contains 4 objects

  @single-result @success
  Scenario Outline: Getting a delegation by id returns the delegation invitation
    Given I am an Owner user
    And the request url is '/delegations/<id>'
    And I am using the default header settings
    When I 'GET' this request
    Then I receive the HTTP status code 'OK'
    And I have received JSON data in the response
    And the response contains the following values:
      | path                         | value                |
      | county_parish_holding_number | <cph_number>         |
      | delegating_user_name         | <delegating_user>    |
      | delegated_user_name          | <delegated_user>     |
      | delegated_user_email         | <delegated_email>    |
      | delegated_user_role_name     | <role_name>          |
  Examples:
    | id                                   | cph_number  | delegating_user  | delegated_user | delegated_email                  | role_name    |
    | dd000001-0000-4000-8000-000000000001 | 44/081/0004 | Max Bladen-Clark | Cedric Brasey   | cedric.brasey@planet-side.co.uk  | test-role-1  |
    | dd000003-0000-4000-8000-000000000003 | 44/082/0004 | Cedric Brasey    | Max Bladen-Clark| max.bladen-clark@esynergy.co.uk  | test-role-1  |

  @error @error-not-found
  Scenario: Getting a delegation by an unknown id returns a 404
    Given I am an Owner user
    And the request url is '/delegations/dd999999-0000-4000-8000-000000000099'
    And I am using the default header settings
    When I 'GET' this request
    Then I receive the HTTP status code 'NotFound'
