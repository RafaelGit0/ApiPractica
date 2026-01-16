using ApiPractica.DTO;

namespace ApiPractica.Interfaces
{
    public interface IAuthService
    {
        UserAuthDto? ValidateUser(LoginDto login);
    }
}