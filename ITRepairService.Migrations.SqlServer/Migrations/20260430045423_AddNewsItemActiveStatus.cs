using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITRepairService.Migrations.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddNewsItemActiveStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "NewsItems",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "NewsItems");
        }
    }
}
