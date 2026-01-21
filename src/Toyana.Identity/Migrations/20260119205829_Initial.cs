using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toyana.Identity.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "admin_users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    password_hash = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    salt = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_admin_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "client_users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_banned = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    salt = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_client_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vendor_roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    vendor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vendor_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vendor_users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    vendor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    is_owner = table.Column<bool>(type: "boolean", nullable: false),
                    is_banned = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    salt = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vendor_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    scope = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    action = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    vendor_role_id = table.Column<Guid>(type: "uuid", nullable: true),
                    vendor_user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_permissions", x => x.id);
                    table.ForeignKey(
                        name: "fk_permissions_vendor_roles_vendor_role_id",
                        column: x => x.vendor_role_id,
                        principalTable: "vendor_roles",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_permissions_vendor_users_vendor_user_id",
                        column: x => x.vendor_user_id,
                        principalTable: "vendor_users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "vendor_user_roles",
                columns: table => new
                {
                    vendor_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vendor_role_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vendor_user_roles", x => new { x.vendor_user_id, x.vendor_role_id });
                    table.ForeignKey(
                        name: "fk_vendor_user_roles_vendor_roles_vendor_role_id",
                        column: x => x.vendor_role_id,
                        principalTable: "vendor_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_vendor_user_roles_vendor_users_vendor_user_id",
                        column: x => x.vendor_user_id,
                        principalTable: "vendor_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vendor_user_permissions",
                columns: table => new
                {
                    permission_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vendor_user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vendor_user_permissions", x => new { x.vendor_user_id, x.permission_id });
                    table.ForeignKey(
                        name: "fk_vendor_user_permissions_permissions_permission_id",
                        column: x => x.permission_id,
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_vendor_user_permissions_vendor_users_vendor_user_id",
                        column: x => x.vendor_user_id,
                        principalTable: "vendor_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_admin_users_username",
                table: "admin_users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_client_users_phone_number",
                table: "client_users",
                column: "phone_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_client_users_username",
                table: "client_users",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_permissions_action_scope",
                table: "permissions",
                columns: new[] { "action", "scope" });

            migrationBuilder.CreateIndex(
                name: "ix_permissions_vendor_role_id",
                table: "permissions",
                column: "vendor_role_id");

            migrationBuilder.CreateIndex(
                name: "ix_permissions_vendor_user_id",
                table: "permissions",
                column: "vendor_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_vendor_user_permissions_permission_id",
                table: "vendor_user_permissions",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "ix_vendor_user_roles_vendor_role_id",
                table: "vendor_user_roles",
                column: "vendor_role_id");

            migrationBuilder.CreateIndex(
                name: "ix_vendor_users_username",
                table: "vendor_users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admin_users");

            migrationBuilder.DropTable(
                name: "client_users");

            migrationBuilder.DropTable(
                name: "vendor_user_permissions");

            migrationBuilder.DropTable(
                name: "vendor_user_roles");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "vendor_roles");

            migrationBuilder.DropTable(
                name: "vendor_users");
        }
    }
}
