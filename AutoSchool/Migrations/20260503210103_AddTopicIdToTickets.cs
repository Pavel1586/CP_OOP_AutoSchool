using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoSchool.Migrations
{
    /// <inheritdoc />
    public partial class AddTopicIdToTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TopicId",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 1,
                column: "TopicId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Tickets",
                keyColumn: "Id",
                keyValue: 2,
                column: "TopicId",
                value: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TopicId",
                table: "Tickets",
                column: "TopicId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Topics_TopicId",
                table: "Tickets",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Topics_TopicId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_TopicId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "TopicId",
                table: "Tickets");
        }
    }
}
