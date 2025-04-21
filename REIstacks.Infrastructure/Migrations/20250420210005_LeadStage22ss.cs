using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REIstacks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LeadStage22ss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_opportunity_stages_organizations_OrganizationId",
                table: "opportunity_stages");

            migrationBuilder.DropIndex(
                name: "IX_opportunity_stages_OrganizationId",
                table: "opportunity_stages");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "opportunity_stages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "opportunity_stages",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_opportunity_stages_OrganizationId",
                table: "opportunity_stages",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_opportunity_stages_organizations_OrganizationId",
                table: "opportunity_stages",
                column: "OrganizationId",
                principalTable: "organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
