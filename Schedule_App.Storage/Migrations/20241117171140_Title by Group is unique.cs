using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Schedule_App.Storage.Migrations
{
    /// <inheritdoc />
    public partial class TitlebyGroupisunique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Groups_Title",
                table: "Groups",
                column: "Title",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Groups_Title",
                table: "Groups");
        }
    }
}
