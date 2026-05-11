@system @header
Feature: CPFeature: Header tests

  @error @error-bad-request
  Scenario: Make sure that a missing x-api-key header returns an error
    Given I am an Owner user
    And the request url is '/cphs?pageNumber=1&pageSize=5'
    When I 'GET' this request
    Then I receive the HTTP status code 'BadRequest'
    And the response contains the following values:
      | path                 | value                         |
      | error.code           | missing_header                |
      | error.message        | Header x-api-key is required. |
      | error.path           | /cphs                         |
      | error.details.header | x-api-key                     |

  @error @error-bad-request
  Scenario: Make sure that an invalid x-api-key header returns an error
    Given I am an Owner user
    And the request url is '/cphs?pageNumber=1&pageSize=5'
    And I am using the following headers:
      | header    | value     |
      | x-api-key | not-a-key |
    When I 'GET' this request
    Then I receive the HTTP status code 'BadRequest'
    And the response contains the following values:
      | path                 | value                          |
      | error.code           | invalid_api_key                |
      | error.message        | Header x-api-key is not valid. |
      | error.path           | /cphs                          |
      | error.details.header | x-api-key                      |

  @error @error-bad-request
  Scenario: Make sure that a missing x-correlation-id returns an error
    Given I am an Owner user
    And the request url is '/cphs?pageNumber=1&pageSize=5'
    And I am using the following headers:
      | header    | value |
      | x-api-key | test  |
    When I 'GET' this request
    Then I receive the HTTP status code 'BadRequest'
    And the response contains the following values:
      | path                 | value                                |
      | error.code           | missing_header                       |
      | error.message        | Header x-correlation-id is required. |
      | error.path           | /cphs                                |
      | error.details.header | x-correlation-id                     |

  @success
  Scenario: When i try to get a cph list with an x-api-key and x-correlation-id header then this should pass.
    Given I am an Owner user
    And the request url is '/cphs?pageNumber=1&pageSize=5'
    And I am using the following headers:
      | header           | value                                |
      | x-api-key        | test                                 |
      | x-correlation-id | 42b1849c-2dee-4cb8-a7b3-b011727b062b |
    When I 'GET' this request
    Then I receive the HTTP status code 'Ok'
