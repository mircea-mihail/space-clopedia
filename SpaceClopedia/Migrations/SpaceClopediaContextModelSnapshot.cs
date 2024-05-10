﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SpaceClopedia.ContextModels;

#nullable disable

namespace SpaceClopedia.Migrations
{
    [DbContext(typeof(SpaceClopediaContext))]
    partial class SpaceClopediaContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SpaceClopedia.Models.ArticolModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Autor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AutorModificare")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Continut")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DataCreare")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DataModificare")
                        .HasColumnType("datetime2");

                    b.Property<int>("DomeniuId")
                        .HasColumnType("int");

                    b.Property<int>("NumarVersiune")
                        .HasColumnType("int");

                    b.Property<string>("Titlu")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TitluPoza")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DomeniuId");

                    b.ToTable("Articol");
                });

            modelBuilder.Entity("SpaceClopedia.Models.DomeniuModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("NumeDomeniu")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Domeniu");
                });

            modelBuilder.Entity("SpaceClopedia.Models.UtilizatorModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DataInregistrare")
                        .HasColumnType("datetime2");

                    b.Property<string>("NumeUtilizator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Parola")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Rol")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Utilizator");
                });

            modelBuilder.Entity("SpaceClopedia.Models.ArticolModel", b =>
                {
                    b.HasOne("SpaceClopedia.Models.DomeniuModel", "Domeniu")
                        .WithMany()
                        .HasForeignKey("DomeniuId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Domeniu");
                });
#pragma warning restore 612, 618
        }
    }
}
