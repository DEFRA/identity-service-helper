# Configuration reference

Configuration for **identity-service-helper**. Part of the [runbook](runbook.md).

## Sources and precedence

Loaded in [Program.cs](../../src/Api/Program.cs), later sources override earlier ones:

1. [appsettings.json](../../src/Api/appsettings.json) (always required)
2. `appsettings.{Environment}.json` (e.g. [appsettings.Development.json](../../src/Api/appsettings.Development.json) when `ASPNETCORE_ENVIRONMENT=Development`)
3. **Environment variables** — nested keys use `__` (double underscore), e.g. `ConnectionStrings__PostgresConnection`, `KrdsApi__ClientSecret`
4. Command-line arguments

In CDP environments, values marked *"Set in cdp-app-config"* in appsettings are supplied as
environment variables / secrets through the platform's `cdp-app-config` mechanism.

## Settings

### Top level

| Key | Purpose | Default / notes |
|---|---|---|
| `DefraIdentityApiKey` | The API key every request must present in the `x-api-key` header | `test` in repo defaults — **must be overridden (secret) in deployed environments** |
| `TraceHeader` | Header propagated to outbound HTTP calls and used to enrich logs with a correlation id | `x-cdp-request-id` |
| `AllowedHosts` | ASP.NET host filtering | `*` |
| `SERVICE_VERSION` (env var) | Stamped on every log record as `service.version` | Unset locally |
| `ASPNETCORE_FORWARDEDHEADERS_ENABLED` | Trust `X-Forwarded-*` from the platform proxy | `true` (set in the [Dockerfile](../../Dockerfile)) |
| `PORT` | HTTP port the container listens on | `8080` in compose |
| `CDP_HTTPS_PROXY` | Egress proxy used by the named `proxy` HTTP client | Set by the platform |

### Database — `PostgresConfiguration` + `ConnectionStrings`

Consumed by [Postgres.Database ServiceCollectionExtensions](../../src/Database/Postgres.Database/ServiceCollectionExtensions.cs).

| Key | Purpose | Default / notes |
|---|---|---|
| `PostgresConfiguration:UseIamAuthentication` | Use short-lived RDS IAM tokens instead of passwords (via `PostgresIamTokenGeneratorService`, region from `AWS:Region`, default `eu-west-2`) | `false` |
| `PostgresConfiguration:DefaultHost` | Read-write cluster endpoint | `identity-service-helper.cluster-cpiiyum4wb06.eu-west-2.rds.amazonaws.com` |
| `PostgresConfiguration:ReadOnlyHost` | Read-only cluster endpoint | `identity-service-helper.cluster-ro-cpiiyum4wb06.eu-west-2.rds.amazonaws.com` |
| `PostgresConfiguration:Port` | PostgreSQL port | `5432` |
| `PostgresConfiguration:Name` | Database name | Set in cdp settings |
| `PostgresConfiguration:User` | Database user | Set in cdp settings |
| `ConnectionStrings:PostgresConnection` | Full connection string for the **read-write** `PostgresDbContext` (used when not using IAM auth, e.g. locally) | Local: `User ID=identity_service_helper_ddl;Password=postgres;Host=localhost;Database=identity_service_helper;` |
| `ConnectionStrings:ReadOnlyPostgresConnection` | Connection string for the **read-only**, no-tracking `ReadOnlyPostgresDbContext` | Same as above locally |

Hard-coded resilience (not configurable): 5 retries on transient SQLSTATEs, max retry delay 10 s,
command timeout 60 s. Sensitive-data logging is enabled **only** when IAM auth is off (local dev).

### AWS — `AWS`

Consumed by [QueueManagement ServiceCollectionExtensions](../../src/Integrations/Queues/QueueManagement/ServiceCollectionExtensions.cs).

| Key | Purpose | Default / notes |
|---|---|---|
| `AWS:UseLocalStack` | When `true`, builds an explicit `AmazonSQSClient` against `ServiceURL`; when `false`, the default AWS credential chain (task/instance IAM role) is used | `true` only in Development |
| `AWS:ServiceURL` | LocalStack endpoint | `http://localhost:4566` (compose override: `http://localstack:4567`) |
| `AWS:Region` | AWS region (also used for RDS IAM token signing) | `eu-west-2` |
| `AWS:AccessKey` / `AWS:SecretKey` | Static credentials — **LocalStack only** (`test`/`test` from [compose/aws.env](../../compose/aws.env)) | Never set in deployed environments |

### Queue — `QueueOptions:IntakeQueueOptions`

| Key | Purpose | Default |
|---|---|---|
| `Url` | SQS queue URL polled for intake messages (`identity_service_helper_intake`) | Set in cdp-app-config; local: `http://localhost:4566/000000000000/identity_service_helper_intake` |
| `WaitTimeSeconds` | SQS long-poll wait | `20` |
| `MaxNumberOfMessages` | Max messages per receive | `1` |
| `SupportedMessageTypes` | Message types handled | `["ls_keeper_data_import_complete"]` |

### KRDS — `KrdsApi`

Consumed by the [Krds integration](../../src/Integrations/Krds/); token acquired with OAuth2
client credentials against Cognito.

| Key | Purpose | Default / notes |
|---|---|---|
| `Url` | KRDS API base URL | `https://ls-keeper-data-api.api.ext-test.cdp.defra.gov.uk/api/` |
| `Key` | KRDS API key | **Secret** — set in cdp-app-config |
| `ClientId` / `ClientSecret` | OAuth2 client credentials | **Secret** — set in cdp-app-config |
| `TokenUrl` | Cognito token endpoint | `https://ls-keeper-data-api-8ec5c.auth.eu-west-2.amazoncognito.com/oauth2/token` |

### Scheduling — `Scheduling`

Quartz cron expressions (seconds-precision), bound in
[Scheduling ServiceCollectionExtensions](../../src/Integrations/Schedules/Scheduling/ServiceCollectionExtensions.cs).
**Startup throws if a cron value is missing.**

| Key | Job | Default |
|---|---|---|
| `Scheduling:KeeperReferenceData:Cron` | `KeeperReferenceDataJob` (KRDS sites sync) | `0 0 0 ? * SUN` — weekly, Sunday 00:00 UTC |
| `Scheduling:Messaging:Cron` | `MessagingJob` (outbox dispatch) | `0/15 * * * * ?` — every 15 seconds |

### Email — `Email`

GOV.UK Notify, consumed by the [Messaging integration](../../src/Integrations/Messaging/).

| Key | Purpose | Notes |
|---|---|---|
| `Email:ApiKey` | Notify API key | **Secret**. Repo default is a Notify *test* key (`authtestkey-…`) |
| `Email:ReplyToId` | Notify reply-to address id | Set in cdp-app-config |

### Logging — `Serilog`

| Environment | Sink / format |
|---|---|
| Deployed (appsettings.json) | Console with `Elastic.CommonSchema.Serilog.EcsTextFormatter` (ECS JSON, one line per event) |
| Development | Console with a human-readable output template |

Minimum level `Information` (also for `Microsoft`/`System` overrides).

### Dead configuration

`PolledServices:AzureB2CSyncService` (in both appsettings files) has **no references in code** —
nothing reads or schedules it. Treat as legacy; do not expect a B2C sync to run.

## Local infrastructure ports

From [compose.yml](../../compose.yml) / [compose.override.yml](../../compose.override.yml):

| Service | compose.yml | compose.override.yml |
|---|---|---|
| API (`identity-service-helper`) | `8080` | `3001` (container still 8080) |
| LocalStack gateway | `4566` | `4567` |
| PostgreSQL | `5432` | `5432` |
| Redis | `6379` | `6380` (declared in compose; the API has no Redis configuration) |
| Docker network | `cdp-tenant` (created) | `identity-services` (**external — create it first**: `docker network create identity-services`) |

LocalStack bootstrap ([compose/start-localstack.sh](../../compose/start-localstack.sh)) creates SQS queue
`identity_service_helper_intake`, SNS topic `ls_keeper_data_import_complete`, and subscribes the
queue to the topic. Container images: `postgres:16-alpine`, `liquibase/liquibase:5.0.1`,
`localstack/localstack:4.10.0`, `redis:7.2.3-alpine3.18`.

## Secrets checklist (deployed environments)

- `DefraIdentityApiKey`
- `KrdsApi__Key`, `KrdsApi__ClientId`, `KrdsApi__ClientSecret`
- `Email__ApiKey` (and `Email__ReplyToId`)
- Database credentials (`ConnectionStrings__*` or `PostgresConfiguration__*` with IAM auth)
- `QueueOptions__IntakeQueueOptions__Url` (environment-specific, not secret but per-env)
