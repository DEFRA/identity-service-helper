-- changeset gary:raw::includeAll runOnChange:true
SET search_path TO "defra-ci", public;

create table krds_sync_logs
(
    id              uuid                                   not null
        constraint "PK_krds_sync_logs"
            primary key,
    correlation_id  uuid                                   not null,
    source_endpoint text                                   not null,
    http_status     integer                                not null,
    processed_ok    boolean                                not null,
    message         text                                   not null,
    received_at     timestamp with time zone default now() not null,
    processed_at    timestamp with time zone               not null
);

alter table krds_sync_logs
    owner to postgres;

create index "IX_krds_sync_logs_received_at"
    on krds_sync_logs (received_at);

create table roles
(
    id          uuid default gen_random_uuid() not null
        constraint "PK_roles"
            primary key,
    name        varchar                        not null,
    description varchar                        not null
);

alter table roles
    owner to postgres;

create table user_accounts
(
    id            uuid                     default gen_random_uuid() not null
        constraint "PK_user_accounts"
            primary key,
    email_address varchar                                            not null,
    display_name  citext                                             not null,
    first_name    varchar                                            not null,
    last_name     varchar                                            not null,
    krds_id       uuid,
    sam_id        uuid,
    created_at    timestamp with time zone default now()             not null,
    created_by_id uuid                                               not null
        constraint "FK_user_accounts_user_accounts_created_by_id"
            references user_accounts
            on delete cascade,
    deleted_at    timestamp with time zone,
    deleted_by_id uuid
        constraint "FK_user_accounts_user_accounts_deleted_by_id"
            references user_accounts,
    is_deleted    boolean                  default false             not null
);

alter table user_accounts
    owner to postgres;

create index "IX_user_accounts_created_by_id"
    on user_accounts (created_by_id);

create index "IX_user_accounts_deleted_by_id"
    on user_accounts (deleted_by_id);

create unique index "IX_UserAccounts_EmailAddress"
    on user_accounts (email_address);

create table applications
(
    id            uuid                     default gen_random_uuid() not null
        constraint "PK_applications"
            primary key,
    name          text                                               not null,
    client_id     uuid                                               not null,
    tenant_name   text                                               not null,
    description   text                                               not null,
    created_at    timestamp with time zone default now()             not null,
    created_by_id uuid                                               not null
        constraint "FK_applications_user_accounts_created_by_id"
            references user_accounts
            on delete cascade,
    deleted_at    timestamp with time zone,
    deleted_by_id uuid
        constraint "FK_applications_user_accounts_deleted_by_id"
            references user_accounts,
    is_deleted    boolean                  default false             not null
);

comment on column applications.name is 'Human readable name for the application e.g Keeper Portal';

comment on column applications.client_id is 'Azure AD B2C application Client ID';

comment on column applications.tenant_name is 'Azure AD B2C tenant name e.g defra.onmicrosoft.com';

alter table applications
    owner to postgres;

create index "IX_applications_created_by_id"
    on applications (created_by_id);

create index "IX_applications_deleted_by_id"
    on applications (deleted_by_id);

create table county_parish_holdings
(
    id            uuid                     default gen_random_uuid() not null
        constraint "PK_county_parish_holdings"
            primary key,
    identifier    varchar                                            not null,
    expired_at    timestamp with time zone,
    created_at    timestamp with time zone default now()             not null,
    created_by_id uuid                                               not null
        constraint "FK_county_parish_holdings_user_accounts_created_by_id"
            references user_accounts
            on delete cascade,
    deleted_at    timestamp with time zone,
    deleted_by_id uuid
        constraint "FK_county_parish_holdings_user_accounts_deleted_by_id"
            references user_accounts,
    is_deleted    boolean                  default false             not null
);

alter table county_parish_holdings
    owner to postgres;

create index "IX_county_parish_holdings_created_by_id"
    on county_parish_holdings (created_by_id);

create index "IX_county_parish_holdings_deleted_by_id"
    on county_parish_holdings (deleted_by_id);

create index "IX_county_parish_holdings_identifier"
    on county_parish_holdings (identifier);

create table application_roles
(
    application_id uuid not null
        constraint application_role_mapping_application_id_fk
            references applications
            on delete cascade,
    role_id        uuid not null
        constraint application_role_mapping_role_id_fk
            references roles
            on delete cascade,
    constraint "PK_application_roles"
        primary key (application_id, role_id)
);

alter table application_roles
    owner to postgres;

create index "IX_application_roles_role_id"
    on application_roles (role_id);

create table application_user_account_holding_assignments
(
    id                       uuid                     default gen_random_uuid() not null
        constraint "PK_application_user_account_holding_assignments"
            primary key,
    county_parish_holding_id uuid                                               not null
        constraint "FK_application_user_account_holding_assignments_county_parish_~"
            references county_parish_holdings
            on delete cascade,
    application_id           uuid                                               not null
        constraint "FK_application_user_account_holding_assignments_applications_a~"
            references applications
            on delete cascade,
    role_id                  uuid                                               not null
        constraint "FK_application_user_account_holding_assignments_roles_role_id"
            references roles
            on delete cascade,
    user_account_id          uuid                                               not null
        constraint "FK_application_user_account_holding_assignments_user_accounts~2"
            references user_accounts
            on delete cascade,
    created_at               timestamp with time zone default now()             not null,
    created_by_id            uuid                                               not null
        constraint "FK_application_user_account_holding_assignments_user_accounts_~"
            references user_accounts
            on delete cascade,
    deleted_at               timestamp with time zone,
    deleted_by_id            uuid
        constraint "FK_application_user_account_holding_assignments_user_accounts~1"
            references user_accounts,
    is_deleted               boolean                  default false             not null
);

alter table application_user_account_holding_assignments
    owner to postgres;

create index "IX_application_user_account_holding_assignments_application_id"
    on application_user_account_holding_assignments (application_id);

create index "IX_application_user_account_holding_assignments_county_parish_~"
    on application_user_account_holding_assignments (county_parish_holding_id);

create index "IX_application_user_account_holding_assignments_created_by_id"
    on application_user_account_holding_assignments (created_by_id);

create index "IX_application_user_account_holding_assignments_deleted_by_id"
    on application_user_account_holding_assignments (deleted_by_id);

create index "IX_application_user_account_holding_assignments_role_id"
    on application_user_account_holding_assignments (role_id);

create index "IX_application_user_account_holding_assignments_user_account_id"
    on application_user_account_holding_assignments (user_account_id);

create table delegations
(
    id                       uuid                     default gen_random_uuid() not null
        constraint "PK_delegations"
            primary key,
    application_id           uuid                                               not null
        constraint "FK_delegations_applications_application_id"
            references applications
            on delete cascade,
    user_id                  uuid                                               not null
        constraint "FK_delegations_user_accounts_user_id"
            references user_accounts
            on delete cascade,
    "CountyParishHoldingsId" uuid
        constraint "FK_delegations_county_parish_holdings_CountyParishHoldingsId"
            references county_parish_holdings,
    "RolesId"                uuid
        constraint "FK_delegations_roles_RolesId"
            references roles,
    "RolesId1"               uuid
        constraint "FK_delegations_roles_RolesId1"
            references roles,
    "UserAccountsId"         uuid
        constraint "FK_delegations_user_accounts_UserAccountsId"
            references user_accounts,
    created_at               timestamp with time zone default now()             not null,
    created_by_id            uuid                                               not null,
    deleted_at               timestamp with time zone,
    deleted_by_id            uuid,
    is_deleted               boolean                  default false             not null
);

alter table delegations
    owner to postgres;

create index "IX_delegations_application_id"
    on delegations (application_id);

create index "IX_delegations_CountyParishHoldingsId"
    on delegations ("CountyParishHoldingsId");

create index "IX_delegations_RolesId"
    on delegations ("RolesId");

create index "IX_delegations_RolesId1"
    on delegations ("RolesId1");

create index "IX_delegations_user_id"
    on delegations (user_id);

create index "IX_delegations_UserAccountsId"
    on delegations ("UserAccountsId");

create table delegation_invitations
(
    id                    uuid                     default gen_random_uuid() not null
        constraint "PK_delegation_invitations"
            primary key,
    delegation_id         uuid                                               not null
        constraint "FK_delegation_invitations_delegations_delegation_id"
            references delegations
            on delete cascade,
    invited_user_id       uuid                                               not null,
    invited_email         varchar                                            not null,
    invitation_token      char(64)                                           not null,
    token_expires_at      timestamp with time zone                           not null,
    delegated_role_id     uuid                                               not null
        constraint "FK_delegation_invitations_roles_delegated_role_id"
            references roles
            on delete cascade,
    delegated_permissions jsonb,
    invited_at            timestamp with time zone                           not null,
    accpeted_at           timestamp with time zone                           not null,
    registered_at         timestamp with time zone                           not null,
    activated_at          timestamp with time zone                           not null,
    revoked_at            timestamp with time zone                           not null,
    expired_at            timestamp with time zone                           not null,
    created_at            timestamp with time zone default now()             not null,
    created_by_id         uuid                                               not null
        constraint "FK_delegation_invitations_user_accounts_created_by_id"
            references user_accounts
            on delete cascade,
    deleted_at            timestamp with time zone,
    deleted_by_id         uuid
        constraint "FK_delegation_invitations_user_accounts_deleted_by_id"
            references user_accounts,
    is_deleted            boolean                  default false             not null
);

alter table delegation_invitations
    owner to postgres;

create index "IX_delegation_invitations_created_by_id"
    on delegation_invitations (created_by_id);

create index "IX_delegation_invitations_delegated_role_id"
    on delegation_invitations (delegated_role_id);

create index "IX_delegation_invitations_delegation_id"
    on delegation_invitations (delegation_id);

create index "IX_delegation_invitations_deleted_by_id"
    on delegation_invitations (deleted_by_id);

create table delegations_county_parish_holdings
(
    id                       uuid                     default gen_random_uuid() not null
        constraint "PK_delegations_county_parish_holdings"
            primary key,
    delegation_id            uuid                                               not null
        constraint "FK_delegations_county_parish_holdings_delegations_delegation_id"
            references delegations
            on delete cascade,
    county_parish_holding_id uuid                                               not null
        constraint "FK_delegations_county_parish_holdings_county_parish_holdings_c~"
            references county_parish_holdings
            on delete cascade,
    created_at               timestamp with time zone default now()             not null,
    created_by_id            uuid                                               not null
        constraint "FK_delegations_county_parish_holdings_user_accounts_created_by~"
            references user_accounts
            on delete cascade,
    deleted_at               timestamp with time zone,
    deleted_by_id            uuid
        constraint "FK_delegations_county_parish_holdings_user_accounts_deleted_by~"
            references user_accounts,
    is_deleted               boolean                  default false             not null
);

alter table delegations_county_parish_holdings
    owner to postgres;

create index "IX_delegations_county_parish_holdings_county_parish_holding_id"
    on delegations_county_parish_holdings (county_parish_holding_id);

create index "IX_delegations_county_parish_holdings_created_by_id"
    on delegations_county_parish_holdings (created_by_id);

create index "IX_delegations_county_parish_holdings_delegation_id"
    on delegations_county_parish_holdings (delegation_id);

create index "IX_delegations_county_parish_holdings_deleted_by_id"
    on delegations_county_parish_holdings (deleted_by_id);

