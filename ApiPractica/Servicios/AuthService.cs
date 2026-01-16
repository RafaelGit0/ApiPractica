using ApiPractica.DTO;
using ApiPractica.Interfaces;
using ApiPractica.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiPractica.Servicios
{
    public class AuthService(PersonDbContext db) : IAuthService
    {
        private readonly PersonDbContext _db = db;

        public UserAuthDto? ValidateUser(LoginDto login)
        {
            var user = _db.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefault(u => u.UserName == login.UserName);

            if (user == null)
                return null;

            if (!VerifyPassword(login.Password, user.PasswordHash))
                return null;

            var role = user.UserRoles
               .Select(ur => ur.Role.Name)
               .FirstOrDefault() ?? "User";

            return new UserAuthDto(
                user.Id,
                user.UserName,
                role
            );

        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }
}
