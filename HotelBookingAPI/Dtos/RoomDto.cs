using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.Dtos;

public class RoomDto
{
    [Required(ErrorMessage = "O nome do quarto é obrigatório.")]
    [StringLength(100,MinimumLength = 5,ErrorMessage = "O nome do quarto deve ter entre 5 e 100 caracteres.")]
    public string RoomName { get; set; } = string.Empty;

    [Required(ErrorMessage = "O tipo do quarto é obrigatório.")]
    [StringLength(25,MinimumLength = 4,ErrorMessage = "O tipo do quarto deve ter entre 4 e 25 caracteres.")]
    public string Type { get; set; } = string.Empty;

    [Range(1,int.MaxValue,ErrorMessage = "A quantidade de quartos deve ser maior que zero.")]
    public int RoomsQuantity { get; set; }

    public string? Description { get; set; }

    [Range(1,int.MaxValue,ErrorMessage = "A capacidade total deve ser maior que zero.")]
    public int Capacity { get; set; }

    [Range(0,int.MaxValue,ErrorMessage = "A capacidade de adultos deve ser maior ou igual a zero.")]
    public int AdultCapacity { get; set; }

    [Range(0,int.MaxValue,ErrorMessage = "A capacidade de crianças deve ser maior ou igual a zero.")]
    public int ChildCapacity { get; set; }

    [Range(0.01,double.MaxValue,ErrorMessage = "O preço por noite deve ser maior que zero.")]
    public double PricePerNight { get; set; }
    public bool IsAvailable { get; set; }
    public bool HasAirConditioning { get; set; } = false;
    public bool HasWiFi { get; set; } = false;
    public bool HasTV { get; set; } = false;
    public bool IsAccessible { get; set; } = false;
    public List<string>? Amenities { get; set; } = new( );
}
