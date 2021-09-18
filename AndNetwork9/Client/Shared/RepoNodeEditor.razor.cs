using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AndNetwork9.Client.Services;
using AndNetwork9.Shared.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace AndNetwork9.Client.Shared
{
    public partial class RepoNodeEditor
    {
        public enum VersionLevel
        {
            Version,
            Modification,
            Prototype,
        }

        [Inject]
        public HttpClient Client { get; set; }
        [Parameter]
        public Repo Repo { get; set; }
        [Parameter]
        public Action<RepoNode> RepoUpdated { get; set; }
        public string Description { get; set; }
        public VersionLevel Level { get; set; }
        public byte[] File { get; set; }

        private async void FileChanged(InputFileChangeEventArgs e)
        {
            IBrowserFile file = e.File;
            await using Stream stream = file.OpenReadStream();
            await using MemoryStream memoryStream = new();
            await stream.CopyToAsync(memoryStream);

            File = memoryStream.ToArray();
        }

        private async Task Send()
        {
            Console.WriteLine(string.Join(Environment.NewLine,
                Repo.Nodes.Select(x =>
                    $"V = {x.Version}, M = {x.Modification}, P = {x.Prototype}, S - {x.Description}")));
            Console.WriteLine();
            int version = Repo.Nodes.Any() ? Repo.Nodes.Max(x => x.Version) : 0;
            int modification = 0;
            int prototype = 0;
            Console.WriteLine($"0: V = {version}, M = {modification}, P = {prototype}");
            if (Level == VersionLevel.Version)
            {
                version += 1;
                Console.WriteLine($"1: V = {version}, M = {modification}, P = {prototype}");
            }
            else
            {
                modification = Repo.Nodes.Any()
                    ? Repo.Nodes.Where(x => x.Version == version).Max(x => x.Modification)
                    : 0;
                Console.WriteLine($"2: V = {version}, M = {modification}, P = {prototype}");
                if (Level == VersionLevel.Modification)
                {
                    modification += 1;
                    Console.WriteLine($"3: V = {version}, M = {modification}, P = {prototype}");
                }
                else if (Level == VersionLevel.Prototype)
                {
                    RepoNode[] nodes = Repo.Nodes.Where(x => x.Version == version)
                        .Where(x => x.Modification == modification).ToArray();
                    prototype = nodes.Any() ? nodes.Max(x => x.Prototype) + 1 : 1;
                    Console.WriteLine($"4: V = {version}, M = {modification}, P = {prototype}");
                    Console.WriteLine($"5: V = {version}, M = {modification}, P = {prototype}");
                }
            }

            RepoNodeWithData newNode = new()
            {
                Description = Description,
                Repo = Repo,
                AuthorId = AuthStateProvider.CurrentMember.Id,
                Author = AuthStateProvider.CurrentMember,
                CreateTime = DateTime.UtcNow,
                Data = File,
                RepoId = Repo.Id,
                Version = version,
                Modification = modification,
                Prototype = prototype,
            };
            Console.WriteLine($"F: V = {version}, M = {modification}, P = {prototype}");
            HttpResponseMessage response = await Client.PutAsJsonAsync($"api/Repo/{Repo.Id}/node", newNode);
            RepoUpdated(await response.Content.ReadFromJsonAsync<RepoNode>());
        }
    }
}