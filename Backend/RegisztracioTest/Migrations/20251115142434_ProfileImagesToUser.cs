using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegisztracioTest.Migrations
{
    /// <inheritdoc />
    public partial class ProfileImagesToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileImages",
                table: "Users",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImages",
                table: "Users");
        }
    }
}
