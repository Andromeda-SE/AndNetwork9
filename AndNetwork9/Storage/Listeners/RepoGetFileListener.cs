using System.Collections.Generic;
using System.Threading.Tasks;
using AndNetwork9.Shared.Backend;
using AndNetwork9.Shared.Backend.Rabbit;
using AndNetwork9.Shared.Backend.Senders.Storage;
using AndNetwork9.Shared.Storage;
using RabbitMQ.Client;

namespace AndNetwork9.Storage.Listeners
{
    public class RepoGetFileListener : BaseRabbitListenerWithResponse<RepoNode, byte[]>
    {
        private readonly ClanDataContext _data;
        private readonly RepoManager _repoManager;

        public RepoGetFileListener(IConnection connection, ClanDataContext data) : base(connection,
            RepoGetFileSender.QUEUE_NAME)
        {
            _data = data;
            _repoManager = new();
        }

        protected override Task<byte[]> GetResponseAsync(RepoNode request)
        {
            RepoNode? node = _data.RepoNodes.Find(new
            {
                request.RepoId,
                request.Version,
                request.Modification,
                request.Prototype,
            });
            if (node is null) throw new KeyNotFoundException();
            return Task.FromResult(_repoManager.GetFile(node));
        }
    }
}