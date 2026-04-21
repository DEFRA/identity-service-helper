-- liquibase formatted sql

-- changeset cedricbrasey:1773744435706-04-03 splitStatements:false
create table county_parish_holding_delegations_notifications
(
    delegation_id uuid    not null
        constraint cph_delegations_notifications_cph_delegation_id_fk
            references county_parish_holding_delegations,
    message_id    integer not null
        constraint cph_delegations_notifications_external_messagimg_id_fk
            references external_messaging,
    constraint cph_delegations_notification_pk
        primary key (message_id, delegation_id)
);
