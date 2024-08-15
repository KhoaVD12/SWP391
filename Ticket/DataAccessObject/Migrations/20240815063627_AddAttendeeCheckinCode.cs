using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendeeCheckinCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CheckInStatus",
                table: "Attendee",
                type: "int",
                unicode: false,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(max)",
                oldUnicode: false);

            migrationBuilder.AddColumn<string>(
                name: "CheckInCode",
                table: "Attendee",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckInCode",
                table: "Attendee");

            migrationBuilder.AlterColumn<string>(
                name: "CheckInStatus",
                table: "Attendee",
                type: "varchar(max)",
                unicode: false,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldUnicode: false);
        }
    }
}
