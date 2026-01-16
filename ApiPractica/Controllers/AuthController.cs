using ApiPractica.DTO;
using ApiPractica.Interfaces;
using ApiPractica.Servicios;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/auth")]
public class AuthController(IConfiguration config, IAuthService authService, IRefreshTokenService refreshTokenService, ITokenService tokenService) : ControllerBase
{
    private readonly IConfiguration _config = config;
    private readonly IAuthService _authService = authService;
    private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;
    private readonly ITokenService _tokenService = tokenService;

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto login)
    {
        // 1️⃣ Validar credenciales
        var authUser = _authService.ValidateUser(login);

        if (authUser == null)
            return Unauthorized("Credenciales inválidas");

        // 2️⃣ Generar JWT (USANDO TOKEN SERVICE)
        var jwtResult = _tokenService.Generate(authUser);

        // 3️⃣ Refresh Token
        var refreshToken = _refreshTokenService.Generate();
        await _refreshTokenService.SaveAsync(authUser.UserId, refreshToken);

        // 4️⃣ Response
        return Ok(new AuthResponseDto
        {
            Token = jwtResult.Token,
            RefreshToken = refreshToken,
            Role = jwtResult.Role,
            ExpiresAt = jwtResult.ExpiresAt
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto dto)
    {
        // 1️⃣ Validar refresh token
        var stored = await _refreshTokenService.GetAsync(dto.RefreshToken);

        if (stored == null)
            return Unauthorized();

        // 2️⃣ Revocar refresh token actual
        await _refreshTokenService.RevokeAsync(dto.RefreshToken);

        // 3️⃣ Nuevo refresh token
        var newRefresh = _refreshTokenService.Generate();
        await _refreshTokenService.SaveAsync(stored.UserId, newRefresh);

        // 4️⃣ Crear UserAuthDto desde User
        var role = stored.User.UserRoles
            .FirstOrDefault()?.Role.Name ?? "User";

        var authUser = new UserAuthDto(
            stored.User.Id,
            stored.User.UserName,
            role
        );

        // 5️⃣ Generar JWT (MISMO TOKEN SERVICE)
        var jwtResult = _tokenService.Generate(authUser);

        // 6️⃣ Response
        return Ok(new AuthResponseDto
        {
            Token = jwtResult.Token,
            RefreshToken = newRefresh,
            Role = jwtResult.Role,
            ExpiresAt = jwtResult.ExpiresAt
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto dto)
    {
        await _refreshTokenService.RevokeAsync(dto.RefreshToken);
        return Ok();
    }

}
