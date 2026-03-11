-- liquibase formatted sql

-- changeset test-container:999-1 runAlways:true context:testcontainer splitStatements:false
DO $$
DECLARE
    ADMIN_EMAIL_ADDRESS CONSTANT text := 'system.admin@defra.gov.uk';
    ADMIN_USER_ID CONSTANT uuid := 'df6990d6-e61a-4e14-aa79-5479dc7b3569';
BEGIN
    
-- clear down
TRUNCATE TABLE
    animal_species,
    roles,

    application_roles,
    application_user_account_holding_assignments,

    delegation_invitations,
    delegations_county_parish_holdings,
    delegations,

    user_accounts,

    krds_sync_logs,
    animal_species,
    roles,
    applications,
    county_parish_holdings
RESTART IDENTITY
CASCADE;

-- load data
insert into animal_species (id, name, is_active) values
    ('CHK', 'Chicken', false),
    ('CTT', 'Cattle', true),
    ('DR', 'Deer', false),
    ('GT', 'Goat', false),
    ('HRS', 'Horse', false),
    ('PG', 'Pig', false);

insert into roles (id, name, description) values
    ('0c15ba2f-b4ba-406a-a0ae-213de64600a9', 'test-role-1', 'Test Role 1'),
    ('817647b3-d5d2-45e9-8833-df36d8264102', 'test-role-2', 'Test Role 2'),
    ('c63207ab-68f7-4613-b94a-492939eb6116', 'test-role-3', 'Test Role 3'),
    ('306fa0fc-bd1a-45d3-9fef-e6f11a85b601', 'test-role-4', 'Test Role 4');

insert into user_accounts (id, display_name, email_address, first_name, last_name, created_at, created_by_id) values
    (ADMIN_USER_ID, 'Admin User', ADMIN_EMAIL_ADDRESS, 'Admin', 'User', '2026-02-24 00:00:00.000000 +00:00', ADMIN_USER_ID),
    ('0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1', 'Test User', 'test@test.com', 'test', 'user', '2026-02-24 00:00:00.000000 +00:00', ADMIN_USER_ID),
    ('42bde7a0-9efe-402a-a7c3-9161be7b00ba', 'Test User 1', 'test1@test.com', 'test 1', 'user 1', '2026-03-01 00:00:00.000000 +00:00', ADMIN_USER_ID),
    ('1e21b685-2247-4d96-bf39-f7dc30f356c2', 'Test User 2', 'test2@test.com', 'test 2', 'user 2', '2026-03-02 00:00:00.000000 +00:00', ADMIN_USER_ID),
    ('83bf35f9-fd59-4c8a-b70a-7d95a1aab2b6', 'Test User 3', 'test3@test.com', 'test 3', 'user 3', '2026-03-03 00:00:00.000000 +00:00', ADMIN_USER_ID),
    ('d1354eb1-dd1c-471e-bd0e-2626e2e21366', 'Test User 4', 'test4@test.com', 'test 4', 'user 4', '2026-03-04 00:00:00.000000 +00:00', ADMIN_USER_ID),
    ('2f778d40-965c-4479-b94d-86f66d100952', 'Test User 5', 'test5@test.com', 'test 5', 'user 5', '2026-03-05 00:00:00.000000 +00:00', ADMIN_USER_ID),
    ('dd1b98c5-0874-4718-a37c-74cdec359567', 'Test User 6', 'test6@test.com', 'test 6', 'user 6', '2026-03-06 00:00:00.000000 +00:00', ADMIN_USER_ID),
    ('2c6d9ca2-6e78-4fdd-88eb-61fa6cc7f319', 'Test User 7', 'test7@test.com', 'test 7', 'user 7', '2026-03-07 00:00:00.000000 +00:00', ADMIN_USER_ID),
    ('054037a5-e666-4d2f-9d4d-aa5efce35d7b', 'Test User 8', 'test8@test.com', 'test 8', 'user 8', '2026-03-08 00:00:00.000000 +00:00', ADMIN_USER_ID),
    ('7e9e5585-7b59-4d10-b330-1c95d83a4670', 'Test User 9', 'test9@test.com', 'test 9', 'user 9', '2026-03-09 00:00:00.000000 +00:00', ADMIN_USER_ID),
    ('4852c883-6dc2-4880-aaa8-c20110955c90', 'Test User 10', 'test10@test.com', 'test 10', 'user 10', '2026-03-10 00:00:00.000000 +00:00', ADMIN_USER_ID),
    ('6d0d343b-cbba-4cb7-bd3f-d7a9407f248d', 'Test User 11', 'test11@test.com', 'test 11', 'user 11', '2026-03-11 00:00:00.000000 +00:00', ADMIN_USER_ID),
    ('74bbb0da-5a57-48f9-abcb-7bedbfe87ede', 'Test User 12', 'test12@test.com', 'test 12', 'user 12', '2026-03-12 00:00:00.000000 +00:00', ADMIN_USER_ID);

insert into county_parish_holdings (id, identifier, created_at, created_by_id, expired_at, deleted_at, deleted_by_id) values
    ('088967e7-71b8-457a-9001-5b71f24798fd', '44/000/0007', '2026-02-07 00:00:00.000000 +00:00', ADMIN_USER_ID, '2026-02-12 00:00:00.000000 +00:00', '2026-02-13 00:00:00.000000 +00:00', ADMIN_USER_ID),
    ('4435a146-d0ac-4260-8a27-c550e0ed9563', '44/000/0001', '2026-02-01 00:00:00.000000 +00:00', ADMIN_USER_ID, null, null, null),
    ('1eb0f2fb-a332-4cd5-8a20-02d7adfd7156', '44/000/0003', '2026-02-03 00:00:00.000000 +00:00', ADMIN_USER_ID, null, null, null),
    ('204459b1-3a07-4e65-9122-91c1699e3d3f', '44/000/0002', '2026-02-02 00:00:00.000000 +00:00', ADMIN_USER_ID, null, null, null),
    ('82181a8b-7f7f-470c-9263-2b94675599df', '44/000/0006', '2026-02-06 00:00:00.000000 +00:00', ADMIN_USER_ID, null, '2026-02-11 00:00:00.000000 +00:00', ADMIN_USER_ID),
    ('02f8043f-510a-41aa-8012-db316ae7fefa', '44/000/0004', '2026-02-04 00:00:00.000000 +00:00', ADMIN_USER_ID, null, null, null),
    ('7973060a-d483-4ad4-9716-c70415ed620a', '44/000/0005', '2026-02-05 00:00:00.000000 +00:00', ADMIN_USER_ID, '2026-02-10 00:00:00.000000 +00:00', null, null);

insert into applications (id, name, client_id, tenant_name, description, created_at, created_by_id) values
    ('112788f5-4cb5-4acc-a3f5-d8b2b0e20945', 'Test Livestock Service 1', 'df9ab2b8-1f01-4eda-bbdf-13814d91ebb6', 'Test Tenant 1', 'Test Description 1', '2026-03-01 00:00:00.000000 +00:00', ADMIN_USER_ID),
    ('5466ef9b-aa6b-4b7d-9aac-6c6e55a66ab9', 'Test Livestock Service 2', '543ebe7b-e4cd-4969-9cba-ca8223b0b3c4', 'Test Tenant 2', 'Test Description 2', '2026-03-02 00:00:00.000000 +00:00', ADMIN_USER_ID);

insert into application_roles (application_id, role_id) values
    ('112788f5-4cb5-4acc-a3f5-d8b2b0e20945', '0c15ba2f-b4ba-406a-a0ae-213de64600a9'),
    ('112788f5-4cb5-4acc-a3f5-d8b2b0e20945', '817647b3-d5d2-45e9-8833-df36d8264102'),
    ('5466ef9b-aa6b-4b7d-9aac-6c6e55a66ab9', 'c63207ab-68f7-4613-b94a-492939eb6116'),
    ('5466ef9b-aa6b-4b7d-9aac-6c6e55a66ab9', '306fa0fc-bd1a-45d3-9fef-e6f11a85b601');

insert into application_user_account_holding_assignments (county_parish_holding_id, species_type, user_account_id, application_id, role_id, created_by_id, created_at, deleted_by_id, deleted_at) values
  ('4435a146-d0ac-4260-8a27-c550e0ed9563', 'CTT', '42bde7a0-9efe-402a-a7c3-9161be7b00ba', '112788f5-4cb5-4acc-a3f5-d8b2b0e20945', '0c15ba2f-b4ba-406a-a0ae-213de64600a9', ADMIN_USER_ID, '2026-03-01 00:00:00.000000 +00:00', null, null),
  ('4435a146-d0ac-4260-8a27-c550e0ed9563', 'CTT', '1e21b685-2247-4d96-bf39-f7dc30f356c2', '112788f5-4cb5-4acc-a3f5-d8b2b0e20945', '817647b3-d5d2-45e9-8833-df36d8264102', ADMIN_USER_ID, '2026-03-02 00:00:00.000000 +00:00', null, null),
  ('4435a146-d0ac-4260-8a27-c550e0ed9563', 'CTT', '83bf35f9-fd59-4c8a-b70a-7d95a1aab2b6', '5466ef9b-aa6b-4b7d-9aac-6c6e55a66ab9', 'c63207ab-68f7-4613-b94a-492939eb6116', ADMIN_USER_ID, '2026-03-03 00:00:00.000000 +00:00', ADMIN_USER_ID, '2026-03-05 00:00:00.000000 +00:00'),
  ('4435a146-d0ac-4260-8a27-c550e0ed9563', 'CTT', 'd1354eb1-dd1c-471e-bd0e-2626e2e21366', '5466ef9b-aa6b-4b7d-9aac-6c6e55a66ab9', '306fa0fc-bd1a-45d3-9fef-e6f11a85b601', ADMIN_USER_ID, '2026-03-04 00:00:00.000000 +00:00', null, null),
  ('204459b1-3a07-4e65-9122-91c1699e3d3f', 'CTT', '42bde7a0-9efe-402a-a7c3-9161be7b00ba', '112788f5-4cb5-4acc-a3f5-d8b2b0e20945', '0c15ba2f-b4ba-406a-a0ae-213de64600a9', ADMIN_USER_ID, '2026-03-01 00:00:00.000000 +00:00', null, null),
  ('204459b1-3a07-4e65-9122-91c1699e3d3f', 'CTT', '1e21b685-2247-4d96-bf39-f7dc30f356c2', '112788f5-4cb5-4acc-a3f5-d8b2b0e20945', '817647b3-d5d2-45e9-8833-df36d8264102', ADMIN_USER_ID, '2026-03-02 00:00:00.000000 +00:00', null, null);

END
$$;