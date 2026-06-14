using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TVSeriesLibrary.Migrations
{
    /// <inheritdoc />
    public partial class AddMovieDataToFavorites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MovieName",
                table: "FavoriteMovies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PosterUrl",
                table: "FavoriteMovies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "FavoriteMovies",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "FavoriteMovies",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MovieName",
                table: "FavoriteMovies");

            migrationBuilder.DropColumn(
                name: "PosterUrl",
                table: "FavoriteMovies");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "FavoriteMovies");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "FavoriteMovies");
        }
    }
}
