using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toyana.Identity.Migrations
{
    /// <inheritdoc />
    public partial class AddVendorOrganizationAndRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vendor_user_permissions");

            migrationBuilder.DropTable(
                name: "vendor_user_roles");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "vendor_roles");

            migrationBuilder.RenameColumn(
                name: "vendor_id",
                table: "vendor_users",
                newName: "vendor_organization_id");

            migrationBuilder.RenameColumn(
                name: "is_owner",
                table: "vendor_users",
                newName: "is_deleted");

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "vendor_users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "role",
                table: "vendor_users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "vendor_organizations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    business_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    tax_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vendor_organizations", x => x.id);
                    table.ForeignKey(
                        name: "fk_vendor_organizations_vendor_users_owner_id",
                        column: x => x.owner_id,
                        principalTable: "vendor_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_vendor_users_vendor_organization_id",
                table: "vendor_users",
                column: "vendor_organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_vendor_organizations_owner_id",
                table: "vendor_organizations",
                column: "owner_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_vendor_organizations_tax_id",
                table: "vendor_organizations",
                column: "tax_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_vendor_users_vendor_organizations_vendor_organization_id",
                table: "vendor_users",
                column: "vendor_organization_id",
                principalTable: "vendor_organizations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_vendor_users_vendor_organizations_vendor_organization_id",
                table: "vendor_users");

            migrationBuilder.DropTable(
                name: "vendor_organizations");

            migrationBuilder.DropIndex(
                name: "ix_vendor_users_vendor_organization_id",
                table: "vendor_users");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "vendor_users");

            migrationBuilder.DropColumn(
                name: "role",
                table: "vendor_users");

            migrationBuilder.RenameColumn(
                name: "vendor_organization_id",
                table: "vendor_users",
                newName: "vendor_id");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "vendor_users",
                newName: "is_owner");

            migrationBuilder.CreateTable(
                name: "vendor_roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    vendor_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vendor_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    action = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    scope = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
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
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_vendor_user_roles_vendor_users_vendor_user_id",
                        column: x => x.vendor_user_id,
                        principalTable: "vendor_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vendor_user_permissions",
                columns: table => new
                {
                    vendor_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permission_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vendor_user_permissions", x => new { x.vendor_user_id, x.permission_id });
                    table.ForeignKey(
                        name: "fk_vendor_user_permissions_permissions_permission_id",
                        column: x => x.permission_id,
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_vendor_user_permissions_vendor_users_vendor_user_id",
                        column: x => x.vendor_user_id,
                        principalTable: "vendor_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

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
        }
    }
}
