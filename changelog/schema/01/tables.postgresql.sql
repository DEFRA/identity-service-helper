-- liquibase formatted sql

-- changeset system:initial-seed-3
create table krds_sync_logs
(
    id              uuid                                   not null
        constraint "PK_ksl"
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
    owner to identity_service_helper_ddl;

create index "IX_ksl_received_at"
    on krds_sync_logs (received_at);

create table roles
(
    id          uuid default gen_random_uuid() not null
        constraint "PK_r"
            primary key,
    name        varchar                        not null,
    description varchar                        not null
);

alter table roles
    owner to identity_service_helper_ddl;

create table user_accounts
(
    id            uuid                     default gen_random_uuid() not null
        constraint "PK_ua"
            primary key,
    email_address varchar                                            not null,
    display_name  citext                                             not null,
    first_name    varchar                                            not null,
    last_name     varchar                                            not null,
    krds_id       uuid,
    sam_id        uuid,
    created_at    timestamp with time zone default now()             not null,
    created_by_id uuid                                               not null
        constraint "FK_ua_ua_created_by_id"
            references user_accounts
            on delete cascade,
    deleted_at    timestamp with time zone,
    deleted_by_id uuid
        constraint "FK_ua_ua_deleted_by_id"
            references user_accounts
);

alter table user_accounts
    owner to identity_service_helper_ddl;

create index "IX_ua_created_by_id"
    on user_accounts (created_by_id);

create index "IX_ua_deleted_by_id"
    on user_accounts (deleted_by_id);

create unique index "IX_ua_email_address"
    on user_accounts (email_address);

create table applications
(
    id            uuid                     default gen_random_uuid() not null
        constraint "PK_app"
            primary key,
    name          text                                               not null,
    client_id     uuid                                               not null,
    tenant_name   text                                               not null,
    description   text                                               not null,
    created_at    timestamp with time zone default now()             not null,
    created_by_id uuid                                               not null
        constraint "FK_app_ua_created_by_id"
            references user_accounts
            on delete cascade,
    deleted_at    timestamp with time zone,
    deleted_by_id uuid
        constraint "FK_app_ua_deleted_by_id"
            references user_accounts,
    scopes        varchar(500),
    secret        varchar(74),
    redirect_uris varchar(1000)
);

alter table applications
    owner to identity_service_helper_ddl;

create index "IX_app_created_by_id"
    on applications (created_by_id);

create index "IX_app_deleted_by_id"
    on applications (deleted_by_id);

create table county_parish_holdings
(
    id            uuid                     default gen_random_uuid() not null
        constraint "PK_cph"
            primary key,
    identifier    varchar                                            not null,
    expired_at    timestamp with time zone,
    created_at    timestamp with time zone default now()             not null,
    created_by_id uuid                                               not null
        constraint "FK_cph_ua_created_by_id"
            references user_accounts
            on delete cascade,
    deleted_at    timestamp with time zone,
    deleted_by_id uuid
        constraint "FK_cph_ua_deleted_by_id"
            references user_accounts
);

alter table county_parish_holdings
    owner to identity_service_helper_ddl;

create index "IX_cph_created_by_id"
    on county_parish_holdings (created_by_id);

create index "IX_cph_deleted_by_id"
    on county_parish_holdings (deleted_by_id);

create index "IX_cph_identifier"
    on county_parish_holdings (identifier);


create table user_account_county_parish_holding_assignments
(
    id                       uuid                     default gen_random_uuid() not null
        constraint "PK_uacpha"
            primary key,
    county_parish_holding_id uuid                                               not null
        constraint "FK_uacpha_cph_cph_id"
            references county_parish_holdings
            on delete cascade,
    role_id                  uuid                                               not null
        constraint "FK_uacpha_r_role_id"
            references roles
            on delete cascade,
    user_account_id          uuid                                               not null
        constraint "FK_uacpha_ua_user_account_id"
            references user_accounts
            on delete cascade,
    created_at               timestamp with time zone default now()             not null,
    created_by_id            uuid                                               not null
        constraint "FK_uacpha_ua_created_by_id"
            references user_accounts
            on delete cascade,
    deleted_at               timestamp with time zone,
    deleted_by_id            uuid
        constraint "FK_uacpha_ua_deleted_by_id"
            references user_accounts
);

create index "IX_uacpha_cph_id"
    on user_account_county_parish_holding_assignments (county_parish_holding_id);

create index "IX_uacpha_created_by_id"
    on user_account_county_parish_holding_assignments (created_by_id);

create index "IX_uacpha_deleted_by_id"
    on user_account_county_parish_holding_assignments (deleted_by_id);

create index "IX_uacpha_role_id"
    on user_account_county_parish_holding_assignments (role_id);

create index "IX_uacpha_user_account_id"
    on user_account_county_parish_holding_assignments (user_account_id);

create table animal_species
(
    id        varchar(20)           not null
        constraint "PK_as"
            primary key,
    name      varchar(128)          not null,
    is_active boolean default false not null
);

alter table animal_species
    owner to identity_service_helper_ddl;

create table county_parish_holding_delegations
(
    id                       uuid default gen_random_uuid() not null
        constraint "PK_cphd"
            primary key,
    county_parish_holding_id uuid                           not null
        constraint "FK_cphd_cph_cph_id"
            references county_parish_holdings,
    delegating_user_id       uuid                           not null
        constraint "FK_cphd_ua_delegating_user_id"
            references user_accounts,
    delegated_user_id        uuid
        constraint "FK_cphd_ua_delegated_user_id"
            references user_accounts,
    delegated_user_email     varchar(256)                   not null,
    delegated_user_role_id   uuid                           not null
        constraint "FK_cphd_r_role_id"
            references roles,
    invitation_token         char(64)                       not null,
    invitation_expires_at    timestamp with time zone       not null,
    invitation_accepted_at   timestamp with time zone,
    invitation_rejected_at   timestamp with time zone,
    revoked_at               timestamp with time zone,
    revoked_by_id            uuid
        constraint "FK_cphd_ua_revoked_by_id"
            references user_accounts,
    expires_at               timestamp with time zone,
    created_at               timestamp with time zone       not null,
    created_by_id            uuid                           not null
        constraint "FK_cphd_ua_created_by_id"
            references user_accounts,
    deleted_at               timestamp with time zone,
    deleted_by_id            uuid
        constraint "FK_cphd_ua_deleted_by_id"
            references user_accounts
);

alter table county_parish_holding_delegations
    owner to identity_service_helper_ddl;

create table external_messaging
(
    id                bigint generated by default as identity
        constraint "PK_em"
            primary key,
    message_type      smallint default 1       not null,
    message_recipient text                     not null,
    template_id       uuid                     not null,
    notify_id         uuid                     not null,
    request_payload   text,
    sent_at           timestamp with time zone,
    response_code     integer,
    response_message  text,
    exception_message text,
    created_at        timestamp with time zone not null,
    created_by_id     uuid
        constraint "FK_em_ua_created_by_id"
            references user_accounts
);

alter table external_messaging
    owner to identity_service_helper_ddl;

create table county_parish_holding_delegations_notifications
(
    delegation_id uuid    not null
        constraint "FK_cphdn_cphd_delegation_id"
            references county_parish_holding_delegations,
    message_id    integer not null
        constraint "FK_cphdn_em_id"
            references external_messaging,
    constraint "PK_cphdn"
        primary key (message_id, delegation_id)
);

alter table county_parish_holding_delegations_notifications
    owner to identity_service_helper_ddl;

create table county_parish_holding_animal_species
(
    county_parish_holding_id uuid        not null
        constraint "FK_cphas_cph_id"
            references county_parish_holdings,
    animal_species_id        varchar(20) not null
        constraint "FK_cphas_as_id"
            references animal_species,
    created_at               timestamp with time zone       not null,
    created_by_id            uuid                           not null
        constraint "FK_cphas_ua_created_by_id"
            references user_accounts,
    deleted_at               timestamp with time zone,
    deleted_by_id            uuid
        constraint "FK_cphas_ua_deleted_by_id"
            references user_accounts,
    constraint "PK_cphas"
      primary key (county_parish_holding_id, animal_species_id)
);

alter table county_parish_holding_animal_species
    owner to identity_service_helper_ddl;

