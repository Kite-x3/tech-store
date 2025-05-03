using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechStore.Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class AddedAuthorIdToReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorID",
                table: "Reviews",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorID",
                table: "Reviews");
        }
    }
}
