using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blogsphere.Api.Gateway.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "blogsphere");

            migrationBuilder.CreateTable(
                name: "Clusters",
                schema: "blogsphere",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClusterId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LoadBalancingPolicy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    HealthCheckEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    HealthCheckPath = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    HealthCheckInterval = table.Column<int>(type: "integer", nullable: false),
                    HealthCheckTimeout = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clusters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Destinations",
                schema: "blogsphere",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DestinationId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ClusterId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destinations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Destinations_Clusters_ClusterId",
                        column: x => x.ClusterId,
                        principalSchema: "blogsphere",
                        principalTable: "Clusters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                schema: "blogsphere",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RouteId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Methods = table.Column<string>(type: "text", nullable: true),
                    RateLimiterPolicy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Metadata = table.Column<string>(type: "text", nullable: true),
                    ClusterId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Routes_Clusters_ClusterId",
                        column: x => x.ClusterId,
                        principalSchema: "blogsphere",
                        principalTable: "Clusters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Headers",
                schema: "blogsphere",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Values = table.Column<string>(type: "text", nullable: true),
                    Mode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RouteId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Headers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Headers_Routes_RouteId",
                        column: x => x.RouteId,
                        principalSchema: "blogsphere",
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transforms",
                schema: "blogsphere",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PathPattern = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    RouteId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transforms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transforms_Routes_RouteId",
                        column: x => x.RouteId,
                        principalSchema: "blogsphere",
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clusters_ClusterId",
                schema: "blogsphere",
                table: "Clusters",
                column: "ClusterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Destinations_ClusterId",
                schema: "blogsphere",
                table: "Destinations",
                column: "ClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_Destinations_DestinationId",
                schema: "blogsphere",
                table: "Destinations",
                column: "DestinationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Headers_RouteId",
                schema: "blogsphere",
                table: "Headers",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_ClusterId",
                schema: "blogsphere",
                table: "Routes",
                column: "ClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_RouteId",
                schema: "blogsphere",
                table: "Routes",
                column: "RouteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transforms_RouteId",
                schema: "blogsphere",
                table: "Transforms",
                column: "RouteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Destinations",
                schema: "blogsphere");

            migrationBuilder.DropTable(
                name: "Headers",
                schema: "blogsphere");

            migrationBuilder.DropTable(
                name: "Transforms",
                schema: "blogsphere");

            migrationBuilder.DropTable(
                name: "Routes",
                schema: "blogsphere");

            migrationBuilder.DropTable(
                name: "Clusters",
                schema: "blogsphere");
        }
    }
}
