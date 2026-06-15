@application @query
Feature: Application query tests

  @list-result @success
  Scenario: Get all applications returns the seeded applications
    Given I am an Owner user
    And the request url is '/applications'
    And I am using the default header settings
    When I 'GET' this request
    Then I receive the HTTP status code 'OK'
    And I have received JSON data in the response
    And the response array contains at least 3 objects

  @single-result @success
  Scenario: Getting an application by client id returns the application
    Given I am an Owner user
    And the request url is '/applications/df9ab2b8-1f01-4eda-bbdf-13814d91ebb6'
    And I am using the default header settings
    When I 'GET' this request
    Then I receive the HTTP status code 'OK'
    And I have received JSON data in the response
    And the response contains the following values:
      | path           | value                          |
      | id             | df9ab2b8-1f01-4eda-bbdf-13814d91ebb6 |
      | name           | Test Livestock Service 1      |
      | tenant_name    | Test Tenant 1                 |
      | description    | Test Description 1            |
      | secret         | secret123                      |
      | scopes[0]      | scope1                         |
      | scopes[1]      | scope2                         |
      | redirect_uris[0] | https://localhost:5001/signin-oidc |
      | redirect_uris[1] | https://localhost:5001/signout-callback-oidc |

  @error @error-not-found
  Scenario: Getting an application by an unknown client id returns a 404
    Given I am an Owner user
    And the request url is '/applications/99999999-9999-9999-9999-999999999999'
    And I am using the default header settings
    When I 'GET' this request
    Then I receive the HTTP status code 'NotFound'
