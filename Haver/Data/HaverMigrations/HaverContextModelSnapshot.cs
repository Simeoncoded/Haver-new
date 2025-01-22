﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using haver.Data;

#nullable disable

namespace haver.Data.HaverMigrations
{
    [DbContext(typeof(HaverContext))]
    partial class HaverContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.10");

            modelBuilder.Entity("haver.Models.Customer", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("Phone")
                        .IsUnique();

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("haver.Models.Engineer", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Engineers");
                });

            modelBuilder.Entity("haver.Models.Machine", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Class")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SerialNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Size")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SizeDeck")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("SerialNumber")
                        .IsUnique();

                    b.ToTable("Machines");
                });

            modelBuilder.Entity("haver.Models.MachineSchedule", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("AirSeal")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Base")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("CoatingLining")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DeliveryDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Dissembly")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DueDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("MachineID")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Media")
                        .HasColumnType("INTEGER");

                    b.Property<int>("NoteID")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("PODueDate")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("PackageRDate")
                        .HasColumnType("TEXT");

                    b.Property<bool>("SparePMedia")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("SpareParts")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("MachineID");

                    b.HasIndex("NoteID")
                        .IsUnique();

                    b.ToTable("MachineSchedules");
                });

            modelBuilder.Entity("haver.Models.MachineScheduleEngineer", b =>
                {
                    b.Property<int>("MachineScheduleID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("EngineerID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ID")
                        .HasColumnType("INTEGER");

                    b.HasKey("MachineScheduleID", "EngineerID");

                    b.HasIndex("EngineerID");

                    b.HasIndex("MachineScheduleID", "EngineerID")
                        .IsUnique();

                    b.ToTable("MachineScheduleEngineers");
                });

            modelBuilder.Entity("haver.Models.Note", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("AssemblyHours")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("BudgetHours")
                        .HasColumnType("TEXT");

                    b.Property<int?>("NamePlate")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PreOrder")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("ReworkHours")
                        .HasColumnType("TEXT");

                    b.Property<string>("Scope")
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Notes");
                });

            modelBuilder.Entity("haver.Models.PackageRelease", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("MachineScheduleID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasMaxLength(400)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("PReleaseDateA")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("PReleaseDateP")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("MachineScheduleID")
                        .IsUnique();

                    b.HasIndex("Name", "MachineScheduleID")
                        .IsUnique();

                    b.ToTable("PackageReleases");
                });

            modelBuilder.Entity("haver.Models.SalesOrder", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("AppDwgRcd")
                        .HasColumnType("TEXT");

                    b.Property<int>("CustomerID")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DwgIsDt")
                        .HasColumnType("TEXT");

                    b.Property<int>("MachineScheduleID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("OrderNumber")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("TEXT");

                    b.Property<string>("PoNumber")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Price")
                        .HasColumnType("TEXT");

                    b.Property<string>("ShippingTerms")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("SoDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("VendorID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("CustomerID");

                    b.HasIndex("MachineScheduleID");

                    b.HasIndex("OrderNumber")
                        .IsUnique();

                    b.HasIndex("VendorID");

                    b.ToTable("SalesOrders");
                });

            modelBuilder.Entity("haver.Models.Vendor", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int?>("Name")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Vendors");
                });

            modelBuilder.Entity("haver.Models.MachineSchedule", b =>
                {
                    b.HasOne("haver.Models.Machine", "Machine")
                        .WithMany("MachineSchedules")
                        .HasForeignKey("MachineID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("haver.Models.Note", "Note")
                        .WithOne("MachineSchedule")
                        .HasForeignKey("haver.Models.MachineSchedule", "NoteID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Machine");

                    b.Navigation("Note");
                });

            modelBuilder.Entity("haver.Models.MachineScheduleEngineer", b =>
                {
                    b.HasOne("haver.Models.Engineer", "Engineer")
                        .WithMany("MachineScheduleEngineers")
                        .HasForeignKey("EngineerID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("haver.Models.MachineSchedule", "Schedule")
                        .WithMany("MachineScheduleEngineers")
                        .HasForeignKey("MachineScheduleID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Engineer");

                    b.Navigation("Schedule");
                });

            modelBuilder.Entity("haver.Models.PackageRelease", b =>
                {
                    b.HasOne("haver.Models.MachineSchedule", "Schedule")
                        .WithOne("PackageRelease")
                        .HasForeignKey("haver.Models.PackageRelease", "MachineScheduleID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Schedule");
                });

            modelBuilder.Entity("haver.Models.SalesOrder", b =>
                {
                    b.HasOne("haver.Models.Customer", "Customer")
                        .WithMany("SalesOrders")
                        .HasForeignKey("CustomerID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("haver.Models.MachineSchedule", "MachineSchedule")
                        .WithMany("SalesOrders")
                        .HasForeignKey("MachineScheduleID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("haver.Models.Vendor", "Vendor")
                        .WithMany("SalesOrders")
                        .HasForeignKey("VendorID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("MachineSchedule");

                    b.Navigation("Vendor");
                });

            modelBuilder.Entity("haver.Models.Customer", b =>
                {
                    b.Navigation("SalesOrders");
                });

            modelBuilder.Entity("haver.Models.Engineer", b =>
                {
                    b.Navigation("MachineScheduleEngineers");
                });

            modelBuilder.Entity("haver.Models.Machine", b =>
                {
                    b.Navigation("MachineSchedules");
                });

            modelBuilder.Entity("haver.Models.MachineSchedule", b =>
                {
                    b.Navigation("MachineScheduleEngineers");

                    b.Navigation("PackageRelease");

                    b.Navigation("SalesOrders");
                });

            modelBuilder.Entity("haver.Models.Note", b =>
                {
                    b.Navigation("MachineSchedule");
                });

            modelBuilder.Entity("haver.Models.Vendor", b =>
                {
                    b.Navigation("SalesOrders");
                });
#pragma warning restore 612, 618
        }
    }
}
