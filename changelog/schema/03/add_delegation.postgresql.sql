-- liquibase formatted sql

-- changeset cedricbrasey:1773744435706-4 splitStatements:false
CREATE TABLE "county_parish_holding_delegations"
(
    "id"                       UUID DEFAULT gen_random_uuid() NOT NULL
        CONSTRAINT "county_parish_holdings_delegations_pk"
            PRIMARY KEY,
    "county_parish_holding_id" UUID                           NOT NULL,
    "delegating_user_id"       UUID                           NOT NULL,
    "delegated_user_id"        UUID,
    "delegated_user_email"     VARCHAR(256)                   NOT NULL,
    "delegated_user_role_id"   UUID                           NOT NULL,
    "invitation_token"         CHAR(64)                       NOT NULL,
    "invitation_expires_at"    TIMESTAMP WITH TIME ZONE       NOT NULL,
    "invitation_accepted_at"   TIMESTAMP WITH TIME ZONE,
    "invitation_rejected_at"   TIMESTAMP WITH TIME ZONE,
    "revoked_at"               TIMESTAMP WITH TIME ZONE,
    "revoked_by_id"            UUID,
    "expires_at"               TIMESTAMP WITH TIME ZONE,
    "created_at"               TIMESTAMP WITH TIME ZONE       NOT NULL,
    "created_by_id"            UUID                           NOT NULL,
    "deleted_at"               TIMESTAMP WITH TIME ZONE,
    "deleted_by_id"            UUID
);

-- changeset cedricbrasey:1773744435706-5 splitStatements:false
        ALTER TABLE "county_parish_holding_delegations" ADD CONSTRAINT "cph_delegations_county_parish_holdings_id_fk" FOREIGN KEY ("county_parish_holding_id") REFERENCES "county_parish_holdings" ("id") ON UPDATE NO ACTION ON DELETE NO ACTION;

-- changeset cedricbrasey:1773744435706-6 splitStatements:false
ALTER TABLE "county_parish_holding_delegations"
    ADD CONSTRAINT "cph_delegations_roles_id_fk" FOREIGN KEY ("delegated_user_role_id") REFERENCES "roles" ("id") ON UPDATE NO ACTION ON DELETE NO ACTION;

-- changeset cedricbrasey:1773744435706-7 splitStatements:false
ALTER TABLE "county_parish_holding_delegations"
    ADD CONSTRAINT "cph_delegations_user_accounts_id_created_by_fk" FOREIGN KEY ("created_by_id") REFERENCES "user_accounts" ("id") ON UPDATE NO ACTION ON DELETE NO ACTION;

-- changeset cedricbrasey:1773744435706-8 splitStatements:false
ALTER TABLE "county_parish_holding_delegations"
    ADD CONSTRAINT "cph_delegations_user_accounts_id_delegated_fk" FOREIGN KEY ("delegated_user_id") REFERENCES "user_accounts" ("id") ON UPDATE NO ACTION ON DELETE NO ACTION;

-- changeset cedricbrasey:1773744435706-9 splitStatements:false
ALTER TABLE "county_parish_holding_delegations"
    ADD CONSTRAINT "cph_delegations_user_accounts_id_delegating_fk" FOREIGN KEY ("delegating_user_id") REFERENCES "user_accounts" ("id") ON UPDATE NO ACTION ON DELETE NO ACTION;

-- changeset cedricbrasey:1773744435706-10 splitStatements:false
ALTER TABLE "county_parish_holding_delegations"
    ADD CONSTRAINT "cph_delegations_user_accounts_id_deleted_by_fk" FOREIGN KEY ("deleted_by_id") REFERENCES "user_accounts" ("id") ON UPDATE NO ACTION ON DELETE NO ACTION;

-- changeset cedricbrasey:1773744435706-11 splitStatements:false
ALTER TABLE "county_parish_holding_delegations"
    ADD CONSTRAINT "cph_delegations_user_accounts_id_revoked_by_fk" FOREIGN KEY ("revoked_by_id") REFERENCES "user_accounts" ("id") ON UPDATE NO ACTION ON DELETE NO ACTION;
