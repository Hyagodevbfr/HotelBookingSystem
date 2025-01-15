using Flunt.Br;
using Flunt.Notifications;
using HotelBookingAPI.Dtos;
using System;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.Models;

public class Guest : Notifiable<Notification>
{
    public int Id { get; set; }
    public string TravelerId { get; set; } = string.Empty;
    public Traveler? Traveler { get; set; }
    public List<GuestBooking>? GuestBookings { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? NationalId { get; set; } = string.Empty;
    public string RegistrationId { get; set; } = string.Empty;
    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; }

    public bool HasSpecialNeeds { get; set; }
    public string? SpecialNeedsDetails { get; set; }
    public string? DietaryPreferences { get; set; }

    [DataType(DataType.Date)]
    public DateTime CreatedOn { get; set; }
    public string? EditedBy { get; set; }
    [DataType(DataType.Date)]
    public DateTime? EditedOn { get; set; }

    public Guest(){}

    public Guest(GuestDto guestDto)
    {
        FirstName = guestDto.FirstName;
        LastName = guestDto.LastName;
        NationalId = guestDto.NationalId;
        RegistrationId = guestDto.RegistrationId;
        BirthDate = guestDto.BirthDate;
        HasSpecialNeeds = guestDto.HasSpecialNeeds;
        SpecialNeedsDetails = guestDto.SpecialNeedsDetails;
        DietaryPreferences = guestDto.DietaryPreferences;
    }

    private void Validate()
    {
        AddNotifications(new Contract( )
            .Requires( )

            .IsNotNullOrWhiteSpace(FirstName,"FirstName","Digite um nome válido.")
            .IsBetween(FirstName.Length!,2,19,"O nome precisa ter entre 2 a 19 caracteres.")

            .IsNotNullOrWhiteSpace(LastName,"LastName","Digite um sobrenome válido.")
            .IsBetween(LastName.Length!,2,50,"O sobrenome precisa ter entre até 50 caracteres.")

            .IsTrue(BirthDate <= DateTime.Now, "BirthDate", "Digite uma data de nascimento válida.")
        );
    }
}
