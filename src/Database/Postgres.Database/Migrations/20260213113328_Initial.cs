// <copyright file="20260213113328_Initial.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

#nullable disable

namespace Defra.Identity.Postgres.Database.Migrations
{
    using System;
    using System.Text.Json;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:citext", ",,")
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,");

            migrationBuilder.CreateTable(
                name: "krds_sync_logs",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    correlation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_endpoint = table.Column<string>(type: "text", nullable: false),
                    http_status = table.Column<int>(type: "integer", nullable: false),
                    processed_ok = table.Column<bool>(type: "boolean", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    received_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()"),
                    processed_at = table.Column<DateTime>(type: "TimestampTz", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_krds_sync_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "varchar", maxLength: 128, nullable: false),
                    description = table.Column<string>(type: "varchar", maxLength: 2048, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_accounts",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    email_address = table.Column<string>(type: "varchar", maxLength: 256, nullable: false),
                    display_name = table.Column<string>(type: "citext", nullable: false),
                    first_name = table.Column<string>(type: "varchar", maxLength: 256, nullable: false),
                    last_name = table.Column<string>(type: "varchar", maxLength: 256, nullable: false),
                    krds_id = table.Column<Guid>(type: "uuid", nullable: true),
                    sam_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()"),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "TimestampTz", nullable: true),
                    deleted_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_accounts", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_accounts_user_accounts_created_by_id",
                        column: x => x.created_by_id,
                        principalSchema: "public",
                        principalTable: "user_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_accounts_user_accounts_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalSchema: "public",
                        principalTable: "user_accounts",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "applications",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    name = table.Column<string>(type: "text", nullable: false, comment: "Human readable name for the application e.g Keeper Portal"),
                    client_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Azure AD B2C application Client ID"),
                    tenant_name = table.Column<string>(type: "text", nullable: false, comment: "Azure AD B2C tenant name e.g defra.onmicrosoft.com"),
                    description = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()"),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "TimestampTz", nullable: true),
                    deleted_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_applications", x => x.id);
                    table.ForeignKey(
                        name: "FK_applications_user_accounts_created_by_id",
                        column: x => x.created_by_id,
                        principalSchema: "public",
                        principalTable: "user_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_applications_user_accounts_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalSchema: "public",
                        principalTable: "user_accounts",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "county_parish_holdings",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    identifier = table.Column<string>(type: "varchar", maxLength: 11, nullable: false),
                    expired_at = table.Column<DateTime>(type: "TimestampTz", nullable: true),
                    created_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()"),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "TimestampTz", nullable: true),
                    deleted_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_county_parish_holdings", x => x.id);
                    table.ForeignKey(
                        name: "FK_county_parish_holdings_user_accounts_created_by_id",
                        column: x => x.created_by_id,
                        principalSchema: "public",
                        principalTable: "user_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_county_parish_holdings_user_accounts_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalSchema: "public",
                        principalTable: "user_accounts",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "application_roles",
                schema: "public",
                columns: table => new
                {
                    application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_roles", x => new { x.application_id, x.role_id });
                    table.ForeignKey(
                        name: "application_role_mapping_application_id_fk",
                        column: x => x.application_id,
                        principalSchema: "public",
                        principalTable: "applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "application_role_mapping_role_id_fk",
                        column: x => x.role_id,
                        principalSchema: "public",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "application_user_account_holding_assignments",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    county_parish_holding_id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()"),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "TimestampTz", nullable: true),
                    deleted_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_user_account_holding_assignments", x => x.id);
                    table.ForeignKey(
                        name: "FK_application_user_account_holding_assignments_applications_a~",
                        column: x => x.application_id,
                        principalSchema: "public",
                        principalTable: "applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_application_user_account_holding_assignments_county_parish_~",
                        column: x => x.county_parish_holding_id,
                        principalSchema: "public",
                        principalTable: "county_parish_holdings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_application_user_account_holding_assignments_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "public",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_application_user_account_holding_assignments_user_accounts_~",
                        column: x => x.created_by_id,
                        principalSchema: "public",
                        principalTable: "user_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_application_user_account_holding_assignments_user_accounts~1",
                        column: x => x.deleted_by_id,
                        principalSchema: "public",
                        principalTable: "user_accounts",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_application_user_account_holding_assignments_user_accounts~2",
                        column: x => x.user_account_id,
                        principalSchema: "public",
                        principalTable: "user_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "delegations",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    CountyParishHoldingsId = table.Column<Guid>(type: "uuid", nullable: true),
                    RolesId = table.Column<Guid>(type: "uuid", nullable: true),
                    RolesId1 = table.Column<Guid>(type: "uuid", nullable: true),
                    UserAccountsId = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()"),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "TimestampTz", nullable: true),
                    deleted_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_delegations", x => x.id);
                    table.ForeignKey(
                        name: "FK_delegations_applications_application_id",
                        column: x => x.application_id,
                        principalSchema: "public",
                        principalTable: "applications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_delegations_county_parish_holdings_CountyParishHoldingsId",
                        column: x => x.CountyParishHoldingsId,
                        principalSchema: "public",
                        principalTable: "county_parish_holdings",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_delegations_roles_RolesId",
                        column: x => x.RolesId,
                        principalSchema: "public",
                        principalTable: "roles",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_delegations_roles_RolesId1",
                        column: x => x.RolesId1,
                        principalSchema: "public",
                        principalTable: "roles",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_delegations_user_accounts_UserAccountsId",
                        column: x => x.UserAccountsId,
                        principalSchema: "public",
                        principalTable: "user_accounts",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_delegations_user_accounts_user_id",
                        column: x => x.user_id,
                        principalSchema: "public",
                        principalTable: "user_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "delegation_invitations",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    delegation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    invited_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    invited_email = table.Column<string>(type: "varchar", maxLength: 256, nullable: false),
                    invitation_token = table.Column<string>(type: "char(64)", maxLength: 64, nullable: false),
                    token_expires_at = table.Column<DateTime>(type: "TimestampTz", nullable: false),
                    delegated_role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    delegated_permissions = table.Column<JsonDocument>(type: "jsonb", nullable: true),
                    invited_at = table.Column<DateTime>(type: "TimestampTz", nullable: false),
                    accpeted_at = table.Column<DateTime>(type: "TimestampTz", nullable: false),
                    registered_at = table.Column<DateTime>(type: "TimestampTz", nullable: false),
                    activated_at = table.Column<DateTime>(type: "TimestampTz", nullable: false),
                    revoked_at = table.Column<DateTime>(type: "TimestampTz", nullable: false),
                    expired_at = table.Column<DateTime>(type: "TimestampTz", nullable: false),
                    created_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()"),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "TimestampTz", nullable: true),
                    deleted_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_delegation_invitations", x => x.id);
                    table.ForeignKey(
                        name: "FK_delegation_invitations_delegations_delegation_id",
                        column: x => x.delegation_id,
                        principalSchema: "public",
                        principalTable: "delegations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_delegation_invitations_roles_delegated_role_id",
                        column: x => x.delegated_role_id,
                        principalSchema: "public",
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_delegation_invitations_user_accounts_created_by_id",
                        column: x => x.created_by_id,
                        principalSchema: "public",
                        principalTable: "user_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_delegation_invitations_user_accounts_deleted_by_id",
                        column: x => x.deleted_by_id,
                        principalSchema: "public",
                        principalTable: "user_accounts",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "delegations_county_parish_holdings",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    delegation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    county_parish_holding_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()"),
                    created_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "TimestampTz", nullable: true),
                    deleted_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_delegations_county_parish_holdings", x => x.id);
                    table.ForeignKey(
                        name: "FK_delegations_county_parish_holdings_county_parish_holdings_c~",
                        column: x => x.county_parish_holding_id,
                        principalSchema: "public",
                        principalTable: "county_parish_holdings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_delegations_county_parish_holdings_delegations_delegation_id",
                        column: x => x.delegation_id,
                        principalSchema: "public",
                        principalTable: "delegations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_delegations_county_parish_holdings_user_accounts_created_by~",
                        column: x => x.created_by_id,
                        principalSchema: "public",
                        principalTable: "user_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_delegations_county_parish_holdings_user_accounts_deleted_by~",
                        column: x => x.deleted_by_id,
                        principalSchema: "public",
                        principalTable: "user_accounts",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_application_roles_role_id",
                schema: "public",
                table: "application_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_application_user_account_holding_assignments_application_id",
                schema: "public",
                table: "application_user_account_holding_assignments",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "IX_application_user_account_holding_assignments_county_parish_~",
                schema: "public",
                table: "application_user_account_holding_assignments",
                column: "county_parish_holding_id");

            migrationBuilder.CreateIndex(
                name: "IX_application_user_account_holding_assignments_created_by_id",
                schema: "public",
                table: "application_user_account_holding_assignments",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_application_user_account_holding_assignments_deleted_by_id",
                schema: "public",
                table: "application_user_account_holding_assignments",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_application_user_account_holding_assignments_role_id",
                schema: "public",
                table: "application_user_account_holding_assignments",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_application_user_account_holding_assignments_user_account_id",
                schema: "public",
                table: "application_user_account_holding_assignments",
                column: "user_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_applications_created_by_id",
                schema: "public",
                table: "applications",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_applications_deleted_by_id",
                schema: "public",
                table: "applications",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_county_parish_holdings_created_by_id",
                schema: "public",
                table: "county_parish_holdings",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_county_parish_holdings_deleted_by_id",
                schema: "public",
                table: "county_parish_holdings",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_county_parish_holdings_identifier",
                schema: "public",
                table: "county_parish_holdings",
                column: "identifier");

            migrationBuilder.CreateIndex(
                name: "IX_delegation_invitations_created_by_id",
                schema: "public",
                table: "delegation_invitations",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_delegation_invitations_delegated_role_id",
                schema: "public",
                table: "delegation_invitations",
                column: "delegated_role_id");

            migrationBuilder.CreateIndex(
                name: "IX_delegation_invitations_delegation_id",
                schema: "public",
                table: "delegation_invitations",
                column: "delegation_id");

            migrationBuilder.CreateIndex(
                name: "IX_delegation_invitations_deleted_by_id",
                schema: "public",
                table: "delegation_invitations",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_delegations_application_id",
                schema: "public",
                table: "delegations",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "IX_delegations_CountyParishHoldingsId",
                schema: "public",
                table: "delegations",
                column: "CountyParishHoldingsId");

            migrationBuilder.CreateIndex(
                name: "IX_delegations_RolesId",
                schema: "public",
                table: "delegations",
                column: "RolesId");

            migrationBuilder.CreateIndex(
                name: "IX_delegations_RolesId1",
                schema: "public",
                table: "delegations",
                column: "RolesId1");

            migrationBuilder.CreateIndex(
                name: "IX_delegations_user_id",
                schema: "public",
                table: "delegations",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_delegations_UserAccountsId",
                schema: "public",
                table: "delegations",
                column: "UserAccountsId");

            migrationBuilder.CreateIndex(
                name: "IX_delegations_county_parish_holdings_county_parish_holding_id",
                schema: "public",
                table: "delegations_county_parish_holdings",
                column: "county_parish_holding_id");

            migrationBuilder.CreateIndex(
                name: "IX_delegations_county_parish_holdings_created_by_id",
                schema: "public",
                table: "delegations_county_parish_holdings",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_delegations_county_parish_holdings_delegation_id",
                schema: "public",
                table: "delegations_county_parish_holdings",
                column: "delegation_id");

            migrationBuilder.CreateIndex(
                name: "IX_delegations_county_parish_holdings_deleted_by_id",
                schema: "public",
                table: "delegations_county_parish_holdings",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_krds_sync_logs_received_at",
                schema: "public",
                table: "krds_sync_logs",
                column: "received_at");

            migrationBuilder.CreateIndex(
                name: "IX_user_accounts_created_by_id",
                schema: "public",
                table: "user_accounts",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_accounts_deleted_by_id",
                schema: "public",
                table: "user_accounts",
                column: "deleted_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_EmailAddress",
                schema: "public",
                table: "user_accounts",
                column: "email_address",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application_roles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "application_user_account_holding_assignments",
                schema: "public");

            migrationBuilder.DropTable(
                name: "delegation_invitations",
                schema: "public");

            migrationBuilder.DropTable(
                name: "delegations_county_parish_holdings",
                schema: "public");

            migrationBuilder.DropTable(
                name: "krds_sync_logs",
                schema: "public");

            migrationBuilder.DropTable(
                name: "delegations",
                schema: "public");

            migrationBuilder.DropTable(
                name: "applications",
                schema: "public");

            migrationBuilder.DropTable(
                name: "county_parish_holdings",
                schema: "public");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user_accounts",
                schema: "public");
        }
    }
}
