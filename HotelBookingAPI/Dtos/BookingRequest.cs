﻿using HotelBookingAPI.Enums;
using HotelBookingAPI.Models;

namespace HotelBookingAPI.Dtos;

public class BookingRequest
{
    public int RoomId { get; set; }
    public string TravelerId { get; set; } = string.Empty;
    public int TotalGuests { get; set; }
    public List<GuestDto>? Guests { get; set; }
    public List<GuestBookingDto>? GuestBookings { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; }
}
