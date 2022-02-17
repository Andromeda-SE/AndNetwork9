using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace And9.Lib.API;

public class AuthOptions
{
    public const string ISSUER = "Andromeda-Server-9";
    public const string AUDIENCE = "Andromeda-Client-9";
    public const int LIFETIME_MINUTES = 90;

    public const int REDIS_DATABASE_ID = 1;

    public static string IssuerKey { get; set; } = string.Empty;

    public static SymmetricSecurityKey GetSymmetricSecurityIssuerKey() => new(Encoding.UTF8.GetBytes(IssuerKey));
}