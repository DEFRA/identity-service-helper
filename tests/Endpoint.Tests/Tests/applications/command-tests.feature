@application @command
Feature: Application command tests

  @single-result @command-create @success
  Scenario: Creating an application returns the created application
    Given I am an Owner user
    And the request url is '/applications'
    And I am using the default header settings
    And the JSON payload is:
    """
    {
      "id": "11111111-1111-1111-1111-111111111111",
      "name": "New Test Application",
      "tenant_name": "New Test Tenant",
      "description": "New Test Description",
      "scopes": ["scope1", "scope2"],
      "redirect_uris": ["https://localhost:6001/signin-oidc"],
      "secret": "newsecret123"
    }
    """
    When I 'POST' this request
    Then I receive the HTTP status code 'Created'
    And I have received JSON data in the response
    And the response contains the following values:
      | path             | value                                |
      | id               | 11111111-1111-1111-1111-111111111111 |
      | name             | New Test Application                 |
      | tenant_name      | New Test Tenant                      |
      | description      | New Test Description                 |
      | secret           | newsecret123                         |
      | scopes[0]        | scope1                               |
      | scopes[1]        | scope2                               |
      | redirect_uris[0] | https://localhost:6001/signin-oidc   |

  @single-result @command-create @error @error-unprocessable
  Scenario: Creating an application with a missing name returns a 422
    Given I am an Owner user
    And the request url is '/applications'
    And I am using the default header settings
    And the JSON payload is:
    """
    {
      "id": "22222222-2222-2222-2222-222222222222",
      "name": "",
      "tenant_name": "New Test Tenant",
      "description": "New Test Description",
      "scopes": ["scope1"],
      "redirect_uris": ["https://localhost:6001/signin-oidc"],
      "secret": "newsecret123"
    }
    """
    When I 'POST' this request
    Then I receive the HTTP status code 'UnprocessableEntity'

  @single-result @command-update @success
  Scenario: Updating an application returns the updated application
    Given I am an Owner user
    And the request url is '/applications'
    And I am using the default header settings
    And the JSON payload is:
    """
    {
      "id": "33333333-3333-3333-3333-333333333333",
      "name": "Update Test Application",
      "tenant_name": "Update Test Tenant",
      "description": "Update Test Description",
      "scopes": ["scope1"],
      "redirect_uris": ["https://localhost:6001/signin-oidc"],
      "secret": "secret123"
    }
    """
    When I 'POST' this request
    Then I receive the HTTP status code 'Created'
    And I have received JSON data in the response
    Given the request url is '/applications'
    And I am using the default header settings
    When I 'GET' this request
    Then I receive the HTTP status code 'OK'
    And I have received JSON data in the response
    
    Given the request url is '/applications/33333333-3333-3333-3333-333333333333'
    And I am using the default header settings
    And the JSON payload is:
    """
    {
      "name": "Updated Application Name",
      "tenant_name": "Updated Tenant Name",
      "description": "Updated Description",
      "scopes": ["scope1", "scope2", "scope3"],
      "redirect_uris": ["https://localhost:6002/signin-oidc"],
      "secret": "updatedsecret123"
    }
    """
    When I 'PUT' this request
    Then I receive the HTTP status code 'OK'
    And I have received JSON data in the response
    And the response contains the following values:
      | path             | value                                |
      | id               | 33333333-3333-3333-3333-333333333333 |
      | name             | Updated Application Name             |
      | tenant_name      | Updated Tenant Name                  |
      | description      | Updated Description                  |
      | secret           | updatedsecret123                     |
      | scopes[0]        | scope1                               |
      | scopes[1]        | scope2                               |
      | scopes[2]        | scope3                               |
      | redirect_uris[0] | https://localhost:6002/signin-oidc   |

  @single-result @command-update @error @error-unprocessable
  Scenario: Updating an application with a missing name returns a 422
    Given I am an Owner user
    And the request url is '/applications/df9ab2b8-1f01-4eda-bbdf-13814d91ebb6'
    And I am using the default header settings
    And the JSON payload is:
    """
    {
      "name": "",
      "tenant_name": "Test Tenant 1",
      "description": "Test Description 1",
      "scopes": ["scope1", "scope2"],
      "redirect_uris": ["https://localhost:5001/signin-oidc"],
      "secret": "secret123"
    }
    """
    When I 'PUT' this request
    Then I receive the HTTP status code 'UnprocessableEntity'

  @single-result @command-update @error @error-not-found
  Scenario: Updating an unknown application returns a 404
    Given I am an Owner user
    And the request url is '/applications/99999999-9999-9999-9999-999999999999'
    And I am using the default header settings
    And the JSON payload is:
    """
    {
      "name": "Unknown Application",
      "tenant_name": "Unknown Tenant",
      "description": "Unknown Description",
      "scopes": ["scope1"],
      "redirect_uris": ["https://localhost:6001/signin-oidc"],
      "secret": "secret123"
    }
    """
    When I 'PUT' this request
    Then I receive the HTTP status code 'NotFound'

  @single-result @command-delete @success
  Scenario: Deleting an application removes it and deleting again fails
    Given I am an Owner user
    And the request url is '/applications'
    And I am using the default header settings
    And the JSON payload is:
    """
    {
      "id": "44444444-4444-4444-4444-444444444444",
      "name": "Delete Test Application",
      "tenant_name": "Delete Test Tenant",
      "description": "Delete Test Description",
      "scopes": ["scope1"],
      "redirect_uris": ["https://localhost:6001/signin-oidc"],
      "secret": "secret123"
    }
    """
    When I 'POST' this request
    Then I receive the HTTP status code 'Created'
    And I have received JSON data in the response
    Given the request url is '/applications/44444444-4444-4444-4444-444444444444'
    And I am using the default header settings
    When I 'DELETE' this request
    Then I receive the HTTP status code 'NoContent'
    Given the request url is '/applications/44444444-4444-4444-4444-444444444444'
    And I am using the default header settings
    When I 'GET' this request
    Then I receive the HTTP status code 'NotFound'
    Given the request url is '/applications/44444444-4444-4444-4444-444444444444'
    And I am using the default header settings
    When I 'DELETE' this request
    Then I receive the HTTP status code 'NotFound'

  @single-result @command-delete @error @error-not-found
  Scenario: Deleting an unknown application returns a 404
    Given I am an Owner user
    And the request url is '/applications/99999999-9999-9999-9999-999999999999'
    And I am using the default header settings
    When I 'DELETE' this request
    Then I receive the HTTP status code 'NotFound'
