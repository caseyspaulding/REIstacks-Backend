using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REIstacks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AssignLead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "leads");

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedToProfileId",
                table: "leads",
                type: "uniqueidentifier",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_leads_AssignedToProfileId",
                table: "leads",
                column: "AssignedToProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_leads_profiles_AssignedToProfileId",
                table: "leads",
                column: "AssignedToProfileId",
                principalTable: "profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_leads_profiles_AssignedToProfileId",
                table: "leads");

            migrationBuilder.DropIndex(
                name: "IX_leads_AssignedToProfileId",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "AssignedToProfileId",
                table: "leads");

            migrationBuilder.AddColumn<string>(
                name: "AssignedTo",
                table: "leads",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
