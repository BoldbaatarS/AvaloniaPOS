using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudApi.Migrations
{
    /// <inheritdoc />
    public partial class AddProductPriceFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Users_BranchId",
                table: "Users",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_BranchId",
                table: "Categories",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Branch_BranchId",
                table: "Categories",
                column: "BranchId",
                principalTable: "Branch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Branch_BranchId",
                table: "Users",
                column: "BranchId",
                principalTable: "Branch",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Branch_BranchId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Branch_BranchId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_BranchId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Categories_BranchId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Categories");
        }
    }
}
