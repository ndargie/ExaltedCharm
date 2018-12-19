using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ExaltedCharm.Api.Migrations
{
    public partial class abilities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CasteAbilities_Skills_AbilityId",
                table: "CasteAbilities");

            migrationBuilder.DropForeignKey(
                name: "FK_Charms_Skills_AbilityId",
                table: "Charms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Skills",
                table: "Skills");

            migrationBuilder.RenameTable(
                name: "Skills",
                newName: "Abilities");

            migrationBuilder.RenameIndex(
                name: "IX_Skills_Name",
                table: "Abilities",
                newName: "IX_Abilities_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Abilities",
                table: "Abilities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CasteAbilities_Abilities_AbilityId",
                table: "CasteAbilities",
                column: "AbilityId",
                principalTable: "Abilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Charms_Abilities_AbilityId",
                table: "Charms",
                column: "AbilityId",
                principalTable: "Abilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CasteAbilities_Abilities_AbilityId",
                table: "CasteAbilities");

            migrationBuilder.DropForeignKey(
                name: "FK_Charms_Abilities_AbilityId",
                table: "Charms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Abilities",
                table: "Abilities");

            migrationBuilder.RenameTable(
                name: "Abilities",
                newName: "Skills");

            migrationBuilder.RenameIndex(
                name: "IX_Abilities_Name",
                table: "Skills",
                newName: "IX_Skills_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Skills",
                table: "Skills",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CasteAbilities_Skills_AbilityId",
                table: "CasteAbilities",
                column: "AbilityId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Charms_Skills_AbilityId",
                table: "Charms",
                column: "AbilityId",
                principalTable: "Skills",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
