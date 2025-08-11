using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantPOS.Migrations
{
    /// <inheritdoc />
    public partial class AddTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tables_Halls_HallModelId",
                table: "Tables");

            migrationBuilder.DropIndex(
                name: "IX_Tables_HallModelId",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "HallModelId",
                table: "Tables");

            migrationBuilder.AddColumn<double>(
                name: "PositionX",
                table: "Tables",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PositionY",
                table: "Tables",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_Tables_HallId",
                table: "Tables",
                column: "HallId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tables_Halls_HallId",
                table: "Tables",
                column: "HallId",
                principalTable: "Halls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tables_Halls_HallId",
                table: "Tables");

            migrationBuilder.DropIndex(
                name: "IX_Tables_HallId",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "PositionX",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "PositionY",
                table: "Tables");

            migrationBuilder.AddColumn<Guid>(
                name: "HallModelId",
                table: "Tables",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tables_HallModelId",
                table: "Tables",
                column: "HallModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tables_Halls_HallModelId",
                table: "Tables",
                column: "HallModelId",
                principalTable: "Halls",
                principalColumn: "Id");
        }
    }
}
