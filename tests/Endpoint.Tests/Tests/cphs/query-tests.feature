@cph @query
Feature: CPH query tests

  @paged-result @success @data-live-only
  Scenario: Get a maximum of 5 cphs from the first page excluding expired
    Given I am an Owner user
    And the request url is '/cphs?pageNumber=1&pageSize=5'
    And I am using the default header settings
    When I 'GET' this request
    Then I receive the HTTP status code 'OK'
    And I have received JSON data in the response
    And the paged response is on page 1 has a total count of 6 and contains 5 objects
    And the response contains the following values:
      | path                | value       |
      | items[0].cph_number | 44/000/0001 |
      | items[1].cph_number | 44/000/0002 |
      | items[2].cph_number | 44/000/0003 |
      | items[3].cph_number | 44/000/0004 |
      | items[4].cph_number | 44/000/0011 |

  @paged-result @success @data-live-only
  Scenario: Get a maximum of 5 cphs from the second page excluding expired
    Given I am an Owner user
    And the request url is '/cphs?pageNumber=2&pageSize=5'
    And I am using the default header settings
    When I 'GET' this request
    Then I receive the HTTP status code 'OK'
    And I have received JSON data in the response
    And the paged response is on page 2 has a total count of 6 and contains 1 objects
    And the response contains the following values:
      | path                | value       |
      | items[0].cph_number | 44/000/0023 |

  @paged-result @success @data-all
  Scenario: Get a maximum of 5 cphs from the first page including expired
    Given I am an Owner user
    And the request url is '/cphs?pageNumber=1&pageSize=5&expired=true'
    And I am using the default header settings
    When I 'GET' this request
    Then I receive the HTTP status code 'OK'
    And I have received JSON data in the response
    And the paged response is on page 1 has a total count of 9 and contains 5 objects
    And the response contains the following values:
      | path                | value       |
      | items[0].cph_number | 44/000/0001 |
      | items[1].cph_number | 44/000/0002 |
      | items[2].cph_number | 44/000/0003 |
      | items[3].cph_number | 44/000/0004 |
      | items[4].cph_number | 44/000/0005 |
    
  @paged-result @success @data-all
  Scenario: Get a maximum of 5 cphs from the second page including expired
    Given I am an Owner user
    And the request url is '/cphs?pageNumber=2&pageSize=5&expired=true'
    And I am using the default header settings
    When I 'GET' this request
    Then I receive the HTTP status code 'OK'
    And I have received JSON data in the response
    And the paged response is on page 2 has a total count of 9 and contains 4 objects
    And the response contains the following values:
      | path                | value       |
      | items[0].cph_number | 44/000/0011 |
      | items[1].cph_number | 44/000/0023 |
      | items[2].cph_number | 44/000/0024 |
      | items[3].cph_number | 44/000/0025 |

  @single-result @success @data-live-only
  Scenario Outline: Getting a CPH by an id returns a CPH
    Given I am an Owner user
    And the request url is '/cphs/<uri_value>'
    And I am using the default header settings
    When I 'GET' this request
    Then I receive the HTTP status code 'OK'
    And I have received JSON data in the response
    And the value of property "<property_name>" is the same as "<check_value>"
  Examples:  
    | id          | uri_value                            | property_name | check_value                          |
    | internal id | 4435a146-d0ac-4260-8a27-c550e0ed9563 | cph_number    | 44/000/0001                          |
    | cph number  | 44/000/0001                          | id            | 4435a146-d0ac-4260-8a27-c550e0ed9563 |

  @error @error-not-found
  Scenario Outline: Getting a CPH by an unkown id returns a 404
    Given I am an Owner user
    And the request url is '/cphs/<uri_value>'
    And I am using the default header settings
    When I 'GET' this request
    Then I receive the HTTP status code 'NotFound'
    And I have an empty response
  Examples:  
    | id          | uri_value                            |
    | internal id | 4435a146-9999-9999-8a27-c550e0ed9563 |
    | cph number  | 99/999/9999                          |

  @paged-result @sorted @success @data-live-only
  Scenario: Get a maximum of 5 cphs from the first page excluding expired and order by descending on cph_number
    Given I am an Owner user
    And the request url is '/cphs?pageNumber=1&pageSize=5&orderByDescending=true'
    And I am using the default header settings
    When I 'GET' this request
    Then I receive the HTTP status code 'OK'
    And I have received JSON data in the response
    And the paged response is on page 1 has a total count of 6 and contains 5 objects
    And the response contains the following values:
      | path                | value       |
      | items[0].cph_number | 44/000/0023 |
      | items[1].cph_number | 44/000/0011 |
      | items[2].cph_number | 44/000/0004 |
      | items[3].cph_number | 44/000/0003 |
      | items[4].cph_number | 44/000/0002 |
    
  @paged-result @sorted @success @data-all
  Scenario: Get a maximum of 5 cphs from the first page including expired and order by descending on cph_number
    Given I am an Owner user
    And the request url is '/cphs?pageNumber=1&pageSize=5&expired&orderByDescending=true'
    And I am using the default header settings
    When I 'GET' this request
    Then I receive the HTTP status code 'OK'
    And I have received JSON data in the response
    And the paged response is on page 1 has a total count of 9 and contains 5 objects
    And the response contains the following values:
      | path                | value       |
      | items[0].cph_number | 44/000/0025 |
      | items[1].cph_number | 44/000/0024 |
      | items[2].cph_number | 44/000/0023 |
      | items[3].cph_number | 44/000/0011 |
      | items[4].cph_number | 44/000/0005 |
