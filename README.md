# Identity Service Helper

Core delivery C# ASP.NET backend service.

* [Overview](#overview)
* [Architecture](#architecture)
* [Integrations](#integrations)
* [API and authentication](#api-and-authentication)
* [Configuration](#configuration)
* [Docker Compose](#docker-compose)
* [PostgreSQL](#postgresql)
* [Running](#running)
* [Testing](#testing)
* [HTTP Client Tests](#http-client-tests)
* [How to create Database Migrations](#how-to-create-database-migrations)
* [SonarCloud](#sonarcloud)
* [Dependabot](#dependabot)
* [About the licence](#about-the-licence)


## Overview

Identity Service Helper is a CDP-style C# / ASP.NET Core backend service. It keeps data in PostgreSQL and exposes an HTTP API (secured with an API key) for reading and writing it, while keeping that data in step with external systems through a set of background integrations.

At a high level the service:

- exposes a JSON HTTP API over its PostgreSQL-backed data, authenticated with an API key;
- synchronises Keeper Reference Data (sites, parties, species and related reference data) from the external **KRDS** API;
- consumes events from an **AWS SQS** queue (for example, notification that a keeper-data import has completed);
- runs **scheduled jobs** (Quartz) that drive the KRDS synchronisation and messaging on a configurable cadence;
- sends outbound messages through **GOV.UK Notify**, using an outbox table so messages are persisted before dispatch.

`SampleEntity` is the example domain entity used to demonstrate the CRUD slice; replace or extend it with the real entities for the service.

## Architecture

The solution follows a layered structure under `src/`:

- **Api** - the ASP.NET Core host: minimal-API endpoints, API-key / correlation-id / operator-id middleware, request validation, error handling and OpenAPI.
- **Services** - application/business logic for the domain operations.
- **Models** - request and response contracts and their validators.
- **Repositories** - data-access abstractions over the database.
- **Database/Postgres.Database** - EF Core `DbContext` (separate read-write and read-only contexts), entity configuration and migrations.
- **Integrations/** - external-system adapters: `Krds`, `Queues`, `Schedules`, `Messaging` and `Ingest` (see [Integrations](#integrations)).

Tests live under `tests/` (see [Testing](#testing)).

## Integrations

- **KRDS** (`Integrations/Krds`) - REST client that fetches Keeper Reference Data (sites, parties, roles, species) from the external KRDS API. It handles its own access-token acquisition for authenticating to KRDS.
- **Queues** (`Integrations/Queues`) - consumes AWS SQS messages (for example `KeeperDataImportComplete`). LocalStack provides SQS locally.
- **Scheduling** (`Integrations/Schedules`) - Quartz jobs that run the KRDS sync and messaging on configured schedules.
- **Messaging** (`Integrations/Messaging`) - sends outbound messages to GOV.UK Notify, persisting them first in an outbox table.
- **Ingest** (`Integrations/Ingest`) - maps and loads data fetched from external systems into the service's database.

## API and authentication

The service exposes a JSON HTTP API. All endpoints require an API key, supplied in a request header and checked by middleware. Requests also carry a correlation id and an operator id (the id of the user or service on whose behalf the call is made); the operator id is recorded against the data for auditing. A health endpoint is exposed for platform checks.

## Configuration

Configuration is supplied through `appsettings*.json` and, in CDP environments, environment variables. The main groups are the PostgreSQL connection strings, the API key, the KRDS API settings and credentials, the AWS / SQS settings, and the scheduling intervals.

## Docker Compose

A Docker Compose template is in [compose.yml](compose.yml).

A local environment with:

- Localstack for AWS services (S3, SQS)
- Redis
- Postgres
- This service.
- A commented out frontend example.

```bash
docker compose up --build -d
```

A more extensive setup is available in [github.com/DEFRA/cdp-local-environment](https://github.com/DEFRA/cdp-local-environment)

## PostgreSQL

The database is configured to use PostgreSQL and to use Liquibase to manage migrations.

The best way to run Liquibase locally is to use Homebrew to install
```shell
brew install liquibase
```

> Liquibase properties file in the root of the project defines all the properties that can be used 
> to configure the database.

Liquibase is a Java-based tool that can be used to manage database migrations. Therefore you will need to have Java 
installed. On both Linux and MacOS you can install Java using Homebrew.

```shell
  brew install openjdk@25
```

Running liquibase migrations locally:
 

``` bash
 liquibase update 

```

## Running

Run the identity-service-helper:
```bash
dotnet run --project ./src/Api --launch-profile Development
```

## Testing

Run the tests with:

```bash
dotnet test
```

The unit tests use [xUnit](https://xunit.net/) and mock their dependencies with [NSubstitute](https://nsubstitute.github.io/). The database-backed tests run against a real PostgreSQL instance started on demand with [Testcontainers](https://testcontainers.com/), so Docker must be running for those tests to pass.

## HTTP Client Tests

To run HTTP Client tests locally, you can use the [ijhttp](https://github.com/asimmon/ijhttp) tool.
On Mac/Linux install using `brew install ijhttp`; on Windows use Chocolatey `choco install ijhttp`.
Then run the following command from the root of the project.
```shell
ijhttp --env-file ./tests/Endpoint.Tests/Tests/http-client.env.json --env local ./tests/Endpoint.Tests/**/**/**.http
```

To include the HTTP client tests in code coverage, install [`dotnet-coverage`](https://learn.microsoft.com/en-us/dotnet/core/additional-tools/dotnet-coverage) and run:

```shell
bash ./tests/Endpoint.Tests/run-http-tests-with-coverage.sh --configuration Release --env local
```

This writes `coverage/Api.Http.cobertura.xml`, which can be merged with the existing `dotnet test` Cobertura files. The Cake build now exposes a `TestAll` target to run both the `.csproj` tests and the HTTP tests before generating the combined coverage report.

## How to create Database Migrations

On this project we make use of a hybrid approach to database migrations, making use of **Liquibase** to manage the database 
schema and  **Entity Framework Core Migrations**.  This may at first seem counter-intuitive, but it has proven to be a 
reliable approach, although it does require some additional configuration and manual steps.

If you make changes to the database schema, you will need to create a new migration.

To create a new migration, change into the `src/Database` directory and run dotnet ef migrations as follows:

```bash
cd src/Database
dotnet ef migrations add <migration-name>    ### <Migration-name> to be replaced with a meaningful name  i.e. AddDeltaTable

```
The above command will create a new migration in the `Migrations` directory, within the Database project.
In the Database folder there is a Console Application project named `Postgres.Database.Console` that can be run, which will
execute the migration against your local database.

The Console application will then automatically apply this migration to the database. In the local development environment, if
the project is run, there is code within the `ServiceCollectionExtensions` class that will automatically apply the
migrations to the database on Debug builds.

``` csharp
   #if DEBUG
    // Migrate the database on startup in development mode
     if (scope.ServiceProvider.GetRequiredService<IHostEnvironment>().IsDevelopment())
     {
       context.Database.Migrate();
     }
    #endif
 ```

> Note: We still need to run the migrations locally, to test migrations, etc.

Once you are satisfied with the migration, and it works and has been tested locally, and you have created Unit Tests etc., you can then generate a SQL script using 
the EF Core Migrations script command, defining the output folder as the schema folder in the project.

```bash
### <meaningful-name> to be replaced with a meaningful name  i.e. AddDeltaTable
 dotnet ef migrations script  <meaningful-name> -o ../../database/schema/<meaningful-name>.sql

```
This will generate a SQL script that can be used to apply the migration to a production environment. However,
before the script is applied there may be some manual steps required to edit the file.

> Note: The SQL script generated by EF Core Migrations script command will include statements that cannot be used in Liquibase
> migration scripts.

Once you have edited your script to make it parseable and usable in Liquibase, you can add the script to the Liquibase
changelog file.

``` bash

# Remove any references to Migrations table creations etc

CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);


# Remove any Transaction Scope statements
# Liquibase will wrap everything in a transaction scope by default 

START TRANSACTION;
DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'defra-ci') THEN
        CREATE SCHEMA "defra-ci";
    END IF;
END $EF$;

## Should just become 
 CREATE SCHEMA "defra-ci";
```

## SonarCloud

Example SonarCloud configurations are available in the GitHub Action workflows.

## Dependabot

We have added an example dependabot configuration file to the repository. You can enable it by renaming
the [.github/example.dependabot.yml](.github/example.dependabot.yml) to `.github/dependabot.yml`

## About the licence

The Open Government Licence (OGL) was developed by the Controller of Her Majesty's Stationery Office (HMSO) to enable
information providers in the public sector to license the use and re-use of their information under a common open
licence.

It is designed to encourage use and re-use of information freely and flexibly, with only a few conditions.
