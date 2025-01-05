namespace HotelBookingAPI.Dtos;

public record RoomSearchRequest 
(
    string? RoomName,
    DateTime CheckInDate,
    DateTime CheckOutDate,
    int AdultCapacity,
    int ChildCapacity
);
