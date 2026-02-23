-- liquibase formatted sql

-- changeset gary:seeding-1
--- Insert ALL Roles
INSERT INTO public.roles ( name, description) VALUES ('agent', 'Agent');
INSERT INTO public.roles ( name, description) VALUES ('citizen', 'Citizen');
INSERT INTO public.roles ( name, description) VALUES ('cph-holder', 'CPH Holder');
INSERT INTO public.roles ( name, description) VALUES ('customer', 'Customer');
INSERT INTO public.roles ( name, description) VALUES ('exporter', 'Exporter');
INSERT INTO public.roles ( name, description) VALUES ('facility-operator', 'Facility Operator');
INSERT INTO public.roles ( name, description) VALUES ('facility-owner', 'Facility Owner');
INSERT INTO public.roles ( name, description) VALUES ('land-owner', 'Land Owner');
INSERT INTO public.roles ( name, description) VALUES ('livestock-keeper', 'Livestock Keeper');
INSERT INTO public.roles ( name, description) VALUES ('livestock-owner', 'Livestock Owner');
INSERT INTO public.roles ( name, description) VALUES ('one-off-exporter', 'One Off Exporter');
INSERT INTO public.roles ( name, description) VALUES ('registrant', 'Registrant');


-- changeset gary:seeding-2 context:!prod
---- Insert Initial Users  
WITH seed AS (
    SELECT gen_random_uuid() AS id
),
     initial AS (
INSERT INTO public.user_accounts (
    id,
    email_address,
    display_name,
    first_name,
    last_name,
    created_by_id
)
SELECT
    s.id,
    'system.admin@defra.gov.uk',
    'System',
    'Admin',
    'User',
    s.id
FROM seed s
    RETURNING id
     )
INSERT INTO public.user_accounts (
    email_address,
    display_name,
    first_name,
    last_name,
    created_by_id
)
SELECT v.email_address, v.display_name, v.first_name, v.last_name, i.id
FROM (VALUES
          ('cedric.brasey@esynergy.co.uk', 'Cedric Brasey', 'Cedric', 'Brasey'),
          ('cedric.brasey@defra.gov.uk', 'Cedric Brasey', 'Cedric', 'Brasey')
     ) AS v(email_address, display_name, first_name, last_name)
         CROSS JOIN initial i
    RETURNING *;

--- Insert Initial Applications  
WITH "system_user" AS (SELECT id
                       FROM public.user_accounts
                       WHERE email_address = 'system.admin@defra.gov.uk'
                       LIMIT 1)
INSERT INTO public.applications (name, client_id, tenant_name, description, created_by_id)
SELECT v.name, v.client_id::uuid, v.tenant_name, v.description, su.id
FROM (VALUES ('ScotEID - Livestock Service', '93781ffd-55be-4444-80d5-57ca9f6fb846', 'x', 'x'),
             ('Animal Disease Movement Licensing', 'e838f542-c0a0-4436-895f-17aae56b09eb', 'x', 'x'),
             ('England - Livestock Service', 'f6b380bd-2326-4b20-a53b-d2601cd23049', 'x', 'x')) AS v(name, client_id, tenant_name, description)
         CROSS JOIN "system_user" su;

--- Insert Initial CPH  
WITH "system_user" AS (SELECT id
                       FROM public.user_accounts
                       WHERE email_address = 'system.admin@defra.gov.uk'
    LIMIT 1)
INSERT
INTO public.county_parish_holdings (identifier, created_by_id)
SELECT v.identifier, su.id
FROM (VALUES ('11/111/1945' ),
             ('12/112/5442'),
             ('10/105/9257'),
             ('14/114/6186'),
             ('10/106/2922'),
             ('10/109/2006'),
             ('10/108/1555'),
             ('16/116/9960'),
             ('17/117/2034'),
             ('15/115/1345'),
             ('10/110/9041'),
             ('10/107/7964'),
             ('10/103/4806'),
             ('20/120/7280'),
             ('18/118/9794'),
             ('10/101/8550'),
             ('19/119/3783'),
             ('10/104/4490'),
             ('13/113/9849'),
             ('10/102/4749')) AS v(identifier)
         CROSS JOIN "system_user" su;
