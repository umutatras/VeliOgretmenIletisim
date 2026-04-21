using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VeliOgretmenIletisim.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteToStudentTeacher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StudentTeachers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StudentTeachers");
        }
    }
}
