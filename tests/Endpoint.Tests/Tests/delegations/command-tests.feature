@delegation @command
Feature: CPH delegation command tests

  @single-result @command-create @success
  Scenario: Creating a delegation invite returns the created delegation
    Given I am an Owner user
    And the request url is '/delegations'
    And I am using the default header settings
    And the JSON payload is:
    """
    {
      "county_parish_holding_id": "aa810001-0000-4000-8000-000000000001",
      "delegating_user_id": "0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1",
      "delegated_user_role_id": "817647b3-d5d2-45e9-8833-df36d8264102",
      "delegated_user_email": "test3@test.com"
    }
    """
    When I 'POST' this request
    Then I receive the HTTP status code 'Created'
    And I have received JSON data in the response
    And the response contains the following values:
      | path                          | value                            |
      | county_parish_holding_number  | 44/081/0001                      |
      | delegating_user_name          | Test User                        |
      | delegated_user_name           | Test User 3                      |
      | delegated_user_email          | test3@test.com                   |
      | delegated_user_role_name      | test-role-2                      |

  @single-result @command-create @error @error-unprocessable
  Scenario: Creating a delegation invite with an invalid email returns a 422
    Given I am an Owner user
    And the request url is '/delegations'
    And I am using the default header settings
    And the JSON payload is:
    """
    {
      "county_parish_holding_id": "02f8043f-510a-41aa-8012-db316ae7fefa",
      "delegating_user_id": "0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1",
      "delegated_user_role_id": "0c15ba2f-b4ba-406a-a0ae-213de64600a9",
      "delegated_user_email": "not-an-email"
    }
    """
    When I 'POST' this request
    Then I receive the HTTP status code 'UnprocessableEntity'

  @single-result @command-create @error @error-not-found
  Scenario Outline: Creating a delegation invite with an unknown reference returns a 404
    Given I am an Owner user
    And the request url is '/delegations'
    And I am using the default header settings
    And the JSON payload is:
    """
    {
      "county_parish_holding_id": "<cph_id>",
      "delegating_user_id": "<delegating_user_id>",
      "delegated_user_role_id": "<role_id>",
      "delegated_user_email": "<email>"
    }
    """
    When I 'POST' this request
    Then I receive the HTTP status code 'NotFound'
  Examples:
    | scenario                      | cph_id                                | delegating_user_id                    | role_id                               | email             |
    | unknown cph                   | 99999999-9999-9999-9999-999999999999  | 0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1   | 0c15ba2f-b4ba-406a-a0ae-213de64600a9  | test4@test.com    |
    | unknown delegating user       | 4435a146-d0ac-4260-8a27-c550e0ed9563   | 99999999-9999-9999-9999-999999999999  | 0c15ba2f-b4ba-406a-a0ae-213de64600a9  | test5@test.com    |
    | unknown delegated user role   | 4435a146-d0ac-4260-8a27-c550e0ed9563   | 0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1   | 99999999-9999-9999-9999-999999999999 | test6@test.com    |
    | unknown delegated user email  | 4435a146-d0ac-4260-8a27-c550e0ed9563   | 0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1   | 0c15ba2f-b4ba-406a-a0ae-213de64600a9  | unknown@test.com  |

  @single-result @command-accept @success
  Scenario: Accepting a delegation invite as the delegated user marks it as accepted
    Given I am an Owner user
    And the request url is '/delegations'
    And I am using the default header settings
    And the JSON payload is:
    """
    {
      "county_parish_holding_id": "d4a7f0d7-39eb-40fe-b68d-547a5f494738",
      "delegating_user_id": "0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1",
      "delegated_user_role_id": "0c15ba2f-b4ba-406a-a0ae-213de64600a9",
      "delegated_user_email": "test4@test.com"
    }
    """
    When I 'POST' this request
    Then I receive the HTTP status code 'Created'
    And I have received JSON data in the response
    And I save the value of property "id" to the context using name "accept_delegation_id"
    Given the request url is '/delegations/<accept_delegation_id>:accept'
    And I am using the default header settings
    And I am using the following headers:
      | header        | value                                |
      | x-operator-id | d1354eb1-dd1c-471e-bd0e-2626e2e21366 |
    When I 'POST' this request
    Then I receive the HTTP status code 'NoContent'

  @single-result @command-accept @error @error-forbidden
  Scenario: Accepting a delegation invite as a user other than the delegated user is forbidden
    Given I am an Owner user
    And the request url is '/delegations'
    And I am using the default header settings
    And the JSON payload is:
    """
    {
      "county_parish_holding_id": "ebc992ae-2b95-4549-9fa3-4484c8349b89",
      "delegating_user_id": "0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1",
      "delegated_user_role_id": "817647b3-d5d2-45e9-8833-df36d8264102",
      "delegated_user_email": "test5@test.com"
    }
    """
    When I 'POST' this request
    Then I receive the HTTP status code 'Created'
    And I have received JSON data in the response
    And I save the value of property "id" to the context using name "forbidden_delegation_id"
    Given the request url is '/delegations/<forbidden_delegation_id>:accept'
    And I am using the default header settings
    When I 'POST' this request
    Then I receive the HTTP status code 'Forbidden'

  @single-result @command-accept @error @error-bad-request
  Scenario: Accepting an already accepted delegation invite returns a Bad Request
    Given I am an Owner user
    And the request url is '/delegations'
    And I am using the default header settings
    And the JSON payload is:
    """
    {
      "county_parish_holding_id": "aa810002-0000-4000-8000-000000000002",
      "delegating_user_id": "0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1",
      "delegated_user_role_id": "c63207ab-68f7-4613-b94a-492939eb6116",
      "delegated_user_email": "test6@test.com"
    }
    """
    When I 'POST' this request
    Then I receive the HTTP status code 'Created'
    And I have received JSON data in the response
    And I save the value of property "id" to the context using name "already_accepted_delegation_id"
    Given the request url is '/delegations/<already_accepted_delegation_id>:accept'
    And I am using the default header settings
    And I am using the following headers:
      | header        | value                                |
      | x-operator-id | dd1b98c5-0874-4718-a37c-74cdec359567 |
    When I 'POST' this request
    Then I receive the HTTP status code 'NoContent'
    Given the request url is '/delegations/<already_accepted_delegation_id>:accept'
    And I am using the default header settings
    And I am using the following headers:
      | header        | value                                |
      | x-operator-id | dd1b98c5-0874-4718-a37c-74cdec359567 |
    When I 'POST' this request
    Then I receive the HTTP status code 'BadRequest'

  @single-result @command-reject @success
  Scenario: Rejecting a delegation invite marks it as rejected and rejecting again fails
    Given I am an Owner user
    And the request url is '/delegations'
    And I am using the default header settings
    And the JSON payload is:
    """
    {
      "county_parish_holding_id": "aa810003-0000-4000-8000-000000000003",
      "delegating_user_id": "0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1",
      "delegated_user_role_id": "306fa0fc-bd1a-45d3-9fef-e6f11a85b601",
      "delegated_user_email": "test7@test.com"
    }
    """
    When I 'POST' this request
    Then I receive the HTTP status code 'Created'
    And I have received JSON data in the response
    And I save the value of property "id" to the context using name "reject_delegation_id"
    Given the request url is '/delegations/<reject_delegation_id>:reject'
    And I am using the default header settings
    And I am using the following headers:
      | header        | value                                |
      | x-operator-id | 2c6d9ca2-6e78-4fdd-88eb-61fa6cc7f319 |
    When I 'POST' this request
    Then I receive the HTTP status code 'NoContent'
    Given the request url is '/delegations/<reject_delegation_id>:reject'
    And I am using the default header settings
    And I am using the following headers:
      | header        | value                                |
      | x-operator-id | 2c6d9ca2-6e78-4fdd-88eb-61fa6cc7f319 |
    When I 'POST' this request
    Then I receive the HTTP status code 'BadRequest'

  @single-result @command-revoke @success
  Scenario: Revoking a delegation invite marks it as revoked and revoking again fails
    Given I am an Owner user
    And the request url is '/delegations'
    And I am using the default header settings
    And the JSON payload is:
    """
    {
      "county_parish_holding_id": "ab820002-0000-4000-8000-000000000002",
      "delegating_user_id": "0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1",
      "delegated_user_role_id": "0c15ba2f-b4ba-406a-a0ae-213de64600a9",
      "delegated_user_email": "test1@test.com"
    }
    """
    When I 'POST' this request
    Then I receive the HTTP status code 'Created'
    And I have received JSON data in the response
    And I save the value of property "id" to the context using name "revoke_delegation_id"
    Given the request url is '/delegations/<revoke_delegation_id>:revoke'
    And I am using the default header settings
    When I 'POST' this request
    Then I receive the HTTP status code 'NoContent'
    Given the request url is '/delegations/<revoke_delegation_id>:revoke'
    And I am using the default header settings
    When I 'POST' this request
    Then I receive the HTTP status code 'BadRequest'

  @single-result @command-expire @command-delete @success
  Scenario: Expiring a delegation invite prevents further actions and it can then be deleted
    Given I am an Owner user
    And the request url is '/delegations'
    And I am using the default header settings
    And the JSON payload is:
    """
    {
      "county_parish_holding_id": "ab820003-0000-4000-8000-000000000003",
      "delegating_user_id": "0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1",
      "delegated_user_role_id": "0c15ba2f-b4ba-406a-a0ae-213de64600a9",
      "delegated_user_email": "test2@test.com"
    }
    """
    When I 'POST' this request
    Then I receive the HTTP status code 'Created'
    And I have received JSON data in the response
    And I save the value of property "id" to the context using name "expire_delegation_id"
    Given the request url is '/delegations/<expire_delegation_id>:expire'
    And I am using the default header settings
    When I 'POST' this request
    Then I receive the HTTP status code 'NoContent'
    Given the request url is '/delegations/<expire_delegation_id>:expire'
    And I am using the default header settings
    When I 'POST' this request
    Then I receive the HTTP status code 'BadRequest'
    Given the request url is '/delegations/<expire_delegation_id>'
    And I am using the default header settings
    When I 'DELETE' this request
    Then I receive the HTTP status code 'NoContent'
    Given the request url is '/delegations/<expire_delegation_id>'
    And I am using the default header settings
    When I 'DELETE' this request
    Then I receive the HTTP status code 'BadRequest'

  @single-result @error @error-not-found
  Scenario Outline: Performing an action on an unknown delegation returns a 404
    Given I am an Owner user
    And the request url is '/delegations/dd999999-0000-4000-8000-000000000099<suffix>'
    And I am using the default header settings
    When I '<verb>' this request
    Then I receive the HTTP status code 'NotFound'
  Examples:
    | suffix  | verb   |
    |         | GET    |
    |         | DELETE |
    | :accept | POST   |
    | :reject | POST   |
    | :revoke | POST   |
    | :expire | POST   |
