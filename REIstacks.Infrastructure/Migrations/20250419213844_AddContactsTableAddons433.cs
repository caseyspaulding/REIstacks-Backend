using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REIstacks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContactsTableAddons433 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "tags",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "tags",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "TagType",
                table: "tags",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "tags",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "StatusId",
                table: "contacts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusId",
                table: "contact_phones",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Boards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Boards_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "contact_statuses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contact_statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "contact_tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContactId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contact_tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contact_tags_contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_contact_tags_tags_TagId",
                        column: x => x.TagId,
                        principalTable: "tags",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "phone_statuses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_phone_statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "phone_tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhoneId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_phone_tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_phone_tags_contact_phones_PhoneId",
                        column: x => x.PhoneId,
                        principalTable: "contact_phones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_phone_tags_tags_TagId",
                        column: x => x.TagId,
                        principalTable: "tags",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "property_activities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyId = table.Column<int>(type: "int", nullable: false),
                    ActivityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TargetUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RelatedEntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RelatedEntityId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PageSource = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_property_activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_property_activities_profiles_TargetUserId",
                        column: x => x.TargetUserId,
                        principalTable: "profiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_property_activities_profiles_UserId",
                        column: x => x.UserId,
                        principalTable: "profiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_property_activities_properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "properties",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "property_files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_property_files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_property_files_profiles_UserId",
                        column: x => x.UserId,
                        principalTable: "profiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_property_files_properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "properties",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "prospect_list_presets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilterCriteria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSystemPreset = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prospect_list_presets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_prospect_list_presets_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BoardPhases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BoardId = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardPhases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoardPhases_Boards_BoardId",
                        column: x => x.BoardId,
                        principalTable: "Boards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyBoards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyId = table.Column<int>(type: "int", nullable: false),
                    BoardId = table.Column<int>(type: "int", nullable: false),
                    PhaseId = table.Column<int>(type: "int", nullable: true),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyBoards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyBoards_BoardPhases_PhaseId",
                        column: x => x.PhaseId,
                        principalTable: "BoardPhases",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PropertyBoards_Boards_BoardId",
                        column: x => x.BoardId,
                        principalTable: "Boards",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PropertyBoards_properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "properties",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_contacts_StatusId",
                table: "contacts",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_contact_phones_StatusId",
                table: "contact_phones",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_BoardPhases_BoardId",
                table: "BoardPhases",
                column: "BoardId");

            migrationBuilder.CreateIndex(
                name: "IX_Boards_OrganizationId",
                table: "Boards",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_contact_tags_ContactId",
                table: "contact_tags",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_contact_tags_TagId",
                table: "contact_tags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_phone_tags_PhoneId",
                table: "phone_tags",
                column: "PhoneId");

            migrationBuilder.CreateIndex(
                name: "IX_phone_tags_TagId",
                table: "phone_tags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_property_activities_PropertyId_Timestamp",
                table: "property_activities",
                columns: new[] { "PropertyId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_property_activities_TargetUserId",
                table: "property_activities",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_property_activities_UserId",
                table: "property_activities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_property_files_PropertyId",
                table: "property_files",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_property_files_UserId",
                table: "property_files",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyBoards_BoardId",
                table: "PropertyBoards",
                column: "BoardId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyBoards_PhaseId",
                table: "PropertyBoards",
                column: "PhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyBoards_PropertyId",
                table: "PropertyBoards",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_prospect_list_presets_OrganizationId",
                table: "prospect_list_presets",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_contact_phones_phone_statuses_StatusId",
                table: "contact_phones",
                column: "StatusId",
                principalTable: "phone_statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_contacts_contact_statuses_StatusId",
                table: "contacts",
                column: "StatusId",
                principalTable: "contact_statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contact_phones_phone_statuses_StatusId",
                table: "contact_phones");

            migrationBuilder.DropForeignKey(
                name: "FK_contacts_contact_statuses_StatusId",
                table: "contacts");

            migrationBuilder.DropTable(
                name: "contact_statuses");

            migrationBuilder.DropTable(
                name: "contact_tags");

            migrationBuilder.DropTable(
                name: "phone_statuses");

            migrationBuilder.DropTable(
                name: "phone_tags");

            migrationBuilder.DropTable(
                name: "property_activities");

            migrationBuilder.DropTable(
                name: "property_files");

            migrationBuilder.DropTable(
                name: "PropertyBoards");

            migrationBuilder.DropTable(
                name: "prospect_list_presets");

            migrationBuilder.DropTable(
                name: "BoardPhases");

            migrationBuilder.DropTable(
                name: "Boards");

            migrationBuilder.DropIndex(
                name: "IX_contacts_StatusId",
                table: "contacts");

            migrationBuilder.DropIndex(
                name: "IX_contact_phones_StatusId",
                table: "contact_phones");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "TagType",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "contacts");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "contact_phones");
        }
    }
}
