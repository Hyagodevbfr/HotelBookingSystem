namespace HotelBookingAPI.Dtos;

public record RoomSearchResponse
(
    int Id,
    string RoomName,
    int Capacity,
    double PricePerNight,
    int Nights,
    double TotalStay
);

