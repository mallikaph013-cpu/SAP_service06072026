using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITRepairService.Migrations.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class AddItAssignmentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedItName",
                table: "RepairTickets",
                type: "TEXT",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AssignedItUserId",
                table: "RepairTickets",
                type: "TEXT",
                maxLength: 450,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedItName",
                table: "RepairTickets");

            migrationBuilder.DropColumn(
                name: "AssignedItUserId",
                table: "RepairTickets");
        }
    }
}
