using System.Runtime.CompilerServices;
using And9.Lib.Broker;
using And9.Service.Election.Database;
using And9.Service.Election.Senders;
using RabbitMQ.Client;

namespace And9.Service.Election.Listeners;

public class CurrentElectionListener : BaseRabbitListenerWithStreamResponse<int, Abstractions.Models.Election>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public CurrentElectionListener(IConnection connection, ILogger<BaseRabbitListener> logger, IServiceScopeFactory serviceScopeFactory)
        : base(connection, CurrentElectionSender.QUEUE_NAME, logger) => _serviceScopeFactory = serviceScopeFactory;

    protected override async IAsyncEnumerable<Abstractions.Models.Election> GetResponseAsync(int request, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();
        ElectionDataContext context = scope.ServiceProvider.GetRequiredService<ElectionDataContext>();

        await foreach (Abstractions.Models.Election election in
                       context.GetCurrentElectionsWithVotesAsync().WithCancellation(cancellationToken).ConfigureAwait(false))
            yield return election;
    }
}