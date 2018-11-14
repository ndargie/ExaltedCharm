using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ExaltedCharm.Api.Migrations
{
    public partial class AddedUniqueConstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ExaltedTypes",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExaltedTypes_Name",
                table: "ExaltedTypes",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Durations_Name",
                table: "Durations",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharmTypes_Name",
                table: "CharmTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Charms_Name",
                table: "Charms",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExaltedTypes_Name",
                table: "ExaltedTypes");

            migrationBuilder.DropIndex(
                name: "IX_Durations_Name",
                table: "Durations");

            migrationBuilder.DropIndex(
                name: "IX_CharmTypes_Name",
                table: "CharmTypes");

            migrationBuilder.DropIndex(
                name: "IX_Charms_Name",
                table: "Charms");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ExaltedTypes",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
