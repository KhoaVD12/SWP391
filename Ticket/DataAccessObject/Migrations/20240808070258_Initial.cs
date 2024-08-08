using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Booth",
                columns: table => new
                {
                    BoothId = table.Column<int>(type: "int", nullable: false),
                    SponsorId = table.Column<int>(type: "int", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    Location = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booth", x => x.BoothId);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    PaymentDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => x.TicketId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "varchar(8)", unicode: false, maxLength: 8, nullable: false),
                    Role = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    Status = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    UserRoleEnum = table.Column<int>(type: "int", nullable: false),
                    UserStatusEnum = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Venue",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gift",
                columns: table => new
                {
                    GiftId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    BoothId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gift", x => x.GiftId);
                    table.ForeignKey(
                        name: "FK_Gift_Booth",
                        column: x => x.BoothId,
                        principalTable: "Booth",
                        principalColumn: "BoothId");
                });

            migrationBuilder.CreateTable(
                name: "BoothRequest",
                columns: table => new
                {
                    BoothRequestId = table.Column<int>(type: "int", nullable: false),
                    SponsorId = table.Column<int>(type: "int", nullable: false),
                    BoothId = table.Column<int>(type: "int", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoothRequest", x => x.BoothRequestId);
                    table.ForeignKey(
                        name: "FK_BoothRequest_Booth",
                        column: x => x.BoothId,
                        principalTable: "Booth",
                        principalColumn: "BoothId");
                    table.ForeignKey(
                        name: "FK_BoothRequest_User",
                        column: x => x.SponsorId,
                        principalTable: "User",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Event",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    OrganizerId = table.Column<int>(type: "int", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    VenueId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_Event_User",
                        column: x => x.OrganizerId,
                        principalTable: "User",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Event_Venue",
                        column: x => x.VenueId,
                        principalTable: "Venue",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Attendee",
                columns: table => new
                {
                    AttendeeId = table.Column<string>(type: "varchar(8)", unicode: false, maxLength: 8, nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CheckInStatus = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendee_1", x => x.AttendeeId);
                    table.ForeignKey(
                        name: "FK_Attendee_Event",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "EventId");
                    table.ForeignKey(
                        name: "FK_Attendee_Ticket1",
                        column: x => x.TicketId,
                        principalTable: "Ticket",
                        principalColumn: "TicketId");
                });

            migrationBuilder.CreateTable(
                name: "AttendeeDetail",
                columns: table => new
                {
                    DetailId = table.Column<int>(type: "int", nullable: false),
                    AttendeeId = table.Column<string>(type: "varchar(8)", unicode: false, maxLength: 8, nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Phone = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendeeDetail", x => x.DetailId);
                    table.ForeignKey(
                        name: "FK_AttendeeDetail_Attendee",
                        column: x => x.AttendeeId,
                        principalTable: "Attendee",
                        principalColumn: "AttendeeId");
                });

            migrationBuilder.CreateTable(
                name: "GiftReception",
                columns: table => new
                {
                    GiftReceptionId = table.Column<int>(type: "int", nullable: false),
                    AttendeeId = table.Column<string>(type: "varchar(8)", unicode: false, maxLength: 8, nullable: false),
                    GiftId = table.Column<int>(type: "int", nullable: false),
                    ReceptionDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiftReception", x => x.GiftReceptionId);
                    table.ForeignKey(
                        name: "FK_GiftReception_Attendee",
                        column: x => x.AttendeeId,
                        principalTable: "Attendee",
                        principalColumn: "AttendeeId");
                    table.ForeignKey(
                        name: "FK_GiftReception_Gift",
                        column: x => x.GiftId,
                        principalTable: "Gift",
                        principalColumn: "GiftId");
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    AttendeeId = table.Column<string>(type: "varchar(8)", unicode: false, maxLength: 8, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_Transaction_Attendee",
                        column: x => x.AttendeeId,
                        principalTable: "Attendee",
                        principalColumn: "AttendeeId");
                    table.ForeignKey(
                        name: "FK_Transaction_Payment",
                        column: x => x.PaymentMethod,
                        principalTable: "Payment",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendee_EventId",
                table: "Attendee",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendee_TicketId",
                table: "Attendee",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendeeDetail_AttendeeId",
                table: "AttendeeDetail",
                column: "AttendeeId");

            migrationBuilder.CreateIndex(
                name: "IX_BoothRequest_BoothId",
                table: "BoothRequest",
                column: "BoothId");

            migrationBuilder.CreateIndex(
                name: "IX_BoothRequest_SponsorId",
                table: "BoothRequest",
                column: "SponsorId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_OrganizerId",
                table: "Event",
                column: "OrganizerId");

            migrationBuilder.CreateIndex(
                name: "IX_Event_VenueId",
                table: "Event",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_Gift_BoothId",
                table: "Gift",
                column: "BoothId");

            migrationBuilder.CreateIndex(
                name: "IX_GiftReception_AttendeeId",
                table: "GiftReception",
                column: "AttendeeId");

            migrationBuilder.CreateIndex(
                name: "IX_GiftReception_GiftId",
                table: "GiftReception",
                column: "GiftId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_AttendeeId",
                table: "Transaction",
                column: "AttendeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_PaymentMethod",
                table: "Transaction",
                column: "PaymentMethod");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendeeDetail");

            migrationBuilder.DropTable(
                name: "BoothRequest");

            migrationBuilder.DropTable(
                name: "GiftReception");

            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.DropTable(
                name: "Gift");

            migrationBuilder.DropTable(
                name: "Attendee");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Booth");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Venue");
        }
    }
}
