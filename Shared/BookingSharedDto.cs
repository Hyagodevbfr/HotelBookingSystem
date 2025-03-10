using System.ComponentModel.DataAnnotations;

namespace Shared;

public class BookingSharedDto
{
    public BookingSharedDto(){}
    public class BookingShared
    {
        public int BookingId { get; set; }
        public string TravelerFullName { get; set; } = string.Empty;
        public string TravelerEmail { get; set; } = string.Empty;
        public string TravelerNationalId { get; set; } = string.Empty;

        //Room Info
        public string? TypeRoom { get; set; }
        public string? RoomName { get; set; }
        public string CheckIn { get; set; } = null!;
        public string CheckOut { get; set; } = null!;
        public string TotalPrice { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
