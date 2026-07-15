using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITRepairService.Migrations
{
    public partial class AddDriveAccessDepartmentToRepairTicket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DriveAccessDepartment",
                table: "RepairTicket",
                type: "TEXT",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriveAccessDepartment",
                table: "RepairTicket");
        }
    }
}