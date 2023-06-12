using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookify.WEB.Data.Migrations
{
    /// <inheritdoc />
    public partial class IndexingAndUnquefyBookTitleWithUserIdInBookTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Books_Title_AuthorId",
                table: "Books",
                columns: new[] { "Title", "AuthorId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Books_Title_AuthorId",
                table: "Books");
        }
    }
}
