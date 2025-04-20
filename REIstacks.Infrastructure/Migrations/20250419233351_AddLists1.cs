using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REIstacks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLists1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "property_lists",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "lists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_property_lists_OrganizationId",
                table: "property_lists",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_property_lists_organizations_OrganizationId",
                table: "property_lists",
                column: "OrganizationId",
                principalTable: "organizations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_property_lists_organizations_OrganizationId",
                table: "property_lists");

            migrationBuilder.DropIndex(
                name: "IX_property_lists_OrganizationId",
                table: "property_lists");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "property_lists");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "lists");
        }
    }
}
