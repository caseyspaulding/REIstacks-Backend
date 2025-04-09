using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REIstacks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateleadtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ZipCode",
                table: "leads",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "State",
                table: "leads",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2)",
                oldMaxLength: 2,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdditionalInfo",
                table: "leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ConsentPrivacyPolicy",
                table: "leads",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ConsentTextMessages",
                table: "leads",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "leads",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "leads",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PreferredContactMethod",
                table: "leads",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropertyCity",
                table: "leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropertyCondition",
                table: "leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropertyIssues",
                table: "leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropertyState",
                table: "leads",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropertyStreetAddress",
                table: "leads",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PropertyZipCode",
                table: "leads",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReasonForSelling",
                table: "leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Step",
                table: "leads",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TargetPrice",
                table: "leads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Timeline",
                table: "leads",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalInfo",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "ConsentPrivacyPolicy",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "ConsentTextMessages",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "PreferredContactMethod",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "PropertyCity",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "PropertyCondition",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "PropertyIssues",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "PropertyState",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "PropertyStreetAddress",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "PropertyZipCode",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "ReasonForSelling",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "Step",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "TargetPrice",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "Timeline",
                table: "leads");

            migrationBuilder.AlterColumn<string>(
                name: "ZipCode",
                table: "leads",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "State",
                table: "leads",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
