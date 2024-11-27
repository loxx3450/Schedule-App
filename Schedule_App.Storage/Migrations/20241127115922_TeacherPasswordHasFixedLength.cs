using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Schedule_App.Storage.Migrations
{
    /// <inheritdoc />
    public partial class TeacherPasswordHasFixedLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Teachers",
                type: "character varying(48)",
                maxLength: 48,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Teachers",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(48)",
                oldMaxLength: 48);
        }
    }
}
