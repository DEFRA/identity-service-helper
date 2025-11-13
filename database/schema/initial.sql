CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
                                                       "MigrationId" character varying(150) NOT NULL,
                                                       "ProductVersion" character varying(32) NOT NULL,
                                                       CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
DO $EF$
    BEGIN
        IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'defra-ci') THEN
            CREATE SCHEMA "defra-ci";
        END IF;
    END $EF$;

DO $EF$
    BEGIN
        IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'defra-ci') THEN
            CREATE SCHEMA "defra-ci";
        END IF;
    END $EF$;

CREATE EXTENSION IF NOT EXISTS citext;
DO $EF$
    BEGIN
        IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'defra-ci') THEN
            CREATE SCHEMA "defra-ci";
        END IF;
    END $EF$;

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE "defra-ci".application (
                                        id uuid NOT NULL,
                                        name text NOT NULL,
                                        b2c_client_id uuid NOT NULL,
                                        b2c_tenant text NOT NULL,
                                        description text NOT NULL,
                                        status text NOT NULL DEFAULT 'active',
                                        created_at TimestampTz NOT NULL DEFAULT (now()),
                                        updated_at TimestampTz NOT NULL DEFAULT (now()),
                                        CONSTRAINT "PK_application" PRIMARY KEY (id)
);

CREATE TABLE "defra-ci".enrolment (
                                      id uuid NOT NULL,
                                      b2c_object_id uuid NOT NULL,
                                      application_id uuid NOT NULL,
                                      cph_id text NOT NULL,
                                      role text NOT NULL,
                                      scope jsonb NOT NULL,
                                      status text NOT NULL DEFAULT 'active',
                                      enrolled_at TimestampTz NOT NULL DEFAULT (now()),
                                      expires_at TimestampTz NOT NULL,
                                      created_at TimestampTz NOT NULL DEFAULT (now()),
                                      updated_at TimestampTz NOT NULL DEFAULT (now()),
                                      CONSTRAINT "PK_enrolment" PRIMARY KEY (id)
);

CREATE TABLE "defra-ci".user_account (
                                         id uuid NOT NULL,
                                         upn citext NOT NULL,
                                         display_name varchar NOT NULL,
                                         account_enabled boolean NOT NULL DEFAULT TRUE,
                                         created_at TimestampTz NOT NULL DEFAULT (now()),
                                         updated_at TimestampTz NOT NULL DEFAULT (now()),
                                         CONSTRAINT "PK_user_account" PRIMARY KEY (id)
);

CREATE TABLE "defra-ci".federation (
                                       id uuid NOT NULL,
                                       user_account_id uuid NOT NULL,
                                       b2c_tenant text NOT NULL,
                                       b2c_object_id uuid NOT NULL,
                                       trust_level text NOT NULL DEFAULT 'standard',
                                       sync_status text NOT NULL DEFAULT 'linked',
                                       last_synced_at TimestampTz NOT NULL,
                                       created_at TimestampTz NOT NULL DEFAULT (now()),
                                       updated_at TimestampTz NOT NULL DEFAULT (now()),
                                       CONSTRAINT "PK_federation" PRIMARY KEY (id),
                                       CONSTRAINT "FK_federation_user_account_user_account_id" FOREIGN KEY (user_account_id) REFERENCES "defra-ci".user_account (id)
);

CREATE UNIQUE INDEX "IX_enrolment_b2c_object_id_role" ON "defra-ci".enrolment (b2c_object_id, role);

CREATE INDEX "IX_federation_b2c_object_id_b2c_tenant" ON "defra-ci".federation (b2c_object_id, b2c_tenant);

CREATE INDEX "IX_federation_user_account_id" ON "defra-ci".federation (user_account_id);

CREATE INDEX "IX_user_account_upn" ON "defra-ci".user_account (upn);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251112134625_initial', '9.0.11');

COMMIT;

