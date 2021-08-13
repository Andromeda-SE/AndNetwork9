using System;
using System.Collections.Generic;
using System.Net;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.Storage;
using AndNetwork9.Shared.Storage;
using LibGit2Sharp;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace AndNetwork9.Storage.Listeners
{
    public class RepoSetFileListener : BaseRabbitListenerWithoutResponse<RepoNodeWithData>
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly RepoManager _repoManager;
        private readonly IServiceScopeFactory _scopeFactory;

        public RepoSetFileListener(IConnection connection, IServiceScopeFactory scopeFactory) : base(connection,
            RepoSetFileSender.QUEUE_NAME)
        {
            _scopeFactory = scopeFactory;
            _repoManager = new();
        }

        public override async void Run(RepoNodeWithData request)
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

            if (node is not null) throw new FailedCallException(HttpStatusCode.Conflict);
            Repo? repo = await data.Repos.FindAsync(request.RepoId);
            if (repo is null) throw new KeyNotFoundException();
            request.Repo = repo;
            await _repoManager.LoadFile(request.Data, request);
            await data.RepoNodes.AddAsync(request);


            await data.SaveChangesAsync();
        }
    }
}