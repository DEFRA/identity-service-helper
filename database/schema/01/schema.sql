-- changeset gary:extensions runOnChange:true
CREATE SCHEMA IF NOT EXISTS "defra-ci";
-- Ensure extensions are in the public schema so they are accessible
CREATE EXTENSION IF NOT EXISTS citext SCHEMA public;
CREATE EXTENSION IF NOT EXISTS "uuid-ossp" SCHEMA public;