using AndNetwork9.Shared;
using LibGit2Sharp;

namespace AndNetwork9.Storage
{
    public static class Extensions
    {
        public static Identity GetIdentity(this Member member) => new(member.ToString(), member.Id.ToString("D"));
    }
}