using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetSalon.Models.Migrations
{
    /// <inheritdoc />
    public partial class AddDatabasePermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SCPermission",
                columns: table => new
                {
                    PermissionID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PermissionCode = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false, collation: "Latin1_General_100_CI_AS"),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCPermission", x => x.PermissionID);
                });

            migrationBuilder.CreateTable(
                name: "RolePermission",
                columns: table => new
                {
                    RolePermissionID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleID = table.Column<long>(type: "bigint", nullable: false),
                    PermissionID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => x.RolePermissionID);
                    table.ForeignKey(
                        name: "FK_RolePermission_SCPermission",
                        column: x => x.PermissionID,
                        principalTable: "SCPermission",
                        principalColumn: "PermissionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermission_SCRole",
                        column: x => x.RoleID,
                        principalTable: "SCRole",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_PermissionID",
                table: "RolePermission",
                column: "PermissionID");

            migrationBuilder.CreateIndex(
                name: "UX_RolePermission_RoleID_PermissionID",
                table: "RolePermission",
                columns: new[] { "RoleID", "PermissionID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_SCPermission_PermissionCode",
                table: "SCPermission",
                column: "PermissionCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolePermission");

            migrationBuilder.DropTable(
                name: "SCPermission");
        }
    }
}
