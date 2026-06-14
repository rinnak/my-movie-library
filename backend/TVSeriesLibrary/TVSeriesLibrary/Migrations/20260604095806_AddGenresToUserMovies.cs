using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TVSeriesLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddGenresToUserMovies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Genres",
                table: "UserMovies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Genres",
                table: "UserMovies");
        }
    }
}
