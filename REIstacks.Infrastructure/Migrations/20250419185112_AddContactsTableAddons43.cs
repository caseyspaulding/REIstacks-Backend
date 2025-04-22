using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REIstacks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddContactsTableAddons43 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_campaign_contacts_marketing_campaigns_CampaignId",
                table: "campaign_contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_LandingPageComponents_LandingPages_LandingPageId",
                table: "LandingPageComponents");

            migrationBuilder.DropForeignKey(
                name: "FK_LandingPages_organizations_OrganizationId",
                table: "LandingPages");

            migrationBuilder.DropForeignKey(
                name: "FK_property_documents_properties_PropertyId",
                table: "property_documents");

            migrationBuilder.CreateTable(
                name: "lists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lists_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "offers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyId = table.Column<int>(type: "int", nullable: false),
                    OfferAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_offers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_offers_properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "properties",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tags_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "property_lists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyId = table.Column<int>(type: "int", nullable: false),
                    ListId = table.Column<int>(type: "int", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_property_lists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_property_lists_lists_ListId",
                        column: x => x.ListId,
                        principalTable: "lists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_property_lists_properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "properties",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "offer_documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_offer_documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_offer_documents_offers_OfferId",
                        column: x => x.OfferId,
                        principalTable: "offers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_offer_documents_profiles_UserId",
                        column: x => x.UserId,
                        principalTable: "profiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "property_tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false),
                    TaggedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_property_tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_property_tags_properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_property_tags_tags_TagId",
                        column: x => x.TagId,
                        principalTable: "tags",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_lists_OrganizationId",
                table: "lists",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_offer_documents_OfferId",
                table: "offer_documents",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_offer_documents_UserId",
                table: "offer_documents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_offers_PropertyId",
                table: "offers",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_property_lists_ListId",
                table: "property_lists",
                column: "ListId");

            migrationBuilder.CreateIndex(
                name: "IX_property_lists_PropertyId",
                table: "property_lists",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_property_tags_PropertyId",
                table: "property_tags",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_property_tags_TagId",
                table: "property_tags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_tags_OrganizationId",
                table: "tags",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_campaign_contacts_marketing_campaigns_CampaignId",
                table: "campaign_contacts",
                column: "CampaignId",
                principalTable: "marketing_campaigns",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LandingPageComponents_LandingPages_LandingPageId",
                table: "LandingPageComponents",
                column: "LandingPageId",
                principalTable: "LandingPages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LandingPages_organizations_OrganizationId",
                table: "LandingPages",
                column: "OrganizationId",
                principalTable: "organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_property_documents_properties_PropertyId",
                table: "property_documents",
                column: "PropertyId",
                principalTable: "properties",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_campaign_contacts_marketing_campaigns_CampaignId",
                table: "campaign_contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_LandingPageComponents_LandingPages_LandingPageId",
                table: "LandingPageComponents");

            migrationBuilder.DropForeignKey(
                name: "FK_LandingPages_organizations_OrganizationId",
                table: "LandingPages");

            migrationBuilder.DropForeignKey(
                name: "FK_property_documents_properties_PropertyId",
                table: "property_documents");

            migrationBuilder.DropTable(
                name: "offer_documents");

            migrationBuilder.DropTable(
                name: "property_lists");

            migrationBuilder.DropTable(
                name: "property_tags");

            migrationBuilder.DropTable(
                name: "offers");

            migrationBuilder.DropTable(
                name: "lists");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.AddForeignKey(
                name: "FK_campaign_contacts_marketing_campaigns_CampaignId",
                table: "campaign_contacts",
                column: "CampaignId",
                principalTable: "marketing_campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LandingPageComponents_LandingPages_LandingPageId",
                table: "LandingPageComponents",
                column: "LandingPageId",
                principalTable: "LandingPages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LandingPages_organizations_OrganizationId",
                table: "LandingPages",
                column: "OrganizationId",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_property_documents_properties_PropertyId",
                table: "property_documents",
                column: "PropertyId",
                principalTable: "properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
