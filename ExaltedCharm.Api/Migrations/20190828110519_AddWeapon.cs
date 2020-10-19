using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ExaltedCharm.Api.Migrations
{
    public partial class AddWeapon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WeaponId",
                table: "WeaponTags",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Weapons",
                columns: table => new
                {
                    Range = table.Column<int>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Accuracy = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 200, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Damage = table.Column<int>(nullable: false),
                    DamageType = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    ModifiedBy = table.Column<string>(maxLength: 200, nullable: true),
                    ModifiedDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Rate = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weapons", x => x.Id);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Weapons_Name",
                table: "Weapons",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WeaponTags_Weapons_WeaponId",
                table: "WeaponTags",
                column: "WeaponId",
                principalTable: "Weapons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeaponTags_Weapons_WeaponId",
                table: "WeaponTags");

            migrationBuilder.DropTable(
                name: "Ammo");

            migrationBuilder.DropTable(
                name: "Weapons");

            migrationBuilder.DropIndex(
                name: "IX_WeaponTags_WeaponId",
                table: "WeaponTags");

            migrationBuilder.DropColumn(
                name: "WeaponId",
                table: "WeaponTags");
        }
    }
}
