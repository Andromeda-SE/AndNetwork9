using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.Storage;
using AndNetwork9.Shared.Storage;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RabbitMQ.Client;

namespace AndNetwork9.Storage.Listeners
{
    public class NewRepoListener : BaseRabbitListenerWithResponse<Repo, Repo>
    {
        private readonly ClanDataContext _data;
        private readonly RepoManager _repoManager;

        public NewRepoListener(IConnection connection, ClanDataContext data) : base(connection,
            NewRepoSender.QUEUE_NAME)
        {
            _data = data;
            _repoManager = new();
        }

        protected override async Task<Repo> GetResponseAsync(Repo request)
        {
            if (_data.Repos.Any(x => x.RepoName == request.RepoName))
                throw new ArgumentException(null, nameof(request));

            request.RepoName = Guid.NewGuid().ToString("N");

            _repoManager.CreateRepo(request);
            EntityEntry<Repo> result = await _data.Repos.AddAsync(request with
            {
                Nodes = new List<RepoNode>(),
            });
            await _data.SaveChangesAsync();
            return result.Entity;
        }
    }
}