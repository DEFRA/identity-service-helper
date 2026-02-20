-- liquibase formatted sql

-- changeset cedricbrasey:1771532961448-3 splitStatements:false
ALTER TABLE "application_user_account_holding_assignments" ADD "species_type" VARCHAR(20);
ALTER TABLE "delegation_invitations" ADD "species_type" VARCHAR(20);

-- changeset cedricbrasey:1771532961448-4 splitStatements:false
ALTER TABLE "application_user_account_holding_assignments"
    ADD CONSTRAINT "application_user_account_holding_assignments_animal_species_id_"
        FOREIGN KEY ("species_type")
            REFERENCES "animal_species" ("id")
            ON UPDATE NO ACTION
            ON DELETE NO ACTION
            NOT VALID;
ALTER TABLE "delegation_invitations"
    ADD CONSTRAINT "delegation_invitations_animal_species_id_fk"
        FOREIGN KEY ("species_type")
            REFERENCES "animal_species" ("id")
            ON UPDATE NO ACTION
            ON DELETE NO ACTION
            NOT VALID;

-- changeset cedricbrasey:1771532961448-5 splitStatements:false
UPDATE "application_user_account_holding_assignments"
SET "species_type" = 'CTT'
WHERE "species_type" IS NULL;
UPDATE "delegation_invitations"
SET "species_type" = 'CTT'
WHERE "species_type" IS NULL;

-- changeset cedricbrasey:1771532961448-6 splitStatements:false
ALTER TABLE "application_user_account_holding_assignments"
    VALIDATE CONSTRAINT "application_user_account_holding_assignments_animal_species_id_";
ALTER TABLE "delegation_invitations"
    VALIDATE CONSTRAINT "delegation_invitations_animal_species_id_fk";
ALTER TABLE "application_user_account_holding_assignments"
    ALTER COLUMN "species_type" SET NOT NULL;
ALTER TABLE "delegation_invitations"
    ALTER COLUMN "species_type" SET NOT NULL;
