using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllupProject.Migrations
{
    /// <inheritdoc />
    public partial class CartsTableUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookId",
                table: "CartItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookId",
                table: "CartItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
