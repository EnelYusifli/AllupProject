using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AllupProject.Migrations
{
    /// <inheritdoc />
    public partial class ProductTableModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BookCode",
                table: "Products",
                newName: "ProductCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductCode",
                table: "Products",
                newName: "BookCode");
        }
    }
}
