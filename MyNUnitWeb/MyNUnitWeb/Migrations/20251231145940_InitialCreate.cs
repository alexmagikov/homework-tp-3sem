using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyNUnitWeb.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestRuns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DirectoryName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestRuns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UploadedAssemblies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    Path = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedAssemblies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TestId = table.Column<int>(type: "INTEGER", nullable: false),
                    TestRunId = table.Column<int>(type: "INTEGER", nullable: true),
                    AssemblyName = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    TestName = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    IsPassed = table.Column<bool>(type: "INTEGER", nullable: false),
                    WorkTime = table.Column<long>(type: "INTEGER", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                    IgnoreReason = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestInfos_TestRuns_TestRunId",
                        column: x => x.TestRunId,
                        principalTable: "TestRuns",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestInfos_TestRunId",
                table: "TestInfos",
                column: "TestRunId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestInfos");

            migrationBuilder.DropTable(
                name: "UploadedAssemblies");

            migrationBuilder.DropTable(
                name: "TestRuns");
        }
    }
}
