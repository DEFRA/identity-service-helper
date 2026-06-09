-- liquibase formatted sql

-- changeset system:initial-seed-2 context:"!testcontainer"
INSERT INTO roles (id, name, description) VALUES 
  ('429e63ac-4046-472c-8a47-5ae29a8d1024', 'agent', 'Agent'),
  ('71eefac2-a066-4de4-bde4-369d45e1e5bc', 'citizen', 'Citizen'),
  ('404bfb4a-3396-4d3f-9564-8698071eeb7a', 'cph-holder', 'CPH Holder'),
  ('aa0f1cbf-f26b-41b5-b3cc-58edde1b1fb4', 'customer', 'Customer'),
  ('dae37e0c-029b-452f-8853-bd64cf7f87bb', 'exporter', 'Exporter'),
  ('f47ac10b-58cc-4372-a567-0e02b2c3d479', 'facility-operator', 'Facility Operator'),
  ('3b241101-e2bb-4244-8d18-7b9c9f28d88c', 'facility-owner', 'Facility Owner'),
  ('82f42a91-18e3-4d43-bd26-9d2aa3891465', 'land-owner', 'Land Owner'),
  ('9bc7c989-bf2a-46da-a71d-5fa8a25c3453', 'livestock-keeper', 'Livestock Keeper'),
  ('cbb73d6d-9610-4824-8f4b-bf2f643194a0', 'livestock-owner', 'Livestock Owner'),
  ('e62c11e7-8b09-4b68-b7d1-e6da4adbb3f6', 'one-off-exporter', 'One Off Exporter'),
  ('4f0bb4e3-a62e-4bdf-834c-eb34ec2bdf22', 'registrant', 'Registrant');

INSERT INTO animal_species (id, name, is_active) VALUES
  ('ALP','Alpaca',false),
  ('BFF','Buffalo',false),
  ('BRDS','Avian Birds',false),
  ('BSN','Bison',false),
  ('CHK','Chicken',false),
  ('CML','Camel',false),
  ('CT','Cat',false),
  ('CTT','Cattle',true),
  ('DCK','Duck',false),
  ('DG','Dog',false),
  ('DR','Deer',false),
  ('GNC','Guanaco',false),
  ('GNF','Guinea Fowl',false),
  ('GS','Goose',false),
  ('GT','Goat',false),
  ('HRS','Horse',false),
  ('LLM','Llama',false),
  ('NK','Newt/Salamander',false),
  ('OMS','Opossum',false),
  ('OST','Ostrich',false),
  ('OTHBRDS','Other Birds',false),
  ('OTHBVN','Other Bovine',false),
  ('PG','Pig',false),
  ('PGN','Pigeon',false),
  ('PHS','Pheasant',false),
  ('PRT','Parrot',false),
  ('QL','Quail',false),
  ('SHP','Sheep',false),
  ('TRK','Turkey',false),
  ('VIC','Vicuna',false),
  ('WLDBR','Wild Boar',false);

INSERT INTO user_accounts (id, email_address, display_name, first_name, last_name, krds_id, sam_id, created_at,
   created_by_id) VALUES 
   ('df6990d6-e61a-4e14-aa79-5479dc7b3569',
    'system.admin@defra.gov.uk',
    'System Admin',
    'System',
    'Admin',
    '00000000-0000-0000-0000-000000000000',
    '00000000-0000-0000-0000-000000000000',
    '2021-01-01 00:00:00.000000',
    'df6990d6-e61a-4e14-aa79-5479dc7b3569');

-- test.automation-1: 6 owned CPHs
INSERT INTO user_accounts (id, email_address, display_name, first_name, last_name, krds_id, sam_id, created_at,
   created_by_id) VALUES
   ('44444444-0000-5000-a000-000000000001',
    'test.automation-1@defra.gov.uk',
    'Test Automation 1',
    'Test',
    'Automation 1',
    '00000000-0000-0000-0000-000000000000',
    '00000000-0000-0000-0000-000000000000',
    '2026-01-01 00:00:00.000000',
    'df6990d6-e61a-4e14-aa79-5479dc7b3569');

-- test.automation-2: 1 owned CPH
INSERT INTO user_accounts (id, email_address, display_name, first_name, last_name, krds_id, sam_id, created_at,
   created_by_id) VALUES
   ('44444444-0000-5000-a000-000000000002',
    'test.automation-2@defra.gov.uk',
    'Test Automation 2',
    'Test',
    'Automation 2',
    '00000000-0000-0000-0000-000000000000',
    '00000000-0000-0000-0000-000000000000',
    '2026-01-01 00:00:00.000000',
    'df6990d6-e61a-4e14-aa79-5479dc7b3569');

-- test.automation-1's CPHs (00/083/)
INSERT INTO county_parish_holdings (identifier, created_at, created_by_id)
SELECT v.identifier, now(), 'df6990d6-e61a-4e14-aa79-5479dc7b3569'
FROM (VALUES
    ('00/083/0001'), ('00/083/0002'), ('00/083/0003'),
    ('00/083/0004'), ('00/083/0005'), ('00/083/0006')
) AS v(identifier)
WHERE NOT EXISTS (SELECT 1 FROM county_parish_holdings WHERE identifier = v.identifier);

-- test.automation-2's CPHs (00/084/)
INSERT INTO county_parish_holdings (identifier, created_at, created_by_id)
SELECT v.identifier, now(), 'df6990d6-e61a-4e14-aa79-5479dc7b3569'
FROM (VALUES
    ('00/084/0001')
) AS v(identifier)
WHERE NOT EXISTS (SELECT 1 FROM county_parish_holdings WHERE identifier = v.identifier);

-- test.automation-1 owns all 00/083/
INSERT INTO user_account_county_parish_holding_assignments
    (county_parish_holding_id, user_account_id, role_id, created_by_id, created_at)
SELECT c.id, '44444444-0000-5000-a000-000000000001', '404bfb4a-3396-4d3f-9564-8698071eeb7a', 'df6990d6-e61a-4e14-aa79-5479dc7b3569', now()
FROM county_parish_holdings c
WHERE c.identifier IN ('00/083/0001', '00/083/0002', '00/083/0003', '00/083/0004', '00/083/0005', '00/083/0006')
  AND NOT EXISTS (
      SELECT 1 FROM user_account_county_parish_holding_assignments
      WHERE county_parish_holding_id = c.id AND user_account_id = '44444444-0000-5000-a000-000000000001'
  );

-- test.automation-2 owns 00/084/0001
INSERT INTO user_account_county_parish_holding_assignments
    (county_parish_holding_id, user_account_id, role_id, created_by_id, created_at)
SELECT c.id, '44444444-0000-5000-a000-000000000002', '404bfb4a-3396-4d3f-9564-8698071eeb7a', 'df6990d6-e61a-4e14-aa79-5479dc7b3569', now()
FROM county_parish_holdings c
WHERE c.identifier = '00/084/0001'
  AND NOT EXISTS (
      SELECT 1 FROM user_account_county_parish_holding_assignments
      WHERE county_parish_holding_id = c.id AND user_account_id = '44444444-0000-5000-a000-000000000002'
  );
