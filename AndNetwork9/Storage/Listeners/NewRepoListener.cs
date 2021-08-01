using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.Storage;
using AndNetwork9.Shared.Storage;
using AndNetwork9.Shared.Utility;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace AndNetwork9.Storage.Listeners
{
    public class NewRepoListener : BaseRabbitListenerWithResponse<Repo, Repo>
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly RepoManager _repoManager;

        public NewRepoListener(IConnection connection, IServiceScopeFactory scopeFactory) : base(connection,
            NewRepoSender.QUEUE_NAME)
        {
            _scopeFactory = scopeFactory;
            _repoManager = new();
        }

        protected override async Task<Repo> GetResponseAsync(Repo request)
        {
            await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
            ClanDataContext? data = (ClanDataContext?)scope.ServiceProvider.GetService(typeof(ClanDataContext));
            if (data is null) throw new ApplicationException();
            if (data.Repos.Any(x => x.RepoName == request.RepoName))
                    throw new ArgumentException(null, nameof(request));

            request.RepoName = Guid.NewGuid().ToString("N");

            _repoManager.CreateRepo(request);
            EntityEntry<Comment>? comment = data.Comments.Add(new Comment()
            { 
                AuthorId = request.CreatorId, 
                CreateTime = DateTime.UtcNow, 
                LastEditTime = null, 
                Text = string.Empty,
            });
            await data.SaveChangesAsync();
            request.CommentId = comment.Entity.Id;
            EntityEntry<Repo> result = await data.Repos.AddAsync(request with
            { 
                Nodes = new List<RepoNode>(), 
                Creator = null
            });
            await data.SaveChangesAsync();
            return result.Entity;
        }
    }
}