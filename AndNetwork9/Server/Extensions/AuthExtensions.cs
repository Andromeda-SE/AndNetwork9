using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Auth;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;

namespace AndNetwork9.Server.Extensions;

internal static class AuthExtensions
{
    public const string MEMBER_ID_CLAIM_NAME = "MemberId";
    public const string SESSION_ID_CLAIM_NAME = "SessionId";

    private static byte[] _staticSalt = new byte[16];
    private static readonly RandomNumberGenerator RandomNumberGenerator = RandomNumberGenerator.Create();
    private const int _RANDOM_PASSWORD_LENGTH = 16;
    private static readonly char[] AllowedCharacters = Enumerable.Empty<int>()
        .Concat(Enumerable.Range(0x0030, 10))
        .Concat(Enumerable.Range(0x0041, 26))
        .Concat(Enumerable.Range(0x0061, 26))
        .Append(0x002B)
        .Append(0x002D)
        .Select(x => (char)x).ToArray();

    internal static void SetPassword(this Member member, in string password)
    {
        member.PasswordHash = password.GetPasswordHash();
    }

    internal static string SetRandomPassword(this Member member)
    {
        Span<byte> buffer = stackalloc byte[AllowedCharacters.Length];
        RandomNumberGenerator.GetBytes(buffer);
        StringBuilder builder = new (_RANDOM_PASSWORD_LENGTH);
        for (int i = 0; i < _RANDOM_PASSWORD_LENGTH; i++)
        {
            builder.Append(AllowedCharacters[buffer[i] % AllowedCharacters.Length]);
        }
        string result = builder.ToString();
        member.SetPassword(result);
        return result;
    }

    internal static byte[] GetPasswordHash(this string password) =>
        KeyDerivation.Pbkdf2(password, _staticSalt, KeyDerivationPrf.HMACSHA256, 10, 256 / 8);

    internal static void SetSalt(string value)
    {
        if (value is null) return;
        byte[] result = Convert.FromHexString(value);
        if (result.Length != 16) throw new ArgumentException(null, nameof(value));
        _staticSalt = result;
    }

    internal static async ValueTask<Member?>
        GetCurrentMember(this ControllerBase controller, ClanDataContext data) =>
        await GetCurrentMember(controller.HttpContext.User, data).ConfigureAwait(false);

    internal static async ValueTask<Member?> GetCurrentMember(this ClaimsPrincipal user, ClanDataContext data)
    {
        string? rawValue = user.FindFirst(MEMBER_ID_CLAIM_NAME)?.Value;
        if (rawValue is null) return null;
        return await data.Members.FindAsync(int.Parse(rawValue)).ConfigureAwait(false);
    }

    internal static async ValueTask<AuthSession?> GetCurrentSession(this ClaimsPrincipal user, ClanDataContext data)
    {
        string? rawValue = user.FindFirst(SESSION_ID_CLAIM_NAME)?.Value;
        if (rawValue is null) return null;
        return await data.Sessions.FindAsync(Guid.Parse(rawValue)).ConfigureAwait(false);
    }
}