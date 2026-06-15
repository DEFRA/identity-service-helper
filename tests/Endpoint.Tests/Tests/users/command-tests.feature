@user @command
Feature: User command tests

  @single-result @command-create @success
  Scenario: Creating a user returns the created user account
    Given I am an Owner user
    And the request url is '/users'
    And I am using the default header settings
    And the JSON payload is:
    """
    {
      "email": "new.user.1@test.com",
      "display_name": "New User 1",
      "first_name": "New",
      "last_name": "User 1"
    }
    """
    When I 'POST' this request
    Then I receive the HTTP status code 'Created'
    And I have received JSON data in the response
    And the response contains the following values:
      | path          | value             |
      | email         | new.user.1@test.com |
      | first_name    | New               |
      | last_name     | User 1            |
      | display_name  | New User 1        |
      | active        | True              |

  @single-result @command-create @error @error-unprocessable
  Scenario: Creating a user with an invalid email returns a 422
    Given I am an Owner user
    And the request url is '/users'
    And I am using the default header settings
    And the JSON payload is:
    """
    {
      "email": "not-an-email",
      "display_name": "Invalid User",
      "first_name": "Invalid",
      "last_name": "User"
    }
    """
    When I 'POST' this request
    Then I receive the HTTP status code 'UnprocessableEntity'

  @single-result @command-update @success
  Scenario: Updating a user returns the updated user account
    Given I am an Owner user
    And the request url is '/users'
    And I am using the default header settings
    And the JSON payload is:
    """
    {
      "email": "new.user.2@test.com",
      "display_name": "New User 2",
      "first_name": "New",
      "last_name": "User 2"
    }
    """
    When I 'POST' this request
    Then I receive the HTTP status code 'Created'
    And I have received JSON data in the response
    And I save the value of property "id" to the context using name "update_user_id"
    Given the request url is '/users/<update_user_id>'
    And I am using the default header settings
    And the JSON payload is:
    """
    {
      "email": "new.user.2.updated@test.com",
      "display_name": "New User 2 Updated",
      "first_name": "Updated",
      "last_name": "User 2"
    }
    """
    When I 'PUT' this request
    Then I receive the HTTP status code 'OK'
    And I have received JSON data in the response
    And the response contains the following values:
      | path          | value                       |
      | email         | new.user.2.updated@test.com |
      | first_name    | Updated                     |
      | last_name     | User 2                      |
      | display_name  | New User 2 Updated          |
      | active        | True                        |

  @single-result @command-update @error @error-unprocessable
  Scenario: Updating a user with an invalid email returns a 422
    Given I am an Owner user
    And the request url is '/users'
    And I am using the default header settings
    And the JSON payload is:
    """
    {
      "email": "new.user.3@test.com",
      "display_name": "New User 3",
      "first_name": "New",
      "last_name": "User 3"
    }
    """
    When I 'POST' this request
    Then I receive the HTTP status code 'Created'
    And I have received JSON data in the response
    And I save the value of property "id" to the context using name "invalid_update_user_id"
    Given the request url is '/users/<invalid_update_user_id>'
    And I am using the default header settings
    And the JSON payload is:
    """
    {
      "email": "not-an-email",
      "display_name": "New User 3",
      "first_name": "New",
      "last_name": "User 3"
    }
    """
    When I 'PUT' this request
    Then I receive the HTTP status code 'UnprocessableEntity'

  @single-result @command-update @error @error-not-found
  Scenario: Updating an unknown user returns a 404
    Given I am an Owner user
    And the request url is '/users/99999999-9999-9999-9999-999999999999'
    And I am using the default header settings
    And the JSON payload is:
    """
    {
      "email": "unknown.user@test.com",
      "display_name": "Unknown User",
      "first_name": "Unknown",
      "last_name": "User"
    }
    """
    When I 'PUT' this request
    Then I receive the HTTP status code 'NotFound'

  @single-result @command-delete @success
  Scenario: Deleting a user marks it as inactive and deleting again fails
    Given I am an Owner user
    And the request url is '/users'
    And I am using the default header settings
    And the JSON payload is:
    """
    {
      "email": "new.user.4@test.com",
      "display_name": "New User 4",
      "first_name": "New",
      "last_name": "User 4"
    }
    """
    When I 'POST' this request
    Then I receive the HTTP status code 'Created'
    And I have received JSON data in the response
    And I save the value of property "id" to the context using name "delete_user_id"
    Given the request url is '/users/<delete_user_id>'
    And I am using the default header settings
    When I 'DELETE' this request
    Then I receive the HTTP status code 'NoContent'
    Given the request url is '/users/<delete_user_id>'
    And I am using the default header settings
    When I 'GET' this request
    Then I receive the HTTP status code 'OK'
    And I have received JSON data in the response
    And the response contains the following values:
      | path   | value |
      | active | False |
    Given the request url is '/users/<delete_user_id>'
    And I am using the default header settings
    When I 'DELETE' this request
    Then I receive the HTTP status code 'NotFound'

  @single-result @command-delete @error @error-not-found
  Scenario: Deleting an unknown user returns a 404
    Given I am an Owner user
    And the request url is '/users/99999999-9999-9999-9999-999999999999'
    And I am using the default header settings
    When I 'DELETE' this request
    Then I receive the HTTP status code 'NotFound'
