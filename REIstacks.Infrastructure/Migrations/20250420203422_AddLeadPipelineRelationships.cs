using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REIstacks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLeadPipelineRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "offers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "offer_documents",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContactId",
                table: "leads",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LeadStageId",
                table: "leads",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LeadConvertedDate",
                table: "contacts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LeadOwnerId",
                table: "contacts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LeadStageId",
                table: "contacts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LeadStatus",
                table: "contacts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "lead_stages",
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
                    table.PrimaryKey("PK_lead_stages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lead_stages_organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_offers_OrganizationId",
                table: "offers",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_offer_documents_OrganizationId",
                table: "offer_documents",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_leads_ContactId",
                table: "leads",
                column: "ContactId");

            migrationBuilder.CreateIndex(
                name: "IX_leads_LeadStageId",
                table: "leads",
                column: "LeadStageId");

            migrationBuilder.CreateIndex(
                name: "IX_contacts_LeadStageId",
                table: "contacts",
                column: "LeadStageId");

            migrationBuilder.CreateIndex(
                name: "IX_lead_stages_OrganizationId",
                table: "lead_stages",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_contacts_lead_stages_LeadStageId",
                table: "contacts",
                column: "LeadStageId",
                principalTable: "lead_stages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_leads_contacts_ContactId",
                table: "leads",
                column: "ContactId",
                principalTable: "contacts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_leads_lead_stages_LeadStageId",
                table: "leads",
                column: "LeadStageId",
                principalTable: "lead_stages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_offer_documents_organizations_OrganizationId",
                table: "offer_documents",
                column: "OrganizationId",
                principalTable: "organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_offers_organizations_OrganizationId",
                table: "offers",
                column: "OrganizationId",
                principalTable: "organizations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contacts_lead_stages_LeadStageId",
                table: "contacts");

            migrationBuilder.DropForeignKey(
                name: "FK_leads_contacts_ContactId",
                table: "leads");

            migrationBuilder.DropForeignKey(
                name: "FK_leads_lead_stages_LeadStageId",
                table: "leads");

            migrationBuilder.DropForeignKey(
                name: "FK_offer_documents_organizations_OrganizationId",
                table: "offer_documents");

            migrationBuilder.DropForeignKey(
                name: "FK_offers_organizations_OrganizationId",
                table: "offers");

            migrationBuilder.DropTable(
                name: "lead_stages");

            migrationBuilder.DropIndex(
                name: "IX_offers_OrganizationId",
                table: "offers");

            migrationBuilder.DropIndex(
                name: "IX_offer_documents_OrganizationId",
                table: "offer_documents");

            migrationBuilder.DropIndex(
                name: "IX_leads_ContactId",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "IX_leads_LeadStageId",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "IX_contacts_LeadStageId",
                table: "contacts");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "offers");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "offer_documents");

            migrationBuilder.DropColumn(
                name: "ContactId",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "LeadStageId",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "LeadConvertedDate",
                table: "contacts");

            migrationBuilder.DropColumn(
                name: "LeadOwnerId",
                table: "contacts");

            migrationBuilder.DropColumn(
                name: "LeadStageId",
                table: "contacts");

            migrationBuilder.DropColumn(
                name: "LeadStatus",
                table: "contacts");
        }
    }
}
