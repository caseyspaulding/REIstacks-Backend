using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REIstacks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSkipTrace2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "skip_trace_activities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganizationId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ProcessedCount = table.Column<int>(type: "int", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Saved = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RawResponse = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skip_trace_activities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "skip_trace_breakdowns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SkipTraceActivityId = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skip_trace_breakdowns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_skip_trace_breakdowns_skip_trace_activities_SkipTraceActivityId",
                        column: x => x.SkipTraceActivityId,
                        principalTable: "skip_trace_activities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_skip_trace_breakdowns_SkipTraceActivityId",
                table: "skip_trace_breakdowns",
                column: "SkipTraceActivityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "skip_trace_breakdowns");

            migrationBuilder.DropTable(
                name: "skip_trace_activities");
        }
    }
}
