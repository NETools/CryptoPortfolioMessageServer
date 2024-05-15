﻿// <auto-generated />
using System;
using CryptoPortfolioMessageServer.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CryptoPortfolioMessageServer.Migrations
{
    [DbContext(typeof(CryptoPortfolioDbContext))]
    partial class CryptoPortfolioDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.4");

            modelBuilder.Entity("CryptoPortfolioMessageServer.Models.Persistence.Portfolio", b =>
                {
                    b.Property<int>("PortfolioId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Assets")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("PortfolioId");

                    b.ToTable("Portfolios");
                });

            modelBuilder.Entity("CryptoPortfolioMessageServer.Models.Persistence.Transaction", b =>
                {
                    b.Property<int>("TransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double>("AmountEur")
                        .HasColumnType("REAL");

                    b.Property<string>("CoinId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<double>("PricePerCoin")
                        .HasColumnType("REAL");

                    b.Property<double>("QuantityCoins")
                        .HasColumnType("REAL");

                    b.Property<Guid>("TransactionGuid")
                        .HasColumnType("TEXT");

                    b.Property<int>("TransactionSide")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("TransactionId");

                    b.HasIndex("UserId");

                    b.ToTable("Transaction");
                });

            modelBuilder.Entity("CryptoPortfolioMessageServer.Models.Persistence.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("ActivationId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActivated")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("PortfolioId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("PortfolioId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CryptoPortfolioMessageServer.Models.Persistence.Transaction", b =>
                {
                    b.HasOne("CryptoPortfolioMessageServer.Models.Persistence.User", null)
                        .WithMany("Transactions")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("CryptoPortfolioMessageServer.Models.Persistence.User", b =>
                {
                    b.HasOne("CryptoPortfolioMessageServer.Models.Persistence.Portfolio", "Portfolio")
                        .WithMany()
                        .HasForeignKey("PortfolioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Portfolio");
                });

            modelBuilder.Entity("CryptoPortfolioMessageServer.Models.Persistence.User", b =>
                {
                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
