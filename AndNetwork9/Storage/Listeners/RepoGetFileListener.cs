using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.Storage;
using AndNetwork9.Shared.Storage;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace AndNetwork9.Storage.Listeners
{
    public class RepoGetFileListener : BaseRabbitListenerWithResponse<RepoNode, byte[]>
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly RepoManager _repoManager;

        public RepoGetFileListener(IConnection connection, IServiceScopeFactory scopeFactory) : base(connection,
            RepoGetFileSender.QUEUE_NAME)
        {
            _scopeFactory = scopeFactory;
            _repoManager = new();
        }

        protected override async Task<byte[]> GetResponseAsync(RepoNode request)
        {
            await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
            ClanDataContext? data = (ClanDataContext?)scope.ServiceProvider.GetService(typeof(ClanDataContext));
            if (data is null) throw new ApplicationException();

            RepoNode? node = await data.RepoNodes.FindAsync(
                request.RepoId,
                request.Version,
                request.Modification,
                request.Prototype
            );
            if (node is null) throw new KeyNotFoundException();
            return _repoManager.GetFile(node);
        }
    }
}