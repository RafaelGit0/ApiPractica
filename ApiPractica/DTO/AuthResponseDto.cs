namespace ApiPractica.DTO
{
    public class AuthResponseDto
    {
        public string Token { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = null!;
        public string Role { get; init; } = string.Empty;
        public DateTime ExpiresAt { get; init; }
    }
}
