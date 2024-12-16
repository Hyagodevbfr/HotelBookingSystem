using HotelBookingAPI.Dtos;
using HotelBookingAPI.Infra.Data.Repositories;
using HotelBookingAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace HotelBookingAPI.Services;

public class UserVerifierService: IUserVerifier
{
    private readonly UserManager<AppUser> _userManager;

    public UserVerifierService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }


    public async Task<bool> VerifyUserEmployeeOrAdminOrNull(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if(user is null)
            return false;

        var roles = await _userManager.GetRolesAsync(user);
        if(roles.Contains("Admin") || roles.Contains("Employee"))
            return true;

        return false;
    }

    public ServiceResultDto<AppUser> ValidateUser(object userDto)
    {
        switch(userDto)
        {
            case UserRegisterDto userRegister:
                if(!ValidateCpf(userRegister.NationalId))
                    return ServiceResultDto<AppUser>.Fail("CPF inválido.");

                if(!ValidateRg(userRegister.RegistrationId))
                    return ServiceResultDto<AppUser>.Fail("RG inválido.");
            break;

            case UpdateUserDto updateUser:
                if(!ValidateCpf(updateUser.NationalId))
                    return ServiceResultDto<AppUser>.Fail("CPF inválido.");

                if(!ValidateRg(updateUser.RegistrationId))
                    return ServiceResultDto<AppUser>.Fail("RG inválido.");
            break;

            default:
                return ServiceResultDto<AppUser>.Fail("Não foi possível validar usuário.");

        }
                return ServiceResultDto<AppUser>.SuccessResult(null,"Documentos validados.");
    }
    public async Task<ServiceResultDto<AppUser>> IsDuplicateUser(object userDto)
    {
        string message = string.Empty;
        AppUser? user = userDto switch
        {
            UserRegisterDto userRegister => await _userManager.Users.FirstOrDefaultAsync(
                u => u.NationalId == userRegister.NationalId ||
                     u.RegistrationId == userRegister.RegistrationId),

            UpdateUserDto updateUser => await _userManager.Users.FirstOrDefaultAsync(
                u => u.NationalId == updateUser.NationalId ||
                     u.RegistrationId == updateUser.RegistrationId),

            _ => throw new ArgumentException("Não foi possível concluir a validação.")
        };
        
        if(user is null)
            return ServiceResultDto<AppUser>.SuccessResult(null, "Não há duplicidade.");

        switch(userDto)
        {
            case UserRegisterDto userRegister:
                if(user.NationalId == userRegister.NationalId)
                    message += "CPF já registrado.";
                if(user.RegistrationId == userRegister.RegistrationId)
                    message += "RG já cadastrado.";
            break;

            case UpdateUserDto updateUser:
                if(user.NationalId == updateUser?.NationalId)
                    message += "CPF já registrado.";
                if(user.RegistrationId == updateUser?.RegistrationId)
                    message += "RG já cadastrado.";
            break;
        }
        if(string.IsNullOrEmpty(message))
            return ServiceResultDto<AppUser>.SuccessResult(null, "Nenhuma duplicidade foi encontrada.");

        return ServiceResultDto<AppUser>.Fail(message.Trim());
    }

    public bool ValidateCpf(string cpf)
    {
        if(string.IsNullOrEmpty(cpf)) return false;

        cpf = cpf.Replace(".","").Replace("-","");
        if(cpf.Length != 11 || cpf.All(c => c == cpf[0])) return false;

        int[] multiplicador1 = { 10,9,8,7,6,5,4,3,2 };
        int[] multiplicador2 = { 11,10,9,8,7,6,5,4,3,2 };

        var tempCpf = cpf.Substring(0,9);
        var soma = tempCpf.Select((t,i) => int.Parse(t.ToString( )) * multiplicador1[i]).Sum( );
        var resto = soma % 11;
        var digito = resto < 2 ? 0 : 11 - resto;

        tempCpf += digito;
        soma = tempCpf.Select((t,i) => int.Parse(t.ToString( )) * multiplicador2[i]).Sum( );
        resto = soma % 11;
        digito = resto < 2 ? 0 : 11 - resto;

        return cpf.EndsWith(digito.ToString( ));
    }

    public bool ValidateRg(string rg)
    {
        if(string.IsNullOrEmpty(rg)) return false;

        rg = new string(rg.Where(char.IsDigit).ToArray( ));

        if(rg.Length < 7 || rg.Length > 9)
            return false;

        if(rg.All(c => c == rg[0]))
            return false;

        return true;
    }
}
