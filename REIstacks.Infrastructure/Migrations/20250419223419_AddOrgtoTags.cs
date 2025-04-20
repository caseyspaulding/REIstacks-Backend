using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REIstacks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrgtoTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contact_tags_contacts_ContactId",
                table: "contact_tags");

            migrationBuilder.DropForeignKey(
                name: "FK_property_tags_properties_PropertyId",
                table: "property_tags");

            migrationBuilder.DropForeignKey(
                name: "FK_property_tags_tags_TagId",
                table: "property_tags");

            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "property_tags",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_property_tags_OrganizationId",
                table: "property_tags",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_contact_tags_contacts_ContactId",
                table: "contact_tags",
                column: "ContactId",
                principalTable: "contacts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_property_tags_organizations_OrganizationId",
                table: "property_tags",
                column: "OrganizationId",
                principalTable: "organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_property_tags_properties_PropertyId",
                table: "property_tags",
                column: "PropertyId",
                principalTable: "properties",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_property_tags_tags_TagId",
                table: "property_tags",
                column: "TagId",
                principalTable: "tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contact_tags_contacts_ContactId",
                table: "contact_tags");

            migrationBuilder.DropForeignKey(
                name: "FK_property_tags_organizations_OrganizationId",
                table: "property_tags");

            migrationBuilder.DropForeignKey(
                name: "FK_property_tags_properties_PropertyId",
                table: "property_tags");

            migrationBuilder.DropForeignKey(
                name: "FK_property_tags_tags_TagId",
                table: "property_tags");

            migrationBuilder.DropIndex(
                name: "IX_property_tags_OrganizationId",
                table: "property_tags");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "property_tags");

            migrationBuilder.AddForeignKey(
                name: "FK_contact_tags_contacts_ContactId",
                table: "contact_tags",
                column: "ContactId",
                principalTable: "contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_property_tags_properties_PropertyId",
                table: "property_tags",
                column: "PropertyId",
                principalTable: "properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_property_tags_tags_TagId",
                table: "property_tags",
                column: "TagId",
                principalTable: "tags",
                principalColumn: "Id");
        }
    }
}
