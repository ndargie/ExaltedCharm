using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ExaltedCharm.Api.Migrations
{
    public partial class UpdateWeaponlinks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeaponTags_Weapons_WeaponId",
                table: "WeaponTags");

            migrationBuilder.DropTable(
                name: "Ammo");

            migrationBuilder.DropIndex(
                name: "IX_WeaponTags_WeaponId",
                table: "WeaponTags");

            migrationBuilder.DropColumn(
                name: "WeaponId",
                table: "WeaponTags");

            migrationBuilder.CreateTable(
                name: "AmmoType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(maxLength: 200, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Damage = table.Column<int>(nullable: false),
                    DamageType = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 200, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Rate = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmmoType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeaponWeaponTag",
                columns: table => new
                {
                    WeaponId = table.Column<int>(nullable: false),
                    WeaponTagId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeaponWeaponTag", x => new { x.WeaponId, x.WeaponTagId });
                    table.ForeignKey(
                        name: "FK_WeaponWeaponTag_Weapons_WeaponId",
                        column: x => x.WeaponId,
                        principalTable: "Weapons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeaponWeaponTag_WeaponTags_WeaponTagId",
                        column: x => x.WeaponTagId,
                        principalTable: "WeaponTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RangedWeaponAmmo",
                columns: table => new
                {
                    RangedWeaponId = table.Column<int>(nullable: false),
                    AmmoTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RangedWeaponAmmo", x => new { x.RangedWeaponId, x.AmmoTypeId });
                    table.ForeignKey(
                        name: "FK_RangedWeaponAmmo_AmmoType_AmmoTypeId",
                        column: x => x.AmmoTypeId,
                        principalTable: "AmmoType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RangedWeaponAmmo_Weapons_RangedWeaponId",
                        column: x => x.RangedWeaponId,
                        principalTable: "Weapons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RangedWeaponAmmo_AmmoTypeId",
                table: "RangedWeaponAmmo",
                column: "AmmoTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WeaponWeaponTag_WeaponTagId",
                table: "WeaponWeaponTag",
                column: "WeaponTagId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RangedWeaponAmmo");

            migrationBuilder.DropTable(
                name: "WeaponWeaponTag");

            migrationBuilder.DropTable(
                name: "AmmoType");

            migrationBuilder.AddColumn<int>(
                name: "WeaponId",
                table: "WeaponTags",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Ammo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(maxLength: 200, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Damage = table.Column<int>(nullable: false),
                    DamageType = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 200, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    RangedWeaponId = table.Column<int>(nullable: true),
                    Rate = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ammo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ammo_Weapons_RangedWeaponId",
                        column: x => x.RangedWeaponId,
                        principalTable: "Weapons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeaponTags_WeaponId",
                table: "WeaponTags",
                column: "WeaponId");

            migrationBuilder.CreateIndex(
                name: "IX_Ammo_RangedWeaponId",
                table: "Ammo",
                column: "RangedWeaponId");

            migrationBuilder.AddForeignKey(
                name: "FK_WeaponTags_Weapons_WeaponId",
                table: "WeaponTags",
                column: "WeaponId",
                principalTable: "Weapons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
