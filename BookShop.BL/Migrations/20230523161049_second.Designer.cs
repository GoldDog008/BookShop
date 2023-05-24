﻿// <auto-generated />
using System;
using BookShop.BL.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BookShop.BL.Migrations
{
    [DbContext(typeof(BookShopDBContext))]
    [Migration("20230523161049_second")]
    partial class second
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BookShop.BL.Model.Author", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nchar(30)")
                        .IsFixedLength();

                    b.HasKey("Id");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("BookShop.BL.Model.Book", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("AuthorId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasMaxLength(1500)
                        .HasColumnType("nchar(1500)")
                        .IsFixedLength();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nchar(50)")
                        .IsFixedLength();

                    b.Property<decimal>("Price")
                        .HasColumnType("smallmoney");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("BookShop.BL.Model.Residence", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int?>("ApartmentNumber")
                        .HasColumnType("int");

                    b.Property<string>("City")
                        .HasMaxLength(100)
                        .HasColumnType("nchar(100)")
                        .IsFixedLength();

                    b.Property<int?>("HouseNumber")
                        .HasColumnType("int");

                    b.Property<string>("Region")
                        .HasMaxLength(100)
                        .HasColumnType("nchar(100)")
                        .IsFixedLength();

                    b.Property<string>("Street")
                        .HasMaxLength(100)
                        .HasColumnType("nchar(100)")
                        .IsFixedLength();

                    b.HasKey("Id");

                    b.ToTable("Residence", (string)null);
                });

            modelBuilder.Entity("BookShop.BL.Model.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasMaxLength(20)
                        .HasColumnType("nchar(20)")
                        .IsFixedLength();

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("BookShop.BL.Model.SalesHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BookId")
                        .HasColumnType("int");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<decimal>("Price")
                        .HasColumnType("smallmoney");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BookId");

                    b.HasIndex("UserId");

                    b.ToTable("SalesHistory", (string)null);
                });

            modelBuilder.Entity("BookShop.BL.Model.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nchar(50)")
                        .IsFixedLength();

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nchar(30)")
                        .IsFixedLength();

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nchar(30)")
                        .IsFixedLength();

                    b.Property<string>("Phone")
                        .HasMaxLength(20)
                        .HasColumnType("nchar(20)")
                        .IsFixedLength();

                    b.Property<int?>("ResidenceId")
                        .HasColumnType("int");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("ResidenceId");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BookShop.BL.Model.Book", b =>
                {
                    b.HasOne("BookShop.BL.Model.Author", "Author")
                        .WithMany("Books")
                        .HasForeignKey("AuthorId")
                        .IsRequired()
                        .HasConstraintName("FK_Books_Authors");

                    b.Navigation("Author");
                });

            modelBuilder.Entity("BookShop.BL.Model.SalesHistory", b =>
                {
                    b.HasOne("BookShop.BL.Model.Book", "Book")
                        .WithMany("SalesHistories")
                        .HasForeignKey("BookId")
                        .IsRequired()
                        .HasConstraintName("FK_SalesHistory_Books");

                    b.HasOne("BookShop.BL.Model.User", "User")
                        .WithMany("SalesHistories")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK_SalesHistory_Users");

                    b.Navigation("Book");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BookShop.BL.Model.User", b =>
                {
                    b.HasOne("BookShop.BL.Model.Residence", "Residence")
                        .WithMany("Users")
                        .HasForeignKey("ResidenceId")
                        .HasConstraintName("FK_Users_Residence");

                    b.HasOne("BookShop.BL.Model.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .IsRequired()
                        .HasConstraintName("FK_Users_Roles");

                    b.Navigation("Residence");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("BookShop.BL.Model.Author", b =>
                {
                    b.Navigation("Books");
                });

            modelBuilder.Entity("BookShop.BL.Model.Book", b =>
                {
                    b.Navigation("SalesHistories");
                });

            modelBuilder.Entity("BookShop.BL.Model.Residence", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("BookShop.BL.Model.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("BookShop.BL.Model.User", b =>
                {
                    b.Navigation("SalesHistories");
                });
#pragma warning restore 612, 618
        }
    }
}