using System;
using System.Collections.Generic;
using System.Net;
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
    public class RepoSetFileListener : BaseRabbitListenerWithoutResponse<RepoNodeWithData>
    {
        private readonly RepoManager _repoManager;
        private readonly IServiceScopeFactory _scopeFactory;

        public RepoSetFileListener(IConnection connection, IServiceScopeFactory scopeFactory,
            ILogger<RepoSetFileListener> logger) : base(connection,
            RepoSetFileSender.QUEUE_NAME,
            logger)
        {
            _scopeFactory = scopeFactory;
            _repoManager = new();
        }

        public override async Task Run(RepoNodeWithData request)
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

            if (node is not null) throw new FailedCallException(HttpStatusCode.Conflict);
            Repo? repo = await data.Repos.FindAsync(request.RepoId).ConfigureAwait(false);
            if (repo is null) throw new KeyNotFoundException();
            request.Repo = repo;
            await _repoManager.LoadFile(request.Data, request).ConfigureAwait(false);
            await data.RepoNodes.AddAsync(request).ConfigureAwait(false);


            await data.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}