using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REIstacks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLeadListFilesTable42 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "import_jobs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "import_jobs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TotalImported",
                table: "import_jobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "OccurredAt",
                table: "import_errors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "import_errors",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_import_jobs_OrganizationId",
                table: "import_jobs",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_import_errors_OrganizationId",
                table: "import_errors",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_import_errors_organizations_OrganizationId",
                table: "import_errors",
                column: "OrganizationId",
                principalTable: "organizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_import_jobs_organizations_OrganizationId",
                table: "import_jobs",
                column: "OrganizationId",
                principalTable: "organizations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_import_errors_organizations_OrganizationId",
                table: "import_errors");

            migrationBuilder.DropForeignKey(
                name: "FK_import_jobs_organizations_OrganizationId",
                table: "import_jobs");

            migrationBuilder.DropIndex(
                name: "IX_import_jobs_OrganizationId",
                table: "import_jobs");

            migrationBuilder.DropIndex(
                name: "IX_import_errors_OrganizationId",
                table: "import_errors");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "import_jobs");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "import_jobs");

            migrationBuilder.DropColumn(
                name: "TotalImported",
                table: "import_jobs");

            migrationBuilder.DropColumn(
                name: "OccurredAt",
                table: "import_errors");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "import_errors");
        }
    }
}
