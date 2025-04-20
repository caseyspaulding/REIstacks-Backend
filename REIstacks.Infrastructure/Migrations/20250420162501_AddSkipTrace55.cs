using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REIstacks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSkipTrace55 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RawResponse",
                table: "skip_trace_activities");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "skip_trace_items",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "skip_trace_activities",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "RawResponseJson",
                table: "skip_trace_activities",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "skip_trace_items");

            migrationBuilder.DropColumn(
                name: "RawResponseJson",
                table: "skip_trace_activities");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "skip_trace_activities",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "RawResponse",
                table: "skip_trace_activities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
