using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REIstacks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AssignOpportunity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_leads_lead_stages_LeadStageId",
                table: "leads");

            migrationBuilder.DropForeignKey(
                name: "FK_leads_profiles_AssignedToProfileId",
                table: "leads");

            migrationBuilder.DropForeignKey(
                name: "FK_opportunities_organizations_OrganizationId",
                table: "opportunities");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "opportunities");

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerProfileId",
                table: "opportunities",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_opportunities_OwnerProfileId",
                table: "opportunities",
                column: "OwnerProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_leads_lead_stages_LeadStageId",
                table: "leads",
                column: "LeadStageId",
                principalTable: "lead_stages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_leads_profiles_AssignedToProfileId",
                table: "leads",
                column: "AssignedToProfileId",
                principalTable: "profiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_opportunities_organizations_OrganizationId",
                table: "opportunities",
                column: "OrganizationId",
                principalTable: "organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_opportunities_profiles_OwnerProfileId",
                table: "opportunities",
                column: "OwnerProfileId",
                principalTable: "profiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_leads_lead_stages_LeadStageId",
                table: "leads");

            migrationBuilder.DropForeignKey(
                name: "FK_leads_profiles_AssignedToProfileId",
                table: "leads");

            migrationBuilder.DropForeignKey(
                name: "FK_opportunities_organizations_OrganizationId",
                table: "opportunities");

            migrationBuilder.DropForeignKey(
                name: "FK_opportunities_profiles_OwnerProfileId",
                table: "opportunities");

            migrationBuilder.DropIndex(
                name: "IX_opportunities_OwnerProfileId",
                table: "opportunities");

            migrationBuilder.DropColumn(
                name: "OwnerProfileId",
                table: "opportunities");

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "opportunities",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_leads_lead_stages_LeadStageId",
                table: "leads",
                column: "LeadStageId",
                principalTable: "lead_stages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_leads_profiles_AssignedToProfileId",
                table: "leads",
                column: "AssignedToProfileId",
                principalTable: "profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_opportunities_organizations_OrganizationId",
                table: "opportunities",
                column: "OrganizationId",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
