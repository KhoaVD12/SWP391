using BusinessObject.Ultils;
using DataAccessObject.Enums;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObject.Entities;

public partial class TicketContext : DbContext
{
    public TicketContext()
    {
    }

    public TicketContext(DbContextOptions<TicketContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attendee> Attendees { get; set; }

    public virtual DbSet<AttendeeDetail> AttendeeDetails { get; set; }

    public virtual DbSet<Booth> Booths { get; set; }

    public virtual DbSet<BoothRequest> BoothRequests { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Gift> Gifts { get; set; }

    public virtual DbSet<GiftReception> GiftReceptions { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Venue> Venues { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attendee>(entity =>
        {
            entity.ToTable("Attendee");

            entity.HasIndex(e => e.EventId, "IX_Attendee_EventId");

            entity.HasIndex(e => e.TicketId, "IX_Attendee_TicketId");

            entity.Property(e => e.CheckInStatus).IsUnicode(false);
            entity.Property(e => e.RegistrationDate).HasColumnType("datetime");

            entity.HasOne(d => d.Event).WithMany(p => p.Attendees)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attendee_Event");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Attendees)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attendee_Ticket1");
        });

        modelBuilder.Entity<AttendeeDetail>(entity =>
        {
            entity.ToTable("AttendeeDetail");

            entity.Property(e => e.Email).IsUnicode(false);
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.Phone).IsUnicode(false);

            entity.HasOne(d => d.Attendee).WithMany(p => p.AttendeeDetails)
                .HasForeignKey(d => d.AttendeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AttendeeDetail_Attendee");
        });

        modelBuilder.Entity<Booth>(entity =>
        {
            entity.ToTable("Booth");

            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Location).IsUnicode(false);
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.Status).IsUnicode(false);
        });

        modelBuilder.Entity<BoothRequest>(entity =>
        {
            entity.ToTable("BoothRequest");

            entity.HasIndex(e => e.BoothId, "IX_BoothRequest_BoothId");

            entity.HasIndex(e => e.SponsorId, "IX_BoothRequest_SponsorId");

            entity.Property(e => e.RequestDate).HasColumnType("datetime");
            entity.Property(e => e.Status).IsUnicode(false);

            entity.HasOne(d => d.Booth).WithMany(p => p.BoothRequests)
                .HasForeignKey(d => d.BoothId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BoothRequest_Booth");

            entity.HasOne(d => d.Sponsor).WithMany(p => p.BoothRequests)
                .HasForeignKey(d => d.SponsorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BoothRequest_User");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("Event");

            entity.HasIndex(e => e.OrganizerId, "IX_Event_OrganizerId");

            entity.HasIndex(e => e.VenueId, "IX_Event_VenueId");

            entity.HasIndex(e => e.StaffId, "IX_Event_StaffId");

            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Status).IsUnicode(false);
            entity.Property(e => e.Title).IsUnicode(false);

            entity.HasOne(d => d.Organizer).WithMany(p => p.Events)
                .HasForeignKey(d => d.OrganizerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_Organizer");

            entity.HasOne(d => d.Venue).WithMany(p => p.Events)
                .HasForeignKey(d => d.VenueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_Venue");

            entity.HasOne(e => e.Staff)
                .WithMany(u => u.AssignedEvents)
                .HasForeignKey(e => e.StaffId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Event_Staff");
        });

        modelBuilder.Entity<Gift>(entity =>
        {
            entity.ToTable("Gift");

            entity.HasIndex(e => e.BoothId, "IX_Gift_BoothId");

            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Name).IsUnicode(false);

            entity.HasOne(d => d.Booth).WithMany(p => p.Gifts)
                .HasForeignKey(d => d.BoothId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Gift_Booth");
        });

        modelBuilder.Entity<GiftReception>(entity =>
        {
            entity.ToTable("GiftReception");

            entity.HasIndex(e => e.GiftId, "IX_GiftReception_GiftId");

            entity.Property(e => e.ReceptionDate).HasColumnType("datetime");

            entity.HasOne(d => d.Attendee).WithMany(p => p.GiftReceptions)
                .HasForeignKey(d => d.AttendeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GiftReception_Attendee");

            entity.HasOne(d => d.Gift).WithMany(p => p.GiftReceptions)
                .HasForeignKey(d => d.GiftId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GiftReception_Gift");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payment");

            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.Status).IsUnicode(false);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("Ticket");

            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TicketSaleEndDate).HasColumnType("datetime");

            entity.HasOne(d => d.Event).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ticket_Event");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("Transaction");

            entity.HasIndex(e => e.PaymentMethod, "IX_Transaction_PaymentMethod");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Status).IsUnicode(false);

            entity.HasOne(d => d.Attendee).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.AttendeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_Attendee");

            entity.HasOne(d => d.PaymentMethodNavigation).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.PaymentMethod)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Transaction_Payment");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.Email).IsUnicode(false);
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.Password).IsUnicode(false);
            entity.Property(e => e.Role).IsUnicode(false);
            entity.Property(e => e.Status).IsUnicode(false);
        });

        modelBuilder.Entity<Venue>(entity =>
        {
            entity.ToTable("Venue");

            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.Status).IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        // Hash the admin's password
        var hashedPassword = HashPass.HashWithSHA256("Admin@123");

        // Create the admin user
        var adminUser = new User
        {
            Id = 1,
            Name = "Admin",
            Email = "admin@gmail.com",
            Password = hashedPassword,
            Role = "Admin",
            Status = "Active"
        };

        // Seed the admin user
        modelBuilder.Entity<User>().HasData(adminUser);
    }
}