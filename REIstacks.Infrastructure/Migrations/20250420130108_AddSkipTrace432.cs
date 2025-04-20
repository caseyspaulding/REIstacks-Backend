using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REIstacks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSkipTrace432 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "skip_trace_items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SkipTraceActivityId = table.Column<int>(type: "int", nullable: false),
                    ContactId = table.Column<int>(type: "int", nullable: true),
                    PropertyId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RawResponseJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skip_trace_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_skip_trace_items_skip_trace_activities_SkipTraceActivityId",
                        column: x => x.SkipTraceActivityId,
                        principalTable: "skip_trace_activities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_skip_trace_items_SkipTraceActivityId",
                table: "skip_trace_items",
                column: "SkipTraceActivityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "skip_trace_items");
        }
    }
}
