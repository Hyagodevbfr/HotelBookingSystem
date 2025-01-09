using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.Dtos;

public record RoomSearchRequest 
(
    string? RoomName,
    [DataType(DataType.Date)]
    DateTime CheckInDate,
    [DataType(DataType.Date)]
    DateTime CheckOutDate,
    int AdultCapacity,
    int ChildCapacity
);
