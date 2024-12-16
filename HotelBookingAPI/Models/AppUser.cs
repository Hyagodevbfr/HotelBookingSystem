﻿using HotelBookingAPI.Dtos;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.Models;

public class AppUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true; // Indica se o hóspede está ativo
    public string NationalId { get; set; } = string.Empty; // CPF
    public string RegistrationId { get; set; } = string.Empty; // RG
    public DateTime BirthDate { get; set; } // Data de nascimento
    public DateTime CreatedOn { get; set; }
    public string? EditedBy { get; set; }
    public DateTime? EditedOn { get; set; }
    public AppUser(){}
}
