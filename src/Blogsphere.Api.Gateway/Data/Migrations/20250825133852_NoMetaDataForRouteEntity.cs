using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blogsphere.Api.Gateway.Data.Migrations
{
    /// <inheritdoc />
    public partial class NoMetaDataForRouteEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Metadata",
                schema: "blogsphere",
                table: "Routes");

            migrationBuilder.CreateIndex(
                name: "IX_SubscribedApis_ApiName",
                schema: "blogsphere",
                table: "SubscribedApis",
                column: "ApiName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubscribedApis_ApiPath",
                schema: "blogsphere",
                table: "SubscribedApis",
                column: "ApiPath",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SubscribedApis_ApiName",
                schema: "blogsphere",
                table: "SubscribedApis");

            migrationBuilder.DropIndex(
                name: "IX_SubscribedApis_ApiPath",
                schema: "blogsphere",
                table: "SubscribedApis");

            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                schema: "blogsphere",
                table: "Routes",
                type: "text",
                nullable: true);
        }
    }
}
