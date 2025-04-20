using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REIstacks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOpportunities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "offers");

            migrationBuilder.AddColumn<int>(
                name: "OfferStatusId",
                table: "offers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OpportunityId",
                table: "offers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "offers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "offer_statuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_offer_statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "opportunity_stages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_opportunity_stages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_opportunity_stages_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "opportunities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StageId = table.Column<int>(type: "int", nullable: false),
                    CloseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OwnerId = table.Column<int>(type: "int", nullable: true),
                    PropertyId = table.Column<int>(type: "int", nullable: true),
                    ContactId = table.Column<int>(type: "int", nullable: true),
                    DealId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_opportunities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_opportunities_contacts_ContactId",
                        column: x => x.ContactId,
                        principalTable: "contacts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_opportunities_deals_DealId",
                        column: x => x.DealId,
                        principalTable: "deals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_opportunities_opportunity_stages_StageId",
                        column: x => x.StageId,
                        principalTable: "opportunity_stages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_opportunities_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_opportunities_properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "properties",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_offers_OfferStatusId",
                table: "offers",
                column: "OfferStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_offers_OpportunityId",
                table: "offers",
                column: "OpportunityId");

            migrationBuilder.CreateIndex(
                name: "IX_opportunities_ContactId",
                table: "opportunities",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_opportunities_DealId",
                table: "opportunities",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_opportunities_OrganizationId",
                table: "opportunities",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_opportunities_PropertyId",
                table: "opportunities",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_opportunities_StageId",
                table: "opportunities",
                column: "StageId");

            migrationBuilder.CreateIndex(
                name: "IX_opportunity_stages_OrganizationId",
                table: "opportunity_stages",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_offers_offer_statuses_OfferStatusId",
                table: "offers",
                column: "OfferStatusId",
                principalTable: "offer_statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_offers_opportunities_OpportunityId",
                table: "offers",
                column: "OpportunityId",
                principalTable: "opportunities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_offers_offer_statuses_OfferStatusId",
                table: "offers");

            migrationBuilder.DropForeignKey(
                name: "FK_offers_opportunities_OpportunityId",
                table: "offers");

            migrationBuilder.DropTable(
                name: "offer_statuses");

            migrationBuilder.DropTable(
                name: "opportunities");

            migrationBuilder.DropTable(
                name: "opportunity_stages");

            migrationBuilder.DropIndex(
                name: "IX_offers_OfferStatusId",
                table: "offers");

            migrationBuilder.DropIndex(
                name: "IX_offers_OpportunityId",
                table: "offers");

            migrationBuilder.DropColumn(
                name: "OfferStatusId",
                table: "offers");

            migrationBuilder.DropColumn(
                name: "OpportunityId",
                table: "offers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "offers");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "offers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
