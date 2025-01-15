namespace HotelBookingAPI.Dtos;

public record RoomSearchResponse
(
    int roomId,
    string RoomName,
    int Capacity,
    double PricePerNight,
    int Nights,
    double TotalStay
);

