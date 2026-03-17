-- liquibase formatted sql

-- changeset cedricbrasey:1773744435706-12 splitStatements:false
ALTER TABLE "delegation_invitations" DROP CONSTRAINT "FK_delegation_invitations_delegations_delegation_id";

-- changeset cedricbrasey:1773744435706-13 splitStatements:false
ALTER TABLE "delegation_invitations" DROP CONSTRAINT "FK_delegation_invitations_roles_delegated_role_id";

-- changeset cedricbrasey:1773744435706-14 splitStatements:false
ALTER TABLE "delegation_invitations" DROP CONSTRAINT "FK_delegation_invitations_user_accounts_created_by_id";

-- changeset cedricbrasey:1773744435706-15 splitStatements:false
ALTER TABLE "delegation_invitations" DROP CONSTRAINT "FK_delegation_invitations_user_accounts_deleted_by_id";

-- changeset cedricbrasey:1773744435706-16 splitStatements:false
ALTER TABLE "delegations" DROP CONSTRAINT "FK_delegations_applications_application_id";

-- changeset cedricbrasey:1773744435706-17 splitStatements:false
ALTER TABLE "delegations_county_parish_holdings" DROP CONSTRAINT "FK_delegations_county_parish_holdings_county_parish_holdings_c~";

-- changeset cedricbrasey:1773744435706-18 splitStatements:false
ALTER TABLE "delegations_county_parish_holdings" DROP CONSTRAINT "FK_delegations_county_parish_holdings_delegations_delegation_id";

-- changeset cedricbrasey:1773744435706-19 splitStatements:false
ALTER TABLE "delegations_county_parish_holdings" DROP CONSTRAINT "FK_delegations_county_parish_holdings_user_accounts_created_by~";

-- changeset cedricbrasey:1773744435706-20 splitStatements:false
ALTER TABLE "delegations_county_parish_holdings" DROP CONSTRAINT "FK_delegations_county_parish_holdings_user_accounts_deleted_by~";

-- changeset cedricbrasey:1773744435706-21 splitStatements:false
ALTER TABLE "delegations" DROP CONSTRAINT "FK_delegations_user_accounts_user_id";

-- changeset cedricbrasey:1773744435706-22 splitStatements:false
ALTER TABLE "delegation_invitations" DROP CONSTRAINT "delegation_invitations_animal_species_id_fk";

-- changeset cedricbrasey:1773744435706-23 splitStatements:false
DROP TABLE "delegation_invitations";

-- changeset cedricbrasey:1773744435706-24 splitStatements:false
DROP TABLE "delegations";

-- changeset cedricbrasey:1773744435706-25 splitStatements:false
DROP TABLE "delegations_county_parish_holdings";
