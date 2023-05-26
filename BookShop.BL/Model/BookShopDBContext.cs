using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.EntityFrameworkCore;

namespace BookShop.BL.Model;

public partial class BookShopDBContext : DbContext
{
    public BookShopDBContext()
    {
    }

    public BookShopDBContext(DbContextOptions<BookShopDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Residence> Residences { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SalesHistory> SalesHistories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["BookShopDBConnectionString"].ConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .IsFixedLength();
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.Property(e => e.Description)
                .HasMaxLength(1500)
                .IsFixedLength();
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.Price).HasColumnType("smallmoney");

            entity.HasOne(d => d.Author).WithMany(p => p.Books)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Books_Authors");
        });

        modelBuilder.Entity<Residence>(entity =>
        {
            entity.ToTable("Residence");

            entity.Property(e => e.City)
                .HasMaxLength(100)
                .IsFixedLength();
            entity.Property(e => e.Region)
                .HasMaxLength(100)
                .IsFixedLength();
            entity.Property(e => e.Street)
                .HasMaxLength(100)
                .IsFixedLength();
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsFixedLength();
        });

        modelBuilder.Entity<SalesHistory>(entity =>
        {
            entity.ToTable("SalesHistory");

            entity.Property(e => e.Date).HasColumnType("date");
            entity.Property(e => e.Price).HasColumnType("smallmoney");

            entity.HasOne(d => d.Book).WithMany(p => p.SalesHistories)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SalesHistory_Books");

            entity.HasOne(d => d.User).WithMany(p => p.SalesHistories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SalesHistory_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.FirstName)
                .HasMaxLength(30)
                .IsFixedLength();
            entity.Property(e => e.LastName)
                .HasMaxLength(30)
                .IsFixedLength();
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsFixedLength();

            entity.HasIndex(e => e.Email)
                  .IsUnique();

            entity.HasOne(d => d.Residence).WithMany(p => p.Users)
                .HasForeignKey(d => d.ResidenceId)
                .HasConstraintName("FK_Users_Residence");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
