# Troubleshooting

Symptom → cause → resolution for **identity-service-helper**. Part of the [runbook](runbook.md).

## Error response formats

The service produces errors in three shapes — knowing which one you're looking at tells you
*where* the request failed:

### 1. Header validation (middleware) — request never reached the endpoint

Produced by [JsonErrorMiddleware](../../src/Api/Middleware/JsonErrorMiddleware.cs) subclasses, always HTTP 400:

```json
{
  "error": {
    "code": "missing_header",
    "message": "Header x-api-key is required.",
    "traceId": "<HttpContext.TraceIdentifier>",
    "path": "/users",
    "details": { "header": "x-api-key" }
  }
}
```

| `code` | Meaning |
|---|---|
| `missing_header` | `x-api-key`, `x-correlation-id`, or `x-operator-id` absent/blank |
| `invalid_api_key` | `x-api-key` present but doesn't match `DefraIdentityApiKey` |
| `invalid_header` | `x-operator-id` present but not a valid GUID |

### 2. Request body validation — HTTP 422

FluentValidation failures from `ValidationFilter<T>` return `422 Unprocessable Entity`
with problem details describing the failing properties.

### 3. Unhandled/domain exceptions — RFC 7807 problem details

Produced by [ApiExceptionHandler](../../src/Api/Exceptions/ApiExceptionHandler.cs):

| Exception | Status | Title |
|---|---|---|
| `NotFoundException` | 404 | Not Found |
| `ConflictException` | 409 | Conflict |
| `BusinessRuleException` | 400 | Bad Request |
| `ArgumentException` | 400 | Bad Request |
| `UnauthorizedAccessException` | 403 | Forbidden |
| anything else | 500 | Internal Server Error |

The response includes `detail` (exception message), `instance` (path) and a `traceId` extension.
5xx are logged as errors with the full exception; 4xx as warnings.

## Tracing a request through the logs

Deployed logs are ECS JSON on stdout. Keys to pivot on:

- The platform trace header `x-cdp-request-id` — enriched onto every log event as the correlation id ([CdpLogging.cs](../../src/Api/Utility/Logging/CdpLogging.cs)).
- On errors, the exception handler adds `CorrelationId` (from `x-correlation-id`), `TraceId`, `Path`, `StatusCode`.
- `traceId` in any error response body = `HttpContext.TraceIdentifier` — search the logs for it.
- `service.version` identifies the deployed build.

## Symptoms

### API returns 400 for every request

- **Cause**: missing/wrong `x-api-key` or missing `x-correlation-id` (see envelope `code`).
- **Check**: does the `error.code` say `invalid_api_key`? Then the caller's key doesn't match the environment's `DefraIdentityApiKey`.
- **Fix**: align the caller's key with the environment secret. Remember `/health` is the only endpoint exempt from these headers.

### Mutations fail with 400 but reads work

- **Cause**: `x-operator-id` missing or not a GUID — required on every POST/PUT/DELETE (`RequiresOperatorId` metadata).
- **Fix**: send a valid GUID identifying the acting user/service; it is stored for audit.

### Service won't start

| Log/symptom | Cause | Fix |
|---|---|---|
| `Cron expression is missing for KeeperReferenceData…` / `…Messaging…` (`ArgumentException`) | `Scheduling:*:Cron` not supplied | Restore the config keys ([reference](configuration.md#scheduling--scheduling)) |
| `Configuration section 'QueueOptions' not found` (`InvalidOperationException`) | Queue config missing | Restore `QueueOptions:IntakeQueueOptions` |
| Startup hangs then DB errors | PostgreSQL unreachable — the app opens a connection at startup (`UsePostgresDatabase`) | Verify connectivity/credentials; see below |
| `appsettings.json` not found | Working directory wrong — config is loaded with `optional: false` from the current directory | Run from the publish/app directory |

### Database errors (500s, `Npgsql` exceptions in logs)

- Transient errors (deadlock, too-many-connections, connection drops) are retried automatically 5× with up to 10 s delay; sustained failures indicate a real outage or credential problem.
- **IAM auth environments** (`PostgresConfiguration:UseIamAuthentication=true`): token generation needs valid AWS credentials and the right `AWS:Region`; check for `PostgresIamTokenGeneratorService` errors. IAM tokens are short-lived — clock skew or lost IAM role permissions (`rds-db:connect`) break new connections.
- **Read-only vs read-write**: reads go through `ReadOnlyPostgresDbContext` (the `ReadOnlyHost` / `ReadOnlyPostgresConnection`); if only reads fail, suspect the RO endpoint.
- Command timeout is 60 s — queries exceeding it fail; look for lock contention.

### Liquibase / schema drift

- Compare `public.databasechangelog` against [changelog/db.changelog.xml](../../changelog/db.changelog.xml); each changeset should appear exactly once (`exectype` `EXECUTED`).
- A stuck `databasechangeloglock` (from a crashed migration run) blocks future runs: check `SELECT * FROM databasechangeloglock;` and release with `liquibase release-locks` (or clear the lock row) **after confirming no migration is actually running**.
- Locally, the `liquibase` compose service must complete successfully before the API starts (`service_completed_successfully`); `docker compose logs liquibase` shows why it failed (usually Postgres not healthy yet or a malformed changeset).
- Remember the hybrid workflow: EF Core migrations generate SQL that is hand-edited into Liquibase changesets ([README](../../README.md#how-to-create-database-migrations)) — EF's `__EFMigrationsHistory` bookkeeping and transaction statements must be stripped, otherwise Liquibase fails to parse.

### KRDS sync not updating data

- **Where to look**: `krds_sync_logs` table (endpoint, HTTP status, success flag, correlation id) and job logs (`KeeperReferenceDataJob`).
- Job logs `failed` with HTTP 401/403 → token problem: verify `KrdsApi:ClientId/ClientSecret/TokenUrl` (Cognito) and `KrdsApi:Key`.
- Job never appears in the logs → check `Scheduling:KeeperReferenceData:Cron` — default is **weekly (Sunday 00:00 UTC)**, so absence during the week is normal; the job also skips overlap (`DisallowConcurrentExecution`).
- The scheduled job fetches only sites changed in the **last 24 hours**; a full refresh is driven by the import-complete message flow below.

### SQS messages not being processed / queue depth growing

- Confirm the poller started: startup wiring is in [QueueManagement](../../src/Integrations/Queues/QueueManagement/ServiceCollectionExtensions.cs); processing logs `Processing KeeperDataImportComplete message.`
- Handler returns *Failed* (message becomes visible again and is retried) when either CPH or Roles ingest fails — look for ingest errors immediately after the processing log line.
- Verify `QueueOptions:IntakeQueueOptions:Url` matches the environment's queue and that the runtime identity has SQS receive/delete permissions.
- Only `ls_keeper_data_import_complete` messages are handled; other message types are not supported.
- **Local**: LocalStack must be healthy and bootstrapped — `docker compose logs localstack` should show the queue and topic created by [start-localstack.sh](../../compose/start-localstack.sh). With the override file, the queue URL points at port `4567`.

### Emails / SMS not being sent

- Messages are queued in the `external_messaging` outbox and dispatched by `MessagingJob` every 15 s; per-message Notify responses (`notify_id`, response code/message) are recorded on the row.
- Job log line `Processed Email X-S, Y-F, Text Z-S, W-F` — non-zero `-F` counts mean Notify rejections: inspect the stored response columns.
- No job log lines at all → scheduling/config problem (see *Service won't start*).
- Sandbox/test Notify keys (repo default `authtestkey-…`) accept but do not deliver — verify `Email:ApiKey` is the correct live key in deployed environments.

### Local compose stack problems

| Symptom | Fix |
|---|---|
| API container restarts, DB connection refused | Postgres not healthy yet or Liquibase failed — `docker compose ps`, then `docker compose logs liquibase postgres` |
| `network identity-services not found` (override file) | `docker network create identity-services` |
| Port already in use | compose uses 8080/5432/4566/6379 (override: 3001/5432/4567/6380) — stop the conflicting process or use the override mapping |
| Tests fail with Docker errors | Testcontainers needs a running Docker daemon; DB tests spin up `postgres:16` + `liquibase` containers |

### Correlation id present but no logs found

- The **request-scoped** id the API validates is `x-correlation-id`; the **log-enrichment** id comes from `x-cdp-request-id` (`TraceHeader`). Callers should send both; search logs by whichever the caller supplied, and by the `traceId` from any error body.
