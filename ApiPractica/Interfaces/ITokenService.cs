using ApiPractica.DTO;
using ApiPractica.Models;

namespace ApiPractica.Interfaces
{
    public interface ITokenService
    {
        JwtResult Generate(UserAuthDto user);
    }
}