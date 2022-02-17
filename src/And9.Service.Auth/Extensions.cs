using System.Security.Cryptography;
using And9.Service.Auth.Database.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace And9.Service.Auth;

internal static class Extensions
{
    private const int _RANDOM_PASSWORD_LENGTH = 16;
    private static byte[] _staticSalt = new byte[16];
    private static readonly RandomNumberGenerator RandomNumberGenerator = RandomNumberGenerator.Create();

    private static readonly char[] AllowedPasswordCharacters = Enumerable.Empty<int>()
        .Concat(Enumerable.Range(0x0030, 10))
        .Concat(Enumerable.Range(0x0041, 26))
        .Concat(Enumerable.Range(0x0061, 26))
        .Append(0x002B)
        .Append(0x002D)
        .Select(x => (char)x).ToArray();

    internal static void SetPassword(this PasswordHash passwordHash, in string password)
    {
        passwordHash.Hash = password.GetPasswordHash();
    }

    internal static string SetRandomPassword(this PasswordHash passwordHash)
    {
        Span<byte> buffer = stackalloc byte[AllowedPasswordCharacters.Length];
        RandomNumberGenerator.GetBytes(buffer);
        Span<char> passwordRaw = stackalloc char[_RANDOM_PASSWORD_LENGTH];
        for (int i = 0; i < _RANDOM_PASSWORD_LENGTH; i++) passwordRaw[i] = AllowedPasswordCharacters[buffer[i] % AllowedPasswordCharacters.Length];
        string result = new(passwordRaw);
        passwordHash.SetPassword(result);
        return result;
    }

    internal static string GetRandomHex(int length = 32)
    {
        Span<char> values = stackalloc char[length];
        for (int i = 0; i < length; i++)
        {
            int value = RandomNumberGenerator.GetInt32(0, 16);
            values[i] = value.ToString("X")[0];
        }

        return new(values);
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
}