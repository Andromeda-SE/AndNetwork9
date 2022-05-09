using And9.Lib.Broker;

namespace And9.Service.Award.Senders;

public static class Extensions
{
    public static BrokerBuilder AddAwardSenders(this BrokerBuilder builder)
    {
        builder.AppendSenderWithResponse<CreateAwardSender, Abstractions.Models.Award, int>();
        builder.AppendSenderWithCollectionResponse<ReadAllAwardsSender, int, Abstractions.Models.Award>();
        builder.AppendSenderWithCollectionResponse<ReadByMemberIdAwardSender, int, Abstractions.Models.Award>();
        builder.AppendSenderWithResponse<ReadAwardSender, int, Abstractions.Models.Award?>();
        return builder;
    }
}