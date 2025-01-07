using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBookingAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixesDatabaseInconsistency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Travelers",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmergencyContact = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmergencyContactName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HasSpecialNeeds = table.Column<bool>(type: "bit", nullable: false),
                    SpecialNeedsDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DietaryPreferences = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastReservationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Travelers", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Travelers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TravelerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TravelerUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingHistories_Travelers_TravelerId",
                        column: x => x.TravelerId,
                        principalTable: "Travelers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingHistories_Travelers_TravelerUserId",
                        column: x => x.TravelerUserId,
                        principalTable: "Travelers",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Guests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TravelerId = table.Column<int>(type: "int", nullable: false),
                    TravelerUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NationalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistrationId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HasSpecialNeeds = table.Column<bool>(type: "bit", nullable: false),
                    SpecialNeedsDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DietaryPreferences = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Guests_Travelers_TravelerUserId",
                        column: x => x.TravelerUserId,
                        principalTable: "Travelers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TravelerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    CheckInDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckOutDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EditedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BookingHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TravelerUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_BookingHistories_BookingHistoryId",
                        column: x => x.BookingHistoryId,
                        principalTable: "BookingHistories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Bookings_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Travelers_TravelerId",
                        column: x => x.TravelerId,
                        principalTable: "Travelers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Travelers_TravelerUserId",
                        column: x => x.TravelerUserId,
                        principalTable: "Travelers",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingHistories_TravelerId",
                table: "BookingHistories",
                column: "TravelerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingHistories_TravelerUserId",
                table: "BookingHistories",
                column: "TravelerUserId",
                unique: true,
                filter: "[TravelerUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingHistoryId",
                table: "Bookings",
                column: "BookingHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_RoomId",
                table: "Bookings",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TravelerId",
                table: "Bookings",
                column: "TravelerId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TravelerUserId",
                table: "Bookings",
                column: "TravelerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Guests_TravelerUserId",
                table: "Guests",
                column: "TravelerUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Guests");

            migrationBuilder.DropTable(
                name: "BookingHistories");

            migrationBuilder.DropTable(
                name: "Travelers");
        }
    }
}
