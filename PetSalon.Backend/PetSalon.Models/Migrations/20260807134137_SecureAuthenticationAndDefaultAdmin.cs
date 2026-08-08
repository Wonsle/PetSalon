using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetSalon.Models.Migrations
{
    /// <inheritdoc />
    public partial class SecureAuthenticationAndDefaultAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF EXISTS (
                    SELECT 1
                    FROM dbo.SCUser
                    WHERE UserName IS NULL OR LTRIM(RTRIM(UserName)) = N''
                )
                    THROW 51000, 'SCUser contains an empty UserName. Resolve the data before applying this migration.', 1;

                IF EXISTS (
                    SELECT UserName COLLATE Latin1_General_100_CI_AS
                    FROM dbo.SCUser
                    GROUP BY UserName COLLATE Latin1_General_100_CI_AS
                    HAVING COUNT(*) > 1
                )
                    THROW 51001, 'SCUser contains duplicate UserName values. Resolve the data before applying this migration.', 1;

                IF EXISTS (
                    SELECT 1
                    FROM dbo.SCRole
                    WHERE RoleName IS NULL OR LTRIM(RTRIM(RoleName)) = ''
                )
                    THROW 51002, 'SCRole contains an empty RoleName. Resolve the data before applying this migration.', 1;

                IF EXISTS (
                    SELECT RoleName
                    FROM dbo.SCRole
                    GROUP BY RoleName
                    HAVING COUNT(*) > 1
                )
                    THROW 51003, 'SCRole contains duplicate RoleName values. Resolve the data before applying this migration.', 1;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "SCUser",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                collation: "Latin1_General_100_CI_AS",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "SCUser",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "MustChangePassword",
                table: "SCUser",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "RoleName",
                table: "SCRole",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    UserRoleID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SCUserID = table.Column<long>(type: "bigint", nullable: false),
                    SCRoleID = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.UserRoleID);
                    table.ForeignKey(
                        name: "FK_UserRole_SCRole",
                        column: x => x.SCRoleID,
                        principalTable: "SCRole",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_SCUser",
                        column: x => x.SCUserID,
                        principalTable: "SCUser",
                        principalColumn: "SCUserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "UX_SCUser_UserName",
                table: "SCUser",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_SCRole_RoleName",
                table: "SCRole",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_SCRoleID",
                table: "UserRole",
                column: "SCRoleID");

            migrationBuilder.CreateIndex(
                name: "UX_UserRole_SCUserID_SCRoleID",
                table: "UserRole",
                columns: new[] { "SCUserID", "SCRoleID" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "UserRole");
            migrationBuilder.DropIndex(name: "UX_SCUser_UserName", table: "SCUser");
            migrationBuilder.DropIndex(name: "UX_SCRole_RoleName", table: "SCRole");
            migrationBuilder.DropColumn(name: "IsActive", table: "SCUser");
            migrationBuilder.DropColumn(name: "MustChangePassword", table: "SCUser");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "SCUser",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldCollation: "Latin1_General_100_CI_AS");

            migrationBuilder.AlterColumn<string>(
                name: "RoleName",
                table: "SCRole",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50);
        }
    }
}
