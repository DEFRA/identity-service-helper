@cph @command
Feature: CPH command tests

  @single-result @command-expire
  Scenario Outline: Expire a cph using the <scenario> results in a <http_result> result
    Given I am an Owner user
    And the request url is '/cphs/<uri_value>:expire'
    And I am using the default header settings
    When I 'POST' this request
    Then I receive the HTTP status code '<http_result>'

  @success
  Examples:  
    | scenario              | uri_value                            | http_result |
    | available internal id | 204459b1-3a07-4e65-9122-91c1699e3d3f | NoContent   |
    | available cph number  | 44/000/0004                          | NoContent   |
  
  @error @error-not-found
  Examples:  
    | scenario            | uri_value                            | http_result |
    | deleted internal id | 82181a8b-7f7f-470c-9263-2b94675599df | NotFound    |
    | deleted cph number  | 44/000/0006                          | NotFound    |
    | unknown internal id | d4a7f0d7-9999-9999-9999-547a5f494738 | NotFound    |
    | unknown cph number  | 99/999/9876                          | NotFound    |

  @error @error-conflict
  Examples:  
    | scenario            | uri_value                            | http_result |
    | expired internal id | 7973060a-d483-4ad4-9716-c70415ed620a | Conflict    |
    | expired cph number  | 44/000/0005                          | Conflict    |
    
  @single-result @command-delete
  Scenario Outline: Delete a cph using the <scenario> results in a <http_result> result
    Given I am an Owner user
    And the request url is '/cphs/<uri_value>'
    And I am using the default header settings
    When I 'DELETE' this request
    Then I receive the HTTP status code '<http_result>'

  @success
  Examples:  
    | scenario              | uri_value                            | http_result |
    | available internal id | 4435a146-d0ac-4260-8a27-c550e0ed9563 | NoContent   |
    | available cph number  | 44/000/0003                          | NoContent   |
    | expired internal id   | d9a711ec-722d-49b6-abcc-23f0795e3886 | NoContent   |
    | expired cph number    | 44/000/0024                          | NoContent   |

  @error @error-not-found
  Examples:  
    | scenario              | uri_value                            | http_result |
    | deleted internal id   | 82181a8b-7f7f-470c-9263-2b94675599df | NotFound    |
    | deleted cph number    | 44/000/0006                          | NotFound    |
    | unknown internal id   | d4a7f0d7-9999-9999-9999-547a5f494738 | NotFound    |
    | unknown cph number    | 99/999/9876                          | NotFound    |
          