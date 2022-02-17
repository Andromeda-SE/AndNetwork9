using And9.Lib.Broker;
using And9.Lib.Broker.Crud;
using And9.Service.Core.Abstractions.Models;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace And9.Service.Core.Senders;

public class CandidateRequestCrudSender : RabbitCrudSender<CandidateRegisteredRequest>
{
    public const string QUEUE_NAME = "And9.Service.Core.CandidateRequset";
    public const CrudFlags CRUD_FLAGS = CrudFlags.Read;

    public CandidateRequestCrudSender(
        IConnection connection,
        ILogger<BaseRabbitSenderWithResponse<CandidateRegisteredRequest, int>> createLogger,
        ILogger<BaseRabbitSenderWithResponse<int, CandidateRegisteredRequest>> readLogger,
        ILogger<BaseRabbitSenderWithStreamResponse<object, CandidateRegisteredRequest>> readCollectionLogger,
        ILogger<BaseRabbitSenderWithResponse<CandidateRegisteredRequest, CandidateRegisteredRequest>> updateLogger,
        ILogger<BaseRabbitSenderWithoutResponse<int>> deleteLogger)
        : base(connection, QUEUE_NAME, CRUD_FLAGS, createLogger, readLogger, readCollectionLogger, updateLogger, deleteLogger) { }
}