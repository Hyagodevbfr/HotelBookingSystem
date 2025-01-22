using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBookingAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCheckinAndCheckoutTimeFromBookingAndAddToRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckInTime",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CheckOutTime",
                table: "Bookings");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "CheckInTime",
                table: "Rooms",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "CheckOutTime",
                table: "Rooms",
                type: "time",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckInTime",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "CheckOutTime",
                table: "Rooms");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "CheckInTime",
                table: "Bookings",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "CheckOutTime",
                table: "Bookings",
                type: "time",
                nullable: true);
        }
    }
}
