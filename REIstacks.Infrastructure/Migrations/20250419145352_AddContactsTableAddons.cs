using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REIstacks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContactsTableAddons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "property_documents");

            migrationBuilder.DropColumn(
                name: "DocumentType",
                table: "property_documents");

            migrationBuilder.RenameColumn(
                name: "UploadedAt",
                table: "property_documents",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "BlobUrl",
                table: "property_documents",
                newName: "UserId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "property_documents",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "property_documents",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                table: "property_documents",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "contact_emails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContactId = table.Column<int>(type: "int", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsDoNotEmail = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contact_emails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contact_emails_contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contact_phones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContactId = table.Column<int>(type: "int", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PhoneType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    IsDoNotCall = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contact_phones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contact_phones_contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "property_communications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyId = table.Column<int>(type: "int", nullable: false),
                    ContactId = table.Column<int>(type: "int", nullable: true),
                    CommunicationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_property_communications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_property_communications_contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "contacts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_property_communications_profiles_UserId",
                        column: x => x.UserId,
                        principalTable: "profiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_property_communications_properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "properties",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "property_interaction_counts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyId = table.Column<int>(type: "int", nullable: false),
                    CallAttempts = table.Column<int>(type: "int", nullable: false),
                    DirectMailAttempts = table.Column<int>(type: "int", nullable: false),
                    SMSAttempts = table.Column<int>(type: "int", nullable: false),
                    RVMAttempts = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_property_interaction_counts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_property_interaction_counts_properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "property_notes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_property_notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_property_notes_properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_contact_emails_ContactId",
                table: "contact_emails",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_contact_phones_ContactId",
                table: "contact_phones",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_property_communications_ContactId",
                table: "property_communications",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_property_communications_PropertyId",
                table: "property_communications",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_property_communications_UserId",
                table: "property_communications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_property_interaction_counts_PropertyId",
                table: "property_interaction_counts",
                column: "PropertyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_property_notes_PropertyId",
                table: "property_notes",
                column: "PropertyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contact_emails");

            migrationBuilder.DropTable(
                name: "contact_phones");

            migrationBuilder.DropTable(
                name: "property_communications");

            migrationBuilder.DropTable(
                name: "property_interaction_counts");

            migrationBuilder.DropTable(
                name: "property_notes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "property_documents");

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "property_documents");

            migrationBuilder.DropColumn(
                name: "FileUrl",
                table: "property_documents");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "property_documents",
                newName: "BlobUrl");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "property_documents",
                newName: "UploadedAt");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "property_documents",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentType",
                table: "property_documents",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
