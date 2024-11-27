using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Schedule_App.Storage.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalValidationAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Teachers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(48)",
                oldMaxLength: 48);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Teachers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(48)",
                oldMaxLength: 48);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Teachers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Teachers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Subjects",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(58)",
                oldMaxLength: 58);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Groups",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(48)",
                oldMaxLength: 48);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Classrooms",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(38)",
                oldMaxLength: 38);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Teachers",
                type: "character varying(48)",
                maxLength: 48,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Teachers",
                type: "character varying(48)",
                maxLength: 48,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Teachers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Teachers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Subjects",
                type: "character varying(58)",
                maxLength: 58,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Groups",
                type: "character varying(48)",
                maxLength: 48,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Classrooms",
                type: "character varying(38)",
                maxLength: 38,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
