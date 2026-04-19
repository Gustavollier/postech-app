using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PosTechChallenge.Applicacao.Dto.Autenticacao;
using PosTechChallenge.Applicacao.Interface.Services;

namespace PosTechChallenge.Applicacao.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly int _expirationMinutes;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _secretKey = _configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Jwt:SecretKey não configurado.");
        _issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer não configurado.");
        _audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience não configurado.");
        var expirationStr = _configuration["Jwt:AccessTokenExpirationMinutes"];
        _expirationMinutes = int.TryParse(expirationStr, out var exp) ? exp : 15;
    }

    public TokenResponseDto GerarToken(int funcionarioId, string cargo)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, funcionarioId.ToString()),
            new(ClaimTypes.Role, cargo),
            new("FuncionarioId", funcionarioId.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_expirationMinutes),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = credentials
        };

        JwtSecurityTokenHandler tokenHandler = new();

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        string tokenString = tokenHandler.WriteToken(token);

        var response = new TokenResponseDto(
            AccessToken: tokenString,
            RefreshToken: null,
            ExpiresIn: _expirationMinutes * 60,
            FuncionarioId: funcionarioId,
            Cargo: cargo);

        return response;
    }
}
