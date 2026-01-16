using ApiPractica.Models;

namespace ApiPractica.Interfaces
{
    public interface IRefreshTokenService
    {
        string Generate();
        Task<RefreshToken?> GetAsync(string token);
        Task RevokeAsync(string token);
        Task SaveAsync(int userId, string token);
    }
}