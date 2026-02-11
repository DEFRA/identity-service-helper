-- liquibase formatted sql

--- Insert Initial Status Types  
-- changeset gary:seeding splitStatements:false
SET search_path TO "defra-ci", public;
INSERT INTO "defra-ci".status_type (name, description) VALUES ('NEW', 'An item that is not currently usable.');
INSERT INTO "defra-ci".status_type (name, description) VALUES ( 'ACTIVE', 'An item that can be used.');
INSERT INTO "defra-ci".status_type (name, description) VALUES ('SUSPENDED', 'An item that is available but not currently active.');
INSERT INTO "defra-ci".status_type (name, description) VALUES ('DELETED', 'An item that is soft deleted and should not be selectable.');


---- Insert Initial Users  

WITH seed AS (
    SELECT gen_random_uuid() AS id
),
     initial AS (
INSERT INTO "defra-ci".user_account (
    id,
    email_address,
    status_type_id,
    display_name,
    first_name,
    last_name,
    created_by
)
SELECT
    s.id,
    'system.admin@defra.gov.uk',
    2::smallint,
    'System',
    'Admin',
    'User',
    s.id
FROM seed s
    RETURNING id  
     )
INSERT INTO "defra-ci".user_account (
    email_address,
    status_type_id,
    display_name,
    first_name,
    last_name,
    created_by
)
SELECT v.email_address, v.status_type_id, v.display_name, v.first_name, v.last_name, i.id
FROM (VALUES
          ('cedric.brasey@esynergy.co.uk',    2::smallint, 'Cedric Brasey', 'Cedric', 'Brasey'),
          ('cedric.brasey@planet-side.co.uk', 2::smallint, 'Cedric Brasey', 'Cedric', 'Brasey')
     ) AS v(email_address, status_type_id, display_name, first_name, last_name)
         CROSS JOIN initial i
    RETURNING *;

--- Insert Initial Applications  

INSERT INTO "defra-ci".application ( status_type_id, name, client_id, tenant_name, description) VALUES ( 2, 'ScotEID - Livestock Service', '93781ffd-55be-4444-80d5-57ca9f6fb846', 'x', 'x');
INSERT INTO "defra-ci".application ( status_type_id, name, client_id, tenant_name, description) VALUES ( 2, 'Animal Disease Movement Licensing', 'e838f542-c0a0-4436-895f-17aae56b09eb', 'x', 'x');
INSERT INTO "defra-ci".application (status_type_id, name, client_id, tenant_name, description) VALUES ( 2, 'England - Livestock Service', 'f6b380bd-2326-4b20-a53b-d2601cd23049', 'x', 'x');



--- Insert Initial CPH  
WITH "system_user" AS (SELECT id
                       FROM "defra-ci".user_account
                       WHERE email_address = 'system.admin@defra.gov.uk'
    LIMIT 1)
INSERT
INTO "defra-ci".county_parish_holding (identifier, status_type_id, created_by, processed_at, updated_by)
SELECT v.identifier, v.status_type_id, su.id, now(), su.id
FROM (VALUES ('11/111/1945', 2::smallint),
             ('10/105/9257', 2::smallint),
             ('12/112/5442', 2::smallint),
             ('14/114/6186', 2::smallint),
             ('10/106/2922', 2::smallint),
             ('10/109/2006', 2::smallint),
             ('10/108/1555', 2::smallint),
             ('16/116/9960', 2::smallint),
             ('17/117/2034', 2::smallint),
             ('15/115/1345', 2::smallint),
             ('10/110/9041', 2::smallint),
             ('10/107/7964', 2::smallint),
             ('10/103/4806', 2::smallint),
             ('20/120/7280', 2::smallint),
             ('18/118/9794', 2::smallint),
             ('10/101/8550', 2::smallint),
             ('19/119/3783', 2::smallint),
             ('10/104/4490', 2::smallint),
             ('13/113/9849', 2::smallint),
             ('10/102/4749', 2::smallint)) AS v(identifier, status_type_id)
         CROSS JOIN "system_user" su;


--- Insert ALL Roles
INSERT INTO "defra-ci".role ( name, description) VALUES ('agent', 'Agent');
INSERT INTO "defra-ci".role ( name, description) VALUES ('citizen', 'Citizen');
INSERT INTO "defra-ci".role ( name, description) VALUES ('cph-holder', 'CPH Holder');
INSERT INTO "defra-ci".role ( name, description) VALUES ('customer', 'Customer');
INSERT INTO "defra-ci".role ( name, description) VALUES ('exporter', 'Exporter');
INSERT INTO "defra-ci".role ( name, description) VALUES ('facility-operator', 'Facility Operator');
INSERT INTO "defra-ci".role ( name, description) VALUES ('facility-owner', 'Facility Owner');
INSERT INTO "defra-ci".role ( name, description) VALUES ('land-owner', 'Land Owner');
INSERT INTO "defra-ci".role ( name, description) VALUES ('livestock-keeper', 'Livestock Keeper');
INSERT INTO "defra-ci".role ( name, description) VALUES ('livestock-owner', 'Livestock Owner');
INSERT INTO "defra-ci".role ( name, description) VALUES ('one-off-exporter', 'One Off Exporter');
INSERT INTO "defra-ci".role ( name, description) VALUES ('registrant', 'Registrant');