﻿// <auto-generated />
using ExaltedCharm.Api.Entities;
using ExaltedCharm.Api.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;

namespace ExaltedCharm.Api.Migrations
{
    [DbContext(typeof(CharmContext))]
    partial class CharmContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ExaltedCharm.Api.Entities.Ability", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(200);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(200);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(40);

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Abilities");
                });

            modelBuilder.Entity("ExaltedCharm.Api.Entities.AmmoType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(200);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<int>("Damage");

                    b.Property<int>("DamageType");

                    b.Property<string>("Description");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(200);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("Name");

                    b.Property<int>("Rate");

                    b.HasKey("Id");

                    b.ToTable("AmmoType");
                });

            modelBuilder.Entity("ExaltedCharm.Api.Entities.Attribute", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(200);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(200);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(40);

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.ToTable("Attributes");
                });

            modelBuilder.Entity("ExaltedCharm.Api.Entities.Caste", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(200);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<int>("ExaltedTypeId");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(200);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(40);

                    b.HasKey("Id");

                    b.HasIndex("ExaltedTypeId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Castes");
                });

            modelBuilder.Entity("ExaltedCharm.Api.Entities.CasteAbility", b =>
                {
                    b.Property<int>("CasteId");

                    b.Property<int>("AbilityId");

                    b.HasKey("CasteId", "AbilityId");

                    b.HasIndex("AbilityId");

                    b.ToTable("CasteAbilities");
                });

            modelBuilder.Entity("ExaltedCharm.Api.Entities.Charm", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AbilityId");

                    b.Property<int>("CharmTypeId");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(200);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description")
                        .HasMaxLength(800);

                    b.Property<int>("DurationId");

                    b.Property<int>("EssanseRequirement");

                    b.Property<int>("ExaltedTypeId");

                    b.Property<int>("HealthCost");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(200);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<int>("MoteCost");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<int>("WillpowerCost");

                    b.HasKey("Id");

                    b.HasIndex("AbilityId");

                    b.HasIndex("CharmTypeId");

                    b.HasIndex("DurationId");

                    b.HasIndex("ExaltedTypeId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Charms");
                });

            modelBuilder.Entity("ExaltedCharm.Api.Entities.CharmType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(200);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(200);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("CharmTypes");
                });

            modelBuilder.Entity("ExaltedCharm.Api.Entities.Duration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(200);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(200);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Durations");
                });

            modelBuilder.Entity("ExaltedCharm.Api.Entities.ExaltedType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(200);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<string>("Examples");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(200);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(40);

                    b.Property<string>("NecromancyLimit")
                        .HasMaxLength(100);

                    b.Property<string>("SorceryLimit")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("ExaltedTypes");
                });

            modelBuilder.Entity("ExaltedCharm.Api.Entities.Keyword", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(200);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(200);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("Keywords");
                });

            modelBuilder.Entity("ExaltedCharm.Api.Entities.KeywordCharm", b =>
                {
                    b.Property<int>("CharmId");

                    b.Property<int>("KeywordId");

                    b.HasKey("CharmId", "KeywordId");

                    b.HasIndex("KeywordId");

                    b.ToTable("KeywordCharms");
                });

            modelBuilder.Entity("ExaltedCharm.Api.Entities.RangedWeaponAmmo", b =>
                {
                    b.Property<int>("RangedWeaponId");

                    b.Property<int>("AmmoTypeId");

                    b.HasKey("RangedWeaponId", "AmmoTypeId");

                    b.HasIndex("AmmoTypeId");

                    b.ToTable("RangedWeaponAmmo");
                });

            modelBuilder.Entity("ExaltedCharm.Api.Entities.Weapon", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Accuracy");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(200);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<int>("Damage");

                    b.Property<int>("DamageType");

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(200);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<int>("Rate");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Weapons");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Weapon");
                });

            modelBuilder.Entity("ExaltedCharm.Api.Entities.WeaponTag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(200);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(200);

                    b.Property<DateTime?>("ModifiedDate");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("WeaponTags");
                });

            modelBuilder.Entity("ExaltedCharm.Api.Entities.WeaponWeaponTag", b =>
                {
                    b.Property<int>("WeaponId");

                    b.Property<int>("WeaponTagId");

                    b.HasKey("WeaponId", "WeaponTagId");

                    b.HasIndex("WeaponTagId");

                    b.ToTable("WeaponWeaponTag");
                });

            modelBuilder.Entity("ExaltedCharm.Api.Entities.RangedWeapon", b =>
                {
                    b.HasBaseType("ExaltedCharm.Api.Entities.Weapon");

                    b.Property<int>("Range");

                    b.ToTable("RangedWeapon");

                    b.HasDiscriminator().HasValue("RangedWeapon");
                });

            modelBuilder.Entity("ExaltedCharm.Api.Entities.Caste", b =>
                {
                    b.HasOne("ExaltedCharm.Api.Entities.ExaltedType", "ExaltedType")
                        .WithMany("Castes")
                        .HasForeignKey("ExaltedTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ExaltedCharm.Api.Entities.CasteAbility", b =>
                {
                    b.HasOne("ExaltedCharm.Api.Entities.Ability", "Ability")
                        .WithMany("Castes")
                        .HasForeignKey("AbilityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ExaltedCharm.Api.Entities.Caste", "Caste")
                        .WithMany("Abilities")
                        .HasForeignKey("CasteId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ExaltedCharm.Api.Entities.Charm", b =>
                {
                    b.HasOne("ExaltedCharm.Api.Entities.Ability", "Ability")
                        .WithMany("Charms")
                        .HasForeignKey("AbilityId");

                    b.HasOne("ExaltedCharm.Api.Entities.CharmType", "CharmType")
                        .WithMany("Charms")
                        .HasForeignKey("CharmTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ExaltedCharm.Api.Entities.Duration", "Duration")
                        .WithMany("Charms")
                        .HasForeignKey("DurationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ExaltedCharm.Api.Entities.ExaltedType", "ExaltedType")
                        .WithMany()
                        .HasForeignKey("ExaltedTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ExaltedCharm.Api.Entities.KeywordCharm", b =>
                {
                    b.HasOne("ExaltedCharm.Api.Entities.Charm", "Charm")
                        .WithMany("Keywords")
                        .HasForeignKey("CharmId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ExaltedCharm.Api.Entities.Keyword", "Keyword")
                        .WithMany("Charms")
                        .HasForeignKey("KeywordId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ExaltedCharm.Api.Entities.RangedWeaponAmmo", b =>
                {
                    b.HasOne("ExaltedCharm.Api.Entities.AmmoType", "AmmoType")
                        .WithMany("RangedWeapons")
                        .HasForeignKey("AmmoTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ExaltedCharm.Api.Entities.RangedWeapon", "RangedWeapon")
                        .WithMany("AmmoTypes")
                        .HasForeignKey("RangedWeaponId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ExaltedCharm.Api.Entities.WeaponWeaponTag", b =>
                {
                    b.HasOne("ExaltedCharm.Api.Entities.Weapon", "Weapon")
                        .WithMany("Tags")
                        .HasForeignKey("WeaponId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ExaltedCharm.Api.Entities.WeaponTag", "WeaponTag")
                        .WithMany("Weapons")
                        .HasForeignKey("WeaponTagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
