using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoSchool.Migrations
{
    /// <inheritdoc />
    public partial class AddTheoryCreditsAndTrainingDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TrainingPlannedEndDate",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TrainingStartDate",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TheoryCredits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TopicId = table.Column<int>(type: "int", nullable: true),
                    PlannedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Room = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TheoryCredits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TheoryCredits_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TheoryCredits_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1000,
                columns: new[] { "TrainingPlannedEndDate", "TrainingStartDate" },
                values: new object[] { null, null });

            migrationBuilder.CreateIndex(
                name: "IX_TheoryCredits_TopicId",
                table: "TheoryCredits",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_TheoryCredits_UserId",
                table: "TheoryCredits",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TheoryCredits");

            migrationBuilder.DropColumn(
                name: "TrainingPlannedEndDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TrainingStartDate",
                table: "Users");
        }
    }
}
