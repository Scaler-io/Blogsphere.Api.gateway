using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blogsphere.Api.Gateway.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeApiProductNameUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ApiProducts_ProductName",
                schema: "blogsphere",
                table: "ApiProducts",
                column: "ProductName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApiProducts_ProductName",
                schema: "blogsphere",
                table: "ApiProducts");
        }
    }
}
