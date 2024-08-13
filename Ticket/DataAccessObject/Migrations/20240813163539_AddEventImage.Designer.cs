﻿// <auto-generated />
using System;
using DataAccessObject.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataAccessObject.Migrations
{
    [DbContext(typeof(TicketContext))]
    [Migration("20240813163539_AddEventImage")]
    partial class AddEventImage
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DataAccessObject.Entities.Attendee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CheckInStatus")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<DateTime>("RegistrationDate")
                        .HasColumnType("datetime");

                    b.Property<int>("TicketId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "EventId" }, "IX_Attendee_EventId");

                    b.HasIndex(new[] { "TicketId" }, "IX_Attendee_TicketId");

                    b.ToTable("Attendee", (string)null);
                });

            modelBuilder.Entity("DataAccessObject.Entities.AttendeeDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AttendeeId")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AttendeeId");

                    b.ToTable("AttendeeDetail", (string)null);
                });

            modelBuilder.Entity("DataAccessObject.Entities.Booth", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<string>("Location")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<int>("SponsorId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Booth", (string)null);
                });

            modelBuilder.Entity("DataAccessObject.Entities.BoothRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BoothId")
                        .HasColumnType("int");

                    b.Property<DateTime>("RequestDate")
                        .HasColumnType("datetime");

                    b.Property<int>("SponsorId")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "BoothId" }, "IX_BoothRequest_BoothId");

                    b.HasIndex(new[] { "SponsorId" }, "IX_BoothRequest_SponsorId");

                    b.ToTable("BoothRequest", (string)null);
                });

            modelBuilder.Entity("DataAccessObject.Entities.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<DateOnly>("EndDate")
                        .HasColumnType("date");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrganizerId")
                        .HasColumnType("int");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.Property<string>("Status")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<int>("VenueId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "OrganizerId" }, "IX_Event_OrganizerId");

                    b.HasIndex(new[] { "VenueId" }, "IX_Event_VenueId");

                    b.ToTable("Event", (string)null);
                });

            modelBuilder.Entity("DataAccessObject.Entities.Gift", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BoothId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "BoothId" }, "IX_Gift_BoothId");

                    b.ToTable("Gift", (string)null);
                });

            modelBuilder.Entity("DataAccessObject.Entities.GiftReception", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AttendeeId")
                        .HasColumnType("int");

                    b.Property<int>("GiftId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ReceptionDate")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.HasIndex("AttendeeId");

                    b.HasIndex(new[] { "GiftId" }, "IX_GiftReception_GiftId");

                    b.ToTable("GiftReception", (string)null);
                });

            modelBuilder.Entity("DataAccessObject.Entities.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<DateOnly>("PaymentDate")
                        .HasColumnType("date");

                    b.Property<string>("Status")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Payment", (string)null);
                });

            modelBuilder.Entity("DataAccessObject.Entities.Ticket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<DateTime>("TicketSaleEndDate")
                        .HasColumnType("datetime");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.ToTable("Ticket", (string)null);
                });

            modelBuilder.Entity("DataAccessObject.Entities.Transaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int>("AttendeeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime");

                    b.Property<int>("PaymentMethod")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AttendeeId");

                    b.HasIndex(new[] { "PaymentMethod" }, "IX_Transaction_PaymentMethod");

                    b.ToTable("Transaction", (string)null);
                });

            modelBuilder.Entity("DataAccessObject.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<int>("Role")
                        .IsUnicode(false)
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.HasKey("Id");

                    b.ToTable("User", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Email = "admin@gmail.com",
                            Name = "Admin",
                            Password = "e86f78a8a3caf0b60d8e74e5942aa6d86dc150cd3c03338aef25b7d2d7e3acc7",
                            Role = 0,
                            Status = "Active"
                        });
                });

            modelBuilder.Entity("DataAccessObject.Entities.Venue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .IsUnicode(false)
                        .HasColumnType("varchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Venue", (string)null);
                });

            modelBuilder.Entity("DataAccessObject.Entities.Attendee", b =>
                {
                    b.HasOne("DataAccessObject.Entities.Event", "Event")
                        .WithMany("Attendees")
                        .HasForeignKey("EventId")
                        .IsRequired()
                        .HasConstraintName("FK_Attendee_Event");

                    b.HasOne("DataAccessObject.Entities.Ticket", "Ticket")
                        .WithMany("Attendees")
                        .HasForeignKey("TicketId")
                        .IsRequired()
                        .HasConstraintName("FK_Attendee_Ticket1");

                    b.Navigation("Event");

                    b.Navigation("Ticket");
                });

            modelBuilder.Entity("DataAccessObject.Entities.AttendeeDetail", b =>
                {
                    b.HasOne("DataAccessObject.Entities.Attendee", "Attendee")
                        .WithMany("AttendeeDetails")
                        .HasForeignKey("AttendeeId")
                        .IsRequired()
                        .HasConstraintName("FK_AttendeeDetail_Attendee");

                    b.Navigation("Attendee");
                });

            modelBuilder.Entity("DataAccessObject.Entities.BoothRequest", b =>
                {
                    b.HasOne("DataAccessObject.Entities.Booth", "Booth")
                        .WithMany("BoothRequests")
                        .HasForeignKey("BoothId")
                        .IsRequired()
                        .HasConstraintName("FK_BoothRequest_Booth");

                    b.HasOne("DataAccessObject.Entities.User", "Sponsor")
                        .WithMany("BoothRequests")
                        .HasForeignKey("SponsorId")
                        .IsRequired()
                        .HasConstraintName("FK_BoothRequest_User");

                    b.Navigation("Booth");

                    b.Navigation("Sponsor");
                });

            modelBuilder.Entity("DataAccessObject.Entities.Event", b =>
                {
                    b.HasOne("DataAccessObject.Entities.User", "Organizer")
                        .WithMany("Events")
                        .HasForeignKey("OrganizerId")
                        .IsRequired()
                        .HasConstraintName("FK_Event_User");

                    b.HasOne("DataAccessObject.Entities.Venue", "Venue")
                        .WithMany("Events")
                        .HasForeignKey("VenueId")
                        .IsRequired()
                        .HasConstraintName("FK_Event_Venue");

                    b.Navigation("Organizer");

                    b.Navigation("Venue");
                });

            modelBuilder.Entity("DataAccessObject.Entities.Gift", b =>
                {
                    b.HasOne("DataAccessObject.Entities.Booth", "Booth")
                        .WithMany("Gifts")
                        .HasForeignKey("BoothId")
                        .IsRequired()
                        .HasConstraintName("FK_Gift_Booth");

                    b.Navigation("Booth");
                });

            modelBuilder.Entity("DataAccessObject.Entities.GiftReception", b =>
                {
                    b.HasOne("DataAccessObject.Entities.Attendee", "Attendee")
                        .WithMany("GiftReceptions")
                        .HasForeignKey("AttendeeId")
                        .IsRequired()
                        .HasConstraintName("FK_GiftReception_Attendee");

                    b.HasOne("DataAccessObject.Entities.Gift", "Gift")
                        .WithMany("GiftReceptions")
                        .HasForeignKey("GiftId")
                        .IsRequired()
                        .HasConstraintName("FK_GiftReception_Gift");

                    b.Navigation("Attendee");

                    b.Navigation("Gift");
                });

            modelBuilder.Entity("DataAccessObject.Entities.Ticket", b =>
                {
                    b.HasOne("DataAccessObject.Entities.Event", "Event")
                        .WithMany("Tickets")
                        .HasForeignKey("EventId")
                        .IsRequired()
                        .HasConstraintName("FK_Ticket_Event");

                    b.Navigation("Event");
                });

            modelBuilder.Entity("DataAccessObject.Entities.Transaction", b =>
                {
                    b.HasOne("DataAccessObject.Entities.Attendee", "Attendee")
                        .WithMany("Transactions")
                        .HasForeignKey("AttendeeId")
                        .IsRequired()
                        .HasConstraintName("FK_Transaction_Attendee");

                    b.HasOne("DataAccessObject.Entities.Payment", "PaymentMethodNavigation")
                        .WithMany("Transactions")
                        .HasForeignKey("PaymentMethod")
                        .IsRequired()
                        .HasConstraintName("FK_Transaction_Payment");

                    b.Navigation("Attendee");

                    b.Navigation("PaymentMethodNavigation");
                });

            modelBuilder.Entity("DataAccessObject.Entities.Attendee", b =>
                {
                    b.Navigation("AttendeeDetails");

                    b.Navigation("GiftReceptions");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("DataAccessObject.Entities.Booth", b =>
                {
                    b.Navigation("BoothRequests");

                    b.Navigation("Gifts");
                });

            modelBuilder.Entity("DataAccessObject.Entities.Event", b =>
                {
                    b.Navigation("Attendees");

                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("DataAccessObject.Entities.Gift", b =>
                {
                    b.Navigation("GiftReceptions");
                });

            modelBuilder.Entity("DataAccessObject.Entities.Payment", b =>
                {
                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("DataAccessObject.Entities.Ticket", b =>
                {
                    b.Navigation("Attendees");
                });

            modelBuilder.Entity("DataAccessObject.Entities.User", b =>
                {
                    b.Navigation("BoothRequests");

                    b.Navigation("Events");
                });

            modelBuilder.Entity("DataAccessObject.Entities.Venue", b =>
                {
                    b.Navigation("Events");
                });
#pragma warning restore 612, 618
        }
    }
}
