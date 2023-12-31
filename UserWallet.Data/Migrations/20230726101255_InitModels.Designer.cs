﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using UserWallet.Data;

#nullable disable

namespace UserWallet.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230726101255_InitModels")]
    partial class InitModels
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseSerialColumns(modelBuilder);

            modelBuilder.Entity("UserWallet.Models.Currency", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("id");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("boolean")
                        .HasColumnName("is_available");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.HasKey("Id");

                    b.ToTable("currencies");
                });

            modelBuilder.Entity("UserWallet.Models.Deposit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("Id"));

                    b.Property<string>("AdditionalData")
                        .IsRequired()
                        .HasColumnType("jsonb")
                        .HasColumnName("additional_data");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric")
                        .HasColumnName("amount");

                    b.Property<string>("CurrencyId")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("currency_id");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("UserId");

                    b.ToTable("deposits");
                });

            modelBuilder.Entity("UserWallet.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseSerialColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsBlocked")
                        .HasColumnType("boolean")
                        .HasColumnName("is_blocked");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("character varying(8)")
                        .HasColumnName("password");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("role");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("character varying(8)")
                        .HasColumnName("username");

                    b.HasKey("Id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("UserWallet.Models.UserBalance", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.Property<string>("CurrencyId")
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)")
                        .HasColumnName("currency_id");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric")
                        .HasColumnName("amount");

                    b.HasKey("UserId", "CurrencyId");

                    b.HasIndex("CurrencyId");

                    b.ToTable("userBalances");
                });

            modelBuilder.Entity("UserWallet.Models.Deposit", b =>
                {
                    b.HasOne("UserWallet.Models.Currency", "Currency")
                        .WithMany()
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UserWallet.Models.User", "User")
                        .WithMany("Deposits")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Currency");

                    b.Navigation("User");
                });

            modelBuilder.Entity("UserWallet.Models.UserBalance", b =>
                {
                    b.HasOne("UserWallet.Models.Currency", "Currency")
                        .WithMany()
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UserWallet.Models.User", "User")
                        .WithMany("Balances")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Currency");

                    b.Navigation("User");
                });

            modelBuilder.Entity("UserWallet.Models.User", b =>
                {
                    b.Navigation("Balances");

                    b.Navigation("Deposits");
                });
#pragma warning restore 612, 618
        }
    }
}
