using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REIstacks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLists21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "property_notes",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "property_documents",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_property_notes_OrganizationId",
                table: "property_notes",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_property_documents_OrganizationId",
                table: "property_documents",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_property_documents_organizations_OrganizationId",
                table: "property_documents",
                column: "OrganizationId",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_property_notes_organizations_OrganizationId",
                table: "property_notes",
                column: "OrganizationId",
                principalTable: "organizations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_property_documents_organizations_OrganizationId",
                table: "property_documents");

            migrationBuilder.DropForeignKey(
                name: "FK_property_notes_organizations_OrganizationId",
                table: "property_notes");

            migrationBuilder.DropIndex(
                name: "IX_property_notes_OrganizationId",
                table: "property_notes");

            migrationBuilder.DropIndex(
                name: "IX_property_documents_OrganizationId",
                table: "property_documents");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "property_notes");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "property_documents");
        }
    }
}
