using And9.Lib.Broker;
using And9.Service.Auth.Abstractions.Models;

namespace And9.Service.Auth.Senders;

public static class Extensions
{
    public static BrokerBuilder AddAuthSenders(this BrokerBuilder builder)
    {
        builder.AppendSenderWithResponse<GeneratePasswordSender, int, string>();
        builder.AppendSenderWithResponse<LoginSender, AuthCredentials, string>();
        builder.AppendSenderWithoutResponse<SetPasswordSender, (int memberId, string newPassword)>();
        return builder;
    }
}