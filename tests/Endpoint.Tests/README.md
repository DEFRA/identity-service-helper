# Tests for the Endpoints

## Configuration
### http-client.env.json
This file holds the default values that are used in the test runs.
``` json
{
  "local": {
    // data-scaffold settings
    "host": "http://localhost:5000",
    "db-url": "postgresql://localhost:5432/identity_service_helper",
    "db-user": "identity_service_helper_ddl",
    "db-passowrd": "postgres",
    // app settings
    "api-key": "test",
    "invalid-api-key": "invalid-api-key",
    "operator-id": "05FB274F-B1ED-4F3F-9F2C-2B7118CA722A",
    "correlation-id": "05FB274F-B1ED-4F3F-9F2C-2B7118CA722A",
    "application-id": "05FB274F-B1ED-4F3F-9F2C-2B7118CA722A",
    "id": ""
  }
}
```
### http-client.private.env.json
This file holds the values that are specific to your workstation.

``` json
{
  "local": {
    "dotnet-root": "<PATH_TO_DOTNET>",
    "repo-root": "<PATH_TO_YOUR_REPO>"
  }
}
```
`dotnet-root` points to where the dotnet executable is. 
on mac/linux -> /usr/local/share/dotnet
on windows -> C:\Program Files\dotnet

`repo-root` points to the root of the checked out repo.