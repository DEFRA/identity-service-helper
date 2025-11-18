using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Livestock.Auth.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
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
                name: "application",
                schema: "defra-ci",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false, comment: "Human readable name for the application e.g Keeper Portal"),
                    client_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Azure AD B2C application Client ID"),
                    tenant_name = table.Column<string>(type: "text", nullable: false, comment: "Azure AD B2C tenant name e.g defra.onmicrosoft.com"),
                    description = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false, defaultValue: "active", comment: "active/inactive/deprecated"),
                    created_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "krds_sync_log",
                schema: "defra-ci",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    correlation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    upn = table.Column<string>(type: "citext", nullable: false),
                    payload_sha256 = table.Column<string>(type: "text", nullable: false),
                    source_endpoint = table.Column<string>(type: "text", nullable: false),
                    http_status = table.Column<int>(type: "integer", nullable: false),
                    processed_ok = table.Column<bool>(type: "boolean", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    received_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()"),
                    processed_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_krds_sync_log", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_account",
                schema: "defra-ci",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    upn = table.Column<string>(type: "citext", nullable: false),
                    display_name = table.Column<string>(type: "varchar", maxLength: 256, nullable: false),
                    account_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_account", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "enrolment",
                schema: "defra-ci",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    application_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cph_id = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<string>(type: "text", nullable: false),
                    scope = table.Column<string>(type: "jsonb", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false, defaultValue: "active"),
                    enrolled_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()"),
                    expires_at = table.Column<DateTime>(type: "TimestampTz", nullable: false),
                    created_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_enrolment", x => x.id);
                    table.ForeignKey(
                        name: "FK_enrolment_application_application_id",
                        column: x => x.application_id,
                        principalSchema: "defra-ci",
                        principalTable: "application",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_enrolment_user_account_user_account_id",
                        column: x => x.user_account_id,
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
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_name = table.Column<string>(type: "text", nullable: false),
                    object_id = table.Column<Guid>(type: "uuid", nullable: false),
                    trust_level = table.Column<string>(type: "text", nullable: false, defaultValue: "standard"),
                    sync_status = table.Column<string>(type: "text", nullable: false, defaultValue: "linked"),
                    last_synced_at = table.Column<DateTime>(type: "TimestampTz", nullable: false),
                    created_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()"),
                    updated_at = table.Column<DateTime>(type: "TimestampTz", nullable: false, defaultValueSql: "now()")
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
                name: "IX_enrolment_application_id",
                schema: "defra-ci",
                table: "enrolment",
                column: "application_id");

            migrationBuilder.CreateIndex(
                name: "IX_enrolment_user_account_id_role",
                schema: "defra-ci",
                table: "enrolment",
                columns: new[] { "user_account_id", "role" },
                unique: true);

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
                name: "IX_krds_sync_log_upn",
                schema: "defra-ci",
                table: "krds_sync_log",
                column: "upn");

            migrationBuilder.CreateIndex(
                name: "IX_user_account_upn",
                schema: "defra-ci",
                table: "user_account",
                column: "upn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "enrolment",
                schema: "defra-ci");

            migrationBuilder.DropTable(
                name: "federation",
                schema: "defra-ci");

            migrationBuilder.DropTable(
                name: "krds_sync_log",
                schema: "defra-ci");

            migrationBuilder.DropTable(
                name: "application",
                schema: "defra-ci");

            migrationBuilder.DropTable(
                name: "user_account",
                schema: "defra-ci");
        }
    }
}
