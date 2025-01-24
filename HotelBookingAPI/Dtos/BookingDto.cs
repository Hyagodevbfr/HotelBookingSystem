using HotelBookingAPI.Enums;
using HotelBookingAPI.Models;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.Dtos
{
    public class BookingDto
    {
        public int Id { get; set; }
        public string TravelerId { get; set; } = string.Empty;
        public string TravelerFullName { get; set; } = string.Empty;
        public string TravelerNationalId { get; set; } = string.Empty;
        public Traveler? Traveler { get; set; }
        public List<GuestDto>? Guests { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
    }
}
