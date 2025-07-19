using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blogsphere.Api.Gateway.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddsCreatedByAndUpdatedByCols : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "blogsphere",
                table: "Transforms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "blogsphere",
                table: "Transforms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "blogsphere",
                table: "Routes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "blogsphere",
                table: "Routes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "blogsphere",
                table: "Headers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "blogsphere",
                table: "Headers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "blogsphere",
                table: "Destinations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "blogsphere",
                table: "Destinations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "blogsphere",
                table: "Clusters",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                schema: "blogsphere",
                table: "Clusters",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "blogsphere",
                table: "Transforms");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "blogsphere",
                table: "Transforms");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "blogsphere",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "blogsphere",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "blogsphere",
                table: "Headers");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "blogsphere",
                table: "Headers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "blogsphere",
                table: "Destinations");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "blogsphere",
                table: "Destinations");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "blogsphere",
                table: "Clusters");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                schema: "blogsphere",
                table: "Clusters");
        }
    }
}
