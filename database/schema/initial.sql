

CREATE SCHEMA "defra-ci";
CREATE EXTENSION IF NOT EXISTS citext;
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE "defra-ci".application (
    id uuid NOT NULL,
    name text NOT NULL,
    client_id uuid NOT NULL,
    tenant_name text NOT NULL,
    description text NOT NULL,
    status text NOT NULL DEFAULT 'active',
    created_at TimestampTz NOT NULL DEFAULT (now()),
    updated_at TimestampTz NOT NULL,
    CONSTRAINT "PK_application" PRIMARY KEY (id)
);
COMMENT ON COLUMN "defra-ci".application.name IS 'Human readable name for the application e.g Keeper Portal';
COMMENT ON COLUMN "defra-ci".application.client_id IS 'Azure AD B2C application Client ID';
COMMENT ON COLUMN "defra-ci".application.tenant_name IS 'Azure AD B2C tenant name e.g defra.onmicrosoft.com';
COMMENT ON COLUMN "defra-ci".application.status IS 'active/inactive/deprecated';

CREATE TABLE "defra-ci".krds_sync_log (
    id uuid NOT NULL,
    correlation_id uuid NOT NULL,
    source_endpoint text NOT NULL,
    http_status integer NOT NULL,
    processed_ok boolean NOT NULL,
    message text NOT NULL,
    received_at TimestampTz NOT NULL DEFAULT (now()),
    processed_at TimestampTz NOT NULL,
    CONSTRAINT "PK_krds_sync_log" PRIMARY KEY (id)
);

CREATE TABLE "defra-ci".user_account (
    id uuid NOT NULL,
    upn citext NOT NULL,
    display_name varchar NOT NULL,
    account_enabled boolean NOT NULL DEFAULT TRUE,
    created_at TimestampTz NOT NULL DEFAULT (now()),
    updated_at TimestampTz NOT NULL,
    CONSTRAINT "PK_user_account" PRIMARY KEY (id)
);

CREATE TABLE "defra-ci".enrolment (
    id uuid NOT NULL,
    user_account_id uuid NOT NULL,
    application_id uuid NOT NULL,
    cph_id text NOT NULL,
    role text NOT NULL,
    scope jsonb NOT NULL,
    status text NOT NULL DEFAULT 'active',
    enrolled_at TimestampTz NOT NULL DEFAULT (now()),
    expires_at TimestampTz NOT NULL,
    created_at TimestampTz NOT NULL DEFAULT (now()),
    updated_at TimestampTz NOT NULL,
    CONSTRAINT "PK_enrolment" PRIMARY KEY (id),
    CONSTRAINT "FK_enrolment_application_application_id" FOREIGN KEY (application_id) REFERENCES "defra-ci".application (id) ON DELETE CASCADE,
    CONSTRAINT "FK_enrolment_user_account_user_account_id" FOREIGN KEY (user_account_id) REFERENCES "defra-ci".user_account (id) ON DELETE CASCADE
);

CREATE TABLE "defra-ci".federation (
    id uuid NOT NULL,
    user_account_id uuid NOT NULL,
    tenant_name text NOT NULL,
    object_id uuid NOT NULL,
    trust_level text NOT NULL DEFAULT 'standard',
    sync_status text NOT NULL DEFAULT 'linked',
    last_synced_at TimestampTz NOT NULL,
    created_at TimestampTz NOT NULL DEFAULT (now()),
    updated_at TimestampTz NOT NULL,
    CONSTRAINT "PK_federation" PRIMARY KEY (id),
    CONSTRAINT "FK_federation_user_account_user_account_id" FOREIGN KEY (user_account_id) REFERENCES "defra-ci".user_account (id)
);

CREATE INDEX "IX_enrolment_application_id" ON "defra-ci".enrolment (application_id);

CREATE UNIQUE INDEX "IX_enrolment_user_account_id_role" ON "defra-ci".enrolment (user_account_id, role);

CREATE INDEX "IX_federation_object_id_tenant_name" ON "defra-ci".federation (object_id, tenant_name);

CREATE INDEX "IX_federation_user_account_id" ON "defra-ci".federation (user_account_id);

CREATE INDEX "IX_krds_sync_log_received_at" ON "defra-ci".krds_sync_log (received_at);

CREATE INDEX "IX_user_account_upn" ON "defra-ci".user_account (upn);



