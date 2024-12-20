using Flunt.Notifications;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.Models;

public class Entity : Notifiable<Notification>
{
    [Key]
    public int Id { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public string? EditedBy { get; set; }
    public DateTime? EditedOn { get; set; } = DateTime.UtcNow;
}
