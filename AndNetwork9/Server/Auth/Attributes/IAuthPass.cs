using AndNetwork9.Shared;

namespace AndNetwork9.Server.Auth.Attributes
{
    public interface IAuthPass
    {
        bool Pass(Member member);
    }
}