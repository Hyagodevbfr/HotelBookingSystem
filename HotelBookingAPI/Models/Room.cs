using Flunt.Notifications;
using Flunt.Validations;
using HotelBookingAPI.Dtos;

namespace HotelBookingAPI.Models;

public class Room : Entity
{
    public string? Type { get; set; } // Tipo do quarto (ex: Standard, Deluxe, Suite)
    public string? RoomName { get; set; }
    public int RoomsQuantity { get; set; } // Quantidade de quartos disponíveis
    public string? Description { get; set; } // Descrição do quarto (opcional, para detalhes adicionais)
    public int Capacity { get; set; } // Capacidade total do quarto
    public int AdultCapacity { get; set; } // Capacidade de adultos
    public int ChildCapacity { get; set; } // Capacidade de crianças
    public double PricePerNight { get; set; } // Preço por noite
    public TimeOnly? CheckInTime { get; set; }
    public TimeOnly? CheckOutTime { get; set; }
    public bool IsAvailable { get; set; } // Indica se o quarto está disponível para reserva
    public bool HasAirConditioning { get; set; } = false; // Indica se o quarto possui ar-condicionado
    public bool HasWiFi { get; set; } = false; // Indica se o quarto tem Wi-Fi
    public bool HasTV { get; set; } = false; // Indica se o quarto possui TV
    public bool IsAccessible { get; set; } = false; // Indica se o quarto é acessível (ex: PCD)
    public List<string>? Amenities { get; set; } = [];// Lista de comodidades adicionais (ex: cofre, frigobar)

    public List<Booking> Bookings { get; set; } = [];

    public Room(){}
    public Room(RoomDto roomDto)
    {
        RoomName = roomDto.RoomName;
        Type = roomDto.Type;
        RoomsQuantity = roomDto.RoomsQuantity;
        Description = roomDto.Description;
        Capacity = roomDto.Capacity;
        AdultCapacity = roomDto.AdultCapacity;
        ChildCapacity = roomDto.ChildCapacity;
        CheckInTime = roomDto.CheckInTime;
        CheckOutTime = roomDto.CheckOutTime;
        PricePerNight = roomDto.PricePerNight;
        IsAccessible = roomDto.IsAccessible;
        HasAirConditioning = roomDto.HasAirConditioning;
        HasWiFi = roomDto.HasWiFi;
        HasTV = roomDto.HasTV;
        Amenities = roomDto.Amenities;
        
        Validate();
    }
    public void EditInfo(RoomDto roomDto) 
    {
        RoomName = roomDto.RoomName;
        Type = roomDto.Type;
        RoomsQuantity = roomDto.RoomsQuantity;
        Description = roomDto.Description;
        Capacity = roomDto.Capacity;
        AdultCapacity = roomDto.AdultCapacity;
        ChildCapacity = roomDto.ChildCapacity;
        CheckInTime = roomDto.CheckInTime;
        CheckOutTime = roomDto.CheckOutTime;
        PricePerNight = roomDto.PricePerNight;
        IsAccessible = roomDto.IsAccessible;
        HasAirConditioning = roomDto.HasAirConditioning;
        HasWiFi = roomDto.HasWiFi;
        HasTV = roomDto.HasTV;
        Amenities = roomDto.Amenities;

        Validate( );
    }

    private void Validate()
    {
        var contract = new Contract<Room>( )
            .Requires( )
            .IsNotNullOrEmpty(RoomName,"RoomName","Nome do quarto não pode ser vazio.")
            .IsBetween(RoomName!.Length,5,100,"RoomName","O nome do quarto deve ter entre 5 e 100 caracteres.")

            .IsNotNullOrEmpty(Type,"Type","O tipo do quarto não pode ser vazio.")
            .IsBetween(Type!.Length,4,25,"Type","O tipo do quarto deve ter entre 4 e 25 caracteres.")

            .IsGreaterThan(RoomsQuantity,0,"RoomsQuantity","A quantidade de quartos deve ser maior que zero.")
            .IsGreaterThan(PricePerNight,0,"PricePerNight","O preço por noite deve ser maior que zero.")

            .IsGreaterOrEqualsThan(AdultCapacity,0,"AdultCapacity","A capacidade de adultos não pode ser negativa.")
            .IsLowerOrEqualsThan(AdultCapacity,Capacity,"AdultCapacity","A capacidade de adultos não pode exceder a capacidade total.")
            .IsLowerOrEqualsThan(AdultCapacity + ChildCapacity,Capacity,"TotalCapacity","A soma de adultos e crianças deve ser menor ou igual à capacidade total.")
            .IsTrue(AdultCapacity > 0 || ChildCapacity == 0,"ChildCapacity","Não é permitido criar um quarto somente com crianças.");
        AddNotifications(contract);
    }

}
