# API reference

Full endpoint catalogue for **identity-service-helper**. Part of the [runbook](runbook.md).

## Conventions

- **Headers** — every endpoint except `GET /health` requires:
  - `x-api-key` — must equal the environment's `DefraIdentityApiKey`.
  - `x-correlation-id` — any non-empty string, used for tracing.
  - Rows marked **Operator** additionally require `x-operator-id` — a valid **GUID** identifying the acting user/service, recorded for auditing (all POST/PUT/DELETE endpoints).
- **JSON** — request and response bodies use `snake_case` property names (e.g. `first_name`, `county_parish_holding_number`).
- **Query strings** — bound by property name (case-insensitive), e.g. `?pageNumber=1&pageSize=20`.
- **Errors** — header failures return a `400` JSON `error` envelope; body-validation failures return `422`; domain errors return RFC 7807 problem details. See [troubleshooting](troubleshooting.md#error-response-formats).
- Route ids constrained `:guid` return **404** for non-GUID values (no route match).

## Endpoint summary

| Method | Route | Operator | Success | Errors |
|---|---|---|---|---|
| GET | `/health` | — | 200 | — |
| GET | `/users` | — | 200 | — |
| GET | `/users/{id}` | — | 200 | 404 |
| POST | `/users` | ✔ | 201 | 422 |
| PUT | `/users/{id}` | ✔ | 200 | 404, 422 |
| DELETE | `/users/{id}` | ✔ | 204 | 404 |
| GET | `/users/{id}/profile` | — | 200 | 404 |
| GET | `/applications` | — | 200 | — |
| GET | `/applications/{id}` | — | 200 | 404 |
| POST | `/applications` | ✔ | 201 | 422 |
| PUT | `/applications/{id}` | ✔ | 200 | 404, 422 |
| DELETE | `/applications/{id}` | ✔ | 204 | 404 |
| GET | `/roles` | — | 200 | — |
| GET | `/cphs` | — | 200 | 422 |
| GET | `/cphs/{id}` | — | 200 | 404 |
| GET | `/cphs/{county}/{parish}/{holding}` | — | 200 | 404, 422 |
| POST | `/cphs/{id}:expire` | ✔ | 204 | 404, 409 |
| POST | `/cphs/{county}/{parish}/{holding}:expire` | ✔ | 204 | 404, 409, 422 |
| DELETE | `/cphs/{id}` | ✔ | 204 | 404 |
| DELETE | `/cphs/{county}/{parish}/{holding}` | ✔ | 204 | 404, 422 |
| GET | `/delegations` | — | 200 | — |
| GET | `/delegations/{id}` | — | 200 | 404 |
| POST | `/delegations` | ✔ | 201 | 422 |
| POST | `/delegations/{id}:accept` | ✔ | 204 | 400, 403, 404 |
| POST | `/delegations/{id}:reject` | ✔ | 204 | 400, 403, 404 |
| POST | `/delegations/{id}:revoke` | ✔ | 204 | 400, 404 |
| POST | `/delegations/{id}:expire` | ✔ | 204 | 400, 404 |
| DELETE | `/delegations/{id}` | ✔ | 204 | 404 |
| GET | `/animal-species` | — | 200 | — |
| GET | `/animal-species/{id}` | — | 200 | 404 |
| POST | `/animal-species/{id}:toggle` | ✔ | 204 | 404, 409 |

---

## Health

Source: [HealthEndpoints.cs](../../src/Api/Endpoints/Health/HealthEndpoints.cs)

### GET /health

The only endpoint that requires **no headers** (`IgnoreApiKeyCheck`, `IgnoreCorrelationIdCheck`).

```json
200 → { "status": "ok" }
```

## Users

Source: [UsersEndpoints.cs](../../src/Api/Endpoints/Users/UsersEndpoints.cs)

**User response object**

```json
{
  "id": "guid",
  "email": "string",
  "first_name": "string",
  "last_name": "string",
  "display_name": "string",
  "active": true
}
```

### GET /users

- Query: `includeInactive` (optional string flag).
- `200` → array of User.

### GET /users/{id:guid}

- `200` → User; `404` if not found.

### POST /users — *operator required*

- Body (`CreateUser`): `email`, `display_name`, `first_name`, `last_name` (all strings, validated).
- `201` → created User, `Location` header set to `GET /users/{id}`; `422` on validation failure.

### PUT /users/{id:guid} — *operator required*

- Body (`UpdateUserById`): same shape as `CreateUser`; the target id comes from the **route** (mapped by `OperationByGuidIdMappingFilter`).
- `200` → updated User; `404` unknown id; `422` validation failure.

### DELETE /users/{id:guid} — *operator required*

- `204`; `404` unknown id.

### GET /users/{id:guid}/profile

Source: [ProfileEndpoints.cs](../../src/Api/Endpoints/Profiles/ProfileEndpoints.cs)

Aggregated view of a user:

```json
{
  "user_details": { …User },
  "direct_assignments": [ …CphAssignment ],
  "inbound_delegations": [ …CphDelegation ],
  "outbound_delegations": [ …CphDelegation ]
}
```

`CphAssignment`: `id`, `county_parish_holding_id`, `county_parish_holding_number`,
`user_id`, `role_id`, `role_name`, `email`, `display_name`.

- `200` → UserProfile; `404` unknown user.

## Applications

Source: [ApplicationEndpoints.cs](../../src/Api/Endpoints/Applications/ApplicationEndpoints.cs)

**Application object** (response and write shape)

```json
{
  "id": "guid",
  "name": "string",
  "tenant_name": "string",
  "description": "string",
  "scopes": ["string"],
  "redirect_uris": ["string"],
  "secret": "string"
}
```

### GET /applications

- `200` → array of Application.

### GET /applications/{id:guid}

- `200` → Application; `404` if not found.

### POST /applications — *operator required*

- Body (`CreateApplication`): full Application object including `id`.
- `201` → created Application + `Location`; `422` validation failure.

### PUT /applications/{id:guid} — *operator required*

- Body (`UpdateApplicationByClientId`): write fields (`name`, `tenant_name`, `description`, `scopes`, `redirect_uris`, `secret`); id from route.
- `200`; `404`; `422`.

### DELETE /applications/{id:guid} — *operator required*

- `204`; `404`.

## Roles

Source: [RoleEndpoints.cs](../../src/Api/Endpoints/Roles/RoleEndpoints.cs) — read-only reference data synced from KRDS.

### GET /roles

- `200` → array of `{ "id": "guid", "name": "string", "description": "string" }`.

## County Parish Holdings (CPHs)

Source: [CphEndpoints.cs](../../src/Api/Endpoints/Cphs/CphEndpoints.cs)

CPHs are addressable two ways: by **GUID id** or by **CPH number** (`{county:int}/{parish:int}/{holding:int}`,
rerouted internally to the id-based handler). Number-based routes validate the CPH number (`422` on failure).

**Cph response object**

```json
{
  "id": "guid",
  "county_parish_holding_number": "string",
  "expired": false,
  "expired_at": "datetime|null",
  "allowed_species": [ { "id": "string", "name": "string", "is_active": true } ]
}
```

### GET /cphs

Paged listing. Query parameters (`GetCphs` : `PagedQuery`):

| Param | Type | Notes |
|---|---|---|
| `pageNumber` | int | validated by `PagedQueryValidator` |
| `pageSize` | int | |
| `orderBy` | string? | property name to sort by |
| `orderByDescending` | bool? | default `false` |
| `expired` | string? | include expired CPHs |

- `200` → `PagedResults<Cph>`:

```json
{
  "items": [ …Cph ],
  "total_count": 0,
  "total_pages": 0,
  "page_number": 0,
  "page_size": 0
}
```

- `422` invalid paging query.

### GET /cphs/{id:guid} · GET /cphs/{county}/{parish}/{holding}

- `200` → Cph; `404` not found; number route also `422` for an invalid CPH number.

### POST /cphs/{id:guid}:expire · POST /cphs/{county}/{parish}/{holding}:expire — *operator required*

Marks the CPH expired (soft delete).

- `204`; `404` not found; `409` already expired.

### DELETE /cphs/{id:guid} · DELETE /cphs/{county}/{parish}/{holding} — *operator required*

- `204`; `404` not found.

## Delegations

Source: [CphDelegationEndpoints.cs](../../src/Api/Endpoints/Delegations/CphDelegationEndpoints.cs)

A delegation invites a user (by email) to act on a CPH with a given role, then tracks its
lifecycle: **accept / reject / revoke / expire**.

**CphDelegation response object**

```json
{
  "id": "guid",
  "county_parish_holding_id": "guid",
  "county_parish_holding_number": "string",
  "delegating_user_id": "guid",
  "delegating_user_name": "string",
  "delegated_user_id": "guid|null",
  "delegated_user_name": "string|null",
  "delegated_user_role_id": "guid",
  "delegated_user_role_name": "string",
  "delegated_user_email": "string",
  "invitation_expires_at": "datetime|null",
  "invitation_accepted_at": "datetime|null",
  "invitation_rejected_at": "datetime|null",
  "revoked_at": "datetime|null",
  "revoked_by_id": "guid|null",
  "revoked_by_name": "string|null",
  "expires_at": "datetime|null",
  "active": true
}
```

### GET /delegations

- `200` → array of CphDelegation.

### GET /delegations/{id:guid}

- `200` → CphDelegation; `404`.

### POST /delegations — *operator required*

- Body (`CreateCphDelegation`):

```json
{
  "county_parish_holding_id": "guid",
  "delegating_user_id": "guid",
  "delegated_user_role_id": "guid",
  "delegated_user_email": "string"
}
```

- `201` → created CphDelegation + `Location`; `422` validation failure.

### POST /delegations/{id:guid}:accept — *operator required*

- `204`; `400` business rule (e.g. wrong state); `403` operator is not the invited user; `404`.

### POST /delegations/{id:guid}:reject — *operator required*

- `204`; `400`; `403`; `404`.

### POST /delegations/{id:guid}:revoke — *operator required*

- `204`; `400`; `404`.

### POST /delegations/{id:guid}:expire — *operator required*

- `204`; `400`; `404`.

### DELETE /delegations/{id:guid} — *operator required*

- `204`; `404`.

## Animal species

Source: [AnimalSpeciesEndpoints.cs](../../src/Api/Endpoints/Species/AnimalSpeciesEndpoints.cs) —
reference data with **string** ids.

**AnimalSpecies object**: `{ "id": "string", "name": "string", "is_active": true }`

### GET /animal-species

- Query: `includeInactive` (optional string flag).
- `200` → array of AnimalSpecies.

### GET /animal-species/{id}

- `id` is a string (not a GUID).
- `200` → AnimalSpecies; `404`.

### POST /animal-species/{id}:toggle — *operator required*

Enables/disables a species.

- Body (`ToggleAnimalSpeciesById`): `{ "is_active": true }` (id from route).
- `204`; `404`; `409` (already in the requested state).
