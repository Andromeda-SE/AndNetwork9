using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AndNetwork9.Shared;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Auth;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;

namespace AndNetwork9.Server.Extensions
{
    internal static class AuthExtensions
    {
        public const string MEMBER_ID_CLAIM_NAME = "MemberId";
        public const string SESSION_ID_CLAIM_NAME = "SessionId";

        private static byte[] _staticSalt = new byte[16];

        internal static void SetPassword(this Member member, in string password)
        {
            member.PasswordHash = password.GetPasswordHash();
        }

        internal static byte[] GetPasswordHash(this string password)
        {
            return KeyDerivation.Pbkdf2(password, _staticSalt, KeyDerivationPrf.HMACSHA256, 10, 256 / 8);
        }

        internal static void SetSalt(string value)
        {
            if (value is null) return;
            byte[] result = Convert.FromHexString(value);
            if (result.Length != 16) throw new ArgumentException(null, nameof(value));
            _staticSalt = result;
        }

        internal static async ValueTask<Member?>
            GetCurrentMember(this ControllerBase controller, ClanDataContext data)
        {
            return await GetCurrentMember(controller.HttpContext.User, data);
        }

        internal static async ValueTask<Member?> GetCurrentMember(this ClaimsPrincipal user, ClanDataContext data)
        {
            string? rawValue = user.FindFirst(MEMBER_ID_CLAIM_NAME)?.Value;
            if (rawValue is null) return null;
            return await data.Members.FindAsync(int.Parse(rawValue));
        }

        internal static async ValueTask<AuthSession?> GetCurrentSession(this ClaimsPrincipal user, ClanDataContext data)
        {
            string? rawValue = user.FindFirst(SESSION_ID_CLAIM_NAME)?.Value;
            if (rawValue is null) return null;
            return await data.Sessions.FindAsync(Guid.Parse(rawValue));
        }
    }
}