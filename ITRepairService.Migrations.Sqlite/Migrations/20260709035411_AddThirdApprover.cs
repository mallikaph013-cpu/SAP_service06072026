using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITRepairService.Migrations.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class AddThirdApprover : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApprovalLevel",
                table: "RepairTickets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SecondApproverName",
                table: "RepairTickets",
                type: "TEXT",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondApproverUserId",
                table: "RepairTickets",
                type: "TEXT",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThirdApproverName",
                table: "RepairTickets",
                type: "TEXT",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThirdApproverUserId",
                table: "RepairTickets",
                type: "TEXT",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeID",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Manager",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "MustChangePassword",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TelephoneNumber",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalLevel",
                table: "RepairTickets");

            migrationBuilder.DropColumn(
                name: "SecondApproverName",
                table: "RepairTickets");

            migrationBuilder.DropColumn(
                name: "SecondApproverUserId",
                table: "RepairTickets");

            migrationBuilder.DropColumn(
                name: "ThirdApproverName",
                table: "RepairTickets");

            migrationBuilder.DropColumn(
                name: "ThirdApproverUserId",
                table: "RepairTickets");

            migrationBuilder.DropColumn(
                name: "Company",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmployeeID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Manager",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MustChangePassword",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TelephoneNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "AspNetUsers");
        }
    }
}
