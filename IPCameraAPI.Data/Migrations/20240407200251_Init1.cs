using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IPCameraAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IpAddress",
                table: "ApplicationUsers",
                newName: "CameraUsername");

            migrationBuilder.AddColumn<string>(
                name: "CameraIpAddress",
                table: "ApplicationUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CameraPassword",
                table: "ApplicationUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CameraPort",
                table: "ApplicationUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "AlarmEvents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "AlarmEvents",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CameraIpAddress",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "CameraPassword",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "CameraPort",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "AlarmEvents");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "AlarmEvents");

            migrationBuilder.RenameColumn(
                name: "CameraUsername",
                table: "ApplicationUsers",
                newName: "IpAddress");
        }
    }
}
