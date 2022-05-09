﻿using And9.Lib.Broker;
using And9.Lib.Broker.Senders;

namespace And9.Service.Election.Senders;

[QueueName(QUEUE_NAME)]
public class CancelRegisterSender : BrokerSenderWithResponse<int, bool>
{
    public const string QUEUE_NAME = "And9.Service.Election.CancelRegister";
    public CancelRegisterSender(BrokerManager brokerManager) : base(brokerManager) { }
}