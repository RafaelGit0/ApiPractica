using ApiPractica.Interfaces;
using ApiPractica.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace ApiPractica.Servicios
{
    public class RefreshTokenService(PersonDbContext db) : IRefreshTokenService
    {
        private readonly PersonDbContext _db = db;

        public string Generate()
        {
            return Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(64)
            );
        }

        public async Task SaveAsync(int userId, string token)
        {
            _db.RefreshTokens.Add(new RefreshToken
            {
                UserId = userId,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(2),
                IsRevoked = false
            });

            await _db.SaveChangesAsync();
        }

        public Task<RefreshToken?> GetAsync(string token)
        {
            return _db.RefreshTokens
                .Include(rt => rt.User)
                    .ThenInclude(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(rt =>
                    rt.Token == token &&
                    !rt.IsRevoked);
        }

        public async Task RevokeAsync(string token)
        {
            var rt = await _db.RefreshTokens
                .FirstOrDefaultAsync(x =>
                    x.Token == token &&
                    !x.IsRevoked);

            if (rt == null)
                return;

            rt.IsRevoked = true;
            await _db.SaveChangesAsync();
        }
    }
}
