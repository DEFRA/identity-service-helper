using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Defra.Identity.Postgres.Database.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "defra-ci");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:citext", ",,")
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "krds_sync_log",
                schema: "defra-ci",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    correlation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_endpoint = table.Column<string>(type: "text", nullable: false),
                    http_status = table.Column<int>(type: "integer", nullable: false),
                    processed_ok = table.Column<bool>(type: "boolean", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    received_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()"),
                    processed_at = table.Column<DateTime>(type: "TimestampTz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_krds_sync_log", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                schema: "defra-ci",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "varchar", maxLength: 128, nullable: false),
                    description = table.Column<string>(type: "varchar", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "status_type",
                schema: "defra-ci",
                columns: table => new
                {
                    id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "varchar", maxLength: 10, nullable: false),
                    description = table.Column<string>(type: "varchar", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_status_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "application",
                schema: "defra-ci",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    name = table.Column<string>(type: "text", nullable: false, comment: "Human readable name for the application e.g Keeper Portal"),
                    client_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Azure AD B2C application Client ID"),
                    tenant_name = table.Column<string>(type: "text", nullable: false, comment: "Azure AD B2C tenant name e.g defra.onmicrosoft.com"),
                    description = table.Column<string>(type: "text", nullable: false),
                    status_type_id = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    created_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "TimestampTz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application", x => x.id);
                    table.ForeignKey(
                        name: "FK_application_status_type_status_type_id",
                        column: x => x.status_type_id,
                        principalSchema: "defra-ci",
                        principalTable: "status_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "county_parish_holding",
                schema: "defra-ci",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    identifier = table.Column<string>(type: "varchar", maxLength: 11, nullable: false),
                    status_type_id = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    created_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    processed_at = table.Column<DateTime>(type: "TimestampTz", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "TimestampTz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_county_parish_holding", x => x.id);
                    table.ForeignKey(
                        name: "FK_county_parish_holding_status_type_status_type_id",
                        column: x => x.status_type_id,
                        principalSchema: "defra-ci",
                        principalTable: "status_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_account",
                schema: "defra-ci",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    email_address = table.Column<string>(type: "varchar", maxLength: 256, nullable: false),
                    status_type_id = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    display_name = table.Column<string>(type: "citext", nullable: false),
                    first_name = table.Column<string>(type: "varchar", maxLength: 256, nullable: false),
                    last_name = table.Column<string>(type: "varchar", maxLength: 256, nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "TimestampTz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_account", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_account_status_type_status_type_id",
                        column: x => x.status_type_id,
                        principalSchema: "defra-ci",
                        principalTable: "status_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_account_user_account_created_by",
                        column: x => x.created_by,
                        principalSchema: "defra-ci",
                        principalTable: "user_account",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_user_account_user_account_updated_by",
                        column: x => x.updated_by,
                        principalSchema: "defra-ci",
                        principalTable: "user_account",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "application_role",
                schema: "defra-ci",
                columns: table => new
                {
                    application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_role", x => new { x.application_id, x.role_id });
                    table.ForeignKey(
                        name: "application_role_mapping_application_id_fk",
                        column: x => x.application_id,
                        principalSchema: "defra-ci",
                        principalTable: "application",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "application_role_mapping_role_id_fk",
                        column: x => x.role_id,
                        principalSchema: "defra-ci",
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "registration",
                schema: "defra-ci",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    country_parish_holding_id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status_type_id = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    enrolled_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()"),
                    expires_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "TimestampTz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registration", x => x.id);
                    table.ForeignKey(
                        name: "FK_registration_application_application_id",
                        column: x => x.application_id,
                        principalSchema: "defra-ci",
                        principalTable: "application",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_registration_county_parish_holding_country_parish_holding_id",
                        column: x => x.country_parish_holding_id,
                        principalSchema: "defra-ci",
                        principalTable: "county_parish_holding",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_registration_status_type_status_type_id",
                        column: x => x.status_type_id,
                        principalSchema: "defra-ci",
                        principalTable: "status_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "delegation",
                schema: "defra-ci",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    status_type_id = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)1),
                    county_parish_holding_id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    invited_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    invited_by_role_id = table.Column<short>(type: "smallint", nullable: false),
                    invited_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    invited_email = table.Column<string>(type: "varchar", maxLength: 256, nullable: false),
                    invitation_token = table.Column<string>(type: "char(64)", maxLength: 64, nullable: false),
                    token_expires_at = table.Column<DateTime>(type: "char", maxLength: 64, nullable: false),
                    delegated_role_id = table.Column<short>(type: "smallint", nullable: false),
                    delegated_permissions = table.Column<object>(type: "jsonb", nullable: false),
                    invited_at = table.Column<DateTime>(type: "TimestampTz", nullable: false),
                    activated_at = table.Column<DateTime>(type: "TimestampTz", nullable: false),
                    registered_at = table.Column<DateTime>(type: "TimestampTz", nullable: false),
                    revoked_at = table.Column<DateTime>(type: "TimestampTz", nullable: false),
                    ActivatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expired_at = table.Column<DateTime>(type: "TimestampTz", nullable: false),
                    created_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at = table.Column<DateTime>(type: "TimestampTz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_delegation", x => x.id);
                    table.ForeignKey(
                        name: "FK_delegation_application_application_id",
                        column: x => x.application_id,
                        principalSchema: "defra-ci",
                        principalTable: "application",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_delegation_county_parish_holding_county_parish_holding_id",
                        column: x => x.county_parish_holding_id,
                        principalSchema: "defra-ci",
                        principalTable: "county_parish_holding",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_delegation_role_delegated_role_id",
                        column: x => x.delegated_role_id,
                        principalSchema: "defra-ci",
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_delegation_role_invited_by_role_id",
                        column: x => x.invited_by_role_id,
                        principalSchema: "defra-ci",
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_delegation_status_type_status_type_id",
                        column: x => x.status_type_id,
                        principalSchema: "defra-ci",
                        principalTable: "status_type",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_delegation_user_account_invited_by_user_id",
                        column: x => x.invited_by_user_id,
                        principalSchema: "defra-ci",
                        principalTable: "user_account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_delegation_user_account_invited_user_id",
                        column: x => x.invited_user_id,
                        principalSchema: "defra-ci",
                        principalTable: "user_account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "federation",
                schema: "defra-ci",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    user_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_name = table.Column<string>(type: "text", nullable: false),
                    object_id = table.Column<Guid>(type: "uuid", nullable: false),
                    trust_level = table.Column<string>(type: "text", nullable: false, defaultValue: "standard"),
                    sync_status = table.Column<string>(type: "text", nullable: false, defaultValue: "linked"),
                    last_synced_at = table.Column<DateTime>(type: "TimestampTz", nullable: false),
                    created_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "TimestampTz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_federation", x => x.id);
                    table.ForeignKey(
                        name: "FK_federation_user_account_user_account_id",
                        column: x => x.user_account_id,
                        principalSchema: "defra-ci",
                        principalTable: "user_account",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_application_status_type_id",
                schema: "defra-ci",
                table: "application",
                column: "status_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_application_role_role_id",
                schema: "defra-ci",
                table: "application_role",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_county_parish_holding_identifier",
                schema: "defra-ci",
                table: "county_parish_holding",
                column: "identifier");

            migrationBuilder.CreateIndex(
                name: "IX_county_parish_holding_status_type_id",
                schema: "defra-ci",
                table: "county_parish_holding",
                column: "status_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_delegation_application_id",
                schema: "defra-ci",
                table: "delegation",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "IX_delegation_county_parish_holding_id",
                schema: "defra-ci",
                table: "delegation",
                column: "county_parish_holding_id");

            migrationBuilder.CreateIndex(
                name: "IX_delegation_delegated_role_id",
                schema: "defra-ci",
                table: "delegation",
                column: "delegated_role_id");

            migrationBuilder.CreateIndex(
                name: "IX_delegation_invited_by_role_id",
                schema: "defra-ci",
                table: "delegation",
                column: "invited_by_role_id");

            migrationBuilder.CreateIndex(
                name: "IX_delegation_invited_by_user_id",
                schema: "defra-ci",
                table: "delegation",
                column: "invited_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_delegation_invited_user_id",
                schema: "defra-ci",
                table: "delegation",
                column: "invited_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_delegation_status_type_id",
                schema: "defra-ci",
                table: "delegation",
                column: "status_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_federation_object_id_tenant_name",
                schema: "defra-ci",
                table: "federation",
                columns: new[] { "object_id", "tenant_name" });

            migrationBuilder.CreateIndex(
                name: "IX_federation_user_account_id",
                schema: "defra-ci",
                table: "federation",
                column: "user_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_krds_sync_log_received_at",
                schema: "defra-ci",
                table: "krds_sync_log",
                column: "received_at");

            migrationBuilder.CreateIndex(
                name: "IX_registration_application_id",
                schema: "defra-ci",
                table: "registration",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "IX_registration_country_parish_holding_id",
                schema: "defra-ci",
                table: "registration",
                column: "country_parish_holding_id");

            migrationBuilder.CreateIndex(
                name: "IX_registration_status_type_id",
                schema: "defra-ci",
                table: "registration",
                column: "status_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_account_created_by",
                schema: "defra-ci",
                table: "user_account",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_user_account_status_type_id",
                schema: "defra-ci",
                table: "user_account",
                column: "status_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_account_updated_by",
                schema: "defra-ci",
                table: "user_account",
                column: "updated_by");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application_role",
                schema: "defra-ci");

            migrationBuilder.DropTable(
                name: "delegation",
                schema: "defra-ci");

            migrationBuilder.DropTable(
                name: "federation",
                schema: "defra-ci");

            migrationBuilder.DropTable(
                name: "krds_sync_log",
                schema: "defra-ci");

            migrationBuilder.DropTable(
                name: "registration",
                schema: "defra-ci");

            migrationBuilder.DropTable(
                name: "role",
                schema: "defra-ci");

            migrationBuilder.DropTable(
                name: "user_account",
                schema: "defra-ci");

            migrationBuilder.DropTable(
                name: "application",
                schema: "defra-ci");

            migrationBuilder.DropTable(
                name: "county_parish_holding",
                schema: "defra-ci");

            migrationBuilder.DropTable(
                name: "status_type",
                schema: "defra-ci");
        }
    }
}
