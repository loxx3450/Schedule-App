using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Schedule_App.Storage.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewPKToGroupTeacher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupsTeachers",
                table: "GroupsTeachers");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "GroupsTeachers",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupsTeachers",
                table: "GroupsTeachers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GroupsTeachers_GroupId",
                table: "GroupsTeachers",
                column: "GroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GroupsTeachers",
                table: "GroupsTeachers");

            migrationBuilder.DropIndex(
                name: "IX_GroupsTeachers_GroupId",
                table: "GroupsTeachers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "GroupsTeachers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GroupsTeachers",
                table: "GroupsTeachers",
                columns: new[] { "GroupId", "TeacherId" });
        }
    }
}
