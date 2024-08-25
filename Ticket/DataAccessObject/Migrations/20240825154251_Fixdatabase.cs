using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class Fixdatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Venue");

            migrationBuilder.CreateIndex(
                name: "IX_Booth_EventId",
                table: "Booth",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Booth_Event_EventId",
                table: "Booth",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Booth_Event_EventId",
                table: "Booth");

            migrationBuilder.DropIndex(
                name: "IX_Booth_EventId",
                table: "Booth");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Venue",
                type: "varchar(max)",
                unicode: false,
                nullable: false,
                defaultValue: "");
        }
    }
}
