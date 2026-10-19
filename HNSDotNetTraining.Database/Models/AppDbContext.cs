using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HNSDotNetTraining.Database.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblAppointment> TblAppointments { get; set; }

    public virtual DbSet<TblBlog> TblBlogs { get; set; }

    public virtual DbSet<TblCustomer> TblCustomers { get; set; }

    public virtual DbSet<TblPayment> TblPayments { get; set; }

    public virtual DbSet<TblSpaService> TblSpaServices { get; set; }

    public virtual DbSet<TblTherapist> TblTherapists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=HNSDotNetTraining;User Id=sa;Password=sasa@123;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblAppointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Tbl_Appo__8ECDFCC2AF0334BE");

            entity.ToTable("Tbl_Appointment");

            entity.Property(e => e.EndTime).HasPrecision(0);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(30)
                .HasDefaultValue("Unpaid");
            entity.Property(e => e.StartTime).HasPrecision(0);
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Customer).WithMany(p => p.TblAppointments)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tbl_Appoi__Custo__656C112C");

            entity.HasOne(d => d.Service).WithMany(p => p.TblAppointments)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tbl_Appoi__Servi__6754599E");

            entity.HasOne(d => d.Therapist).WithMany(p => p.TblAppointments)
                .HasForeignKey(d => d.TherapistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tbl_Appoi__Thera__66603565");
        });

        modelBuilder.Entity<TblBlog>(entity =>
        {
            entity.HasKey(e => e.BlogId);

            entity.ToTable("Tbl_Blog");

            entity.Property(e => e.BlogAuthor).HasMaxLength(50);
            entity.Property(e => e.BlogTitle).HasMaxLength(50);
        });

        modelBuilder.Entity<TblCustomer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Tbl_Cust__A4AE64D8E6047B6B");

            entity.ToTable("Tbl_Customer");

            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(30);
        });

        modelBuilder.Entity<TblPayment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Tbl_Paym__9B556A387E6839B7");

            entity.ToTable("Tbl_Payment");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.ReceiptNumber).HasMaxLength(50);

            entity.HasOne(d => d.Appointment).WithMany(p => p.TblPayments)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tbl_Payme__Appoi__6D0D32F4");
        });

        modelBuilder.Entity<TblSpaService>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__Tbl_SpaS__C51BB00AAED54FA3");

            entity.ToTable("Tbl_SpaService");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ServiceName).HasMaxLength(100);
        });

        modelBuilder.Entity<TblTherapist>(entity =>
        {
            entity.HasKey(e => e.TherapistId).HasName("PK__Tbl_Ther__4D621932BB9E7A9D");

            entity.ToTable("Tbl_Therapist");

            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            entity.Property(e => e.Specialty).HasMaxLength(150);
            entity.Property(e => e.TherapistName).HasMaxLength(100);
            entity.Property(e => e.WorkingHours)
                .HasMaxLength(100)
                .HasDefaultValue("09:00 - 18:00");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
