using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.Storage;
using AndNetwork9.Shared.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AndNetwork9.Storage.Listeners
{
    public class RepoGetFileListener : BaseRabbitListenerWithResponse<RepoNode, byte[]>
    {
        private readonly RepoManager _repoManager;
        private readonly IServiceScopeFactory _scopeFactory;

        public RepoGetFileListener(IConnection connection, IServiceScopeFactory scopeFactory,
            ILogger<RepoGetFileListener> logger) : base(connection,
            RepoGetFileSender.QUEUE_NAME,
            logger)
        {
            _scopeFactory = scopeFactory;
            _repoManager = new();
        }

        protected override async Task<byte[]> GetResponseAsync(RepoNode request)
        {
            AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
            await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
            ClanDataContext? data = (ClanDataContext?)scope.ServiceProvider.GetService(typeof(ClanDataContext));
            if (data is null) throw new ApplicationException();

            RepoNode? node = await data.RepoNodes.FindAsync(
                request.RepoId,
                request.Version,
                request.Modification,
                request.Prototype
            ).ConfigureAwait(false);
            if (node is null) throw new KeyNotFoundException();
            return _repoManager.GetFile(node);
        }
    }
}