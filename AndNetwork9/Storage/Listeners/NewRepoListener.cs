using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.Storage;
using AndNetwork9.Shared.Storage;
using AndNetwork9.Shared.Utility;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace AndNetwork9.Storage.Listeners
{
    public class NewRepoListener : BaseRabbitListenerWithResponse<Repo, Repo>
    {
        private readonly RepoManager _repoManager;
        private readonly IServiceScopeFactory _scopeFactory;

        public NewRepoListener(IConnection connection, IServiceScopeFactory scopeFactory, ILogger<NewRepoListener> logger) : base(connection,
            NewRepoSender.QUEUE_NAME, logger)
        {
            _scopeFactory = scopeFactory;
            _repoManager = new();
        }

        protected override async Task<Repo> GetResponseAsync(Repo request)
        {
            AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
            await using ConfiguredAsyncDisposable _ = scope.ConfigureAwait(false);
            ClanDataContext? data = (ClanDataContext?)scope.ServiceProvider.GetService(typeof(ClanDataContext));
            if (data is null) throw new ApplicationException();
            if (data.Repos.Any(x => x.RepoName == request.RepoName))
                throw new ArgumentException(null, nameof(request));

            request.RepoName = Guid.NewGuid().ToString("N");

            _repoManager.CreateRepo(request);
            EntityEntry<Comment>? comment = data.Comments.Add(new()
            {
                AuthorId = request.CreatorId,
                CreateTime = DateTime.UtcNow,
                LastEditTime = null,
                Text = string.Empty,
            });
            await data.SaveChangesAsync().ConfigureAwait(false);
            request.CommentId = comment.Entity.Id;
            EntityEntry<Repo> result = await data.Repos.AddAsync(request with
            {
                Nodes = new List<RepoNode>(),
                Creator = null,
            }).ConfigureAwait(false);
            await data.SaveChangesAsync().ConfigureAwait(false);
            return result.Entity;
        }
    }
}