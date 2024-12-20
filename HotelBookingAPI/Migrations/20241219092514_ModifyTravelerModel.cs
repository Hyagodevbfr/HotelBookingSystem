using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBookingAPI.Migrations
{
    /// <inheritdoc />
    public partial class ModifyTravelerModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Travelers_TravelerUserId",
                table: "Bookings");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Travelers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Travelers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "EditedBy",
                table: "Travelers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EditedOn",
                table: "Travelers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TravelerUserId",
                table: "Bookings",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "TravelerId",
                table: "Bookings",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TravelerId",
                table: "Bookings",
                column: "TravelerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Travelers_TravelerId",
                table: "Bookings",
                column: "TravelerId",
                principalTable: "Travelers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Travelers_TravelerUserId",
                table: "Bookings",
                column: "TravelerUserId",
                principalTable: "Travelers",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Travelers_TravelerId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Travelers_TravelerUserId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_TravelerId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Travelers");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Travelers");

            migrationBuilder.DropColumn(
                name: "EditedBy",
                table: "Travelers");

            migrationBuilder.DropColumn(
                name: "EditedOn",
                table: "Travelers");

            migrationBuilder.AlterColumn<string>(
                name: "TravelerUserId",
                table: "Bookings",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TravelerId",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Travelers_TravelerUserId",
                table: "Bookings",
                column: "TravelerUserId",
                principalTable: "Travelers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
