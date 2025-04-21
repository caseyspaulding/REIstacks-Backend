using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REIstacks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DashSKIPu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RawResponseJson",
                table: "skip_trace_items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Age",
                table: "skip_trace_items",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "skip_trace_items",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CurrentAddress",
                table: "skip_trace_items",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DateOfBirth",
                table: "skip_trace_items",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "skip_trace_items",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "skip_trace_items",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "skip_trace_items",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MatchStatus",
                table: "skip_trace_items",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PersonLink",
                table: "skip_trace_items",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "skip_trace_items",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneType",
                table: "skip_trace_items",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "skip_trace_items",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StreetAddress",
                table: "skip_trace_items",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "skip_trace_items",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedAt",
                table: "skip_trace_activities",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "skip_trace_activities",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Failed",
                table: "skip_trace_activities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Matched",
                table: "skip_trace_activities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Pending",
                table: "skip_trace_activities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Total",
                table: "skip_trace_activities",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "skip_trace_items");

            migrationBuilder.DropColumn(
                name: "City",
                table: "skip_trace_items");

            migrationBuilder.DropColumn(
                name: "CurrentAddress",
                table: "skip_trace_items");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "skip_trace_items");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "skip_trace_items");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "skip_trace_items");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "skip_trace_items");

            migrationBuilder.DropColumn(
                name: "MatchStatus",
                table: "skip_trace_items");

            migrationBuilder.DropColumn(
                name: "PersonLink",
                table: "skip_trace_items");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "skip_trace_items");

            migrationBuilder.DropColumn(
                name: "PhoneType",
                table: "skip_trace_items");

            migrationBuilder.DropColumn(
                name: "State",
                table: "skip_trace_items");

            migrationBuilder.DropColumn(
                name: "StreetAddress",
                table: "skip_trace_items");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "skip_trace_items");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "skip_trace_activities");

            migrationBuilder.DropColumn(
                name: "Failed",
                table: "skip_trace_activities");

            migrationBuilder.DropColumn(
                name: "Matched",
                table: "skip_trace_activities");

            migrationBuilder.DropColumn(
                name: "Pending",
                table: "skip_trace_activities");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "skip_trace_activities");

            migrationBuilder.AlterColumn<string>(
                name: "RawResponseJson",
                table: "skip_trace_items",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedAt",
                table: "skip_trace_activities",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
