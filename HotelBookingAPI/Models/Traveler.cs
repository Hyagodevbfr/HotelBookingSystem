using Flunt.Br;
using Flunt.Notifications;
using HotelBookingAPI.Dtos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingAPI.Models;

public class Traveler : Notifiable<Notification>
{
    [Key]
    public string UserId { get; set; } = string.Empty;

    [ForeignKey(nameof(UserId))]
    public AppUser User { get; set; } = null!;

    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public string EmergencyContact { get; set; } = string.Empty;
    public string EmergencyContactName { get; set; } = string.Empty;

    public bool HasSpecialNeeds { get; set; }
    public string? SpecialNeedsDetails { get; set; }
    public string? DietaryPreferences { get; set; }

    public List<Booking>? Bookings { get; set; }
    public BookingHistory? BookingHistory { get; set; }
    public DateTime? LastReservationDate { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public string? EditedBy { get; set; }
    public DateTime? EditedOn { get; set; } = DateTime.UtcNow;

    public Traveler(){}
    public Traveler(CreateTravelerDto createTraveler)
    {
        //UserId = createTraveler.UserId;
        Address = createTraveler.Address;
        City = createTraveler.City;
        PostalCode = createTraveler.PostalCode;
        Country = createTraveler.Country;
        State = createTraveler.State;
        EmergencyContact = createTraveler.EmergencyContact;
        EmergencyContactName = createTraveler.EmergencyContactName;
        HasSpecialNeeds = createTraveler.HasSpecialNeeds;
        SpecialNeedsDetails = createTraveler.SpecialNeedsDetails;
        DietaryPreferences = createTraveler.DietaryPreferences;

        Validate( );
    }
    private void Validate()
    {
        AddNotifications(new Contract( )
            .Requires( )
            .IsNotNullOrEmpty(Address,"Address","Campo endereço não pode ser vazio.")
            .IsBetween(Address!.Length,5,100,"Endereço deve conter entre 5 a 100 caracteres")

            .IsNotNullOrWhiteSpace(City,"City","Campo cidade não pode ser vazio")
            .IsBetween(City!.Length,5,100,"Cidade deve conter entre 5 a 100 caracteres")

            .IsNotNullOrWhiteSpace(Country,"Country","Campo país não pode ser vazio")
            .IsBetween(City!.Length,5,35,"País deve conter entre 5 a 35 caracteres")

            .IsNotNullOrWhiteSpace(State,"State","Campo Estado não pode ser vazio")
            .IsBetween(State!.Length,2,23,"Estado deve conter entre 5 a 100 caracteres")

            .IsNotNullOrWhiteSpace(EmergencyContact,"EmergencyContact","Campo contato de emegência não pode ser vazio")
            .IsBetween(City!.Length,9,17,"Cidade deve conter entre 9 a 17 caracteres")

            .IsNotNullOrWhiteSpace(EmergencyContactName,"EmergencyContactName","Campo Nome do contato de emergência não pode ser vazio")
            .IsBetween(City!.Length,5,100,"Cidade deve conter entre 5 a 35 caracteres")
    );
    }
}
