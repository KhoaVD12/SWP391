using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class FixTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ticket_EventId",
                table: "Ticket");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_EventId",
                table: "Ticket",
                column: "EventId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ticket_EventId",
                table: "Ticket");

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_EventId",
                table: "Ticket",
                column: "EventId");
        }
    }
}
