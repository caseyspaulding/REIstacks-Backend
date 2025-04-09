using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REIstacks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LandingPageComponents_LandingPageId",
                table: "LandingPageComponents");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "LandingPages");

            migrationBuilder.AddColumn<int>(
                name: "OrganizationRoleId",
                table: "profiles",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MetaTitle",
                table: "LandingPages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "LandingPages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HeroImageUrl",
                table: "LandingPages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "LandingPages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_profiles_OrganizationRoleId",
                table: "profiles",
                column: "OrganizationRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_LandingPageComponents_LandingPageId_OrderIndex",
                table: "LandingPageComponents",
                columns: new[] { "LandingPageId", "OrderIndex" });

            migrationBuilder.AddForeignKey(
                name: "FK_profiles_organization_roles_OrganizationRoleId",
                table: "profiles",
                column: "OrganizationRoleId",
                principalTable: "organization_roles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_profiles_organization_roles_OrganizationRoleId",
                table: "profiles");

            migrationBuilder.DropIndex(
                name: "IX_profiles_OrganizationRoleId",
                table: "profiles");

            migrationBuilder.DropIndex(
                name: "IX_LandingPageComponents_LandingPageId_OrderIndex",
                table: "LandingPageComponents");

            migrationBuilder.DropColumn(
                name: "OrganizationRoleId",
                table: "profiles");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "LandingPages");

            migrationBuilder.DropColumn(
                name: "HeroImageUrl",
                table: "LandingPages");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "LandingPages");

            migrationBuilder.AlterColumn<string>(
                name: "MetaTitle",
                table: "LandingPages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "LandingPages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_LandingPageComponents_LandingPageId",
                table: "LandingPageComponents",
                column: "LandingPageId");
        }
    }
}
