using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoSchool.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInstructorSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Photo",
                table: "Instructors",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Transmission",
                table: "Instructors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VehicleBrand",
                table: "Instructors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VehicleCategory",
                table: "Instructors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VehicleModel",
                table: "Instructors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Instructors",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Photo", "Transmission", "VehicleBrand", "VehicleCategory", "VehicleModel" },
                values: new object[] { null, 1, "Lada", "B", "Vesta" });

            migrationBuilder.UpdateData(
                table: "Instructors",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Photo", "Transmission", "VehicleBrand", "VehicleCategory", "VehicleModel" },
                values: new object[] { null, 2, "Kia", "B", "Rio" });

            migrationBuilder.UpdateData(
                table: "Instructors",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Photo", "Transmission", "VehicleBrand", "VehicleCategory", "VehicleModel" },
                values: new object[] { null, 1, "Yamaha", "A", "MT-07" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "Transmission",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "VehicleBrand",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "VehicleCategory",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "VehicleModel",
                table: "Instructors");
        }
    }
}
