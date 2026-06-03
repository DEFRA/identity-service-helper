DO $$
DECLARE
    admin_id           uuid;
    max_id             uuid := 'a17a772c-604e-495d-950e-3dbee2ba6e98';
    cedric_id          uuid := 'cd91b1e0-bae4-4cee-becf-3529cc557311';
    app_id             uuid;
    cph_holder_role_id uuid;
    delegation_role_id uuid;
BEGIN

SELECT id INTO admin_id          FROM user_accounts WHERE email_address = 'system.admin@defra.gov.uk' LIMIT 1;
SELECT id INTO cph_holder_role_id FROM roles WHERE name = 'cph-holder' LIMIT 1;
SELECT id INTO delegation_role_id FROM roles WHERE name = 'agent' LIMIT 1;

-- Max
INSERT INTO user_accounts (id, display_name, email_address, first_name, last_name, created_at, created_by_id)
VALUES (max_id, 'Max Bladen-Clark', 'max.bladen-clark@esynergy.co.uk', 'Max', 'Bladen-Clark', now(), admin_id)
ON CONFLICT (email_address) DO NOTHING;

-- Cedric
INSERT INTO user_accounts (id, display_name, email_address, first_name, last_name, created_at, created_by_id)
VALUES (cedric_id, 'Cedric Brasey', 'cedric.brasey@planet-side.co.uk', 'Cedric', 'Brasey', now(), admin_id)
ON CONFLICT (email_address) DO NOTHING;

-- test application
INSERT INTO applications (name, client_id, tenant_name, description, created_at, created_by_id, scopes, secret, redirect_uris)
VALUES ('Local Dev Client', 'a3d4e5f6-7890-4b1c-a2d3-e4f567890abc', 'Local Tenant', 'Local development client', now(), admin_id, 'openid;profile;email;offline_access', 'secret123', 'https://localhost:3005/callback')
ON CONFLICT DO NOTHING;

SELECT id INTO app_id FROM applications WHERE name = 'Local Dev Client' LIMIT 1;

-- Max's CPHs (00/081/)
INSERT INTO county_parish_holdings (identifier, created_at, created_by_id)
SELECT v.identifier, now(), admin_id
FROM (VALUES
    ('00/081/0001'), ('00/081/0002'), ('00/081/0003'),
    ('00/081/0004'), ('00/081/0005'), ('00/081/0006')
) AS v(identifier)
WHERE NOT EXISTS (SELECT 1 FROM county_parish_holdings WHERE identifier = v.identifier);

-- Cedric's CPHs (00/082/)
INSERT INTO county_parish_holdings (identifier, created_at, created_by_id)
SELECT v.identifier, now(), admin_id
FROM (VALUES
    ('00/082/0001'), ('00/082/0002'), ('00/082/0003'),
    ('00/082/0004'), ('00/082/0005'), ('00/082/0006')
) AS v(identifier)
WHERE NOT EXISTS (SELECT 1 FROM county_parish_holdings WHERE identifier = v.identifier);

-- Max owns all 00/081/
INSERT INTO user_account_county_parish_holding_assignments
    (county_parish_holding_id, user_account_id, role_id, created_by_id, created_at)
SELECT c.id, max_id, cph_holder_role_id, admin_id, now()
FROM county_parish_holdings c
WHERE c.identifier IN ('00/081/0001', '00/081/0002', '00/081/0003', '00/081/0004', '00/081/0005', '00/081/0006')
  AND NOT EXISTS (
      SELECT 1 FROM user_account_county_parish_holding_assignments
      WHERE county_parish_holding_id = c.id AND user_account_id = max_id
  );

-- Cedric owns all 00/082/
INSERT INTO user_account_county_parish_holding_assignments
    (county_parish_holding_id, user_account_id, role_id, created_by_id, created_at)
SELECT c.id, cedric_id, cph_holder_role_id, admin_id, now()
FROM county_parish_holdings c
WHERE c.identifier IN ('00/082/0001', '00/082/0002', '00/082/0003', '00/082/0004', '00/082/0005', '00/082/0006')
  AND NOT EXISTS (
      SELECT 1 FROM user_account_county_parish_holding_assignments
      WHERE county_parish_holding_id = c.id AND user_account_id = cedric_id
  );

-- Max delegates 00/081/0004-0005 to Cedric
INSERT INTO county_parish_holding_delegations
    (county_parish_holding_id, delegating_user_id, delegated_user_id, delegated_user_email, delegated_user_role_id, invitation_token, invitation_expires_at, invitation_accepted_at, created_at, created_by_id)
SELECT c.id, max_id, cedric_id, 'cedric.brasey@planet-side.co.uk', delegation_role_id,
       '', now() + interval '2 days', now(), now(), max_id
FROM county_parish_holdings c
WHERE c.identifier IN ('00/081/0004', '00/081/0005')
  AND NOT EXISTS (
      SELECT 1 FROM county_parish_holding_delegations
      WHERE county_parish_holding_id = c.id AND delegating_user_id = max_id
  );

-- Cedric delegates 00/082/0004-0005 to Max
INSERT INTO county_parish_holding_delegations
    (county_parish_holding_id, delegating_user_id, delegated_user_id, delegated_user_email, delegated_user_role_id, invitation_token, invitation_expires_at, invitation_accepted_at, created_at, created_by_id)
SELECT c.id, cedric_id, max_id, 'max.bladen-clark@esynergy.co.uk', delegation_role_id,
       '', now() + interval '2 days', now(), now(), cedric_id
FROM county_parish_holdings c
WHERE c.identifier IN ('00/082/0004', '00/082/0005')
  AND NOT EXISTS (
      SELECT 1 FROM county_parish_holding_delegations
      WHERE county_parish_holding_id = c.id AND delegating_user_id = cedric_id
  );


END
$$;