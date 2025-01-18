using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBookingAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDbSetGuestBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuestBooking_Bookings_BookingId",
                table: "GuestBooking");

            migrationBuilder.DropForeignKey(
                name: "FK_GuestBooking_Guests_GuestId",
                table: "GuestBooking");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GuestBooking",
                table: "GuestBooking");

            migrationBuilder.RenameTable(
                name: "GuestBooking",
                newName: "GuestBookings");

            migrationBuilder.RenameIndex(
                name: "IX_GuestBooking_BookingId",
                table: "GuestBookings",
                newName: "IX_GuestBookings_BookingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GuestBookings",
                table: "GuestBookings",
                columns: new[] { "GuestId", "BookingId" });

            migrationBuilder.AddForeignKey(
                name: "FK_GuestBookings_Bookings_BookingId",
                table: "GuestBookings",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GuestBookings_Guests_GuestId",
                table: "GuestBookings",
                column: "GuestId",
                principalTable: "Guests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GuestBookings_Bookings_BookingId",
                table: "GuestBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_GuestBookings_Guests_GuestId",
                table: "GuestBookings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GuestBookings",
                table: "GuestBookings");

            migrationBuilder.RenameTable(
                name: "GuestBookings",
                newName: "GuestBooking");

            migrationBuilder.RenameIndex(
                name: "IX_GuestBookings_BookingId",
                table: "GuestBooking",
                newName: "IX_GuestBooking_BookingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GuestBooking",
                table: "GuestBooking",
                columns: new[] { "GuestId", "BookingId" });

            migrationBuilder.AddForeignKey(
                name: "FK_GuestBooking_Bookings_BookingId",
                table: "GuestBooking",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GuestBooking_Guests_GuestId",
                table: "GuestBooking",
                column: "GuestId",
                principalTable: "Guests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
