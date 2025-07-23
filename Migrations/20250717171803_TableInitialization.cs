using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoGameFinder.Migrations
{
    /// <inheritdoc />
    public partial class TableInitialization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VideoGames",
                columns: table => new
                {
                    GameGuid = table.Column<Guid>(type: "uuid", nullable: false),
                    Game = table.Column<string>(type: "text", nullable: false),
                    CopiesSold = table.Column<string>(type: "text", nullable: false),
                    ReleaseDate = table.Column<string>(type: "text", nullable: false),
                    Genre = table.Column<string>(type: "text", nullable: false),
                    Developer = table.Column<string>(type: "text", nullable: false),
                    Publisher = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoGames", x => x.GameGuid);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideoGames");
        }
    }
}
