using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffToEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_User",
                table: "Event");

            migrationBuilder.AddColumn<int>(
                name: "StaffId",
                table: "Event",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Event_StaffId",
                table: "Event",
                column: "StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Organizer",
                table: "Event",
                column: "OrganizerId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Staff",
                table: "Event",
                column: "StaffId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Organizer",
                table: "Event");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_Staff",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_StaffId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "Event");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_User",
                table: "Event",
                column: "OrganizerId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
