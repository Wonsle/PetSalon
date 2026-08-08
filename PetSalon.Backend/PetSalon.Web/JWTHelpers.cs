using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PetSalon.Services.AuthService;

public sealed class JwtHelpers
{
    public const string TokenUseClaim = "token_use";
    public const string PermissionClaimType = "permission";
    public const string PasswordChangeTokenUse = "password_change";
    public const int AccessTokenLifetimeMinutes = 480;
    public const int PasswordChangeTokenLifetimeMinutes = 15;

    private readonly string _issuer;
    private readonly byte[] _signingKey;
    private readonly TimeProvider _timeProvider;

    public JwtHelpers(IConfiguration configuration, TimeProvider timeProvider)
    {
        _issuer = configuration["JwtSettings:Issuer"]
            ?? throw new InvalidOperationException("Required configuration key is invalid: JwtSettings:Issuer");
        var signKey = configuration["JwtSettings:SignKey"]
            ?? throw new InvalidOperationException("Required configuration key is invalid: JwtSettings:SignKey");
        _signingKey = Encoding.UTF8.GetBytes(signKey);
        _timeProvider = timeProvider;
    }

    public JwtTokenResult GenerateAccessToken(AuthenticatedUser user)
    {
        var claims = CreateIdentityClaims(user);
        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(user.Permissions.Select(code => new Claim(PermissionClaimType, code)));
        return GenerateToken(claims, AccessTokenLifetimeMinutes);
    }

    public JwtTokenResult GeneratePasswordChangeToken(AuthenticatedUser user)
    {
        var claims = CreateIdentityClaims(user);
        claims.Add(new Claim(TokenUseClaim, PasswordChangeTokenUse));
        return GenerateToken(claims, PasswordChangeTokenLifetimeMinutes);
    }

    private static List<Claim> CreateIdentityClaims(AuthenticatedUser user) =>
    [
        new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
    ];

    private JwtTokenResult GenerateToken(IEnumerable<Claim> claims, int lifetimeMinutes)
    {
        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var expires = now.AddMinutes(lifetimeMinutes);
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _issuer,
            Subject = new ClaimsIdentity(claims),
            NotBefore = now,
            IssuedAt = now,
            Expires = expires,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(_signingKey),
                SecurityAlgorithms.HmacSha256)
        };

        var handler = new JwtSecurityTokenHandler();
        handler.OutboundClaimTypeMap.Clear();
        return new JwtTokenResult(
            handler.WriteToken(handler.CreateToken(descriptor)),
            checked(lifetimeMinutes * 60));
    }
}

public sealed record JwtTokenResult(string Token, int ExpiresIn);
