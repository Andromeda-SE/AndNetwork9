using System.Collections.Generic;
using System.Net;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.Storage;
using AndNetwork9.Shared.Storage;
using RabbitMQ.Client;

namespace AndNetwork9.Storage.Listeners
{
    public class RepoSetFileListener : BaseRabbitListenerWithoutResponse<RepoNodeWithData>
    {
        private readonly ClanDataContext _data;
        private readonly RepoManager _repoManager;

        public RepoSetFileListener(IConnection connection, ClanDataContext data) : base(connection,
            RepoSetFileSender.QUEUE_NAME)
        {
            _data = data;
            _repoManager = new();
        }

        public override async void Run(RepoNodeWithData request)
        {
            RepoNode? node = await _data.RepoNodes.FindAsync(new
            {
                request.RepoId,
                request.Version,
                request.Modification,
                request.Prototype,
            });

            if (node is not null) throw new FailedCallException(HttpStatusCode.Conflict);
            Repo? repo = await _data.Repos.FindAsync(request.RepoId);
            if (repo is null) throw new KeyNotFoundException();

            await _data.RepoNodes.AddAsync(request);
            await _repoManager.LoadFile(request.Data, request);

            await _data.SaveChangesAsync();
        }
    }
}