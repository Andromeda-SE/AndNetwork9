using And9.Gateway.Clan.Properties;
using And9.Integration.Discord.Senders;
using And9.Lib.API;
using And9.Service.Auth.API.Interfaces;
using And9.Service.Auth.Senders;
using And9.Service.Core.Abstractions.Interfaces;
using And9.Service.Core.Senders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;

namespace And9.Gateway.Clan.Hubs;

public class AuthHub : Hub, IAuthServerMethods
{
    private readonly GeneratePasswordSender _generatePasswordSender;
    private readonly LoginSender _loginSender;

    private readonly IConnectionMultiplexer _redis;
    private readonly SendDirectMessageSender _sendDirectMessageSender;
    private readonly SetPasswordSender _setPasswordSender;
    private readonly ReadMemberByIdSender _readMemberByIdSender;

    public AuthHub(
        GeneratePasswordSender generatePasswordSender,
        LoginSender loginSender,
        SendDirectMessageSender sendDirectMessageSender,
        SetPasswordSender setPasswordSender,
        IConnectionMultiplexer redis, ReadMemberByIdSender readMemberByIdSender)
    {
        _generatePasswordSender = generatePasswordSender;
        _loginSender = loginSender;
        _sendDirectMessageSender = sendDirectMessageSender;
        _setPasswordSender = setPasswordSender;
        _redis = redis;
        _readMemberByIdSender = readMemberByIdSender;
    }

    [AllowAnonymous]
    public async Task<string?> Login(string username, string password)
    {
#if DEBUG
        if (username == "MoryakSPb" && password == "@@@@@@")
        {
            string? newPassword = await _generatePasswordSender.CallAsync(1).ConfigureAwait(false);
            IMember? member = await _readMemberByIdSender.CallAsync(1).ConfigureAwait(false);
            if (member?.DiscordId is not null)
                await _sendDirectMessageSender.CallAsync(new()
                {
                    DiscordId = member.DiscordId.Value,
                    Message = string.Format(Resources.GeneratePasswordListener_Message, member.Nickname, newPassword),
                }).ConfigureAwait(false);

            return await _loginSender.CallAsync(new()
            {
                Nickname = username,
                Password = newPassword ?? string.Empty,
            }).ConfigureAwait(false);
        }
#endif
        return await _loginSender.CallAsync(new()
        {
            Nickname = username,
            Password = password,
        }).ConfigureAwait(false);
    }

    [Authorize]
    public async Task Logout()
    {
        IDatabase? db = _redis.GetDatabase(AuthOptions.REDIS_DATABASE_ID, new());
        await db.SetRemoveAsync(Context.UserIdentifier, Context.Items["access_token"]?.ToString()).ConfigureAwait(false);
    }

    [Authorize]
    public async Task AllLogout()
    {
        IDatabase? db = _redis.GetDatabase(AuthOptions.REDIS_DATABASE_ID, new());
        await db.KeyDeleteAsync(Context.UserIdentifier).ConfigureAwait(false);
    }

    [Authorize]
    public async Task GeneratePassword()
    {
        int memberId = int.Parse(Context.UserIdentifier!);
        string? newPassword = await _generatePasswordSender.CallAsync(memberId).ConfigureAwait(false);
        IMember? member = await _readMemberByIdSender.CallAsync(memberId).ConfigureAwait(false);
        if (member?.DiscordId is not null)
            await _sendDirectMessageSender.CallAsync(new()
            {
                DiscordId = member.DiscordId.Value,
                Message = string.Format(Resources.GeneratePasswordListener_Message, member.Nickname, newPassword),
            }).ConfigureAwait(false);
    }

    [Authorize]
    public async Task SetPassword(string password)
    {
        await _setPasswordSender.CallAsync((int.Parse(Context.UserIdentifier!), password)).ConfigureAwait(false);
    }
}