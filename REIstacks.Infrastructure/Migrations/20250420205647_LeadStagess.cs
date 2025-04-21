using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REIstacks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LeadStagess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_lead_stages_organizations_OrganizationId",
                table: "lead_stages");

            migrationBuilder.DropForeignKey(
                name: "FK_leads_lead_stages_LeadStageId",
                table: "leads");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "lead_stages",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_lead_stages_organizations_OrganizationId",
                table: "lead_stages",
                column: "OrganizationId",
                principalTable: "organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_leads_lead_stages_LeadStageId",
                table: "leads",
                column: "LeadStageId",
                principalTable: "lead_stages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_lead_stages_organizations_OrganizationId",
                table: "lead_stages");

            migrationBuilder.DropForeignKey(
                name: "FK_leads_lead_stages_LeadStageId",
                table: "leads");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationId",
                table: "lead_stages",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_lead_stages_organizations_OrganizationId",
                table: "lead_stages",
                column: "OrganizationId",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_leads_lead_stages_LeadStageId",
                table: "leads",
                column: "LeadStageId",
                principalTable: "lead_stages",
                principalColumn: "Id");
        }
    }
}
