using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBookingAPI.Migrations
{
    /// <inheritdoc />
    public partial class AlterTypeTravelerIdInGuestModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guests_Travelers_TravelerUserId",
                table: "Guests");

            migrationBuilder.DropIndex(
                name: "IX_Guests_TravelerUserId",
                table: "Guests");

            migrationBuilder.DropColumn(
                name: "TravelerUserId",
                table: "Guests");

            migrationBuilder.AlterColumn<string>(
                name: "TravelerId",
                table: "Guests",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "GuestBooking",
                columns: table => new
                {
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    GuestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestBooking", x => new { x.GuestId, x.BookingId });
                    table.ForeignKey(
                        name: "FK_GuestBooking_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuestBooking_Guests_GuestId",
                        column: x => x.GuestId,
                        principalTable: "Guests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Guests_TravelerId",
                table: "Guests",
                column: "TravelerId");

            migrationBuilder.CreateIndex(
                name: "IX_GuestBooking_BookingId",
                table: "GuestBooking",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guests_Travelers_TravelerId",
                table: "Guests",
                column: "TravelerId",
                principalTable: "Travelers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Guests_Travelers_TravelerId",
                table: "Guests");

            migrationBuilder.DropTable(
                name: "GuestBooking");

            migrationBuilder.DropIndex(
                name: "IX_Guests_TravelerId",
                table: "Guests");

            migrationBuilder.AlterColumn<int>(
                name: "TravelerId",
                table: "Guests",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "TravelerUserId",
                table: "Guests",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Guests_TravelerUserId",
                table: "Guests",
                column: "TravelerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Guests_Travelers_TravelerUserId",
                table: "Guests",
                column: "TravelerUserId",
                principalTable: "Travelers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
